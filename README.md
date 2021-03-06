# Live Lights: In-game siren settings editor for GTA V
## Created by PNWParksFan

This is a RagePluginHook plugin for single-player GTA V. 
It enables users to view and modify all siren setting 
parameters live in game and import/export carcols.meta files.

You must have RagePluginHook version 78 or later.

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

[![Latest Version](https://img.shields.io/github/release/pnwparksfan/rph-live-lights?include_prereleases)](https://github.com/pnwparksfan/rph-live-lights/releases)  
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
 - Any changes you make in the Edit Siren Settings menu will immediately be applied to all spawned vehicles which have been set to use the clonsed siren setting.
   - Within the main menu you can change all settings which apply to the overall siren setting entry (e.g. BPM, falloff settings, etc.)
   - There are submenus to edit settings for each individual siren (1-20). The siren submenus siren-specific settings, and have further submenus for corona, flashiness, and rotation settings. 
   - There are separate submenus for headlight and taillight settings. 
   - There is a sequence quick-edit menu which allows you to change the flashiness sequence for all sirens, plus headlights and taillights, from a single menu without having to switch between siren submenus. 
 - When you are finished configuring your siren setting and are satisfied with the results, you can click the Export item on the main Edit Siren Settings menu. 
   - You will be prompted for a file location. If you just enter a filename, the setting entry currently being edited will be saved to `GTA V\Plugins\LiveLights\carcols\<selected_filename>`. If you enter a path relative to the GTA root folder, your file will be saved to that path, e.g. `LML\police-pack\data\new-carcols.meta`. You can also enter an absolute path and the file will be saved to that exact location even if it is not within the GTA V folder, e.g. `C:\GTA V\mods\police\carcols-2.meta`. 
   - Exported files will always contain siren ID `0` by default. You will need to edit the siren ID to the value you wish to use in your carvariations.meta. See info below regarding important notes on siren setting ID limitations. 
   - The exported file will be a fully valid carcols.meta file (except for the ID), but you may want to copy the specific `<Siren>` entry out of the exported file and add it to another file with multiple entries. 
 - All changes will be lost when you exit the game. Make sure to export any savings you want to keep, and add those exported savings to a carcols.meta file which will get loaded by the game. 


&nbsp;

# Siren Setting ID limit and registry

As of GTA V build 1868, there is a hard-coded limit of 255 siren setting IDs 
in the game. Siren setting IDs over 255 can be entered in carcols.meta, but 
will overflow and be converted to a number between 0-255. You can calculate 
the "real" siren ID from an ID over 255 using the modulo operator 
(`MOD(id, 256)` or `id % 256`) in any programming language or spreadsheet 
software. For example, `MOD(1355, 256) = 75`, so if your siren setting is 
saved as ID 1355, it will actually be interpreted by the game as 75, and will 
conflict with any other siren IDs which are also interpreted as 75 
(331, 587, 1099...). All carcols.meta entries should use an ID between 0-255.

To avoid conflicts between mods, I have started a public tracker/registry of 
siren IDs. Feel free to list your own mods in this tracker. If you already 
use your own tracker or are in a modding group which uses a shared tracker, 
you can PM me and I can set up the public tracker to include info from 
your tracker. 

## [GTA V Siren Setting ID Registry](https://docs.google.com/spreadsheets/d/1MG2BDdboYbfAGGIG3HluLg34Ne3K7kN4FXUtTc4ebtw/edit?usp=sharing)
