# FlaME

To use this program:

Download and extract the .zip release, then run the exe

or Download the source code and build in Visual Studio

If you have Suggestions please let me know so I can add it to this Readme!

# What's new

Added Tile Border View Mode

    Toggled with F9

Added settings for color and line thicknesses of the Wireframe and Border View Modes

Added Keybind to create a new Radius Label

    Left Click + R

The move camera slow keybind was moved from R to V

Generator Max Map Size now uses the WZMaxMapSize Global variable rather than being hardcoded to 250

    Generator Minimum Map Size is still hardcoded to 48
    
    and bases must still be atleast 12 tile from the edge of the map

    let me know if these are values you'd like to see changed

# Known Issues

Crash with Win+Shift+S screenshot tool

    With Move Camera Fast bound to shift, attempting to use this screenshot method can on occasion
    
    cause the camera to move down in a way that causes a rendering error and crashes Flame

Droids made with "Convert to Design" fail to appear in game

	I'm not sure where this bug occurs, if it's when compiling or when the game loads the map

# TODO

Figure out what's wrong with Footprint Rotation

Draw a Radius around Radius Type Labels on the map

    Currently they just show their origin point

Fix/Add support for some Map Formats

    Flame is still reliant on a .lev file and a .gam file inside of .wz maps

    and Java Script based maps need to be given a seperate error message as
    
    they are unlikely to ever be supported by Flame

Support for transparent textures

    Warzone uses some transparent textures, even in the Classic tilesets

modification of the tileset loading functions to be able to load from the Warzone2100/data/base directory

load only models requested buy objects

load only textures requested by models

	without these two repathing objectData causes OutOfMemory exception

Once other todos are completed, repath objectData to be able to load from the Warzone2100/data/base directory

# Suggested

Improvements to Generator Functions

    Currently the Generator tools can only be used on maps created by the generator and cannot be used

    on loaded maps (even if they were made with the generator) or for detailing hand scuplted maps

Support for multiple oil level variants on one map

    I'll have to see how the game currently handles diffrent oil levels and make Flame's output match

Markers

    An object or label that can be placed on the map that isn't saved when compiling a map

    Adding these and a hotkey to snap the camera to the next one would be useful for marking POIs like bases

Script Generator for Campaign Scripts

    Scripting is already very easy and creating a generator will have no hope of covering every possible

    script one might want to generate but it might be able to make certain large functions/arguments in

    about 4 clicks instead of typing 4 lines of code

Random Object Brush

	A brush that places one of the selected Objects at random with a specified density

 	Good for Rocks Trees and Buildings

# "Chicken and Egg" Problems

These are Suggestions that would require support from something else before I can start adding support to Flame

Bridges

    ...

Signs

    These would be placed in the editor and displayed in game as text, useful for signatures

    These can probably hyjack the game's debug label view, but not require debug mode

Gateway Types

    These would require an AI that knows how to use them, useful to make AI completely wall off areas and such

Animated Tiles

    I think water is already animated to a degree in code by the game
    
    further support for these textures would allow for tiles like waterfalls or lakes on top of mountains

Multiple Objects per tile

    Allow overlapping features for more detailed maps

    this feature is turned off behind the scenes in Flame because it isn't supported by the game

Off Grid Objects
    
    More detail could be added to maps if Objects were able to be placed on a sub-tile level

# Trouble shooting

Windows Defender Wacatac.B!ml False Positive:

      Sometimes Windows Defender seems to think the executables in this repo are Trojan Horse Viruses of a specific
      
      variety that commonly hides in "Homebrew" and "Cracked" video games

      Don't run anything you don't think is safe and if you don't know if it's safe you can use a number
      
      of online tools such as VirusTotal.com to check

Errors and Exceptions from FlaMe:

      I try my best to keep the error log clean aside from normal and informative error messages.

      If you can't understand why a certain error is happening please screen shot the error message and

      provide the files Flame is loading

      this includes the folders with the tilesets and object data as well as the map file

      When sending maps make sure you send the folder with the map data as well if applicable to the map format

      ( For example, when loading cam1a.gam, files are loaded from a folder called cam1a located in the same folder

      as cam1a.gam)

      I can be found on Discord as Mindstormsman
