
// Unsupported in U#
// ReSharper disable ArrangeObjectCreationWhenTypeEvident

using UnityEngine;
using JetBrains.Annotations;

using UdonSharp;
using VRC.SDKBase;
using VRC.SDK3.Data;

using NetworkEventTarget = VRC.Udon.Common.Interfaces.NetworkEventTarget;

[SelectionBase]
[AddComponentMenu("")]
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
[DefaultExecutionOrder(int.MinValue + 1_000_000)] // Earliest possible due to U#
public class SendCustomNetworkEventDelayed_Manager : UdonSharpBehaviour
{
    #region VALIDATION

    /// <summary>
    /// Safety checks in editor
    /// </summary>
    private void OnValidate()
    {
        if (this.name != nameof(SendCustomNetworkEventDelayed_Manager))
            this.name = nameof(SendCustomNetworkEventDelayed_Manager);

        if (gameObject.activeInHierarchy) return;

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    #endregion // VALIDATION

    /// <summary>
    /// Sad scene find method until static/singleton patterns are allowed. Please cache the result of this.
    /// </summary>
    [PublicAPI]
    public static bool TryGetInstance(out SendCustomNetworkEventDelayed_Manager instance)
    {
        instance = null;

        GameObject manager = GameObject.Find(nameof(SendCustomNetworkEventDelayed_Manager));
        if (!Utilities.IsValid(manager))
        {
            Debug.LogError($"[{nameof(NetworkEventDelayedSeconds)}] Couldn't find manager!\n" +
                           "Does your scene not include it or was it renamed?\n" +
                           $"It should be named {nameof(SendCustomNetworkEventDelayed_Manager)}");

            return false;
        }

        instance = manager.GetComponent<SendCustomNetworkEventDelayed_Manager>();
        if (!Utilities.IsValid(instance))
        {
            Debug.LogError($"[{nameof(NetworkEventDelayedSeconds)}] Found manager, but it has no accompanying script?\n" +
                           $"[{nameof(SendCustomNetworkEventDelayed_Manager)}]");

            return false;
        }

        return true;
    }

    #region DELAYED FRAMES API

    private readonly DataList _queuedEventFrames_Times = new DataList();
    private readonly DataList _queuedEventFrames_Data = new DataList();

    /// <inheritdoc cref="NetworkEventDelayedFrames(UdonSharpBehaviour, int, NetworkEventTarget, string, object, object, object, object, object, object, object, object)"/>
    [PublicAPI]
    public static void NetworkEventDelayedFrames_Expensive(
        UdonSharpBehaviour scriptInstance, int delayFrames,
        NetworkEventTarget target, string eventName,
        object parameter0 = null, object parameter1 = null, object parameter2 = null, object parameter3 = null,
        object parameter4 = null, object parameter5 = null, object parameter6 = null, object parameter7 = null
        )
    {
        if (!TryGetInstance(out var managerScript)) return;
        managerScript.NetworkEventDelayedFrames(scriptInstance, delayFrames, target, eventName, parameter0, parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7);
    }

    /// <summary>
    /// Sends a networked call after <paramref name="delayFrames"/> to the method with <paramref name="eventName"/> on the target UdonSharpBehaviour. The target method must be public.
    /// <remarks>The method is allowed to return a value, but the return value will not be accessible via this method.
    /// Methods with an underscore as their first character will not be callable via SendCustomNetworkEvent, unless they have a [NetworkCallable] attribute..</remarks>
    /// </summary>
    /// <param name="scriptInstance">The UdonSharpBehaviour that will have <paramref name="eventName"/> called on it</param>
    /// <param name="delayFrames">How many frames to wait before sending the network event</param>
    /// <param name="target">Whether to send this event to only the owner of the target behaviour's GameObject, or to everyone in the instance</param>
    /// <param name="eventName">Name of the method to call</param>
    [PublicAPI]
    public void NetworkEventDelayedFrames(UdonSharpBehaviour scriptInstance, int delayFrames,
        NetworkEventTarget target, string eventName,
        // ReSharper disable InvalidXmlDocComment
        object parameter0 = null, object parameter1 = null, object parameter2 = null, object parameter3 = null,
        object parameter4 = null, object parameter5 = null, object parameter6 = null, object parameter7 = null
    // ReSharper restore InvalidXmlDocComment
    )
    {
        object[] parameters = new object[]
        {
            parameter0, parameter1, parameter2, parameter3,
            parameter4, parameter5, parameter6, parameter7
        };

        var eventEntry = new DataList();
        eventEntry.Add(new DataToken(scriptInstance));
        eventEntry.Add(new DataToken(target));
        eventEntry.Add(new DataToken(eventName));

        // First count the valid parameters
        int parameterCount = 0;
        foreach (object t in parameters)
        {
            if (t == null) break;

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
            FireQueuedEvent(eventEntry);
            return;
        }

        // Add firing time to list
        _queuedEventFrames_Times.Add(Time.frameCount + delayFrames);

        // Add event data to list
        _queuedEventFrames_Data.Add(eventEntry);

        // Starts custom loop if not running
        _AttemptStartCheckFramesLoop();
    }

    #endregion // DELAYED FRAMES API

    #region DELAYED SECONDS API

    private readonly DataList _queuedEventSeconds_Times = new DataList();
    private readonly DataList _queuedEventSeconds_Data = new DataList();

    /// <inheritdoc cref="NetworkEventDelayedSeconds(UdonSharpBehaviour, float, NetworkEventTarget, string, object, object, object, object, object, object, object, object)"/>
    [PublicAPI]
    public static void NetworkEventDelayedSeconds_Expensive(
        UdonSharpBehaviour scriptInstance, float delaySeconds,
        NetworkEventTarget target, string eventName,
        object parameter0 = null, object parameter1 = null, object parameter2 = null, object parameter3 = null,
        object parameter4 = null, object parameter5 = null, object parameter6 = null, object parameter7 = null
        )
    {
        if (!TryGetInstance(out var managerScript)) return;
        managerScript.NetworkEventDelayedSeconds(scriptInstance, delaySeconds, target, eventName, parameter0, parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7);
    }

    /// <summary>
    /// Sends a networked call after <paramref name="delaySeconds"/> to the method with <paramref name="eventName"/> on the target UdonSharpBehaviour. The target method must be public.
    /// <remarks>The method is allowed to return a value, but the return value will not be accessible via this method.
    /// Methods with an underscore as their first character will not be callable via SendCustomNetworkEvent, unless they have a [NetworkCallable] attribute..</remarks>
    /// </summary>
    /// <param name="scriptInstance">The UdonSharpBehaviour that will have <paramref name="eventName"/> called on it</param>
    /// <param name="delaySeconds">How long to wait before sending the network event</param>
    /// <param name="target">Whether to send this event to only the owner of the target behaviour's GameObject, or to everyone in the instance</param>
    /// <param name="eventName">Name of the method to call</param>
    [PublicAPI]
    public void NetworkEventDelayedSeconds(UdonSharpBehaviour scriptInstance, float delaySeconds,
        NetworkEventTarget target, string eventName,
        // ReSharper disable InvalidXmlDocComment
        object parameter0 = null, object parameter1 = null, object parameter2 = null, object parameter3 = null,
        object parameter4 = null, object parameter5 = null, object parameter6 = null, object parameter7 = null
    // ReSharper restore InvalidXmlDocComment
    )
    {
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
            FireQueuedEvent(eventEntry);
            return;
        }

        // Add firing time to list
        _queuedEventSeconds_Times.Add(Time.timeAsDouble + delaySeconds);

        // Add event data to list
        _queuedEventSeconds_Data.Add(eventEntry);

        // Starts custom loop if not running
        _AttemptStartCheckSecondsLoop();
    }

