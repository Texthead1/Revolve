# Revolve
A simple application to toggle the evolved state of Trap Villains, in the Unity Engine, for Windows.

As the application is meant to modify Traps, it is suggested to use Portals with a Trap slot. Furthermore, only a single Portal of Power should be used at any given time, and Xbox 360 Portals are supported.

This application is powered by Portal-To-Unity, an in-progress framework for interfacing with the Skylanders Portals of Power in the Unity Engine.

## Usage
A correct `salt.txt` is required at `Assets/StreamingAssets/` in source or `Revolve_Data/StreamingAssets/` in builds to read/write Traps correctly. The contents of the aforementioned file is used as part of the MD5 hash to generate the cryptographic key, otherwise the application will not function correctly. You can use [Bittersweet](https://github.com/Texthead1/Bittersweet-Salt-Extractor) to extract this file from an official source by passing in a legally owned ROM/file.

This application expects the Portals of Power to be using the `libusbK` driver. Please make sure you install the driver first, which can be done via [Zadig](https://zadig.akeo.ie/), and apply it to any Portal of Power you wish to use.