# Live Lights: In-game siren settings editor for GTA V
## Created by PNWParksFan

This is a RagePluginHook plugin for single-player GTA V. 
It enables users to view and modify all siren setting 
parameters live in game and import/export carcols.meta files.

You must have [RagePluginHook](http://ragepluginhook.net/) version 98 or later. 
You can download RPH [bundled with LSPDFR](https://www.lcpdfr.com/downloads/gta5mods/g17media/7792-lspd-first-response/)
or from the [RPH Discord server](https://discord.gg/0v9TP1BOmfwZms7y).

This plugin is primarily intended as a tool for vehicle developers 
to create and modify siren settings. Users are welcome to use it to 
look around and try out making their own custom sequences, but it is 
not intended as a general-use siren controller. 

If you use this mod to make something cool, please mention this mod in 
your readme or credits file, and link to this page. Feel free to send 
me a link to your mod when it's released so I can see what you created 
using this plugin! If you found this plugin particularly useful, you 
can [donate to me](https://parksmods.com/donate/) to support my 
development projects and get member-exclusive benefits. 

[Download the latest version from the releases tab](https://github.com/pnwparksfan/rph-live-lights/releases)

[![Latest Version](https://img.shields.io/github/release/pnwparksfan/rph-live-lights)](https://github.com/pnwparksfan/rph-live-lights/releases)  
[![Download Count](https://img.shields.io/github/downloads/pnwparksfan/rph-live-lights/total)](https://github.com/pnwparksfan/rph-live-lights/releases)    

If you encounter any bugs, please [submit an issue](https://github.com/pnwparksfan/rph-live-lights/issues) or contact me on Discord if we share a mutual server.

[![Issues](https://img.shields.io/github/issues/pnwparksfan/rph-live-lights)](https://github.com/pnwparksfan/rph-live-lights/issues)  
[![Issues](https://img.shields.io/github/issues-closed/pnwparksfan/rph-live-lights)](https://github.com/pnwparksfan/rph-live-lights/issues)


&nbsp;

## Quick feature overview and user guide

 - Configure carcols.meta and carvariations.meta for your vehicle(s) and load your initial carcols settings
 - Launch the game with LiveLights installed
 - Open the RPH console and type `LoadPlugin LiveLights.dll` to load the plugin if it isn't set to load automatically
 - Spawn a vehicle you want to work on
 - Open the LiveLights menu (`-` key on the main keyboard by default)
 - The menu will show the default siren setting name for the current vehicle. You can switch to a different siren setting from the menu if you want.
 - Any siren setting defined in any carcols.ymt/carcols.meta file is not editable in game. You can only edit cloned copies which are created temporarily in memory. Clicking into the Edit Siren Settings menu will automatically create an editable clone of the currently selected siren setting. Once an editable clone has been created, any changes you make to it will only apply to that cloned setting; they will not affect the original version. You can set any individual vehicle to use the cloned copy by selecting it through the menu, but any newly spawned vehicle will use its default, unedited siren setting when spawned. 
 - Any changes you make in the Edit Siren Settings menu will immediately be applied to all spawned vehicles which have been set to use that clonsed siren setting.
   - Within the main menu you can change all settings which apply to the overall siren setting entry (e.g. BPM, falloff settings, etc.)
   - There are submenus to edit settings for each individual siren (1-20). The siren submenus siren-specific settings, and have further submenus for corona, flashiness, and rotation settings. 
   - There are separate submenus for headlight and taillight settings. 
   - There is a sequence quick-edit menu which allows you to change the flashiness sequence for all sirens, plus headlights and taillights, from a single menu without having to switch between siren submenus. 
   - You can use the Copy menu to copy settings between sirens within one settings instance, or to copy between different settings instances. Select dynamically whether to copy everything or only certain properties.
 - When you are finished configuring your siren setting and are satisfied with the results, you can click the Export item on the Edit Siren Settings menu to export that individual setting, or you can use the Export menu from the main menu to select multiple settings to export to a single file.
   - You will be prompted for a file location. If you just enter a filename, the setting entry currently being edited will be saved to `GTA V\Plugins\LiveLights\carcols\<selected_filename>`. If you enter a path relative to the GTA root folder, your file will be saved to that path, e.g. `LML\police-pack\data\new-carcols.meta`. You can also enter an absolute path and the file will be saved to that exact location even if it is not within the GTA V folder, e.g. `C:\GTA V\mods\police\carcols-2.meta`. 
   - You can set the siren ID on each edited siren setting as you make changes. This only affects the siren ID that will be exported; it does not do anything in-game, and nothing prevents you from exporting multiple settings with the same ID (which you should probably avoid).
   - If you do not specify a siren ID it will be exported as `0` by default. You will need to edit the siren ID to the value you wish to use in your carcols.meta/carvariations.meta. See info below regarding important notes on siren setting ID limitations. 
   - The exported file will be a fully valid carcols.meta file and can be used directly in a DLC, LML package, or FiveM resource. 
 - All changes will be lost when you exit the game. Make sure to export any savings you want to keep, and add those exported savings to a carcols.meta file which will get loaded by the game. 


&nbsp;

# Siren Setting ID limit

GTA V has a hard-coded limit of 255 siren setting IDs in the game. 
Siren setting IDs over 255 can be entered in carcols.meta, but 
will overflow and be converted to a number between 0-255, which may 
conflict with other mods. 

## SirenSetting Limit Adjuster

It is strongly recommended to install the [SirenSetting Limit Adjuster](https://www.lcpdfr.com/downloads/gta5mods/scripts/28560-sirensetting-limit-adjuster/) 
(SSLA). This raises the siren setting ID limit to 65535, and increases the per-vehicle siren 
limit from 20 sirens to 32 sirens. LiveLights automatically detects if SSLA is installed and 
how many sirens are supported by your game. If SSLA is installed, you can choose to export 
carcols.meta files with only 20 sirens, or up to 32 sirens. Select this in the export menu.


# Credits 

Special thanks to...
 - LMS and MulleDK: for implementing and maintaining support for the EmergencyLighting SDK in RagePluginHook. 
 - alexguirre: for siren settings research and RageNativeUI
 - cpast: for creating the SirenSetting Limit Adjuster
