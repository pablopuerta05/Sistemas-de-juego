using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("Event Settings")]
    public EventData[] events;

    [Tooltip("how long to wait before this becomes active")]
    public float firstTriggerDelay = 180f;

    [Tooltip("how long to wait between each event")]
    public float triggerInterval = 30f;

    [System.Serializable]
    private class ActiveEvent
    {
        public EventData data;

        public float duration;
        public float cooldown;
    }
    
    private float currentEventCooldown = 0;
    private List<ActiveEvent> runningEvents = new List<ActiveEvent>(); // these are events that have been activated, and are running
    [SerializeField] private PlayerStats player;

    private void Start()
    {
        currentEventCooldown = firstTriggerDelay > 0 ? firstTriggerDelay : triggerInterval;

        // fallback in case Initialize() wasn’t called
        if (player = null)
        {
            Debug.LogWarning("EventManager: no player was assigned, finding him automatically.");
            player = FindAnyObjectByType<PlayerStats>();
        }
    }

    private void Update()
    {
        // cooldown for adding another event to the slate
        currentEventCooldown -= Time.deltaTime;

        if (currentEventCooldown <= 0)
        {
            // get an event and try to execute it
            TriggerRandomEvent();

            // set the cooldown for the event
            currentEventCooldown = triggerInterval;
        }

        UpdateRunningEvents();
    }

    private void TriggerRandomEvent()
    {
        EventData e = GetRandomEvent();
        if (e == null) return;

        if (e.CheckIfWillHappen(player))
        {
            runningEvents.Add(new ActiveEvent { data = e, duration = e.duration, cooldown = 0f });
        }
    }

    private void UpdateRunningEvents()
    {
        // events that we want to remove
        List<ActiveEvent> expiredEvents = new List<ActiveEvent>();

        // cooldown for existing event to see if they should continue running
        foreach (ActiveEvent e in runningEvents)
        {
            // reduce the current duration
            e.duration -= Time.deltaTime;

            if (e.duration <= 0)
            {
                expiredEvents.Add(e);
                continue;
            }

            // reduce the current cooldown
            e.cooldown -= Time.deltaTime;
            if (e.cooldown <= 0)
            {
                // pick player to sic this mob on, then reset the cooldown
                e.data.Activate(player);
                e.cooldown = e.data.GetSpawnInterval();
            }
        }

        // remove all the events that have expired
        foreach (ActiveEvent e in expiredEvents)
            runningEvents.Remove(e);
    }

    public EventData GetRandomEvent()
    {
        // If no events are assigned, don't return anything.
        if (events.Length <= 0) return null;

        // Create a new list to populate with possible Events
        List<EventData> possibleEvents = new List<EventData>();

        // Add the events in event to the possible events only if the event is active
        foreach (EventData e in events)
        {
            if (e.IsActive())
            {
                possibleEvents.Add(e);
            }
        }
        // Randomly pick an event from the possible events to play
        EventData result = possibleEvents[Random.Range(0, possibleEvents.Count)];
        return result;
    }
}
