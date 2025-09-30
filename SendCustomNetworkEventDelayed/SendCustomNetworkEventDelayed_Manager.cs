// Copyright (c) 2025 Occala
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Data;

[AddComponentMenu("")]
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
[DefaultExecutionOrder(int.MinValue + 1_000_000)] // Earliest possible
public class SendCustomNetworkEventDelayed_Manager : UdonSharpBehaviour
{
    [System.NonSerialized] public readonly DataList queuedEventsListDelayedFrames_FiringTimes = new DataList();
    [System.NonSerialized] public readonly DataList queuedEventsListDelayedFrames_EventData = new DataList();

    [System.NonSerialized] public readonly DataList queuedEventsListDelayedSeconds_FiringTimes = new DataList();
    [System.NonSerialized] public readonly DataList queuedEventsListDelayedSeconds_EventData = new DataList();

    /// <summary>
    /// Safety checks name in editor
    /// </summary>
    private void OnValidate()
    {
        if (this.name == nameof(SendCustomNetworkEventDelayed_Manager)) return;
        this.name = nameof(SendCustomNetworkEventDelayed_Manager);
    }

    public static void NetworkEventDelayedFrames(
        UdonSharpBehaviour scriptInstance, int delayFrames,
        VRC.Udon.Common.Interfaces.NetworkEventTarget target, string eventName,
        object parameter0 = null, object parameter1 = null, object parameter2 = null, object parameter3 = null,
        object parameter4 = null, object parameter5 = null, object parameter6 = null, object parameter7 = null
        )
    {
        // Sad scene find method until static/singleton patterns are allowed
        GameObject manager = GameObject.Find(nameof(SendCustomNetworkEventDelayed_Manager));
        if (!Utilities.IsValid(manager))
        {
            Debug.LogError($"[{nameof(NetworkEventDelayedSeconds)}] Couldn't find manager! " +
                $"Does your scene not include it or was it renamed? " +
                $"It should be named {nameof(SendCustomNetworkEventDelayed_Manager)}");

            return;
        }
        SendCustomNetworkEventDelayed_Manager managerScript = manager.GetComponent<SendCustomNetworkEventDelayed_Manager>();
        if (!Utilities.IsValid(managerScript))
        {
            Debug.LogError($"[{nameof(NetworkEventDelayedSeconds)}] Found manager, but it has no accompanying script? " +
                $"[{nameof(SendCustomNetworkEventDelayed_Manager)}]");

            return;
        }

        object[] parameters = new object[]
        {
                parameter0, parameter1, parameter2, parameter3,
                parameter4, parameter5, parameter6, parameter7
        };

        DataList eventEntry = new DataList();
        eventEntry.Add(new DataToken(scriptInstance));
        eventEntry.Add(new DataToken(target));
        eventEntry.Add(new DataToken(eventName));

        // First count the valid parameters
        int parameterCount = 0;
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i] == null)
                break;

            parameterCount++;
        }

        // Store the parameter count
        eventEntry.Add(new DataToken(parameterCount));

        // Add the valid parameters to the event data
        for (int i = 0; i < parameterCount; i++)
        {
            eventEntry.Add(new DataToken(parameters[i]));
        }

        // Fire immediately if delay is negative or zero
        if (delayFrames <= 0)
        {
            managerScript.FireQueuedEvent(eventEntry);
        }

        // Add firing time to list
        managerScript.queuedEventsListDelayedFrames_FiringTimes.Add(Time.frameCount + delayFrames);

        // Add event data to list
        managerScript.queuedEventsListDelayedFrames_EventData.Add(eventEntry);
        managerScript.enabled = true;
    }

    public static void NetworkEventDelayedSeconds(
        UdonSharpBehaviour scriptInstance, float delaySeconds,
        VRC.Udon.Common.Interfaces.NetworkEventTarget target, string eventName,
        object parameter0 = null, object parameter1 = null, object parameter2 = null, object parameter3 = null,
        object parameter4 = null, object parameter5 = null, object parameter6 = null, object parameter7 = null
        )
    {
        // Sad scene find method until static/singleton patterns are allowed
        GameObject manager = GameObject.Find(nameof(SendCustomNetworkEventDelayed_Manager));
        if (!Utilities.IsValid(manager))
        {
            Debug.LogError($"[{nameof(NetworkEventDelayedSeconds)}] Couldn't find manager! " +
                $"Does your scene not include it or was it renamed? " +
                $"It should be named {nameof(SendCustomNetworkEventDelayed_Manager)}");

            return;
        }
        SendCustomNetworkEventDelayed_Manager managerScript = manager.GetComponent<SendCustomNetworkEventDelayed_Manager>();
        if (!Utilities.IsValid(managerScript))
        {
            Debug.LogError($"[{nameof(NetworkEventDelayedSeconds)}] Found manager, but it has no accompanying script? " +
                $"[{nameof(SendCustomNetworkEventDelayed_Manager)}]");

            return;
        }

        object[] parameters = new object[]
        {
                parameter0, parameter1, parameter2, parameter3,
                parameter4, parameter5, parameter6, parameter7
        };

        DataList eventEntry = new DataList();
        eventEntry.Add(new DataToken(scriptInstance));
        eventEntry.Add(new DataToken(target));
        eventEntry.Add(new DataToken(eventName));

        // First count the valid parameters
        int parameterCount = 0;
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i] == null)
                break;

            parameterCount++;
        }

        // Store the parameter count
        eventEntry.Add(new DataToken(parameterCount));

        // Add the valid parameters to the event data
        for (int i = 0; i < parameterCount; i++)
        {
            eventEntry.Add(new DataToken(parameters[i]));
        }

        // Fire immediately if delay is negative or zero
        if (delaySeconds <= 0.0f)
        {
            managerScript.FireQueuedEvent(eventEntry);
        }

        // Add firing time to list
        managerScript.queuedEventsListDelayedSeconds_FiringTimes.Add(Time.timeAsDouble + delaySeconds);

        // Add event data to list
        managerScript.queuedEventsListDelayedSeconds_EventData.Add(eventEntry);
        managerScript.enabled = true;
    }

    /// <summary>
    /// Every frame we check over the queue (list), this is not especially efficient
    /// We do disable the update loop if there's nothing queued at the very least
    /// </summary>
    private void Update()
    {
        // Delayed frames events
        int delayedFramesCount = queuedEventsListDelayedFrames_FiringTimes.Count;
        int frameNow = Time.frameCount;
        for (int i = delayedFramesCount - 1; i >= 0; i--)
        {
            if (frameNow < queuedEventsListDelayedFrames_FiringTimes[i].Int)
                continue;

            FireQueuedEvent(queuedEventsListDelayedFrames_EventData[i].DataList);

            queuedEventsListDelayedFrames_FiringTimes.RemoveAt(i);
            queuedEventsListDelayedFrames_EventData.RemoveAt(i);
        }

        // Delayed seconds events
        int delayedSecondsCount = queuedEventsListDelayedSeconds_FiringTimes.Count;
        double timeNow = Time.timeAsDouble;
        for (int i = delayedSecondsCount - 1; i >= 0; i--)
        {
            if (timeNow < queuedEventsListDelayedSeconds_FiringTimes[i].Double)
                continue;

            FireQueuedEvent(queuedEventsListDelayedSeconds_EventData[i].DataList);

            queuedEventsListDelayedSeconds_FiringTimes.RemoveAt(i);
            queuedEventsListDelayedSeconds_EventData.RemoveAt(i);
        }

        // Check if we can disable the update loop
        bool hasRemainingEvents = queuedEventsListDelayedFrames_FiringTimes.Count > 0 || queuedEventsListDelayedSeconds_FiringTimes.Count > 0;
        if (!hasRemainingEvents)
        {
            this.enabled = false;
        }
    }

    private void FireQueuedEvent(DataList eventData)
    {
        int readIndex = 0;

        // This could break if the script instance is deleted locally, don't do that (you shouldn't be doing that on a networked object anyway)
        UdonSharpBehaviour scriptInstance = (UdonSharpBehaviour)eventData[readIndex].Reference;
        readIndex++;

        VRC.Udon.Common.Interfaces.NetworkEventTarget target = (VRC.Udon.Common.Interfaces.NetworkEventTarget)eventData[readIndex].Reference;
        readIndex++;

        string eventName = eventData[readIndex].String;
        readIndex++;

        int parameterCount = eventData[readIndex].Int;
        readIndex++;

        object[] parameters = new object[parameterCount];
        for (int i = 0; i < parameterCount; i++)
        {
            parameters[i] = eventData[readIndex].Reference;
            readIndex++;
        }

        switch (parameterCount)
        {
            case 0: scriptInstance.SendCustomNetworkEvent(target, eventName); break;
            case 1: scriptInstance.SendCustomNetworkEvent(target, eventName, parameters[0]); break;
            case 2: scriptInstance.SendCustomNetworkEvent(target, eventName, parameters[0], parameters[1]); break;
            case 3: scriptInstance.SendCustomNetworkEvent(target, eventName, parameters[0], parameters[1], parameters[2]); break;
            case 4: scriptInstance.SendCustomNetworkEvent(target, eventName, parameters[0], parameters[1], parameters[2], parameters[3]); break;
            case 5: scriptInstance.SendCustomNetworkEvent(target, eventName, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]); break;
            case 6: scriptInstance.SendCustomNetworkEvent(target, eventName, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]); break;
            case 7: scriptInstance.SendCustomNetworkEvent(target, eventName, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6]); break;
            case 8: scriptInstance.SendCustomNetworkEvent(target, eventName, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7]); break;

            default: scriptInstance.SendCustomNetworkEvent(target, eventName); break;
        }
    }

}
