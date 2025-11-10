using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingTextGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Canvas damageTextCanvas;
    [SerializeField] private Camera referenceCamera;
    [SerializeField] private TMP_FontAsset textFont;
    [SerializeField] private int textFontSize = 24;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private float textSpeed;
    [SerializeField] private float textDuration;

    private readonly Queue<TextMeshProUGUI> textPool = new();

    private void Awake()
    {
        if (!referenceCamera)
            referenceCamera = Camera.main;

        // Precrear el pool
        for (int i = 0; i < poolSize; i++)
        {
            TextMeshProUGUI textObj = CreateNewTextObject();
            textObj.gameObject.SetActive(false);
            textPool.Enqueue(textObj);
        }
    }

    private TextMeshProUGUI CreateNewTextObject()
    {
        GameObject textObj = new GameObject("FloatingText");
        textObj.transform.SetParent(damageTextCanvas.transform);
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();

        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        if (textFont) tmPro.font = textFont;

        return tmPro;
    }

    private TextMeshProUGUI GetFromPool()
    {
        if (textPool.Count > 0)
            return textPool.Dequeue();
        else
            return CreateNewTextObject();
    }

    private void ReturnToPool(TextMeshProUGUI tmPro)
    {
        tmPro.gameObject.SetActive(false);
        textPool.Enqueue(tmPro);
    }

    public void ShowFloatingText(string text, Transform target)
    {
        StartCoroutine(AnimateFloatingText(text, target, textDuration, textSpeed));
    }

    private IEnumerator AnimateFloatingText(string text, Transform target, float duration, float speed)
    {
        TextMeshProUGUI tmPro = GetFromPool();
        tmPro.text = text;
        tmPro.color = Color.white;
        tmPro.gameObject.SetActive(true);

        RectTransform rect = tmPro.GetComponent<RectTransform>();
        float t = 0f;
        float yOffset = 0f;

        while (t < duration)
        {
            yield return null;

            if (target == null)
            {
                ReturnToPool(tmPro);
                yield break;
            }

            t += Time.deltaTime;
            yOffset += speed * Time.deltaTime;
            float alpha = 1 - t / duration;

            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, alpha);

            if (target != null)
            {
                rect.position = referenceCamera.WorldToScreenPoint(target.position + Vector3.up * yOffset);
            }
            else
            {
                break;
            }
        }

        ReturnToPool(tmPro);
    }
}
