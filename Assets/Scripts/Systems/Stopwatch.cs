using UnityEngine;

public class Stopwatch
{
    [Header("Stopwatch")]
    [SerializeField] private GameObject player; // reference to the player's game object
    public float timeLimit; // the time limit in seconds
    private float stopwatchTime; // the current time elapsed since the stopwatch started
    [HideInInspector] public float StopwatchTime => stopwatchTime;

    public void UpdateStopWatch()
    {
        stopwatchTime += Time.deltaTime;

        UIManager.Instance.UpdateStopWatchDisplay();

        if (stopwatchTime >= timeLimit)
        {
            player.SendMessage("Kill");
        }
    }

    // gives us the time since the level has started
    public float GetElapsedTime()
    {
        return stopwatchTime;
    }
}
