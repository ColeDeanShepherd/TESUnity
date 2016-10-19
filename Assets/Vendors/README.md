# Vendors

This folder contains all external assets, libraries, SDKs, etc. For an obvious reason, some assets are not versionned.

VR SDKs are the perfect example because they are often updated.

## Enable advanced VR support
For now there are three SDKs : `Oculus`, `OSVR` and `SteamVR`. Enabling one of this SDKs allows you to enjoy extra.

The first step is to copy the SDK you want in this folder. `OSVR` and `Oculus` comes with a `Plugins` folder, you have to copy it in the folder of its SDK.
Doing this allows you to contribute to the project without pushing unecessary files.

The second step is to open Unity, File / Build Settings / Player Settings and add one of the following preprocessors in the `Scripting Define Symbols` input box.

* OSVR
* OCULUS
* STEAMVR

It'll activate the SDK and all scripts who use this SDK.
