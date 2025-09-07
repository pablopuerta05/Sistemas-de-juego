using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    public float frecuency; // speed of movement
    public float magnitude; // range of movement
    public Vector3 direction; // direction of movement
    private Vector3 initialPosition;

    private Pickup pickup;

    private void Start()
    {
        pickup = GetComponent<Pickup>();

        // save the starting position of the game object
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (pickup && !pickup.hasBeenCollected)
        {
            // sin function for smooth bobbing effect
            transform.position = initialPosition + direction * Mathf.Sin(Time.time * frecuency) * magnitude;
        }
        
    }
}
