// Copyright (c) 2025 Occala
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

https://github.com/Occala/Random-VRC-Scripts/

Install:
Place the prefab in your scene somewhere, don't rename it

Calling this is a bit verbose, a typical call from some script would be as follows:

SendCustomNetworkEventDelayed_Manager.NetworkEventDelayedSeconds(
  this, 1f                                                                                 // The script instance with the event and the delay
  VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ExampleNetworkEvent),         // Event target and name of the event
  64, 3.68f, "hello!"                                                                    // Parameters
);
