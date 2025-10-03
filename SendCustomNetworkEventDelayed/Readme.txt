https://github.com/Occala/Random-VRC-Scripts/

Install:
Place the prefab in your scene somewhere, don't rename it






A typical call without caching the delay manager from some script would be as follows:


using static SendCustomNetworkEventDelayed_Manager;

NetworkEventDelayedSeconds_Expensive(
  this, 1f                                                                                 // The script instance with the event and the delay
  VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ExampleNetworkEvent),         // Event target and name of the event
  64, 3.68f, "hello!"                                                                    // Parameters
  );




Consider caching the delay manager, prior to a for-loop or to reduce the cost of fetching it across normal calls:

SendCustomNetworkEventDelayed_Manager.TryGetInstance(out var _manager);
for (int i = 0; i < 100; i++)
{
  _manager.NetworkEventDelayedFrames(
    this, 1,
    VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ExampleNetworkEvent),
    64, 3.68f, "hello!"
    );
}
