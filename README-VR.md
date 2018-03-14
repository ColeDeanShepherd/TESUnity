# TESUnity VR

TESUnity supports the Oculus Rift, OSVR and the OpenVR API. The world of Morrowind is generated at runtime, so some optimizations can't be done, especially lights and culling optimizations. Because of that, the game can slow down when a new group of cells are loaded.

## OSVR Users
The [OSVR Unity plugin](https://github.com/OSVR/OSVR-Unity) is not as optimized as other VR vendors. It is recommanded to switch to OpenVR (using SteamVR-OSVR) for best performance. However if you want to use the native OSVR integration, it is recommanded to turn off all effects and use the Unlit shader. We'll work with the OSVR Unity Team to find how to solve this problem.

If the [SteamVR-OSVR plugin](https://github.com/OSVR/SteamVR-OSVR) is enabled, it will load the OpenVR integration automatically, preventing you to start the game with the native OSVR integration. To solve that, you can 
* Disable your SteamVR driver
* Start the game like that `TESUnity.exe -vrmode None`.