using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTrigger : MonoBehaviour
{
    private MapController controller;
    public GameObject targetMap;

    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<MapController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
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
