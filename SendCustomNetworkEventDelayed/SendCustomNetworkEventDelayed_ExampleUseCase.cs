// Copyright (c) 2025 Occala
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.UdonNetworkCalling;

namespace Occala.RandomVRCScripts
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SendCustomNetworkEventDelayed_ExampleUseCase : UdonSharpBehaviour
    {
        private void Start()
        {
            SendCustomEventDelayedSeconds(nameof(_RepeatedSend), Random.Range(1f, 5f));
        }

        public void _RepeatedSend()
        {
            float delay = Random.Range(1f, 2.5f);

            SendCustomNetworkEventDelayed_Manager.NetworkEventDelayedSeconds(
                this,
                VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ExampleNetworkEvent),
                delay,
                Random.Range(int.MinValue, int.MaxValue), Random.Range(float.MinValue, float.MaxValue), "hello!"
                );

            SendCustomEventDelayedSeconds(nameof(_RepeatedSend), Random.Range(1f, 5f));
        }

        [NetworkCallable]
        public void ExampleNetworkEvent(int integer, float single, string stringValue)
        {
            Debug.Log($"[{nameof(ExampleNetworkEvent)}] Invoked - Parameters: {integer} | {single} | {stringValue}");
        }

    }

}
