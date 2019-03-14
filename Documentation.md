# ECS160 Windows Documentation
**ECS 160 Project Windows Version**  

Executable is in */Splash/Demo/bin/Debug/demo.exe*  
Test environment(s): *Windows 10* and *Windows 8.1* and *Windows 7 SP1*  

## External Libraries
XNA - Graphics rendering  
IronPython v2.7.9 - For running python scripts  
Roy AStar- For path finding  

## Files
*NOTE: Files usually correspond with class names*  
*Documentation for what classes each file contains with a general description*  

#### AIPlayer.cs

#### AssetDecoratedMap.cs
*AssetDecoratedMap* - class pretty much acting as the game model, mostly interacts with map and hold its data  

#### BasicAI.cs
*BasicAI* - class object that is a controller for AI players (basic hard coded algorithm)  

#### BuildCapabilities.cs 
*BuildCapabilities* - class that servers as the builder for units/buildings (build mode and registering units)  

#### CGraphicTileset.cs 
*CGraphicTileset* - class used to load *almost all* tilesets  

#### inGame\_menu.cs
*inGame_menu* - Code for in game UI.

#### inGameButtons.cs 
*inGameButtons* - Code for in game UI buttons.

#### MainMenu.cs 
*MainMenu : Form* - Class that shows the main menu displaying the buttons to navigate to singleplayer, multiplayer, and options when clicked.

#### MapList.cs 
*MapList : Form* - After selecting singleplayer or host multiplayer game, this class will display the screen that shows a list of maps in the maps folder and displays the mini-map preview for them when selected. Contains the start game button which will bring up the player select menu, and cancel will go back to the main menu.

#### NetworkOptions.cs 
*NetworkOptions : Form* - Class that shows the input for boxes for username, server name, port, and password. the ok button will save the values in a structure.  

#### Options.cs 
*Options* - Code for UI of options menu with buttons leading to sound menu.  

#### PlayerAsset.cs 
*AssetData* - class holding unit/building data loaded in from .dat files in res folder  
*PlayerAsset* - class used to load player assets (data and tilesets)  
*UpgradeData* - class  used to store upgrade data  

#### playerSelectMenu.cs
*playerSelectMenu* - Code for UI for player color selection for each player.  

#### Program.cs 
*Program* - the Main function, basically the program starts here  

#### SoundOptionsMenu.cs 
*SoundOptionsMenu* - Code for UI of sound options, allows for adjusting volume.  

#### SplashScreen.cs 
*SplashScreen : Form* - class that loads the splash screen and also calls init functions to load data into memory such as assets, buttons, map data, tilesets, and other data.

#### Unit.cs
*Unit* - class for unit/building creation event in addition to hold its data   

#### XNA\_InGame.cs
*XNA_InGame* - main class for *all* gameplay elements  


## Functions

*Most documentation for what functions each class contains with a general description is located in the code itself*
