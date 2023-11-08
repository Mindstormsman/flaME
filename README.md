# FlaME

To use this program:

Download and extract the .zip release, then run the exe

or Download the source code and build in Visual Studio

# What's new

Partial support for "radius" label types and "subscriber" label properties

	They cannot be edited or viewed in FlaMe but the are kept when compiling

	Their warning messages have been updated to reflect this

PIE 4 support has been added

Fixed labels not being added to Cyborg Factories (and emplacements?)

Fixed a long standing tile type bug, FlaMe listed all "rubble" as "baked earth" and vice versa

# Known Issues

Most Levels -- Failed to set label ____ on unit with Id ____

	Labels can't be added to building upgrades yet, Important

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
