# TESUnity VR

TESUnity supports the Oculus Rift and OpenVR devices (HTC Vive, Windows Mixed Reality, etc.).

The game requires a lot of CPU and GPU to work at a good framerate. The following presets will help you to configure the game with your own hardware.


### Maximum Performances

| Parameter | Values |
|-----------|---------|
| SunShadows  | `False` |
| LightShadows  | `False` |
| RenderExteriorCellLights | `False` |
| DayNightCycle | `False` |
| GenerateNormalMap | `False` |
|**Effects** | |
|AntiAliasing | `0` |
| PostProcessQuality | 0 |
|WaterBackSideTransparent | `False` |
|**Rendering** | |
| Shader  | `Unlit` |
| RenderPath  | `Forward` |
| CameraFarClip | `250` |
| WaterQuality | `0` |

In the launcher start the game in `Fastest`.

### Mix between performances and quality
| Parameter | Values |
|-----------|---------|
| SunShadows  | `true` |
| LightShadows  | `False` |
| RenderExteriorCellLights | `true` |
| DayNightCycle | `true` |
| GenerateNormalMap | `true` |
|**Effects** | |
|AntiAliasing | `3` |
| PostProcessQuality | 2 |
|WaterBackSideTransparent | `False` |
|**Rendering** | |
| Shader  | `Standard` |
| RenderPath  | `Deferred` |
| CameraFarClip | `500` |
| WaterQuality | `0` |

In the launcher start the game in `Good` or `Beautiful`.