# TESUnity

World viewers for Elder Scrolls games in the Unity game engine with VR support supporting Oculus, OSVR and OpenVR. For advanced VR support, please read the README.md located into the `Vendors` folder.

## Getting Started

**TESUnity requires a valid installation of Morrowind to run!**, you can get it on Steam or Gog.com.

To get started, download the source code as a ZIP file, extract it, and open the TESUnity folder in Unity.

You can copy the `Data Files` folder from your Morrowind installation to the root folder of the project / release. The game will start automatically without asking for a path.

## Configuration file
You can use the `config.ini` file located at the root folder of the project / release folder to configure and tweak your game experience.
The first step is to rename the `config.ini.dist` file to `config.ini`.


| Parameter | Values |
|-----------|---------|
|**Global** | |
| PlayMusic  | `True` or `False` |
| MorrowindPath | The Morrowind's `Data Files` path |
|**Lighting**| |
| AnimateLights  | `True` or `False` |
| SunShadows  | `True` or `False` |
| LightShadows  | `True` or `False` |
| RenderExteriorCellLights | `True` or `False` |
| DayNightCycle | `True` or `False` |
| GenerateNormalMap | `True` or `False` |
| NormalGeneratorIntensity | A value from 0.1 to 1.0 |
|**Effects** | |
| AntiAliasing |  A value from 0 to 3 (0 is disabled) | 
| PostProcessQuality | A value from 0 to 3 (0 is disabled) | 
| WaterBackSideTransparent | `True` or `False` |
|**Rendering** | |
| Shader  | `Unlit` or `Standard` or `Default` or `Bumped` |
| RenderPath  | `Forward` or `Deferred` |
| CameraFarClip | a value from 10 to 10000 |
| WaterQuality | a value from 0 to 2 |
|**VR** | |
| FollowHeadDirection | `True` or `False` |
| RoomScale | `True` or `False` |
| ForceControllers | `True` or `False` |
| XRVignette = | `True` or `False` |
|**Debug** | |
| CreaturesEnabled | `True` or `False` |

## Controls
| Action | Keys | Gamepad | VR |
|--------|------|---------|----|
| Move | W, A, S, D* | Left thumbstick | Left thumbstick |
| Sprint | Left Shift | Left thumbstick button | Left Grip |
| Walk | Left Ctrl | Right thumstick button | Right Grip |
| Use / Open / Attack | Space | Button A | Right Trigger |
| Cancel / Menu | Left click | Button B | Left Menu |
| Take (book mode) | Nothing | Button X | Left Trigger |
| Jump | E | button Y | Left thumbstick |
| Toggle Flight Mode | Tab | Nothing | Nohtin |
| Toggle Lantern | L | Nothing | Right thumbstick |
| Free Cursor Lock | Backquote | Nothing | Nothing |

\* *It uses the AZERTY mapping for French users.*

## Contribute

Bugs and feature requests are listed on the [GitHub issues page](https://github.com/ColeDeanShepherd/TESUnity/issues). Feel free to fork the source code and contribute, or use it in any way that falls under the [MIT License](https://github.com/ColeDeanShepherd/TESUnity/blob/master/LICENSE.txt).

Please create a branch from develop for each "feature" (see [this article](http://nvie.com/posts/a-successful-git-branching-model/)).


Morrowind Data Format Resources
-------------------------------

* [ESM File Format Specification](http://www.mwmythicmods.com/argent/tech/es_format.html)
* [BSA File Format Specification](http://www.uesp.net/wiki/Tes3Mod:BSA_File_Format)
* [NIF File Format Specification](https://github.com/niftools/nifxml/blob/develop/nif.xml)
* [NIF Viewer/Data Inspector](https://github.com/niftools/nifskope)