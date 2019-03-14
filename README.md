# ECS160Windows

**ECS 160 Project Windows Version w9**

Executable is in */Splash/Demo/bin/Debug/demo.exe*  
Have tested in *windows 10* and *windows 8.1*.  

-----
## Group Note

It's the final week~ Browse through sections towards bottom for TODOs.  

If you fix or implemented something, please do delete it from the list (or mention it so someone else can delete).     
If you get stuck on your assigned part for this final week, please do add what you didn't do to the TODO list so others may consider working on it.   

*NOTE*: Although optimal conclusion is to fix everything, that may not be timely possible, so do not worry about adding more to the list (as we won't finish all probably).  

And finally~ We got dis! Go! We're almost there! Then, we'll be free~ (free to get crushed by work piled up in other classes)  

-----

## Progress

**week1 current progress:** There is a splash screen with fade effect, leads to dummy menu screen.

**week2 current progress:** Block chunk map rendering, mp3 and wav sound file playing, cursor recognition

**week3 current progress:** Loaded terrain map, panning, mouse click ingame, music bg

**week4 current progress:** Loaded assets, single asset selection and movement, much of menu UI, much of music

**week5 current progress:** Animations, multiple unit movement/selection, buggy unit collision, some bugs gold mining and lumbering (with no tile updates), auto collection after setting action, some action interrupt  

**week6 current progress:** ingame menu and peasant buttons, basic player select menu, basic options menu, network options menu, sound options, ingame resource display update, functioning building construction, correct unit/building selecting plus selection box, map updates on tree removal  

**week 7 current progress:** Label shows max food, food units are corrected, can't build on other units (pseudo), unit/building/upgrade buttons, player color selection (with color changing units), all 4 units creation, pseudo fog of war, unit upgrades, keep/castle building, selected unit data showing when selected, basic multiplayer menu layout, in-game top-left menu button              

**week 8 current progress:** Basic melee battle (against units & buildings), buggy archer/ranger battle (with buggy arrow), guardtower/cannontower battle (buggy animation), multiplayer menu (in-depth), multi-unit selection data display (clearing graphics error), path finding          

**week 9 current progress:** Auto-path/cut trees, basic patrolling, assets on menu minimap, basic building repair, allow decision to attack friendly units/buildings, auto-attack back for units, corrected in-game menu, progress label upgrades/unit creation, chatbox with some cheat codes, peasant drops resource upon death, unit grouping, basic hard coded AI                  

-----

## Current Assignments 

(Can delete after done/Can add to if you want to do more stuff):  

 * **Alan** - Fog of war rendering on minimap, fix bugs, optimization if have time (only if most bugs are fixed, so probably not)        

 * **Cathy** - Sounds, help finish menus  
 
 * **Jiahui** - Finish menus, add units (gold mine) display in main menu map display, help with sounds, chat box if have time   

 * **Lion & Chris** - Find and fix bugs, help document code, look into linking multiplayer input boxes to variables if have time      

 * **Steven** - Fix bugs, attempt to fix battle bugs if possible   

 * **Vinh** - Write game manual /document code, AI dll (work with AI group for help?)  
 
 * **Everyone** - Help Document code if have time  

-----

### URGENT

 * URGENT: Code needs to be cleaned of unnecessary comments (aka old commented out codes).
 * URGENT: We still have unused files that still need to be deleted.  
 * URGENT: Code needs to be split into many functions/files still. Code is slowly piling up in files.
 * SUPER URGENT: Code function names need to be edited to match original! Many functions already have them, but some don't match at all or is similar so hard to match. (If we can't do this, then so be it)  
 * SUPER URGENT: Help with code documentation! Functions, classes, files, etc. all needs your lovely mouth to describe! (Or we can try DOxygening everything at end as worst case scenario...)   

-----
### TODO - Game BREAKING Bugs! Fix NEEDED!

 * Multiple access to same file may crash game (rare case but does happen). Look into where sound file are (or Unit.cs).  
 * Game crashes (with access violation) as soon as WMPLib background music finishes playing (usually around 3-5 minutes into game).  
 * Menu crashes application (1-3 minutes in of only staying in menu). No idea what is causing this as everytime this happens, VS points to the entire application having access violation.  
 * Game sometimes crashes on boot up due to inability to access files when loading assets, etc. We might need error checking and throw errors (like in original Linux code).  
 * exit code -1073610751 (0xc0020001) (google it with visual studio C# for error)  

-----

### TODO - Major Fix NEEDED:  

 * Map data need to be localized (currently is by reference). Look into AssetDecoratedMap.cs  
 * Timing on buildings construction and pretty much all timers (including animation as original game animation ticks is very fast) need to be fixed. (Unsure of how seconds relate to timer ticks in C Sharp). Timer can be edited by editing DConstructionTime variable in AssetDecoratedMap.cs.    
 * Archer range may be longer than expected when facing certain directions (sometimes).  
 * Sometimes if unit does not reach (xToGo,yToGo), the path finding algorithm doesn't stop running so it will still show the walking animation (when it's supposed to be off). Example: When clicking on non-traversible terrain, this happens (probably need a way to either ignore click or set the move to coordinate to edge of terrain).  