    #endregion // DELAYED SECONDS API

    #region INTERNAL EVENT HANDLING

    private bool _queuedCheckRunning_Frames;
    public void _AttemptStartCheckFramesLoop()
    {
        if (_queuedCheckRunning_Frames) return;

        _queuedCheckRunning_Frames = true;
        SendCustomEventDelayedFrames(nameof(_RepeatedCheckFramesEvents), 1);
    }
    public void _RepeatedCheckFramesEvents()
    {
        int delayedFramesCount = _queuedEventFrames_Times.Count;
        int frameNow = Time.frameCount;
        for (int i = delayedFramesCount - 1; i >= 0; i--)
        {
            if (frameNow < _queuedEventFrames_Times[i].Int)
                continue;

            FireQueuedEvent(_queuedEventFrames_Data[i].DataList);

            _queuedEventFrames_Times.RemoveAt(i);
            _queuedEventFrames_Data.RemoveAt(i);
        }

        if (_queuedEventFrames_Times.Count > 0)
        {
            SendCustomEventDelayedFrames(nameof(_RepeatedCheckFramesEvents), 1);
        }
        else
        {
            _queuedCheckRunning_Frames = false;
        }
    }

    private bool _queuedCheckRunning_Seconds;
    public void _AttemptStartCheckSecondsLoop()
    {
        if (_queuedCheckRunning_Frames) return;

        _queuedCheckRunning_Seconds = true;
        SendCustomEventDelayedFrames(nameof(_RepeatedCheckSecondsEvents), 1);
    }
    public void _RepeatedCheckSecondsEvents()
    {
        int delayedSecondsCount = _queuedEventSeconds_Times.Count;
        double timeNow = Time.timeAsDouble;
        for (int i = delayedSecondsCount - 1; i >= 0; i--)
        {
            if (timeNow < _queuedEventSeconds_Times[i].Double)
                continue;

            FireQueuedEvent(_queuedEventSeconds_Data[i].DataList);

            _queuedEventSeconds_Times.RemoveAt(i);
            _queuedEventSeconds_Data.RemoveAt(i);
        }

        if (_queuedEventSeconds_Times.Count > 0)
        {
            SendCustomEventDelayedFrames(nameof(_RepeatedCheckSecondsEvents), 1);
        }
        else
        {
            _queuedCheckRunning_Seconds = false;
        }
    }

    private static void FireQueuedEvent(DataList eventData)
    {
        int readIndex = 0;

        // Safety check the script instance still existing
        // This could potentially happen in normal use if someone targeted another player's PlayerObject with an event on it
        DataToken scriptInstanceToken = eventData[readIndex];
        if (scriptInstanceToken.TokenType != TokenType.Reference) return; // This probably doesn't happen
        if (!Utilities.IsValid(scriptInstanceToken.Reference)) return; // This happens if it becomes null

        var scriptInstance = (UdonSharpBehaviour)scriptInstanceToken.Reference;
        readIndex++;

        var target = (NetworkEventTarget)eventData[readIndex].Reference;
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

    #endregion // INTERNAL EVENT HANDLING
}

