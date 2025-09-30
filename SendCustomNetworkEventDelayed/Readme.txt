Calling this is a bit verbose, a typical call from some script would be as follows:

SendCustomNetworkEventDelayed_Manager.NetworkEventDelayedSeconds(
  this,                                                                                                // The script instance with the event
  VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ExampleNetworkEvent),                      // Event target and name of the event
  1f,                                                                                                  // Delay
  Random.Range(int.MinValue, int.MaxValue), Random.Range(float.MinValue, float.MaxValue), "hello!"     // Parameters
);
