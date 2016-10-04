# TESUnity

World viewers for Elder Scrolls games in the Unity game engine with VR support supporting Oculus and OpenVR.

## Getting Started

**TESUnity requires a valid installation of Morrowind to run!**, you can get it on Steam or Gog.com.

To get started, download the source code as a ZIP file, extract it, and open the TESUnity folder in Unity.

You can copy the `Data Files` folder from your Morrowind installation to the root folder of the project / release. The game will start automatically without asking for a path.

## Configuration file
You can create a configuration file named `Config.ini` at the root folder of the project / release folder.

| Parameter | Values |
|-----------|---------|
| SunShadows  | `True` or `False` |
| LightShadows  | `True` or `False` |
| PlayMusic  | `True` or `False` |
| Shader  | `Unlit` or `Standard` or `Default` or `Bumped` |
| RenderPath  | `1` = Forward, `3` = Deferred |


## Controls
| Action | Keys | Gamepad |
|--------|------|---------|
| Move | W, A, S, D | Left thumbstick |
| Sprint | Left Shift | Left thumbstick button | 
| Walk | Left Ctrl | Right thumstick button |
| Jump | Space | button Y |
| Interact | E | button A |
| Toggle Flight Mode | Tab | Nothing |
|Toggle Lantern | L | button X |
| Free Cursor Lock | Backquote | Nothing |


## Contribute

Bugs and feature requests are listed on the [GitHub issues page](https://github.com/ColeDeanShepherd/TESUnity/issues). Feel free to fork the source code and contribute, or use it in any way that falls under the [MIT License](https://github.com/ColeDeanShepherd/TESUnity/blob/master/LICENSE.txt).

Please create a branch from develop for each "feature" (see [this article](http://nvie.com/posts/a-successful-git-branching-model/)).


Morrowind Data Format Resources
-------------------------------

* [ESM File Format Specification](http://www.mwmythicmods.com/argent/tech/es_format.html)
* [BSA File Format Specification](http://www.uesp.net/wiki/Tes3Mod:BSA_File_Format)
* [NIF File Format Specification](https://github.com/niftools/nifxml/blob/develop/nif.xml)
* [NIF Viewer/Data Inspector](https://github.com/niftools/nifskope)