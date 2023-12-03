# FlaME

To use this program:

Download and extract the .zip release, then run the exe

or Download the source code and build in Visual Studio

If you have Suggestions please let me know so I can add it to this Readme!

# What's new

Implemented Support for TileTypes.json

    If TileTypes.json is found by Flame it will load it instead of .ttp files

    .ttp files can still be loaded after initialization.

    Json can not be loaded after initialization

Enabled erasing Labels from Objects

Enabled Support for adding Labels to Modules

# Known Issues

No Known Issues!

# TODO

add UI elements to allow editing of Radius Labels and label Subscriber properties

Support for transparent textures

modification of the tileset loading functions to be able to load from the Warzone2100/data/base directory

    Base game uses transparent textures for even classic terrain, Transparency needs support first

load only models requested by objects

load only textures requested by models

	without these two repathing objectData causes OutOfMemory exception

Once other todos are completed, repath objectData to be able to load from the Warzone2100/data/base directory

# Suggested

Support for multiple oil level variants on one map

    I'll have to see how the game currently handles diffrent oil levels and make Flame's output match

Markers

    An object or label that can be placed on the map that isn't saved when compiling a map

    Adding these and a hotkey to snap the camera to the next one would be useful for marking POIs like bases

Script Generator for Campaign Scripts

    Scripting is already very easy and creating a generator will have no hope of covering every possible

    script one might want to generate but it might be able to make certain large functions/arguments in

    about 4 clicks instead of typing 4 lines of code

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

    I think water is already animated to a degree in code
    
    further support for these textures would allow for tiles like waterfalls

Multiple Objects per tile

    This is already possible but Game does not support this, Flame does

Off Grid Objects
    
    More detail could be added to maps if Objects were able to be placed on a sub-tile level

Multiple Tilesets per map

    This can currently be done by manually making a tileset that includes all the tiles from the tilesets used

# Trouble shooting

Errors and Exceptions from FlaMe:

      I try my best to keep the error log clean aside from normal and informative error messages.

      If you can't understand why a certain error is happening please screen shot the error message and

      provide the files Flame is loading

      this includes the folders with the tilesets and object data as well as the map file

      When sending maps make sure you send the folder with the map data as well if applicable to the map format

      ( For example, when loading cam1a.gam, files are loaded from a folder called cam1a located in the same folder

      as cam1a.gam)

      I can be found on Discord as Mindstormsman
