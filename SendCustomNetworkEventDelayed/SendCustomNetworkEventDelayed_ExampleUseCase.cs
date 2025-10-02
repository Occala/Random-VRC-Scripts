
using UnityEngine;

using UdonSharp;
using VRC.SDK3.UdonNetworkCalling;

using static SendCustomNetworkEventDelayed_Manager;

namespace Occala.RandomVRCScripts
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SendCustomNetworkEventDelayed_ExampleUseCase : UdonSharpBehaviour
    {
        private SendCustomNetworkEventDelayed_Manager _manager;
        
        private void Start()
        {
            // Cache the manager instance once for performance
            if (!TryGetInstance(out _manager)) return;
            // Start the repeated sending.
            SendCustomEventDelayedSeconds(nameof(_RepeatedSend), Random.Range(1f, 5f));
        }

        public void _RepeatedSend()
        {
            float delay = Random.Range(1f, 2.5f);

            _manager.NetworkEventDelayedSeconds(
                this, delay,
                VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ExampleNetworkEvent),
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
