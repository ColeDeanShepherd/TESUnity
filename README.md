# TESUnity

Hi my name is ReignOfDave and i have decided to extend the open source world view by @ColeDeanShepherd by trying to recreate Morrowind in unity!

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
|**Effects** | |
|AntiAliasing | `True` or `False` |
|AmbientOcclusion | `True` or `False` |
|Bloom | `True` or `False` |
|WaterBackSideTransparent | `True` or `False` |
|**Rendering** | |
| Shader  | `Unlit` or `Standard` or `Default` or `Bumped` |
| RenderPath  | `1` = Forward, `3` = Deferred |
|**VR** | |
| FollowHeadDirection | `True` or `False` |
| DirectModePreview | `True` or `False` |
|**Debug** | |
| CreaturesEnabled | `True` or `False` |

## Controls
| Action | Keys | Gamepad |
|--------|------|---------|
| Move | W, A, S, D* | Left thumbstick |
| Sprint | Left Shift | Left thumbstick button | 
| Walk | Left Ctrl | Right thumstick button |
| Use / Open / Attack | E | Button A |
| Cancel / Menu | Left click | Button B | 
| Take (book mode) | Nothing | Button X |
| Jump | Space | button Y |
| Toggle Flight Mode | Tab | Nothing |
| Toggle Lantern | L | Nothing |
| Free Cursor Lock | Backquote | Nothing |

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