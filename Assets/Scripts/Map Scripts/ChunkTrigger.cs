using UnityEngine;

public class ChunkTrigger : MonoBehaviour
{
    private MapController controller;
    public GameObject targetMap;

    // Start is called before the first frame update
    void Start()
    {
        controller = FindAnyObjectByType<MapController>();

        if (controller == null)
        {
            Debug.LogError("MapController no encontrado en la escena.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            controller.currentChunk = targetMap;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (controller.currentChunk == targetMap)
            {
                controller.currentChunk = null;
            }
        }
    }
}