-----

### TODO - Minor Fix NEEDED:  

 * Stumps are not showing, instead it shows light grass (can't regrow trees without this). 
 * Although the minimap viewport box moves on click, it seems that it's not centered on the mouse position when it moves (for example, clicking to go to a unit on minimap will bring us close, but shifted to some side).  
 * Fog of war can't display half of buildings properly when they are half covered in fog of war.  
 * Game start position isn't centered around first unit (but on corner instead).  
 * Not all main menu buttons position/resize correctly when resize window.  
 * MapList listbox flickers like crazy when selecting different map.  
 * Unit direction auto set to south east always (when done moving or during battle, etc.). Need to fix so that it faces correct direction (and not reset to south east).  
 * InGame 'small' menu is no longer centered (due to resized game menu). It should re-center itself depending on form size (or simply cover the entire form including buttons section to make it look nice).  
 * Unit collision is fairly good (can walk through units, but not onto same spot), but can be improved.  

-----

### TODO - Current Implementation NEEDED:

 * AI Implementation (either as code or DLL)! Probably need to look into running python scripts in C Sharp.  

-----
### TODO - Minor Lag Issues

 * Lowering memory usage and loading time for player colors.  

 * Spikes in memory usage (yellow tabs in VS), may be due to minimap update or background timers on units.  

 * Need way to free up resource after we close a map. Look into XNA InGame UnLoad function or Google it.  

 * Faster update/refresh of screen = more lag? Is there any way to fix this to make as smooth as original? Major goal, but can be pushed off towards end (unless can fix quickly).  

 * We can't delete units (this might lead to an error due to race condition when we delete unit while we are accessin it). We need some cleanup function that can be called outside of our accesses (multiple threads cause problems ya know~)  

-----


### TODO - Extra NEEDED: 

*List of needs, but unnecessary if we don't have time:*    

 * Fullscreen mode for our in game menu (if it is actually needed).  
 * Nice pretty text that looks closer to original for in game text and button texts.   
 * Look into multiplayer implementation into our game. Implementing data sending and receiving maybe? 
 * Many functions have LOTS of passed in parameters (with similar parameters in most functions). Need a clean way of storing those parameters in one variable/class.  
 * Actual form controls for multiplayer menu (controls that can do stuff like take in and store input, list that shows all servers, etc.).   
 * Implement Unit healing over time. Not quite sure about the timer on this (as this might break battle if heals too fast).  
 * Right clicking on minimap to move units need to be implemented.  
 * When switching to different modes (through button click), the cursour should change to eagle symbol (and back when mode is canceled). Also, eagle changes color when hovering over correct target (relative to mode).  
 * Animation for building death may be a bit off (time wise or maybe animation graphics is skewed).  
 * It looks weird for peasant to be walking above edges of trees (is there a way to hide half the peasant?).  
 * It would look nice if we are able to display partial tiles of buildings hidden by fog of war (not sure if this was already done, it might be).  
 * A nice border for unitdata label box and buttons box.  
 * Buildings need to catch on fire when damaged! Load fire, spawn the fire, animate the fire, and delete the fire all need to be done (Not on our list, but was in original).  
 * FindNearestTree algorithm (in AssetDecoratedMap.cs) needs to be upgraded to actually find nearest tree (relative to unit position).  
 * Add click effect (the 'X' that pops up when clicking for movement)  
 * Make projectile animations more smooth.  
 * Archer/Ranger may dance around in battle.
 * Chatbox (input box) centered on bottom on game (map) screen and is longer (width). 

-----

### Extra Tasks Buffer List

If you want to do an extra task, copy it from above and paste it down here with your name so that no one fights you for it~    

 * JD: An in-game chat box of some sort that can read user input, outputing input to screen would be nice too. Not sure whether I will merge Alan's cheat code prompt. Need to get units to show up on minimap in map selection menu screen.  

 * Cathy: Sound still need to be implemented in almost all units and ingame events. Can look into Unit.cs or write a separate file. In game menu resource display image for food, gold, lumber.  

-----

NOTE: Merge has kind of failed ... my commits bloated to 400k+ which doesn't make sense (Only about 1,000 lines at most changed).  
NOTE: Apologies for bloated files...  
NOTE: Look into TODO sections above for extra tasks you can do~  
NOTE: week6 v06 version seems to have overhead/slowdowns on large maps compared to v05. *FIXED*   

-----
