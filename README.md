# FlaME

To use this program:

Download and extract the .zip release, then run the exe

or Download the source code and build in Visual Studio

If you have Suggestions please let me know so I can add it to this Readme!

# What's new

Built in 64 bit to remove the 2GB memory limit 

    This lets the program use more memory an make larger maps

    Maps up to 2048x2048 in size are possible with ~4GB of memory

    That's 1GB per Million Tiles!

This is a pretty small update, I'm still trying to figure out some of these TODOs

    Some of the TODOs might not be nessesary anymore but would still be nice to keep memory usage low

# Known Issues

Droids made with "Convert to Design" fail to appear in game

	I'm not sure where this bug occurs, if it's when compiling or when the game loads the map

Most Levels -- Failed to set label ____ on unit with Id ____

	Labels can't be added to building upgrades yet, Important

Many Levels -- Radius/Subscriber label property is only partially supported

cam1 sub1-1 -- Failed to find Unit with Id 160831 when creating Label: artifact 1

	This appears to only be used to try to remove an artifact that dosen't exist anymore, Ignore

cam1 sub1-5 -- Failed to find Unit with Id 486 when creating Label: NPRepairFaciliy

	the Repair Facility Id is 513, but this doesn't seem to be used anyway, Ignore

# TODO

allow labels to be added to modules

tileTypes.json implementation

modification of the tileset loading functions to be able to load from the Warzone2100/data/base directory

load only models requested buy objects

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

    I think water is already animated to a degree in code
    
    further support for these textures would allow for tiles like waterfalls or lakes on top of mountains

Multiple Objects per tile

    Allow overlapping features for more detailed maps

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
