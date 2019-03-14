using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Forms; //for testing only

/*
 *  To Create a Unit based on default map data:
 *  Access public List<Asset>[][] mapAssets; with:
 *     SplashScreen.mapUnits.mapAssets[mapChoice];
 * 
 *  Then, iterate through list with:
 *      for (int i = 0; i < SplashScreen.numPlayers[mapChoice]; i++) //iterate through all players
 *      {
 *          foreach(Asset u in SplashScreen.mapUnits.mapAssets[mapChoice][i])
 *          {
 *              Unit peasant1 = new Unit("Peasant",u.x,u.y,i);
 *              //Code to add object image to map location
 *              AddUnit(peasant1.objectImage,peasant1.x,peasant1.y);
 *          }
 *      {
 *      
 * 
 * 
 */

namespace Demo
{
    public class Unit //not based off of Professor's code ... couldn't really understand his code yet
    { //class for creating units (not all buildings yet -definitely not walls)
        public int unitType; //start with none, unit type
        private int direction; //8 directions (units): N, NE, E, SE, S, SW, W, NW ; Default as S
        private int currentFrame = 0; //frame of animation (for building, it is frame for building constructions)
        private int currentAction = 0; //0 for walk, 1 for attack, 2 for gold, 3 for lumber, 4 for death
        private int possibleAnimation;
        public int unitWidth; //unit image width/height
        public int unitHeight;
        public int unitTileWidth =1;
        public int unitTileHeight = 1;
        public float x; //can access a unit's position for rendering and updating anytime
        public float y;

        //Holds previous x y position for unit collision blocking
        public int prevX;
        public int prevY; 

        public int id; //to hold the id or index of this unit in its player array

        public bool onCommand = false;//true if the unit is following some command, not idle
        public int xToGo;//The x it wants to go.
        public int yToGo;//The target y it wants to go.

        //Patrolling variables
        public bool patrolling = false;
        public int patrolX1;
        public int patrolY1;
        public int patrolX2;
        public int patrolY2;

        public int xNextStop=-1;// The next nearby x it wants to go, used by path finding algorithm.
        public int yNextStop=-1;//THe next nearby y it wants to go, used by path finding algorithm
        public float speed = 1;//

        public int owner; //owner of this object
        public Bitmap objectImage;
        public Microsoft.Xna.Framework.Graphics.Texture2D objImg2D;
        public Microsoft.Xna.Framework.Graphics.Texture2D objPreviewImg2D;
        //might need more variables here for number of resources held personally, etc. maybe? and select flag?

        //variable for resources
        public int lumber;
        public int gold;
        //public string prepAction; //to store action while walking there (unless there's another way to do it)
        public List<unitPtr> garrison; //list to units that are garrisoned inside building (such as GoldMine, etc.)
        private List<int> timer; //internal timer for each unit garrisoned (to determine when to kick them out)
        public int invisible; //invisibility flag that can be used for hiding unit? default 0 for not invisible
        public int tintGrey = 0; //whether to tint grey or not (tint grey if explored but outside of current view)

        //to hold target index and owner for usage once reach destination (can only have single target!)
        public unitPtr target; //only units (not buildings) should have targets?
        public unitPtr prevTarget; //used to save previous target to go back to (will not be used if interrupted)

        //holds building data
        public int sightConstruction = 0;
        public int buildTime = 0;
        public int foodConsumption = 0;
        public int usable; //usable flag, building only usable if finished building (by default it is usable)
        public string creating = null; //creating unit flag
        public int defaultUnitTileW = 1;
        public int defaultUnitTileH = 1;
        private double hpInc = 0;
        public bool repairing = false;
        public bool constructing = false; //flag for if peasant is constructing or not
        public bool goldMining = false; //flag for invisiblity for gold mining

        //upgrade flag
        int upgradeIndex = -1; //-1 for none
        public int inUse = 0; //in use flaga (for button show)
        int maxUpgradeTime = 0;
        //string upgradeTo = null; //building to upgrade to

        //to hold unit minimap data (so we don't consistently redeclare!)
        public Microsoft.Xna.Framework.Graphics.Texture2D uRec;
        //public Color[] uCol; // Unit Color


        //Unit battle Data here
        public int curHitPoint; //current hitpoint unit has (doesn't update)
        private double tempHitPoint = -1; //temporary hitpoint for calculating repairs/float increments, start with -1 for null
        public int hitPoint = 1; //max hitpoint unit has (do not edit unless upgraded)
        public int armor = 0;
        public int sight = 0;
        int attackSteps = 0;
        int reloadSteps = 0;
        public int basicDamage = 0;
        public int pierceDamage = 0;
        public int range = 0;
        public int defaultSpeed = 1; //default speed, not the float calculated speed
        public bool battling = false; //flag for if already in battle
        public bool chasing = false; //flag to determine if we are chasing the unit
        private bool decaying = false; //for decaying animation
        private int reloading = 0; //value 0 = done reloading (will be reset to reload time after attack)
        private int projectileImgDir = 0; //a nonchanging direction after it is set for arrow image
        public bool startedBattle = false;
        public bool selfDestroy = false; //flag for keeping track of if the unit can kill friendly unit
        public bool playsoundflag = false; //flag for keeping track of the playsound, should only play once
        public bool meleeSearchTarget = false; //used to set flag for range for melee searching target

        //public int buttonShowed = 0; //flag so that we don't reshow button menu everytime we reselect this unit (default no show as no menu yet)

        public Unit(string type, int x, int y, int owner, int id, ref tempMap gameData) //pass in unit name as string, and location to spawn unit
        {
            //this.uCol = new Color[1]; //init color (not sure why we need to?)
            //this.uCol[0] = Color.Yellow; //set default color to yellow, can be passed in later as variable

            this.x = x;
            this.y = y;
            this.owner = owner;
            this.unitType = nameTranslation(type);
            this.invisible = 0;
            this.id = id;
            this.usable = 1; //default building is usable (will be set otherewise during construction)
            this.timer = new List<int>(); //all units have internal timer (some may not use it)
            this.target = new unitPtr(); // building can have target but no need for previous (not all building need)
            if (this.unitType < SplashScreen.numUnits && this.unitType >= 0) //aka a unit
            {
                // this.direction = 4; //set default direction to south
                this.direction = 0;
                this.lumber = 0;
                this.gold = 0;
                this.prevTarget = new unitPtr();
                
                this.prevX = (int)this.x;
                this.prevY = (int)this.y;
                this.xToGo = (int)this.x;
                this.yToGo = (int)this.y;
                updateImg();
            }
            else if (this.unitType >= SplashScreen.numUnits)
            {   //AKA a building
                this.unitTileWidth = (int)Math.Round(1.0f * this.unitWidth / SplashScreen.tileObject.tileWidth);
                this.unitTileHeight = (int)Math.Round(1.0f * this.unitHeight / SplashScreen.tileObject.tileHeight);
                //this.direction = 0; //set default direction to start construction of building (don't really need)
                this.garrison = new List<unitPtr>(); //all buildings have garrison (needed at least for construction time)
                findInactive();
                if (this.unitType == nameTranslation("GoldMine")) //goldmine init with nature start gold
                {
                    this.lumber = 0;
                    this.gold = SplashScreen.mapUnits.startGold[SplashScreen.mapObject.mapChoice][0]; //not sure how to clone if it is even needed
                    
                    //only init garrison for buildings that have it (not sure if GM needs it)
                    int num = SplashScreen.mapUnits.numPlayer[SplashScreen.mapObject.mapChoice];


                    /*for (int i = 0; i < num; i++) //NOTE:index 0 = player 1 (no nature can garrison)
                    {//we might not even need to init it for each player if the GoldMine changes owner and is locked upon first visit
                        this.garrison[i] = new List<int>();
                    }*/
                }

                updateImg();
            }

            if (this.unitType > -1) //a true unit
            {
                this.unitWidth = SplashScreen.mapUnits.allUnitTileW[this.unitType];
                this.unitHeight = SplashScreen.mapUnits.allUnitTileH[this.unitType];
                initLoadedData(ref gameData); //initiate loaded unit data 
                this.curHitPoint = this.hitPoint; //set current hp to max hp (we need a copy here, not sure if this works)

                //set possibleanimation to number of tiles
                this.possibleAnimation = SplashScreen.mapUnits.allUnitTiles[this.unitType].Length;

                for (int ix = (int)this.x; ix < (int)this.x + this.defaultUnitTileW; ix++) //init block for unit
                {
                    for (int iy = (int)this.y; iy < (int)this.y + this.defaultUnitTileH; iy++)
                    {
                        gameData.pathFindingGriod.BlockCell(new RoyT.AStar.Position(ix, iy));
                        /*if (u.isGoldMine()) //no goldmine special case
                        {
                            thisMapPathGrid.UnblockCell(new Position((int)u.x + u.unitTileWidth - 1, (int)u.y + u.unitTileHeight - 1));
                        }*/
                    }
                }

                int maxBuildTime = this.buildTime * gameData.DConstructTime; //get max build time
                if (this.hitPoint <= maxBuildTime) this.hpInc = (double)this.hitPoint / (double)maxBuildTime; //still not correct (building finishes building beforre finish increment)
                else this.hpInc = (double)maxBuildTime / (double)this.hitPoint;
            }
            else if (this.unitType < -1 && this.unitType > -4) //a projectile
            {
                this.curHitPoint = 1;
                this.hitPoint = 1;
                this.defaultUnitTileH = 1;
                this.defaultUnitTileW = 1;
                this.prevTarget = new unitPtr();
                //this.direction = -1; //init to -1 so that setdirection runs once
            }
            else //dropped resource
            {
                this.chasing = false;
                this.curHitPoint = 1;
                this.hitPoint = 1;
                this.defaultUnitTileH = 1;
                this.defaultUnitTileW = 1;
                this.currentFrame = 0;
                this.setAction("walk");
                gameData.pathFindingGriod.BlockCell(new RoyT.AStar.Position((int)this.x, (int)this.y)); //block the cell
            }


        }

        public bool isBuilding() {
            return this.unitType >= SplashScreen.numUnits;
        }
        public bool isGoldMine() {
            return this.unitType == nameTranslation("GoldMine");
        }

        public void pickedUp(ref tempMap gameData)
        { //call this function with resource unit when we pick up resouce
            if (this.unitType < -3) //a true resouce drop
            {
                this.invisible = 1;
                gameData.pathFindingGriod.UnblockCell(new RoyT.AStar.Position((int)this.x, (int)this.y));

            }
        }

        private bool checkGoToSpot(List<Unit>[] allunits, tempMap gameData, int x, int y)
        { //check if another unit is already going to a certain spot
            int numPlayers = gameData.numPlayers;
            for (int i = 1; i <= numPlayers; i++)
            {
                foreach(Unit u in allunits[i])
                {
                    if (u.xToGo == x && u.yToGo == y) return false; //return false as another unit already going to that spot
                }
            }

            return true; //true for open spot by default
        }
        private bool checkOpenTarget(List<Unit>[] allUnits, ref tempMap gameData, int x, int y)
        { //helper function to check if target space is open
           
            unitPtr temp = gameData.target(allUnits, x, y, this, 0);
            if (temp.owner == -1 && gameData.isTraversible(x,y) && checkGoToSpot(allUnits, gameData, x ,y)) //we did not find a target, so spot is open, also we can actually walk on that spot
            {
                return true; //found spot
            }
            

            return false;
        }

        public void findNearestOpenSpot(List<Unit>[] allUnits, ref tempMap gameData)
        { //this should be called ONLY after setting a target
            //it will find the nearest open space of the target
            if ((this.target.owner == this.owner && this.target.index != this.id) || this.target.owner != this.owner) //only findnearestspot if the  target we are going to is not same player or if it's same player but not same id (aka not ourself)
            {
                if ((this.target.owner >= 0 && this.target.index >= 0) || (this.target.owner == 0 && this.target.index == -1)) //only calculate if there is a target
                {
                    Unit u;
                    if (this.target.index == -1 && this.target.owner == 0) //a tree target
                    {//so create a temporary unit to check direction with
                        u = new Unit("Peasant", this.target.x, this.target.y, 0, 0, ref gameData);
                    }
                    else u = allUnits[this.target.owner][this.target.index]; //a true unit
                    int otherW = u.defaultUnitTileW;
                    int otherH = u.defaultUnitTileH;
                    string defaultDir = this.getRelativeDir(u); //get the default relative direction (helps so we don't search infinitely for open spot)
                    int foundOpenSpot = 0; //flag for whether we found open spot or not, 0 for continue search, 1 for found, -1 for not found
                    //New toGo values to hold new location
                    //int newXToGo = this.xToGo;
                    //int newYToGo = this.yToGo;
                    int newXToGo = (int)u.x;
                    int newYToGo = (int)u.y;
                    string relDir = this.getRelativeDir(u); //get the relative direction of this unit's position relative to target building/unit
                    int relDirInt = dirTranslation(relDir); //relDir as an int for easier calculations
                    
                    while (foundOpenSpot == 0)
                    {
                        
                        switch (relDir)
                        {
                            case "S": //other unit is above of this unit
                                while (newYToGo >= u.y) newYToGo -= 1; //subtract until we hit border
                                if (checkOpenTarget(allUnits,ref gameData, newXToGo, newYToGo)) //we did not find a target, so spot is open
                                {
                                    foundOpenSpot = 1; //found spot
                                }
                                else if (newXToGo + 1 < u.x + otherW) //we have more open spots horizontally
                                {
                                    newXToGo += 1;
                                }
                                else
                                {//try to find new direction
                                    relDirInt += 1;
                                    if (relDirInt >= 8) relDirInt = 0; //8 directions total, if our index exceeds, than reset to start
                                    relDir = dirTranslation(relDirInt);
                                    if (relDir == defaultDir) foundOpenSpot = -1; //if our new direction is the same as our starting direction, means we are done searching all spots
                                    else
                                    {//reset so we can start search in new direction
                                        //newXToGo = this.xToGo;
                                        //newYToGo = this.yToGo; 
                                        newXToGo = (int)u.x;
                                        newYToGo = (int)u.y;
                                        
                                    }
                                }
                                break;
                            case "N": //other unit is below this unit
                                while (newYToGo < u.y + otherH) newYToGo += 1; //subtract until we hit border
                                if (checkOpenTarget(allUnits, ref gameData, newXToGo, newYToGo)) //we did not find a target, so spot is open
                                {
                                    foundOpenSpot = 1; //found spot
                                }
                                else if (newXToGo + 1 < u.x + otherW) //we have more open spots horizontally
                                {
                                    newXToGo += 1;
                                }
                                else
                                {//try to find new direction
                                    relDirInt += 1;
                                    if (relDirInt >= 8) relDirInt = 0; //8 directions total, if our index exceeds, than reset to start
                                    relDir = dirTranslation(relDirInt);
                                    if (relDir == defaultDir) foundOpenSpot = -1; //if our new direction is the same as our starting direction, means we are done searching all spots
                                    else
                                    {//reset so we can start search in new direction
                                        //newXToGo = this.xToGo;
                                        //newYToGo = this.yToGo;
                                        newXToGo = (int)u.x;
                                        newYToGo = (int)u.y;
                                    }
                                }
                                break;
                            case "E": //other unit is left of this unit
                                while (newXToGo >= u.x) newXToGo -= 1; //subtract until we hit border
                                if (checkOpenTarget(allUnits, ref gameData, newXToGo, newYToGo)) //we did not find a target, so spot is open
                                {
                                    foundOpenSpot = 1; //found spot
                                }
                                else if (newYToGo + 1 < u.y + otherH) //we have more open spots horizontally
                                {
                                    newYToGo += 1;
                                }
                                else
                                {//try to find new direction
                                    relDirInt += 1;
                                    if (relDirInt >= 8) relDirInt = 0; //8 directions total, if our index exceeds, than reset to start
                                    relDir = dirTranslation(relDirInt);
                                    if (relDir == defaultDir) foundOpenSpot = -1; //if our new direction is the same as our starting direction, means we are done searching all spots
                                    else
                                    {//reset so we can start search in new direction
                                        //newXToGo = this.xToGo;
                                        //newXToGo = (int)u.x;
                                        //newYToGo = this.yToGo;
                                        newXToGo = (int)u.x;
                                        newYToGo = (int)u.y;
                                    }
                                }
                                break;
                            case "W": //other unit is right of this unit
                                while (newXToGo < u.x + otherW) newXToGo += 1; //subtract until we hit border
                                if (checkOpenTarget(allUnits, ref gameData, newXToGo, newYToGo)) //we did not find a target, so spot is open
                                {
                                    foundOpenSpot = 1; //found spot
                                }
                                else if (newYToGo + 1 < u.y + otherH) //we have more open spots horizontally
                                {
                                    newYToGo += 1;
                                }
                                else
                                {//try to find new direction
                                    relDirInt += 1;
                                    if (relDirInt >= 8) relDirInt = 0; //8 directions total, if our index exceeds, than reset to start
                                    relDir = dirTranslation(relDirInt);
                                    if (relDir == defaultDir) foundOpenSpot = -1; //if our new direction is the same as our starting direction, means we are done searching all spots
                                    else
                                    {//reset so we can start search in new direction
                                        //newXToGo = this.xToGo;
                                        //newYToGo = this.yToGo;
                                        newXToGo = (int)u.x;
                                        newYToGo = (int)u.y;
                                    }
                                }
                                break;
                            case "SE": //other unit is above to the left of this unit
                                while (newYToGo >= u.y) newYToGo -= 1; //subtract until we hit border
                                while (newXToGo >= u.x) newXToGo -= 1; //subtract until we hit border
                                if (checkOpenTarget(allUnits, ref gameData, newXToGo, newYToGo)) //we did not find a target, so spot is open
                                {
                                    foundOpenSpot = 1; //found spot
                                }
                                else
                                {//try to find new direction immediately as this is a corner spot
                                    relDirInt += 1;
                                    if (relDirInt >= 8) relDirInt = 0; //8 directions total, if our index exceeds, than reset to start
                                    relDir = dirTranslation(relDirInt);
                                    if (relDir == defaultDir) foundOpenSpot = -1; //if our new direction is the same as our starting direction, means we are done searching all spots
                                    else
                                    {//reset so we can start search in new direction
                                        //newXToGo = this.xToGo;
                                        //newYToGo = this.yToGo;
                                        newXToGo = (int)u.x;
                                        newYToGo = (int)u.y;
                                    }
                                }
                                break;
                            case "NE": //other unit is above to the left of this unit
                                while (newYToGo < u.y + otherH) newYToGo += 1; //subtract until we hit border
                                while (newXToGo >= u.x) newXToGo -= 1; //subtract until we hit border
                                if (checkOpenTarget(allUnits, ref gameData, newXToGo, newYToGo)) //we did not find a target, so spot is open
                                {
                                    foundOpenSpot = 1; //found spot
                                }
                                else
                                {//try to find new direction immediately as this is a corner spot
                                    relDirInt += 1;
                                    if (relDirInt >= 8) relDirInt = 0; //8 directions total, if our index exceeds, than reset to start
                                    relDir = dirTranslation(relDirInt);
                                    if (relDir == defaultDir) foundOpenSpot = -1; //if our new direction is the same as our starting direction, means we are done searching all spots
                                    else
                                    {//reset so we can start search in new direction
                                        //newXToGo = this.xToGo;
                                        //newYToGo = this.yToGo;
                                        newXToGo = (int)u.x;
                                        newYToGo = (int)u.y;
                                    }
                                }
                                break;
                            case "NW": //other unit is above to the left of this unit
                                while (newYToGo < u.y + otherH) newYToGo += 1; //subtract until we hit border
                                while (newXToGo < u.x + otherW) newXToGo += 1; //subtract until we hit border
                                if (checkOpenTarget(allUnits, ref gameData, newXToGo, newYToGo)) //we did not find a target, so spot is open
                                {
                                    foundOpenSpot = 1; //found spot
                                }
                                else
                                {//try to find new direction immediately as this is a corner spot
                                    relDirInt += 1;
                                    if (relDirInt >= 8) relDirInt = 0; //8 directions total, if our index exceeds, than reset to start
                                    relDir = dirTranslation(relDirInt);
                                    if (relDir == defaultDir) foundOpenSpot = -1; //if our new direction is the same as our starting direction, means we are done searching all spots
                                    else
                                    {//reset so we can start search in new direction
                                        //newXToGo = this.xToGo;
                                        //newYToGo = this.yToGo;
                                        newXToGo = (int)u.x;
                                        newYToGo = (int)u.y;
                                    }
                                }
                                break;
                            case "SW": //other unit is above to the left of this unit
                                while (newYToGo >= u.y) newYToGo -= 1; //subtract until we hit border
                                while (newXToGo < u.x + otherW) newXToGo += 1; //subtract until we hit border
                                if (checkOpenTarget(allUnits, ref gameData, newXToGo, newYToGo)) //we did not find a target, so spot is open
                                {
                                    foundOpenSpot = 1; //found spot
                                }
                                else
                                {//try to find new direction immediately as this is a corner spot
                                    relDirInt += 1;
                                    if (relDirInt >= 8) relDirInt = 0; //8 directions total, if our index exceeds, than reset to start
                                    relDir = dirTranslation(relDirInt);
                                    if (relDir == defaultDir) foundOpenSpot = -1; //if our new direction is the same as our starting direction, means we are done searching all spots
                                    else
                                    {//reset so we can start search in new direction
                                        //newXToGo = this.xToGo;
                                        //newYToGo = this.yToGo;
                                        newXToGo = (int)u.x;
                                        newYToGo = (int)u.y;
                                    }
                                }
                                break;
                        }
                    }

                    if (foundOpenSpot == 1) //we found a open spot
                    {//so set our toGo to that new spot
                        this.xToGo = newXToGo;
                        this.yToGo = newYToGo;
                    }
                    else //we didn't find open spot near target
                    {//so don't move, set (xToGo,yToGo) to current location
                        this.xToGo = (int)this.x;
                        this.yToGo = (int)this.y;
                    }

                }
            }

        }

        public void initProjectile(unitPtr target, float x, float y, int speed, ref List<Unit>[] allUnits, tempMap gameData, int dir)
        { //target is the target the arrow is shooting to
          //float x and y is the location of the archer position

            //Set arrow start to archer position
            if (this.unitType == nameTranslation("Cannonball"))
            {
                this.currentFrame = 0; //reset frame as well (as decay frame > regular frames)
                this.setAction("walk"); //reset action from decay to walk
                this.decaying = false;
            }
            this.prevTarget.reset();
            this.range = 1; //for nearenough checking
            this.x = x;
            this.y = y;
            int owner = target.owner;
            int index = target.index;
            this.target.owner = owner;
            this.target.index = index;
            Unit temp = allUnits[target.owner][target.index];
            //this.setDirection(this.getRelativeDir(temp));
            this.direction = dir;
            this.projectileImgDir = this.direction;
            updateImg();
            this.chasing = true; //set chasing to true
            this.speed = speed;
            this.invisible = 0; //set so projectile is not invisible
            this.timer.Add(1); //no delay on update because arrow is too fast
            this.onCommand = true;
        }

        private void projectileChase(ref List<Unit>[] allUnits, tempMap gameData)
        {
            //this.onCommand = false;
            this.chasing = false;
            if (this.target.owner >= 1 && this.target.index >= 0)
            {
                Unit temp = allUnits[this.target.owner][this.target.index];
                if (temp.curHitPoint <= 0) //target unit has died
                {
                    
                    this.timer.Clear();
                    this.target.reset();
                    this.chasing = false;
                    if (this.unitType == nameTranslation("Arrow"))
                    {
                        //this.decaying = true; //so that we no longer move
                        this.invisible = 1; //simply turn invisible arrow
                        //this.onCommand = false; //no longer move
                        playSound(gameData);  //arrow hit sound effect
                    }
                    else if (nearEnough(temp)) //projectile is actually near enough
                    {//need manual death animation for cannonball
                        this.decaying = true;
                        this.currentFrame = 0;
                        this.setAction("decay");
                        this.timer.Add(gameData.DDecayTime * gameData.DDecayTimer);
                    }
                    else  //projectile is cannonball but not near enough
                    { //simply turn invisible for now
                        this.invisible = 1;
                        playSound(gameData);  //cannon ball hit sound effect
                    }
                    this.onCommand = false;
                }
                else if (this.nearEnough(temp)) //if projectile is near enough to target unit 
                { //destroy projectile (as animation is done)
                    this.timer.Clear(); //clear all timers
                    this.target.reset();
                    this.chasing = false;
                    //this.xToGo = (int)temp.x;
                    //this.yToGo = (int)temp.y;

                    if (this.unitType == nameTranslation("Arrow"))
                    {
                        this.invisible = 1; //simply turn invisible arrow
                        playSound(gameData);  //arrow shooting sound
                    }
                    else //need manual death animation for cannonball
                    {
                        this.decaying = true;
                        this.currentFrame = 0;
                        this.setAction("decay");
                        this.timer.Add(gameData.DDecayTime * gameData.DDecayTimer);                        
                        playSound(gameData);  //cannon ball sound
                    }
                    this.onCommand = false; //no longer move
                }
                else
                { //we are too far so we have to move closer (so keep chasing
                    this.timer.Clear(); //clear all timers
                    this.chasing = true;
                    this.xToGo = (int)temp.x;
                    this.yToGo = (int)temp.y;
                    this.setDirection(this.getRelativeDir(temp)); //change arrow direction for movement
                    this.onCommand = true; //set to tru so projectile will chase again
                    this.timer.Add(1); //add an internal timer for when to update enemy unit location
                }
            }
            /*else
            {
                Unit temp = allUnits[this.prevTarget.owner][this.prevTarget.index];
                if (this.nearEnough(temp)) //if projectile is near enough to target unit 
                { //destroy projectile (as animation is done)
                    this.timer.Clear(); //clear all timers
                    this.target.reset();
                    this.chasing = false;
                    //this.xToGo = (int)temp.x;
                    //this.yToGo = (int)temp.y;

                    if (this.unitType == nameTranslation("Arrow"))
                    {
                        this.invisible = 1; //simply turn invisible arrow
                    }
                    else //need manual death animation for cannonball
                    {
                        this.decaying = true;
                        this.currentFrame = 0;
                        this.setAction("decay");
                        this.timer.Add(gameData.DDecayTime * gameData.DDecayTimer);
                    }
                    this.onCommand = false; //no longer move
                }
                else this.timer.Add(1);

            }*/
        }

        public void changeUnit(string type, ref tempMap gameData)
        {//converts unit from one type to another (essentially allows for upgrades)
            //we still keep most of data of unit (like owner, position, and id, etc.)
            //should not be called for projectiles
            this.unitType = nameTranslation(type); //change unit type
            this.unitWidth = SplashScreen.mapUnits.allUnitTileW[this.unitType];
            this.unitHeight = SplashScreen.mapUnits.allUnitTileH[this.unitType];
            this.invisible = 0;
            this.usable = 1; //default building is usable (will be set otherewise during construction)
            this.currentFrame = 0; //reset frame
            if (this.unitType < SplashScreen.numUnits) //aka a unit
            {
                // this.direction = 4; //set default direction to south
                this.direction = 0;
                this.lumber = 0;
                this.gold = 0;
                this.target = new unitPtr();
                this.prevTarget = new unitPtr();
                updateImg();
            }
            else
            {   //AKA a building
                this.unitTileWidth = (int)Math.Round(1.0f * this.unitWidth / SplashScreen.tileObject.tileWidth);
                this.unitTileHeight = (int)Math.Round(1.0f * this.unitHeight / SplashScreen.tileObject.tileHeight);
                //this.direction = 0; //set default direction to start construction of building (don't really need)
                this.garrison = new List<unitPtr>(); //all buildings have garrison (needed at least for construction time)
                findInactive();
                if (this.unitType == nameTranslation("GoldMine")) //goldmine init with nature start gold
                {
                    this.lumber = 0;
                    this.gold = SplashScreen.mapUnits.startGold[SplashScreen.mapObject.mapChoice][0]; //not sure how to clone if it is even needed

                    //only init garrison for buildings that have it (not sure if GM needs it)
                    int num = SplashScreen.mapUnits.numPlayer[SplashScreen.mapObject.mapChoice];


                    /*for (int i = 0; i < num; i++) //NOTE:index 0 = player 1 (no nature can garrison)
                    {//we might not even need to init it for each player if the GoldMine changes owner and is locked upon first visit
                        this.garrison[i] = new List<int>();
                    }*/
                }
                updateImg();
            }

            initLoadedData(ref gameData); //reload init data

            //set possibleanimation to number of tiles
            this.possibleAnimation = SplashScreen.mapUnits.allUnitTiles[this.unitType].Length;
        }

        public void initLoadedData(ref tempMap gameData)
        { //used to initiate all loaded unit data (called after having unitType set)
            this.defaultSpeed = gameData.unitDataCopy.speed[this.unitType];
            this.speed = (float)this.defaultSpeed/10; //divide 10 to make 1
            this.hitPoint = gameData.unitDataCopy.hitPoints[this.unitType];
            this.armor = gameData.unitDataCopy.armor[this.unitType];
            this.sight = gameData.unitDataCopy.sight[this.unitType];
            this.attackSteps = gameData.unitDataCopy.attackSteps[this.unitType];
            this.reloadSteps = gameData.unitDataCopy.reloadSteps[this.unitType];
            this.basicDamage = gameData.unitDataCopy.basicDamage[this.unitType];
            this.pierceDamage = gameData.unitDataCopy.piercingDamage[this.unitType];
            this.range = gameData.unitDataCopy.range[this.unitType];
            this.sightConstruction = gameData.unitDataCopy.sightConstruction[this.unitType];
            this.buildTime = gameData.unitDataCopy.buildTime[this.unitType];
            this.foodConsumption = gameData.unitDataCopy.foodConsumption[this.unitType];
            this.defaultUnitTileW = gameData.unitDataCopy.size[this.unitType];  //also can be gotten from loaded data of unit
            this.defaultUnitTileH = gameData.unitDataCopy.size[this.unitType];
        }

        public bool startUpgrade(string upgradeStr, ref tempMap gameData)
        {
            if (this.upgradeIndex == -1 && this.usable == 1 && this.inUse == 0) //only can have one upgrade at a time per building
            {
                if (gameData.upgradeUnit(upgradeStr, this.owner)) //if upgrade queue success or already upgrading/upgraded
                {
                    this.inUse = 1; //lock building
                    this.upgradeIndex = SplashScreen.unitUpgradeData.upgradeTranslation(upgradeStr);
                    this.timer.Add(SplashScreen.unitUpgradeData.time[upgradeIndex] * gameData.DConstructTime);
                    this.maxUpgradeTime = SplashScreen.unitUpgradeData.time[upgradeIndex] * gameData.DConstructTime;
                    gameData.reShowData = 1;
                    return true; //return true so buttons are not showing anymore for certain upgrade
                }
                else if (nameTranslation(upgradeStr) > 0) //we are upgrading a unit (probably building) to another upgraded version
                { //so upgrade to new building
                    //we have to check resources here as well
                    int type = nameTranslation(upgradeStr);
                    int goldRequired = SplashScreen.unitData.goldCost[type];
                    int lumberRequired = SplashScreen.unitData.lumberCost[type];
                    if (gameData.gold[this.owner] >= goldRequired && gameData.lumber[this.owner] >= lumberRequired)
                    {
                        
                        gameData.gold[this.owner] -= goldRequired;
                        gameData.lumber[this.owner] -= lumberRequired;
                        changeUnit(upgradeStr, ref gameData); //change unit type
                        this.usable = 0; //upgrading building, so can't use at all (we do need peasant for building upgrade?)
                        this.setBuildFrame(1); //set building frame to construction frame
                        if (gameData.playerChoice == this.owner) gameData.reShowSingleButton = 1;
                        return true;
                    }
                }
            }

            return false; //failed to do everything
        }

        public void endUpgrade(ref tempMap gameData, ref List<Unit>[] allUnits)
        { //call when timer ends for upgrade
            
            if(gameData.finishUpgrade(this.upgradeIndex,this.owner)) //finish the upgrade
            {
                int index = SplashScreen.unitUpgradeData.upgradeTranslation("Ranger"); //get the ranger upgrade index
                if (this.upgradeIndex == index) //if our current upgrade is the ranger upgrade
                {
                    foreach (Unit u in allUnits[this.owner])
                    {
                        if (u.unitType == nameTranslation("Archer"))  //upgrade all archer units
                        {
                            int newHp = 0;
                            double ratio = 0;
                            if (u.curHitPoint > 0) //aka unit is not dead
                            {
                                ratio = (double)u.curHitPoint / (double)u.hitPoint; //get percentage of hitpoints left
                            }
                            u.changeUnit("Ranger", ref gameData); //change unit from archer to ranger
                            newHp = (int)(ratio * u.hitPoint); //get new percentage of hitpoints respect to ranger
                            u.curHitPoint = newHp;
                        }
                    }

                }
                this.inUse = 0; //release lock on building
                //this.usable = 1; //allow building to be used once more
                gameData.updateUnitFlag = this.owner; //set to owner
                this.upgradeIndex = -1;
                if (gameData.playerChoice == this.owner) gameData.reShowSingleButton = 1;
            }

            //Need to update building image if it's a building image (TODO)
        }

        public void actOnTarget(ref List<Unit>[] allUnits, ref tempMap gameData, ref inGameButtons curButtons, ref UnitDatas curLabel) //unit chosen can be changed to list later
        { //perform action on target depending on conditions 
            //target must not be null!
            int index = this.target.index;
            int otherOwner = this.target.owner;
            int tileX = SplashScreen.tileObject.tileWidth;
            int tileY = SplashScreen.tileObject.tileHeight;

            Unit otherTemp;

            //float absX = 0f;
            
            //float absY = 0f;

            //MessageBox.Show(absX.ToString());
            //MessageBox.Show(absY.ToString());
            if (otherOwner != -1) //we actually have a target
            {
                if (index != -1 && otherOwner != -1) //normal unit
                {
                    //absX = Math.Abs(allUnits[otherOwner].ElementAt(index).x - this.x);
                    //absY = Math.Abs(allUnits[otherOwner].ElementAt(index).y - this.y);
                    otherTemp = allUnits[otherOwner][index];
                }
                else // if (otherOwner != -1) //tree target
                {
                    //absX = Math.Abs(this.target.x - this.x);
                    //absY = Math.Abs(this.target.y - this.y);
                    otherTemp = new Unit("Peasant", this.target.x, this.target.y, 0, 0, ref gameData);
                }

                if (index == -1 && this.unitType == nameTranslation("Peasant") && this.getAction() != "attack") //we chop tree! (check if peasant is extra safety) and make sure we aren't already chopping wood
                {
                    if (nearEnough(otherTemp) && gameData.isTree(this.target.x,this.target.y)) //we are actually close to tree (tree is 1-2 tile depending on directionso up to 3 tiles away)
                    {
                        if ((this.gold + this.lumber) < gameData.DLumberPerHarvest)
                        { //make sure our unit actually has space to hold more resources
                            this.setAction("attack");//set action to chopping (which is attack image)
                            this.onCommand = true; //so we get chop wood animation
                            this.timer.Add(gameData.DHarvestTime * gameData.DConstructTime); //add an internal timer for chopping wood
                            this.target.index = -2; //set index to -2 so that we don't keep moving (instead we stand and chop tree)
                        }
                    }
                    //do not reset target here (as we still need it for removeLumber function later)


                }
                else if (this.unitType == nameTranslation("Peasant") && 0 <= index && otherOwner <= 0 && allUnits[otherOwner].ElementAt(index).unitType == nameTranslation("GoldMine")) //gold mine and peasant
                {
                    if (nearEnough(otherTemp)) //we are actually close to building (and not stuck at some random spot)
                    {//NOTE: that test is only temporary as until we find better rule for nearing building
                        if ((this.gold + this.lumber) < gameData.DGoldPerMining)
                        { //make sure our unit actually has space to hold more resources
                            this.setAction("gold"); //change image to gold for walking out later
                            this.target.reset(); //reset target
                            garrisonIn(index, otherOwner, ref allUnits, gameData);

                            //Deselect unit
                            //chosen = null; //clear unit (clear list later) 
                            unitPtr temp = new unitPtr();
                            temp.index = this.id;
                            temp.owner = this.owner;
                            gameData.deselectUnit(temp, ref curButtons, ref curLabel);

                            //can set path/location of nearest building to deliver resources here so that after unit comes out, it will go there
                        }
                    }
                }

                else if (0 <= index && otherOwner != -1 && otherOwner == this.owner && (allUnits[otherOwner].ElementAt(index).unitType == nameTranslation("TownHall") || allUnits[otherOwner].ElementAt(index).unitType == nameTranslation("Keep") || allUnits[otherOwner].ElementAt(index).unitType == nameTranslation("Castle")) && this.unitType == nameTranslation("Peasant") && allUnits[otherOwner].ElementAt(index).usable == 1 && this.selfDestroy == false && this.repairing == false) //townhall, keep, castle storage
                {
                    if (((this.gold) >= gameData.DGoldPerMining) || ((this.lumber) >= gameData.DLumberPerHarvest))
                    {
                        if (nearEnough(otherTemp)) //we are actually close to building (and not stuck at some random spot)
                        {//NOTE: that test is only temporary as until we find better rule for nearing building
                            if ((this.gold) >= gameData.DGoldPerMining)
                            { //make sure our unit actually has resources to store

                                gameData.gold[this.owner] += this.gold; //add gold to coffers
                                this.gold = 0; //reset
                                this.setAction("walk"); //change image to walk after storing 
                                                        //can set path/location of resource outlet to go back to
                            }
                            else if ((this.lumber) >= gameData.DLumberPerHarvest)
                            { //make sure our unit actually has resources to store

                                gameData.lumber[this.owner] += this.lumber; //add gold to coffers
                                this.lumber = 0; //reset
                                this.setAction("walk"); //change image to walk after storing 

                                //can set path/location of resource outlet to go back to
                            }
                            if (this.prevTarget.owner != -1) //we have a previous target
                            {//go back to gather resource
                             //for lumber, set owner to 0, and index to location of tree TODO

                                float setX;
                                float setY;
                                if (0 <= prevTarget.index) //a unit
                                {
                                    setX = allUnits[prevTarget.owner][prevTarget.index].x;
                                    setY = allUnits[prevTarget.owner][prevTarget.index].y;
                                }
                                else //probably a tree
                                {
                                    setX = this.prevTarget.x;
                                    setY = this.prevTarget.y;
                                    this.target.x = this.prevTarget.x;
                                    this.target.y = this.prevTarget.y;
                                }
                                this.target.owner = this.prevTarget.owner;
                                this.target.index = this.prevTarget.index;
                                this.prevTarget.reset();
                                this.xToGo = (int)setX;
                                this.yToGo = (int)setY;
                                this.findNearestOpenSpot(allUnits, ref gameData);
                                this.invisible = 1; //set to true so we don't get blocked

                                this.onCommand = true;
                                //this.invisible = 0;
                            }
                        }
                        //

                    }

                    //this.target.reset(); //reset target
                    //this.prevTarget.reset();
                }
                else if (0 <= index && otherOwner != -1 && otherOwner == this.owner && allUnits[otherOwner].ElementAt(index).unitType == nameTranslation("LumberMill") && this.unitType == nameTranslation("Peasant") && allUnits[otherOwner].ElementAt(index).usable == 1 && this.selfDestroy == false && this.repairing == false) //lumbermill storage
                {
                    if ((this.lumber) >= gameData.DLumberPerHarvest)
                    {
                        if (nearEnough(otherTemp)) //we are actually close to building (and not stuck at some random spot)
                        {//NOTE: that test is only temporary as until we find better rule for nearing building
                            if ((this.lumber) >= gameData.DLumberPerHarvest)
                            { //make sure our unit actually has resources to store

                                gameData.lumber[this.owner] += this.lumber; //add gold to coffers
                                this.lumber = 0; //reset
                                this.setAction("walk"); //change image to walk after storing 

                                //can set path/location of resource outlet to go back to
                            }
                            if (this.prevTarget.owner != -1) //we have a previous target
                            {//go back to gather resource
                             //for lumber, set owner to 0, and index to location of tree TODO

                                float setX;
                                float setY;
                                if (prevTarget.index == -1)  //if there is a tree available
                                {
                                    setX = this.prevTarget.x;
                                    setY = this.prevTarget.y;
                                    this.target.x = this.prevTarget.x;
                                    this.target.y = this.prevTarget.y;
                                    this.target.owner = this.prevTarget.owner;
                                    this.target.index = this.prevTarget.index;
                                    this.prevTarget.reset();
                                    this.xToGo = (int)setX;
                                    this.yToGo = (int)setY;
                                    this.findNearestOpenSpot(allUnits, ref gameData);
                                    this.invisible = 1; //set to true so we don't get blocked

                                    this.onCommand = true;
                                }

                                //this.invisible = 0;
                            }
                        }
                        //

                    }
                }
                else if (this.unitType == nameTranslation("Peasant") && otherOwner != -1 && allUnits[otherOwner].ElementAt(index).usable == 0 && allUnits[otherOwner].ElementAt(index).getAction() != "death" && allUnits[otherOwner].ElementAt(index).getAction() != "decay" && allUnits[otherOwner].ElementAt(index).garrison.Count() == 0) //other building under construction (not dead) and peasant and building doesn't already have constructor
                {
                    if (nearEnough(otherTemp)) //we are actually close to building (and not stuck at some random spot)
                    {//NOTE: that test is only temporary as until we find better rule for nearing building
                        this.target.reset(); //reset target (as after building we exit and stand there)

                        //Deselect unit
                        //chosen = null; //clear unit (clear list later) 
                        unitPtr temp = new unitPtr();
                        temp.index = this.id;
                        temp.owner = this.owner;
                        gameData.deselectUnit(temp, ref curButtons, ref curLabel);

                        garrisonIn(index, otherOwner, ref allUnits, gameData); //garrison last to add timer last

                    }
                }
                else if (1 <= otherOwner && this.unitType < SplashScreen.numUnits && this.startedBattle == false && (this.selfDestroy == true || (this.selfDestroy == false && this.owner != otherOwner))) //if unit owners different and we are unit not building and we are not targeting nature //&& this.battling = false originally instead of this.startedBatlle
                { //we go into attack mode
                  // need to add this.owner != otherOwner && restriction in (removed for easy testing)
                    if (this.battle(this.target, ref allUnits, ref gameData))
                    { //battle successfully activated
                        this.startedBattle = true;
                        this.setAction("attack");//set action to attack
                                                 //this.setDirection(this.getRelativeDir(allUnits[otherOwner][index])); //set current direction to face the unit we are attacking
                        this.onCommand = true; //so we get attack animation

                        attackBack(ref allUnits, ref gameData);

                    }
                    else if (this.chasing == true)
                    { //we did not fail to start battle, just that we aren't near enough
                        this.startedBattle = true;
                        this.chasing = false;
                        this.battling = false; //set to false temporarily for battle check once more
                        if (this.unitType == nameTranslation("Peasant") && (this.gold > 0 || this.lumber > 0)) //peasant with resource
                        {
                            if (this.gold > 0) this.setAction("gold");
                            else this.setAction("lumber");
                        }
                        else this.setAction("walk"); //set to walk animation again
                    }
                    else
                    { //fail to battle target
                      //need to find next nearest target here 

                        if (this.unitType < SplashScreen.numUnits) this.currentFrame = 0;
                        this.selfDestroy = false;
                        this.chasing = false;
                        this.battling = false; //set to false temporarily for battle check once more
                        if (this.unitType == nameTranslation("Peasant") && (this.gold > 0 || this.lumber > 0)) //peasant with resource
                        {
                            if (this.gold > 0) this.setAction("gold");
                            else this.setAction("lumber");
                        }
                        else this.setAction("walk"); //set to walk animation again

                        //else
                        //target.reset(); //reset

                    }
                }
                else if (this.patrolling == true && this.unitType > -1)
                {//unit is patrolling
                    //findBattleTargetMelee(allUnits, gameData); //search for a target at end of patrolling spot before patrolling again
                    this.xToGo = this.patrolX2;
                    this.yToGo = this.patrolY2;
                    this.findNearestOpenSpot(allUnits, ref gameData);
                    this.patrolX2 = this.patrolX1;
                    this.patrolY2 = this.patrolY1;
                    this.patrolX1 = this.xToGo;
                    this.patrolY1 = this.yToGo;
                    this.patrolling = true;
                    this.onCommand = true;
                }
                else if (this.repairing == true && this.unitType == nameTranslation("Peasant") && otherOwner == this.owner && index >= 0 && allUnits[otherOwner][index].unitType >= SplashScreen.numUnits) //make sure it's peasant repairing our building
                {
                    if (this.battle(this.target, ref allUnits, ref gameData))
                    { //battle successfully activated
                        this.startedBattle = true;
                        this.setAction("attack");//set action to attack
                        this.onCommand = true; //so we get attack animation

                    }
                    else
                    { //fail to battle target
                        this.repairing = false;
                        this.selfDestroy = false;
                        this.chasing = false;
                        this.battling = false; //set to false temporarily for battle check once more
                        if (this.unitType == nameTranslation("Peasant") && (this.gold > 0 || this.lumber > 0)) //peasant with resource
                        {
                            if (this.gold > 0) this.setAction("gold");
                            else this.setAction("lumber");
                        }
                        else this.setAction("walk"); //set to walk animation again

                    }
                }
                else if (this.unitType == nameTranslation("Peasant") && ((allUnits[otherOwner][index].unitType == nameTranslation("gold") && this.gold <= 0) || (allUnits[otherOwner][index].unitType == nameTranslation("lumber") && this.lumber <= 0)) && allUnits[otherOwner][index].invisible == 0) //resource pickup
                {
                    allUnits[otherOwner][index].pickedUp(ref gameData);
                    if (allUnits[otherOwner][index].unitType == nameTranslation("gold"))
                    {
                        this.gold += gameData.DGoldPerMining;
                        this.setAction("gold");
                    }
                    else if (allUnits[otherOwner][index].unitType == nameTranslation("lumber"))
                    {
                        this.lumber += gameData.DLumberPerHarvest;
                        this.setAction("lumber");
                    }
                }
            }
            /*else if (meleeUnit() && this.unitType != nameTranslation("Peasant") && startedBattle == false && chasing == false)
            {
                findBattleTargetMelee(allUnits, gameData);
            }*/
            /*else
            { //if all other cases fail
                this.target.reset();
                this.prevTarget.reset();
            }*/
            

        }

        /*private string oppositeDir()
        { //helper function that returns the opposite of this unit's direction
            //for setting the direction of other unit
            switch (this.getDirection())
            {
                case "N":
                    return "S";
                case "S":
                    return "N";
                case "W":
                    return "E";
                case "E":
                    return "W";
                case "NW":
                    return "SE";
                case "SE":
                    return "NW";
                case "SW":
                    return "NE";
                case "NE":
                    return "SW";
            }

            return this.getDirection(); //return default
        }*/

        private string getRelativeDir(Unit other)
        { //gets a direction back that depends on this unit's position and target unit's position
            //used for setting direction during battle

            int xDir = -1;
            int yDir = -1;
            //float xDirVal = 0f;
            //float yDirVal = 0f;

            /*if (other.x > this.x) //aka other unit is to this unit's right
            {
                xDir = 1;
                //xDirVal = Math.Abs(this.x - other.x);
            }
            else if (other.x == this.x) //x position doesn't matter
            {
                xDir = -1;
            }
            if (other.y > this.y) //aka other unit is below this unit
            {
                yDir = 1;
                //yDirVal = Math.Abs(this.y - other.y);
            }
            else if (other.y == this.y) //y direction doesn't matter
            {
                yDir = -1;
            }*/

            /*if (other.x > this.x+this.defaultUnitTileW) //aka other unit is to this unit's right
            {
                xDir = 1;
                //xDirVal = Math.Abs(this.x - other.x);
            }
            else if (other.x < this.x) //other unit to left
            {
                xDir = 0;
            }
            if (other.y > this.y+this.defaultUnitTileH) //aka other unit is below this unit
            {
                yDir = 1;
                //yDirVal = Math.Abs(this.y - other.y);
            }
            else if (other.y < this.y) //other unit above
            {
                yDir = 0;
            }*/

            if (other.x > this.x) //aka other unit is to this unit's right
            {
                xDir = 1;
                //xDirVal = Math.Abs(this.x - other.x);
            }
            else if (other.x+(float)(other.defaultUnitTileW-1) < this.x) //other unit to left
            {
                xDir = 0;
            }
            if (other.y > this.y) //aka other unit is below this unit
            {
                yDir = 1;
                //yDirVal = Math.Abs(this.y - other.y);
            }
            else if (other.y+(float)(other.defaultUnitTileH-1) < this.y) //other unit above
            {
                yDir = 0;
            }

            if (xDir == 1 && yDir == 1) return "SE"; //other unit is below to right
            else if (xDir == 0 && yDir == 0) return "NW"; //other unit is above and to left
            else if (xDir == 1 && yDir == 0) return "NE"; //other unit is to the right and above
            else if (xDir == 0 && yDir == 1) return "SW"; //other unit is below and to the left
            else if (yDir == 0) return "N"; //other unit is above
            else if (yDir == 1) return "S"; //other unit is below
            else if (xDir == 1) return "E"; //other unit is to the right
            else if (xDir == 0) return "W";

            return this.getDirection(); //return current direction by default

        }
        public bool nearEnough(Unit other)
        { //checks if current unit is near enough to the other unit
            
            float rangeConstant = (float)(this.range - 1);
            float nearConstant =  rangeConstant + 0.2f; //how near the unit should be (taking into account range of current unit)
            //if (this.unitType < -1) nearConstant = rangeConstant + 0.1f; //so projectile has to clash with unit before disappearing
            if (this.unitType < -1) nearConstant = 0.1f; //we don't need to check other cases unless it's on edge exactly
            if (meleeUnit() && this.meleeSearchTarget) nearConstant += (float)10; //add range of 10 for searching for target as melee unit

            int otherW = other.defaultUnitTileW;
            int otherH = other.defaultUnitTileH;
            float otherX = other.x;
            float otherY = other.y;

            int thisW = this.defaultUnitTileW;
            int thisH = this.defaultUnitTileH;
            float thisX = this.x;
            float thisY = this.y;

            if (this.unitType < -1) //a projectile check cases
            {
                if (thisX == otherX && thisY == otherY) return true; //if exact same position, obviously is near enough
                if (otherX <= thisX && thisX < otherX + otherW && otherY <= thisY && thisY < otherY + otherH)
                { //this unit is within other unit so obviously can't be any nearer
                    return true;
                }
                if (otherX <= thisX + 1 && thisX + 1 < otherX + otherW && otherY <= thisY + 1 && thisY + 1 < otherY + otherH)
                { //lower right hand corner is within other image
                    return true;
                }

            }

            else
            {
                //if (rangingUnit()) MessageBox.Show(nearConstant.ToString()); //for testing
                string tempDir = this.getDirection();
                tempDir = this.getRelativeDir(other);
                //float tempVal = Math.Abs(otherX + 1 - (thisX + thisW));
                //if (rangingUnit()) MessageBox.Show(tempVal.ToString()); //for testing

                for (int j = 0; j < otherH; j++) //iterate entire building height
                {
                    for (int i = 0; i < otherW; i++) //iterate entire building width
                    {
                        if (tempDir == "SE") //we are facing east
                        { //so only check with respect to x, but against other unit's east edge
                            if (rangingUnit() && Math.Abs(otherX + i - (thisX + thisW)) <= nearConstant && Math.Abs(otherY + j - (thisY + thisH)) <= nearConstant) return true;
                            else if (meleeUnit() && Math.Abs(otherX + i - (thisX + thisW)) <= nearConstant && Math.Abs(otherY + j - (thisY + thisH)) <= nearConstant) return true;
                        }
                        else if (tempDir == "NE") //we are facing east
                        { //so only check with respect to x, but against other unit's east edge
                            if (rangingUnit() && Math.Abs(otherX + i - (thisX + thisW)) <= nearConstant && Math.Abs((otherY + otherH - j) - thisY) <= nearConstant) return true;
                            else if (meleeUnit() && Math.Abs(otherX + i - (thisX + thisW)) <= nearConstant && Math.Abs((otherY + otherH - j) - thisY) <= nearConstant) return true;
                        }
                        else if (tempDir == "NW") //we are facing east
                        { //so only check with respect to x, but against other unit's east edge
                            if (rangingUnit() && Math.Abs((otherX + otherW - i) - thisX) <= nearConstant && Math.Abs((otherY + otherH - j) - thisY) <= nearConstant) return true;
                            else if (meleeUnit() && Math.Abs((otherX + otherW - i) - thisX) <= nearConstant && Math.Abs((otherY + otherH - j) - thisY) <= nearConstant) return true;
                        }
                        else if (tempDir == "SW") //we are facing east
                        { //so only check with respect to x, but against other unit's east edge
                            if (rangingUnit() && Math.Abs((otherX + otherW - i) - thisX) <= nearConstant && Math.Abs(otherY + j - (thisY + thisH)) <= nearConstant) return true;
                            else if (meleeUnit() && Math.Abs((otherX + otherW - i) - thisX) <= nearConstant && Math.Abs(otherY + j - (thisY + thisH)) <= nearConstant) return true;
                        }
                        else if (tempDir == "E") //we are facing east
                        { //so only check with respect to x
                            if (Math.Abs(otherX - (thisX + thisW)) <= nearConstant && Math.Abs((otherY + j) - thisY) <= nearConstant) return true;
                        }
                        else if (tempDir == "W") //we are facing east
                        { //so only check with respect to x, but against other unit's east edge
                            if (Math.Abs((otherX + otherW) - thisX) <= nearConstant && Math.Abs((otherY + j) - thisY) <= nearConstant) return true;
                        }
                        else if (tempDir == "S") //we are facing east
                        { //so only check with respect to x, but against other unit's east edge
                            if (Math.Abs(otherY - (thisY + thisH)) <= nearConstant && Math.Abs((otherX + i) - thisX) <= nearConstant) return true;
                        }
                        else if (tempDir == "N") //we are facing east
                        { //so only check with respect to x, but against other unit's east edge
                            if (Math.Abs((otherY + otherH) - thisY) <= nearConstant && Math.Abs((otherX + i) - thisX) <= nearConstant) return true;
                        }
                    }
                }
            }

            return false;
        }

        public void killed(ref tempMap gameData, ref inGameButtons curButtons, ref UnitDatas curLabel, Unit other, ref List<Unit>[] allUnits)
        { //function to call when a unit dies or to kill off a unit
            //set location to go to current spot (because dead so instant drop)
            this.onCommand = false;
            this.xToGo = (int)this.x;
            this.yToGo = (int)this.y;
            this.target.reset(); //reset the target as we are dead
            this.usable = 0; //set it so you can't use unit anymore
            this.currentFrame = 0; //reset frame to 0
            this.timer.Clear(); //clear all timers

            for (int ix = (int)this.x; ix < (int)this.x + this.defaultUnitTileW; ix++) //unblock the unit tile positions as we are dead
            {
                for (int iy = (int)this.y; iy < (int)this.y + this.defaultUnitTileH; iy++)
                {
                    gameData.pathFindingGriod.UnblockCell(new RoyT.AStar.Position(ix, iy));
                    /*if (u.isGoldMine()) //no goldmine special case
                    {
                        thisMapPathGrid.UnblockCell(new Position((int)u.x + u.unitTileWidth - 1, (int)u.y + u.unitTileHeight - 1));
                    }*/
                }
            }

            //Deselect this unit if selected
            unitPtr temp = new unitPtr();
            temp.owner = this.owner;
            temp.index = this.id;
            gameData.deselectUnit(temp, ref curButtons, ref curLabel); //deselect the unit

            if (this.unitType < SplashScreen.numUnits)
            { //wrapped by this because building doesn't have direction so can skip all direction stuff
                //For death/decay image choosing
                int choose = new Random().Next(0, 1); //choose between two (N or S)
                int choose2 = new Random().Next(0, 1); //choose between two (E or S)
                int curDir = 0; //direction of east or west (0 for west, 1 for east)
                int NSDir = -1; //default negative for case N/S (North = 0; South = 1)

                this.setDirection(this.getRelativeDir(other)); //get direction and face the unit that killed this unit

                switch(this.getDirection())
                {
                    case "E":
                        curDir = 1;
                        NSDir = choose;
                        break;
                    case "W":
                        curDir = 0;
                        NSDir = choose;
                        break;
                    case "NW":
                        curDir = 0;
                        NSDir = 0;
                        break;
                    case "NE":
                        curDir = 1;
                        NSDir = 0;
                        break;
                    case "SW":
                        curDir = 0;
                        NSDir = 1;
                        break;
                    case "SE":
                        curDir = 1;
                        NSDir = 1;
                        break;
                    default:
                        curDir = choose2;
                        NSDir = choose;
                        break;
                }

                string[] NSStr = { "N", "S" };
                string[] WEStr = { "W", "E" };
                string newDir = NSStr[NSDir] + WEStr[curDir];
                this.setDirection(newDir);
                int dirCpy = this.direction;
                this.projectileImgDir = dirCpy; //set a static direction that doesn't change after this for image


                this.setAction("death"); //set image to death animation for unit
                this.timer.Add(gameData.DDeathTime * 100); //100 can be DConstruct time (but looks nicer with 100)
                //this.onCommand = true; 
                this.onCommand = false; //using manual animation

                if (this.unitType == nameTranslation("Peasant")) //peasant drop resource on death
                {
                    if (this.gold > 0) //we are dropping gold
                    {
                        Unit dropped = new Unit("gold", (int)this.x, (int)this.y, 0, allUnits[0].Count(), ref gameData);
                        allUnits[0].Add(dropped);
                        
                    }
                    else if (this.lumber > 0) //we are dropping lumber
                    {
                        Unit dropped = new Unit("lumber", (int)this.x, (int)this.y, 0, allUnits[0].Count(), ref gameData);
                        allUnits[0].Add(dropped);

                    }
                }
            }
            else
            { //skip to building decay (no onCommand animation but manual)
                this.decaying = true;
                this.setAction("decay"); //skip to decay animation
                this.timer.Add(gameData.DDecayTime * gameData.DDecayTimer);
                this.onCommand = false;
            }
            
            
            

        }

        private void doDamage(ref List<Unit>[] allUnits, ref tempMap gameData, ref inGameButtons curButtons, ref UnitDatas curLabel)
        {//this function does damage to current target other unit depending on current unit
            //this function also will serve as our chasing function (for if target is moving)
            if (this.target.owner >= 1) //one last check just in case (it got canceled)
            {
                Unit other = allUnits[this.target.owner][this.target.index];
                if (other.curHitPoint > 0 && this.curHitPoint > 0 && other != this) //one last check on HP and make sure we aren't attacking ourselves?
                {
                    this.timer.Clear();
                    this.currentFrame = 0;
                    //this.onCommand = false;
                    //this.currentFrame = 0; //must always reset frame before setting action
                    //this.setAction("walk"); //set to walk animation again

                    //if (nearEnough(other)) //want to always do damage even if out of range (so comment out)
                    if (reloading == 0 && this.battling == true) //only do damage if we are done reloading and battle animation
                    { //only do damage if near enough so that we don't conflict with chasing timer
                        //Do damage out here because we want to be able to damage even if other unit has walked a bit away (or else we won't be able to damage moving units)
                        //int damageToDo = this.basicDamage + this.pierceDamage - other.armor; //basic damage func (not sure yet)
                        if (repairing == false) //not a repair mode (default for all except peasant)
                        {
                            int regularDamage = this.basicDamage - other.armor;
                            if (regularDamage < 0) regularDamage = 0; //this prevents us from doing negative damage on high armor targets

                            int damageToDo = regularDamage + this.pierceDamage;

                            allUnits[this.target.owner][this.target.index].curHitPoint -= damageToDo;
                            //allUnits[this.target.owner][this.target.index].playSound(gameData);  //building-help
                            gameData.checkHealthDisplay(allUnits[this.target.owner][this.target.index]);

                            //Set reload time after doing damage
                            this.reloading = 0; //reset to zero then add
                            this.reloading += this.reloadSteps * gameData.DAttackSteps; //will not affect melee as melee reloadsteps = 0
                        }
                        else
                        {
                            int regularRepair = this.basicDamage + this.pierceDamage; //set repair to max damage
                            allUnits[this.target.owner][this.target.index].curHitPoint += regularRepair;
                            if (allUnits[other.owner][other.id].curHitPoint > allUnits[other.owner][other.id].hitPoint) allUnits[other.owner][other.id].curHitPoint = allUnits[other.owner][other.id].hitPoint; //check if we over repaired
                            gameData.checkHealthDisplay(allUnits[this.target.owner][this.target.index]);

                            //Can set repair reload step here if needed
                            this.reloading = 0; //reset to zero then add
                            this.reloading += this.reloadSteps * gameData.DAttackSteps; //will not affect melee as melee reloadsteps = 0
                        }
                        //if (this.unitType >= SplashScreen.numUnits) this.battling = false; //turn off battling for battle check for building
                        this.battling = false; //turn battling off so we can call battle again
                    }

                    //this.battling = false;
                    //this.chasing = false;

                    if (other.curHitPoint <= 0 || this.curHitPoint <= 0 || (this.repairing == true && (other.curHitPoint >= other.hitPoint))) //end fight(or repair) condition
                    {
                        this.selfDestroy = false; //reset just in case we attacking friendly unit
                        this.repairing = false;
                        this.onCommand = false;
                        this.timer.Clear(); //clear  the timer after killinng units
                        this.target.reset(); //reset target as battle has finish with this hit
                        this.chasing = false;
                        this.battling = false; //set to false temporarily for battle check once more
                        this.startedBattle = false;
                        
                        if (this.unitType < SplashScreen.numUnits)
                        {
                            //this.onCommand = false;
                            this.currentFrame = 0; //must always reset frame before setting action
                            if (this.unitType == nameTranslation("Peasant") && (this.gold > 0 || this.lumber > 0)) //peasant with resource
                            {
                                if (this.gold > 0) this.setAction("gold");
                                else this.setAction("lumber");
                            }
                            else this.setAction("walk"); //set to walk animation again
                        }
                        //if (this.timer.Count() != 0) timer.Clear(); //clear the timers for chasing
                        if (this.curHitPoint <= 0) //this unit died
                        {
                            if (this.getAction() != "death" && this.getAction() != "decay") //we haven't been killed off yet
                            {
                                this.killed(ref gameData, ref curButtons, ref curLabel, other, ref allUnits);
                            }
                        }
                        else
                        {
                            if (this.patrolling == true) //we were patrolling before, so continue patrolling
                            {
                                this.xToGo = this.patrolX2;
                                this.yToGo = this.patrolY2;
                                this.findNearestOpenSpot(allUnits, ref gameData);
                                this.patrolX2 = this.patrolX1;
                                this.patrolY2 = this.patrolY1;
                                this.patrolX1 = this.xToGo;
                                this.patrolY1 = this.yToGo;
                                this.patrolling = true;
                                this.onCommand = true;
                            }
                        }
                        if (other.curHitPoint <= 0) //other unit died
                        {
                            if (allUnits[other.owner][other.id].getAction() != "death" && allUnits[other.owner][other.id].getAction() != "decay") //other unit hasn't been killed off yet
                            {
                                allUnits[other.owner][other.id].startedBattle = false;
                                allUnits[other.owner][other.id].killed(ref gameData, ref curButtons, ref curLabel, this, ref allUnits);
                            }
                        }
                    }
                    else if (this.unitType >= SplashScreen.numUnits) //for building battle
                    { //we do a complete reset of battle, but don't reset the target (as we are still in battle)
                        this.timer.Clear();
                        this.startedBattle = false;
                        this.battling = false;
                    }
                    /*else if (this.unitType < SplashScreen.numUnits && this.battle(this.target, ref allUnits, ref gameData)) //check if battle for units only
                    {
                        //int damageToDo = this.basicDamage + this.pierceDamage - other.armor; //basic damage func (not sure yet)
                        //allUnits[this.target.owner][this.target.index].curHitPoint -= damageToDo;
                        //if (this.unitType < SplashScreen.numUnits)
                        //{
                            this.setAction("attack");
                            this.onCommand = true; //so we get attack animation
                        //}
                    }
                    else
                    {
                        this.battling = false; //set to false temporarily for battle check once more
                        this.chasing = false;
                        if (this.unitType < SplashScreen.numUnits)
                        {
                            //this.chasing = false;
                            this.currentFrame = 0;
                            this.setAction("walk"); //set to walk animation again
                        }
                        //else this.chasing = true;
                    }*/
                }
                else if (other.curHitPoint <= 0 || this.curHitPoint <= 0 || (this.repairing == true && (other.curHitPoint >= other.hitPoint))) //end fight condition
                {
                    this.repairing = false;
                    this.selfDestroy = false; //reset just in case we attacking friendly unit
                    this.timer.Clear(); //clear  the timer after killinng units
                    this.target.reset(); //reset target as battle has finish with this hit
                    this.chasing = false;
                    this.battling = false; //set to false temporarily for battle check once more
                    this.startedBattle = false;
                    if (this.unitType < SplashScreen.numUnits)
                    {
                        this.currentFrame = 0; //must always reset frame before setting action
                        if (this.unitType == nameTranslation("Peasant") && (this.gold > 0 || this.lumber > 0)) //peasant with resource
                        {
                            if (this.gold > 0) this.setAction("gold");
                            else this.setAction("lumber");
                        }
                        else this.setAction("walk"); //set to walk animation again
                    }
                    this.onCommand = false;
                    //if (this.timer.Count() != 0) timer.Clear(); //clear the timers for chasing
                    if (this.curHitPoint <= 0) //this unit died
                    {
                        if (this.getAction() != "death" && this.getAction() != "decay") //we haven't been killed off yet
                        {
                            this.killed(ref gameData, ref curButtons, ref curLabel, other, ref allUnits);
                        }
                    }
                    else
                    {
                        if (this.patrolling == true) //we were patrolling before, so continue patrolling
                        {
                            this.xToGo = this.patrolX2;
                            this.yToGo = this.patrolY2;
                            this.findNearestOpenSpot(allUnits, ref gameData);
                            this.patrolX2 = this.patrolX1;
                            this.patrolY2 = this.patrolY1;
                            this.patrolX1 = this.xToGo;
                            this.patrolY1 = this.yToGo;
                            this.patrolling = true;
                            this.onCommand = true;
                        }
                    }
                    if (other.curHitPoint <= 0) //other unit died
                    {
                        if (allUnits[other.owner][other.id].getAction() != "death" && allUnits[other.owner][other.id].getAction() != "decay") //other unit hasn't been killed off yet
                        {
                            allUnits[other.owner][other.id].startedBattle = false;
                            allUnits[other.owner][other.id].killed(ref gameData, ref curButtons, ref curLabel, this, ref allUnits);
                        }
                    }
                }
            }

        }

        public bool battle(unitPtr other, ref List<Unit>[] allUnits, ref tempMap gameData)
        {//battle function (this assumes that other is valid unit passed in)
            int owner = other.owner;
            int index = other.index;
            //if (index < 0) return false;
            if (owner >= 1 && index >= 0)
            {
                Unit temp = allUnits[owner][index];
                if ((this.battling == false && this.curHitPoint > 0 && temp.curHitPoint > 0) && ((this.repairing == true && (temp.curHitPoint < temp.hitPoint)) || (this.repairing == false))) //unit currently not attacking other units and both units are alive
                {

                    if (meleeUnit() || rangingUnit()) //a fighting unit
                    {
                        if (this.nearEnough(temp) && this.reloading == 0) //if our two units are within 1 tile of each other (for melee) and we are ready to attack
                        {
                            //this.timer.Clear(); //clear all timers
                            this.battling = true; //set battling to true
                                                  //this.patrolling = false; //turn off patrol for battle
                            this.chasing = false;

                            if (rangingUnit())
                            {
                                if (this.unitType < SplashScreen.numUnits)
                                {
                                    //if ((int)this.x != prevX || (int)this.y != prevY)
                                    {
                                        this.xToGo = (int)this.x;
                                        this.yToGo = (int)this.y;
                                        //this.findNearestOpenSpot(allUnits, ref gameData);
                                    }
                                }
                                //this.setDirection(this.getRelativeDir(temp));
                            }
                            else
                            {
                                //if ((int)temp.x != (int)temp.prevX || (int)temp.y != (int)temp.prevY) //other unit has moved
                                if ((int)this.x != prevX || (int)this.y != prevY)
                                {
                                    //this.xToGo = (int)temp.x;
                                    //this.yToGo = (int)temp.y;
                                    this.xToGo = (int)this.x;
                                    this.yToGo = (int)this.y;
                                    //this.findNearestOpenSpot(allUnits, ref gameData);
                                }
                            }

                            //Set the target
                            this.playSound(gameData);  //battle sound effect
                            this.target.owner = owner;
                            this.target.index = index;
                            if (this.unitType < SplashScreen.numUnits) this.setDirection(this.getRelativeDir(temp)); //set this unit to face other unit during battle
                            this.timer.Add(this.attackSteps * gameData.DAttackSteps); //add an internal timer for how long to deal one hit


                            if (rangingUnit() && SplashScreen.renderProjectiles == 1) //if a ranging unit (for projection creation) and we have renderProjectile set to on
                            {
                                //int arrowSpeed = this.reloadSteps / this.attackSteps;
                                int arrowSpeed = this.reloadSteps / 2;
                                int tempDir = dirTranslation(this.getRelativeDir(temp));
                                //TODO: Create arrow/cannonball here
                                if (this.unitType < SplashScreen.numUnits || this.unitType == nameTranslation("GuardTower")) //an unit
                                { //create arrow here

                                    int foundIdle = 0;
                                    //First search through current list to see if there is already some unused arrow
                                    foreach (Unit u in allUnits[0])
                                    {
                                        if (foundIdle == 0 && u.unitType == nameTranslation("Arrow") && u.invisible == 1) //projectile is arrow and invisible (aka not in use)
                                        {
                                            if (this.unitType < SplashScreen.numUnits) u.initProjectile(this.target, this.x, this.y, arrowSpeed, ref allUnits, gameData, this.direction);
                                            else u.initProjectile(this.target, this.x, this.y, arrowSpeed, ref allUnits, gameData, tempDir);
                                            foundIdle = 1;
                                            playSound(gameData);
                                        }
                                    }
                                    if (foundIdle == 0) //did not find a projectile that is idle
                                    {
                                        Unit tempArrow = new Unit("Arrow", (int)this.x, (int)this.y, 0, 0, ref gameData);
                                        if (this.unitType < SplashScreen.numUnits) tempArrow.initProjectile(this.target, this.x, this.y, arrowSpeed, ref allUnits, gameData, this.direction);
                                        else tempArrow.initProjectile(this.target, this.x, this.y, arrowSpeed, ref allUnits, gameData, tempDir);
                                        allUnits[0].Add(tempArrow);
                                    }
                                }
                                else //cannontower
                                { //cannontower = cannonball
                                    int foundIdle = 0;
                                    //First search through current list to see if there is already chsome unused arrow
                                    foreach (Unit u in allUnits[0])
                                    {
                                        if (foundIdle == 0 && u.unitType == nameTranslation("Cannonball") && u.invisible == 1) //projectile is cannonball and invisible (aka not in use)
                                        {
                                            u.initProjectile(this.target, this.x, this.y, arrowSpeed, ref allUnits, gameData, tempDir);
                                            foundIdle = 1;
                                        }
                                    }
                                    if (foundIdle == 0) //did not find a projectile that is idle
                                    {
                                        Unit tempArrow = new Unit("Cannonball", (int)this.x, (int)this.y, 0, 0, ref gameData);
                                        tempArrow.initProjectile(this.target, this.x, this.y, arrowSpeed, ref allUnits, gameData, this.direction);
                                        allUnits[0].Add(tempArrow);
                                    }
                                }

                            }

                            return true;
                        }
                        else if (this.unitType >= SplashScreen.numUnits) //for building battle
                        {//we are not near enough as a building and obviously building can't move so reset
                            if (reloading == 0) //we aren't reloading, but truly not near enough
                            {
                                this.timer.Clear();
                                this.startedBattle = false;
                                //this.chasing = false;
                                this.battling = false;
                                this.target.reset();
                                //this.onCommand = false;
                            }
                            else //we are reloading
                            {//so simply reset but don't reset target
                                this.timer.Clear();
                                this.startedBattle = false;
                                this.battling = false;
                            }
                        }
                        else
                        { //we are too far so we have to move closer (so simply call damage function to chase)

                            //this.xToGo = (int)temp.x;
                            //this.yToGo = (int)temp.y;
                            //this.onCommand = true; //set to tru so unit will move again
                            //this.onCommand = false;
                            if (meleeUnit())
                            {
                                //this.timer.Clear(); //clear all timers
                                this.battling = false;
                                //this.currentFrame = 0;
                                if (this.unitType == nameTranslation("Peasant") && (this.gold > 0 || this.lumber > 0)) //peasant with resource
                                {
                                    if (this.gold > 0) this.setAction("gold");
                                    else this.setAction("lumber");
                                }
                                else this.setAction("walk"); //set to walk animation again
                                this.target.owner = owner;
                                this.target.index = index;
                                this.chasing = true;
                                //if ((int)temp.x != (int)temp.prevX || (int)temp.y != (int)temp.prevY) //other unit has moved
                                {

                                    this.xToGo = (int)temp.x;
                                    this.yToGo = (int)temp.y;
                                    this.findNearestOpenSpot(allUnits, ref gameData);

                                }
                                this.onCommand = true; //set to tru so unit will move again
                            }
                            else
                            {
                                //this.timer.Clear(); //clear all timers
                                this.battling = false;
                                this.target.owner = owner;
                                this.target.index = index;
                                if (this.unitType < SplashScreen.numUnits) //only do following for non-building (as buildings can't move)
                                {
                                    this.onCommand = false;
                                    //this.currentFrame = 0; //reset frame for standing still
                                    this.setAction("walk");

                                    if (this.nearEnough(temp)) //we are near enough or currently reloading
                                    {  //we are already in range (but reloading)
                                       //stay at current position
                                       //We don't want to move here (as archer in range) not sure how to do yet
                                       //this.xToGo = (int)this.x;
                                       //this.yToGo = (int)this.y;
                                        this.chasing = true;
                                        this.currentFrame = 0; //reset frame for standing still
                                        this.setAction("walk");
                                        //if ((int)this.x != prevX || (int)this.y != prevY)
                                        {
                                            this.xToGo = (int)this.x;
                                            this.yToGo = (int)this.y;
                                            //this.findNearestOpenSpot(allUnits, ref gameData);
                                        }
                                        //this.onCommand = true; //so we don't move around

                                        //this.battling = true;
                                        

                                    }
                                    else
                                    { //go to other unit position
                                        this.chasing = true;
                                        if ((int)temp.x != (int)temp.prevX || (int)temp.y != (int)temp.prevY) //other unit has moved
                                        {
                                            this.xToGo = (int)temp.x;
                                            this.yToGo = (int)temp.y;
                                            this.findNearestOpenSpot(allUnits, ref gameData);
                                        }
                                        this.onCommand = true; //set to tru so unit will move again
                                                               //this.onCommand = false; //set to false so unit stop moving (but it also stops attacking)
                                    }
                                }
                            }


                            //this.timer.Add(this.attackSteps * 1); //add an internal timer for when to update enemy unit location
                        }


                    }
                    /*else if (rangingUnit()) //we are range unit
                    {

                    }*/

                }
                else if ((this.curHitPoint <= 0 || temp.curHitPoint <= 0) && this.startedBattle == true)
                {
                    this.timer.Clear();
                    this.target.reset();
                    this.startedBattle = false;
                    this.battling = false;
                    this.chasing = false;
                }
            }
            return false; //for failing to start battle

        }

        public void attackBack(ref List<Unit>[] allUnits, ref tempMap gameData)
        {//Code for attacking Back (exact same as when used outside of this function)
            if (allUnits[this.target.owner][this.target.index].meleeUnit() || allUnits[this.target.owner][this.target.index].rangingUnit()) //only call fight back if other unit is fighting unit
            {
                unitPtr thisTemp = new unitPtr();
                thisTemp.owner = this.owner;
                thisTemp.index = this.id;
            
                if (allUnits[this.target.owner][this.target.index].startedBattle == false && (allUnits[this.target.owner][this.target.index].onCommand == false || (allUnits[this.target.owner][this.target.index].onCommand == true && allUnits[this.target.owner][this.target.index].patrolling == true))) //only start if other unit is not already battling and not being controlled to move &&  && allUnits[this.target.owner][this.target.index].onCommand == false
                {
                    if (this.selfDestroy == true) allUnits[this.target.owner][this.target.index].selfDestroy = true; //if we are attacking friendly, they will attack back (as they are not patriotic)
                    if (allUnits[this.target.owner][this.target.index].battle(thisTemp, ref allUnits, ref gameData))
                    {
                        allUnits[this.target.owner][this.target.index].startedBattle = true;
                        allUnits[this.target.owner][this.target.index].setAction("attack");
                        allUnits[this.target.owner][this.target.index].onCommand = true;
                    }
                    else
                    {
                        if (allUnits[this.target.owner][this.target.index].unitType < SplashScreen.numUnits) allUnits[this.target.owner][this.target.index].currentFrame = 0;
                        allUnits[this.target.owner][this.target.index].chasing = false;
                        allUnits[this.target.owner][this.target.index].battling = false;
                        if (allUnits[this.target.owner][this.target.index].unitType == nameTranslation("Peasant") && (allUnits[this.target.owner][this.target.index].gold > 0 || allUnits[this.target.owner][this.target.index].lumber > 0)) //peasant with resource
                        {
                            if (allUnits[this.target.owner][this.target.index].gold > 0) allUnits[this.target.owner][this.target.index].setAction("gold");
                            else allUnits[this.target.owner][this.target.index].setAction("lumber");
                        }
                        else allUnits[this.target.owner][this.target.index].setAction("walk"); //set to walk animation again
                        //allUnits[this.target.owner][this.target.index].setAction("walk");
                    }
                }
            }
        }
        private bool meleeUnit()
        { //helper function to check if this unit is a ranging unit
            string[] meleeUnitsStr = { "Peasant", "Footman" };
            for (int i = 0; i < meleeUnitsStr.Length; i++)
            {
                if (nameTranslation(meleeUnitsStr[i]) == this.unitType) return true;
            }

            return false;
        }

        public bool rangingUnit()
        { //helper function to check if this unit is a ranging unit
            string[] rangingUnitsStr = { "Archer", "Ranger", "CannonTower", "GuardTower" };
            for (int i = 0; i < rangingUnitsStr.Length; i++)
            {
                if (nameTranslation(rangingUnitsStr[i]) == this.unitType) return true;
            }

            return false;
        }

        private bool checkSelf(int otherId, int otherOwner)
        {
            if (otherOwner == this.owner && this.id == otherId) return true; //if other unit is this unit
            return false;
        }

        public void findBattleTargetMelee(List<Unit>[] allUnits, tempMap gameData)
        { //for melee units only
            if (this.unitType < SplashScreen.numUnits && reloading == 0 && this.startedBattle == false && this.usable == 1) //only check if ranging unit that is ready for battle and we are not moving && this.onCommand == false 
            {
                if (findNearestTarget(allUnits, gameData)) //if we were able to find a target that was in range (target was already set by findnearesttarget function
                {
                    if (battle(this.target, ref allUnits, ref gameData))
                    {
                        this.startedBattle = true;
                        
                        this.setAction("attack");//set action to attack (should not affect building as building doesn't take action into account)
                        this.onCommand = true; //so we get attack animation
                        

                        attackBack(ref allUnits, ref gameData);
                    }
                    else
                    {
                        this.chasing = false;
                        this.battling = false; //set to false temporarily for battle check once more
                        this.currentFrame = 0; //only change frame for unit (as building uses frame for construction)
                        if (this.unitType == nameTranslation("Peasant") && (this.gold > 0 || this.lumber > 0)) //peasant with resource
                        {
                            if (this.gold > 0) this.setAction("gold");
                            else this.setAction("lumber");
                        }
                        else this.setAction("walk"); //set to walk animation again
                    }
                }
            }
        }

        private bool findNearestTarget(List<Unit>[] allUnits, tempMap gameData)
        {
            int numPlayer = gameData.numPlayers;
            int found = 0;
            unitPtr temp = new unitPtr(); //to store pointer to our target
            double minDist = 999; //to hold the current nearest target's distance to us
            for (int i = 1; i <= numPlayer; i++) //we don't have to search nature player
            {//extra test can be added here to make sure we don't attack same player units
                if (i != this.owner) //we can't select friendly unit as target
                {
                    foreach (Unit u in allUnits[i])
                    {
                        if (!this.checkSelf(u.id, u.owner) && gameData.checkInvisible(u) && u.getAction() != "death" && u.getAction() != "decay" && u.usable == 1) //obviously we can't select ourself as target nor can we select dead target
                        {
                            if (this.meleeUnit()) this.meleeSearchTarget = true;
                            if (u.unitType >= 0 && this.nearEnough(u)) //the other unit is near enough to this unit (aka in range) / removed unitType == 0 later (for test only)
                            {//if a target is within range, compare to already selected target to get nearest target

                                if (found == 0) //we don't have a target yet
                                {
                                    found = 1; //set flag to found
                                    temp.owner = u.owner;
                                    temp.index = u.id;
                                    double xSq = Math.Pow((u.x - this.x), 2);
                                    double ySq = Math.Pow((u.y - this.y), 2);
                                    minDist = Math.Sqrt(xSq + ySq);
                                }
                                else //already had a default target, so now compare the targets to get nearest
                                {
                                    double xSq = Math.Pow((u.x - this.x), 2);
                                    double ySq = Math.Pow((u.y - this.y), 2);
                                    double tempMinDist = Math.Sqrt(xSq + ySq);
                                    if (tempMinDist < minDist) //we found a nearer target
                                    { //set temp to point to new closest target
                                        temp.owner = u.owner;
                                        temp.index = u.id;
                                        minDist = tempMinDist; //set new closest distance
                                    }

                                }
                            }
                        }
                    }
                }
            }

            this.meleeSearchTarget = false; //always turn off melee search after this function
            if (found == 1)
            {//we actually found a target
                this.target = temp; //set the target to our selected target
                return true; //so that we will start battle
            }

            return false;
        }

        public bool QueueUnit(string unitToBuild, ref tempMap gameData)
        {
            Unit temp = new Unit(unitToBuild, 0, 0, this.owner, 0, ref gameData);
            int typeIndex = temp.unitType;
            int goldRequired = SplashScreen.unitData.goldCost[typeIndex];
            int lumberRequired = SplashScreen.unitData.lumberCost[typeIndex];
            int foodRequired = SplashScreen.unitData.foodConsumption[typeIndex];
            //do same check as register
            if (gameData.gold[owner] >= goldRequired && gameData.lumber[owner] >= lumberRequired && temp.unitType < SplashScreen.numUnits && gameData.foodMax[owner] >= foodRequired + gameData.food[owner])
            {
                if (this.creating == null) //only allows for one unit creation
                {
                    //Subtract resources when queueing
                    gameData.gold[owner] -= goldRequired;
                    gameData.lumber[owner] -= lumberRequired;
                    //Queue unit
                    this.creating = unitToBuild;
                    this.inUse = 1;
                    this.timer.Add(temp.buildTime * gameData.DConstructTime); //adds timer of unit that we are building  
                    this.maxUpgradeTime = temp.buildTime * gameData.DConstructTime;
                    gameData.reShowData = 1;

                    //Subtract resource once here (this is temporary, will add back later so that we don't over use resource)
                    /*gameData.gold[owner] -= goldRequired;
                    gameData.lumber[owner] -= lumberRequired;*/
                    //this.timer.Add(1 * gameData.DConstructTime); //this is testing timer
                }
                else return false;
            }
            else return false;


            return true;
        }

        public int getProgress()
        {//returns current progress
            if (this.inUse == 1 && this.timer.Count() != 0) //building is in use
            {
                int curValue = this.timer.ElementAt(0);
                if (this.maxUpgradeTime != 0)
                {
                    int progress = (int)(((double)(this.maxUpgradeTime - curValue) / this.maxUpgradeTime)*100);
                    return progress;
                }

            }

            return -1; //for fail
        }

        public void decTimer(ref List<Unit>[] allUnits, ref tempMap gameData, ref inGameButtons curButtons, ref UnitDatas curLabel)
        { //only need to be called for buildings with timer
            //call this function in game every tick or whatever is used to count time
            //list of all units should be passed in for usage later
            if (this.unitType < SplashScreen.numUnits && this.unitType > -1) //a true unit
            { //run the unit spot block/unblock
                this.unitSpotBlock(ref gameData.pathFindingGriod);

                if (this.startedBattle == true && this.battling == false && this.reloading == 0)
                {
                    if (battle(this.target,ref allUnits, ref gameData))
                    {
                        this.setAction("attack");//set action to attack (should not affect building as building doesn't take action into account)
                        this.onCommand = true; //so we get attack animation

                        attackBack(ref allUnits, ref gameData);
                    }
                    else
                    {
                        this.battling = false; //set to false temporarily for battle check once more
                        if (this.chasing == false) this.currentFrame = 0; //only reset frame for unit when we are not chasing
                        this.chasing = false;
                        if (this.unitType == nameTranslation("Peasant") && (this.gold > 0 || this.lumber > 0)) //peasant with resource
                        {
                            if (this.gold > 0) this.setAction("gold");
                            else this.setAction("lumber");
                        }
                        else this.setAction("walk"); //set to walk animation again
                    }
                }
            }

            if (this.reloading > 0) //alawys update out here as reloading has no relation to if we have timer or not
            { //we have reloading to decrement
                this.reloading -= 1;
            }

            if (rangingUnit() && this.unitType >= SplashScreen.numUnits && reloading == 0 && this.startedBattle == false && this.usable == 1 && this.battling == false && this.target.owner >= 0 && this.target.index >= 0) //for case of building fighting
            {//ranging building with a target
                if (battle(this.target,ref allUnits, ref gameData))
                {
                    this.startedBattle = true;

                    attackBack(ref allUnits, ref gameData);
                }


            }
            else if (rangingUnit() && reloading == 0 && this.startedBattle == false && this.onCommand == false && this.usable == 1) //only check if ranging unit that is ready for battle and we are not moving
            {
                if (findNearestTarget(allUnits, gameData)) //if we were able to find a target that was in range (target was already set by findnearesttarget function
                {
                    if (battle(this.target, ref allUnits, ref gameData))
                    {
                        this.startedBattle = true;
                        if (this.unitType < SplashScreen.numUnits)
                        {
                            this.setAction("attack");//set action to attack (should not affect building as building doesn't take action into account)
                            this.onCommand = true; //so we get attack animation
                        }

                        attackBack(ref allUnits, ref gameData);
                    }
                    else
                    {
                        if (this.unitType < SplashScreen.numUnits)
                        {
                            this.chasing = false;
                            this.battling = false; //set to false temporarily for battle check once more
                            this.currentFrame = 0; //only change frame for unit (as building uses frame for construction)
                            this.setAction("walk"); //set to walk animation again
                        }
                    }
                }
            }

            //if (this.unitType == 3) //goldmine
            
                int numTimers = this.timer.Count();
                for (int i = 0; i < numTimers; i++) //decrement all timers in list once
                {
                    if (this.timer.Count() > i) this.timer[i] -= 1; //check one last time before 
                    if (this.usable == 0 && this.getAction() != "death" && this.getAction() != "decay") //building under construction (and not chop tree nor is death of a unit)
                    {//need to update animation of construction
                        int maxBuildTime = this.buildTime * gameData.DConstructTime; //get max build time
                        int numConstructFrames = getContructFrames();
                        int animateTime = maxBuildTime / numConstructFrames;

                        double ratio = ((double)maxBuildTime - this.timer.ElementAt(0)) / (double)maxBuildTime;
                        if (this.tempHitPoint == -1) this.tempHitPoint = this.curHitPoint; //this.tempHitPoint = this.curHitPoint;
                        if (this.curHitPoint < this.hitPoint)
                        {
                            //this.tempHitPoint += this.hpInc; //add the hp every time (as long as it's less than)
                            //if (this.tempHitPoint > this.hitPoint) this.tempHitPoint = this.hitPoint;
                            //this.curHitPoint = (int)this.tempHitPoint;
                            if (tempHitPoint != 0) //we started with nonzeroHP
                            {
                                int hpRatio = this.hitPoint - (int)this.tempHitPoint; //get ratio of hp we are add
                                this.curHitPoint = (int)(this.tempHitPoint + (hpRatio * ratio));
                                gameData.checkHealthDisplay(this); //refresh hp display if it is the selected unit
                            }
                            else
                            {
                                this.curHitPoint = (int)(this.hitPoint * ratio);
                                gameData.checkHealthDisplay(this); //refresh hp display if it is the selected unit
                            }
                        }
                        //if (this.curHitPoint > this.hitPoint) this.curHitPoint = this.hitPoint;

                        if (this.timer[i] % animateTime == 0) //we reach milestone in construction
                        {
                            animate(gameData); //basically go to next frame of contruction
                        }
                        if (this.timer[i] % 130 == 0 && this.timer[i] % animateTime != 0)
                        {
                            playSound(gameData);  //play building construction sound effect
                        }
                }
                    else if (this.getAction() == "death" && this.unitType > -1 && this.unitType < SplashScreen.numUnits) //death manual animation
                    {
                        int animateTimer = (20 * gameData.DDeathTime)/2;
                        if (this.timer[i] % animateTimer == 0) animate(gameData);
                    }
                    else if (this.getAction() == "decay") //decay manual animation here
                    {
                        //int maxAnimationTime = gameData.DDecayTime * gameData.DConstructTime;
                        //int updateRate = maxAnimationTime / 4; //we have 4 decay frames
                        if (this.unitType > -1) //unit
                        {
                            int divideBy = 1; //default for unit
                            if (this.unitType >= SplashScreen.numUnits) divideBy = 4; //as there will be 16 instead of 4 tiles of decay for building
                            int updateRate = gameData.DDecayTimer / divideBy;
                            if (this.timer[i] % updateRate == 0) //we hit milestone in decaying
                            {
                                animate(gameData);
                            }
                        }
                        else //projectile
                        {
                            int updateRate = gameData.DDecayTimer; //cannonball has exactly 4 death image so no need to /4
                            if (this.timer[i] % updateRate == 0) //we hit milestone in decaying
                            {
                                animate(gameData);
                            }
                        }
                    }
                    checkTimer(0, ref allUnits, ref gameData, ref curButtons, ref curLabel); //always check at zero (most of time we use zero besides goldmine)
                }
            
        }

        private int getContructFrames()
        {//return number of construct frames
            string[] constructStr = { "construct-0", "construct-1", "construct-2" }; //construct-2 usually not used
            int count = 0;
            int index = 0;
            while (index < this.possibleAnimation) //iterate through all possible animations
            {
                switch(SplashScreen.mapUnits.allUnitLines[this.unitType][index + 5])
                {
                    case "construct-0":
                        count++;
                        break;
                    case "construct-1":
                        count++;
                        break;
                    case "constuct-2":
                        count++;
                        break;
                    default:
                        break;
                }
                index++;
            }
            return count;
        }

        private void checkTimer(int index, ref List<Unit>[] allUnits, ref tempMap gameData, ref inGameButtons curButtons, ref UnitDatas curLabel)
        {
            if (0 >= this.timer[index]) //ran out of time
            {
                this.timer.RemoveAt(index); //remove the timer element
                if ((this.getAction() == "death" || this.getAction() == "decay")) //meaning death timer
                {
                    if (this.decaying == false)
                    {
                        this.timer.Clear(); //clear all timers if any on this unit
                        this.decaying = true;
                        this.currentFrame = 0; //reset to 0 frame
                        this.setAction("decay"); //set animation to decay
                        this.timer.Add(gameData.DDecayTime * gameData.DDecayTimer); //add timer for decay
                        this.onCommand = false; //we will do manual animation
                    }
                    else
                    {
                        this.invisible = 1; //turn unit invisible as it finished decaying
                        this.onCommand = false; //no longer animate
                    }
                    /*unitPtr temp = new unitPtr();
                    temp.owner = this.owner;
                    temp.index = this.id;*/
                    //gameData.cleanUpUnits.Add(temp); //add unit for cleanup
                }
                else if (this.unitType == nameTranslation("GoldMine")) //goldmine
                {
                    unGarrison(index, ref allUnits, ref gameData); //ungarrison unit
                    gameData.updateProgress = 1;
                }
                else if (this.usable == 0 && this.unitType >= SplashScreen.numUnits) //done construction
                {
                    playSound(gameData);  //work-completed.wav
                    unGarrison(index, ref allUnits, ref gameData); //ungarrison builders
                    this.curHitPoint = this.hitPoint; //finished building so set HP
                    this.tempHitPoint = -1; //reset to none
                    if (gameData.playerChoice == this.owner) gameData.reShowSingleButton = 1;                    
                }
                else if (this.unitType == nameTranslation("Peasant") && this.currentAction == this.actionTranslation("attack") && this.target.index == -2 && gameData.isTree(this.target.x,this.target.y)) //peasant chopping tree
                {
                    // MessageBox.Show("Done Chopping");
                    //AZ: added * 100 at the end to faster debug 
                    int ret = gameData.RemoveLumber(this.target.x, this.target.y, (int)this.x, (int)this.y, gameData.DLumberPerHarvest*gameData.DChopConstant);
                    //TODO: can update tiles here! Or can do elsewhere with getNewTiles() function in AssetDecoratedMap.cs aka thisMap object
                    //This function can update the terrain
                    //this.target.index = -1; //reset index to -1 

                    if (ret == 1)
                    {
                        //AZ DEBUG: successed chopped the wood, and lumber availability is correctly updated, the x, y are also correct--- 
                        //but the terrain doesn't change, it seems that the terrain data did not update itself. (not the display/graphics problem)
                        //System.Diagnostics.Debug.Print("Done chopping");
                        gameData.getNewTiles(this.target.x, this.target.y);
                    }
                    


                    //Update lumber values
                    this.onCommand = false; //turn off chopping animation
                    this.lumber += gameData.DLumberPerHarvest;
                    this.stopAnimation(); //reset to frame 0
                    this.setAction("lumber"); //set to holding lumber animation
                    
                    unitPtr goTo = gameData.findNearestBuilding(allUnits, this, "lumber");
                    if (goTo.owner != -1) //we actually found a  building
                    {
                        //MessageBox.Show("Found building");
                        this.invisible = 1; //set invisible to true so we don't get stuck on tree
                        float setX = allUnits[goTo.owner][goTo.index].x;
                        float setY = allUnits[goTo.owner][goTo.index].y;
                        //Set previoustarget to the tree (only if there is more to chop)
                        if (ret == 0)
                        {
                            this.prevTarget.owner = 0; //0 for nature as nature owns tree
                            this.prevTarget.index = -1;
                            this.prevTarget.x = this.target.x;
                            this.prevTarget.y = this.target.y;
                        }
                        else //ret == 1
                        {
                            //unitPtr temp = gameData.findNearestTree(this.prevTarget);
                            unitPtr temp = gameData.findNearestTree(this.target, this, 3, allUnits);
                            if (temp.owner == 0 && temp.index == -1) //we found a new tree
                            {//so set the target we go back to after depositing resource to new tree pointer
                                this.prevTarget = temp;
                            }
                            else this.prevTarget.reset(); //reset the target as there is no more tree found
                        }
                        this.target = goTo;
                        
                        this.xToGo = (int)setX;
                        this.yToGo = (int)setY;
                        this.findNearestOpenSpot(allUnits, ref gameData);
                        this.onCommand = true; //tell unit to go to building
                    }
                    //this.invisible = 0;

                }
                else if (this.creating != null && this.unitType >= SplashScreen.numUnits) //we area creating an unit
                {//hopefully this timer no conflict with others
                    unitCreateData temp = new unitCreateData();
                    temp.owner = this.owner;
                    temp.curBuilding = this;
                    temp.unitType = this.creating;
                    this.inUse = 0;

                    gameData.unitsToCreate.Add(temp);
                    if (gameData.playerChoice == this.owner) gameData.reShowSingleButton = 1;
                    //this.creating = null; //set to null
                }
                else if (this.upgradeIndex != -1 && this.usable == 1) //upgrading
                {
                    endUpgrade(ref gameData, ref allUnits);
                }
                else if ((this.battling == true) && this.usable == 1 && this.unitType >= 0) //unit is battling or chasing another unit || this.chasing == true)
                { //we finished one attack step so calc damage here and deal the damage
                    this.doDamage(ref allUnits, ref gameData, ref curButtons, ref curLabel);
                }
                else if (this.unitType < -1 && this.chasing == true) //projectile
                {
                    projectileChase(ref allUnits, gameData);
                }
            }
        }

        public void garrisonIn(int index, int otherOwner, ref List<Unit>[] allUnits, tempMap gameData)
        { //call by a unit onto other building
            //function should only be called for buildings that have garrison function
            //call this function AFTER walking up to the building
            //allUnits is the list containing all units on the map

            unitPtr temp = new unitPtr();
            temp.index = this.id;
            temp.owner = this.owner;
            this.invisible = 1; //turn this unit invisible

            if (allUnits[otherOwner][index].unitType == nameTranslation("GoldMine"))
            {
                if (allUnits[otherOwner][index].garrison.Count() == 0) //only change animation if light is off aka no units inside
                {
                    allUnits[otherOwner][index].animate(gameData); //change animation to active if first unit in
                    this.goldMining = true;
                }

                //gameData.deselectUnit(temp); //already deselected above
                allUnits[otherOwner][index].timer.Add(gameData.DMiningTime * gameData.DConstructTime); //adds timer (5 default) to list                             
                //allUnits[this.owner].Remove(this); //remove unit from list as already in another list?

            }
            else if (allUnits[otherOwner][index].usable == 0 && allUnits[otherOwner][index].garrison.Count() == 0) //construction mode and no units inside garrison yet
            {
                //allUnits[otherOwner][index].timer.Clear(); //clear timers (as there should only be one timer anyways)
                this.constructing = true;
                allUnits[otherOwner][index].timer.Add(this.buildTime * gameData.DConstructTime); //adds timer (10 default) to list for construction time
                allUnits[otherOwner][index].maxUpgradeTime = this.buildTime * gameData.DConstructTime;
                if (gameData.playerChoice == this.owner) gameData.reShowSingleButton = 1;
                //gameData.deselectUnit(temp); //already deselected above
                //each building might have different construction time and is definite longer than above (which is for testting only)
            }
            allUnits[otherOwner][index].garrison.Add(temp); //adds unit to garrison (not sure if work)
            //else nothing happens 
        }

        public void unGarrison(int index, ref List<Unit>[] allUnits, ref tempMap gameData)
        { //call on building unit when you want to put unit back on map
            //function should only be called for buildings that have garrison function
            //call this function for example after mining timer or when clicking on unit button in buildings menu
            //allUnits is the list containing all units on the map
            //the index should be 0 by default for first unit, but can also be other units (of their index inside building)
            //can set movements outside of this
            if (0 <= index && index < this.garrison.Count()) //make sure index is within bounds
            {
                //allUnits[this.garrison[index].owner].Add(this.garrison[index]); //add unit back to main list
                unitPtr temp = garrison.ElementAt(index);
                this.garrison.RemoveAt(index); //remove unit from garrison

                allUnits[temp.owner][temp.index].target.reset(); //reset the target to blank

                if (this.unitType == nameTranslation("GoldMine")) //goldMine ungarrison event
                {
                    if (this.gold >= gameData.DGoldPerMining && allUnits[temp.owner][temp.index].gold < gameData.DGoldPerMining)
                    { //if we have enough gold to give and the unit can carry more gold
                        this.gold -= gameData.DGoldPerMining;
                        allUnits[temp.owner][temp.index].gold += gameData.DGoldPerMining;
                        unitPtr goTo = gameData.findNearestBuilding(allUnits, allUnits[temp.owner][temp.index], "gold");
                        if (goTo.owner != -1) //we actually found a  building
                        {
                            float setX = allUnits[goTo.owner][goTo.index].x;
                            float setY = allUnits[goTo.owner][goTo.index].y;
                            allUnits[temp.owner][temp.index].target = goTo;
                            allUnits[temp.owner][temp.index].prevTarget.owner = this.owner;
                            allUnits[temp.owner][temp.index].prevTarget.index = this.id;
                            allUnits[temp.owner][temp.index].xToGo = (int)setX;
                            allUnits[temp.owner][temp.index].yToGo = (int)setY;
                            allUnits[temp.owner][temp.index].findNearestOpenSpot(allUnits, ref gameData);
                            allUnits[temp.owner][temp.index].onCommand = true;
                        }
                        else allUnits[temp.owner][temp.index].invisible = 0; //set back to not invisible as we can't find building

                        allUnits[temp.owner][temp.index].goldMining = false;

                    }
                }
                else if (this.usable == 0) //construction ungarrison event
                {//should ungarrison all units at once (don't wry about multiple units for now)
                    this.usable = 1; //we are done building so set building to usable
                    allUnits[temp.owner][temp.index].invisible = 0; //set back to not invisible as we let builder exit
                    allUnits[temp.owner][temp.index].constructing = false;
                    gameData.updateFlag = 1; //call update on resource in case building provides population
                }
                //allUnits[temp.owner][temp.index].invisible = 0; //set back to not invisible


                if (this.garrison.Count() == 0) this.animate(gameData); //set image back to inactive (once no more garrisoned units)
            }

            //else nothing happens 
        }

        public int compareOwner(Unit other)
        {//function to compare passed in owner with this object's owner
            //Call this function when you have unit selected and right clicking on another unit
            //the other should be ONE unit from the owner of the units selected (which should be uniform aka same I think)
            //Probably pass in the first unit in the chosen list
            //return flags are designated 0=nature, 1=same owner, 2=enemy (can add more flags for allies)
            if (other.owner == 0) return 0;
            else if (other.owner == this.owner) return 1;
            else return 2;
        }

        public string getType()
        { //return unit type as string
            string[] assetTypes = SplashScreen.allUnitNames;
            return assetTypes[this.unitType];
        }

        public string getDirection()
        {
            string[] directionstrings = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
            return directionstrings[direction];

        }

        public string getAction()
        {
            string[] actionstrings = { "walk", "attack", "gold", "lumber", "death" };
            if (this.unitType != nameTranslation("Peasant") && this.currentAction >= 2) return actionstrings[this.currentAction+2]; //to skip lumber and gold
            if (this.currentAction == -1) return "decay";
            return actionstrings[this.currentAction];

        }

        public void stopAnimation() //stops the animation and reset to standing frame
        { //call this when reached destination for unit
            this.currentFrame = 0;
            updateImg();
        }

        public void resetAnimation()
        {//reset to walking animation (for interrupting tasks like wood chopping or attacking)

            if (this.unitType < SplashScreen.numUnits)
            {
                this.timer.Clear(); //clear all timers here
                this.target.reset();
                this.currentFrame = 0; //originally commented out
                                       //this.timer.Clear(); //clear all timers cause interrupt
                if (this.unitType == nameTranslation("Peasant") && (this.gold > 0 || this.lumber > 0)) //peasant with resource
                {
                    if (this.gold > 0) this.setAction("gold");
                    else this.setAction("lumber");
                }
                else this.setAction("walk"); //set to walk animation again
                this.patrolling = false;
                this.repairing = false;
                //reset battle flags
                this.battling = false;
                this.startedBattle = false;
                this.chasing = false;
                this.selfDestroy = false; //reset so we can't attack friendly units anymore

                updateImg();
            }
            
        }

        public void updateDir()
        {
            //update dir
            Unit u = this;
            float dx = u.xToGo - u.x;
            float dy = u.yToGo - u.y;
            if (dy == 0) { dy = 0.0001f; }
            if (dx == 0) { dx = 0.0001f; }
            if (dy < 0)//if facing north
            {
                if (Math.Abs(dx / dy) < 0.5)
                {
                    u.setDirection("N");
                }
                else if (dx < 0)
                { // to west

                    if (Math.Abs(dy / dx) < 0.5)//purely west
                    {
                        u.setDirection("W");
                    }
                    else
                    {
                        u.setDirection("NW");
                    }

                }
                else
                { // to east
                    if (Math.Abs(dy / dx) < 0.5)
                    {
                        u.setDirection("E");
                    }
                    else
                    {
                        u.setDirection("NE");
                    }
                }

            }
            else
            {
                if (Math.Abs(dx / dy) < 0.5)
                {
                    u.setDirection("S");
                }
                else if (dx < 0)
                { // to west

                    if (Math.Abs(dy / dx) < 0.5)// purely west
                    {
                        u.setDirection("W");
                    }
                    else
                    {
                        u.setDirection("SW");
                    }

                }
                else
                { // to east
                    if (Math.Abs(dy / dx) < 0.5)
                    {
                        u.setDirection("E");
                    }
                    else
                    {
                        u.setDirection("SE");
                    }
                }

            }


        }

        public void updateNextDir()
        {
            //update dir
            Unit u = this;
            float dx = u.xNextStop - u.x;
            float dy = u.yNextStop - u.y;
            if (dy ==0 && dx ==0) { u.setDirection(u.getDirection());return; }
            if (dy == 0) { dy = 0.0001f; }
            if (dx == 0) { dx = 0.0001f; }
            if (dy < 0)//if facing north
            {
                if (Math.Abs(dx / dy) < 0.5)
                {
                    u.setDirection("N");
                }
                else if (dx < 0)
                { // to west

                    if (Math.Abs(dy / dx) < 0.5)//purely west
                    {
                        u.setDirection("W");
                    }
                    else
                    {
                        u.setDirection("NW");
                    }

                }
                else
                { // to east
                    if (Math.Abs(dy / dx) < 0.5)
                    {
                        u.setDirection("E");
                    }
                    else
                    {
                        u.setDirection("NE");
                    }
                }

            }
            else
            {
                if (Math.Abs(dx / dy) < 0.5)
                {
                    u.setDirection("S");
                }
                else if (dx < 0)
                { // to west

                    if (Math.Abs(dy / dx) < 0.5)// purely west
                    {
                        u.setDirection("W");
                    }
                    else
                    {
                        u.setDirection("SW");
                    }

                }
                else
                { // to east
                    if (Math.Abs(dy / dx) < 0.5)
                    {
                        u.setDirection("E");
                    }
                    else
                    {
                        u.setDirection("SE");
                    }
                }

            }


        }

        public void unitSpotBlock(ref RoyT.AStar.Grid g)
        { //this function will block the unit's current position 
            if ((int)this.x != prevX || (int)this.y != prevY)
            {//if our unit's coordinate has changed, aka unit has moved
                if (this.prevX != -1 && this.prevY != -1) g.UnblockCell(new RoyT.AStar.Position(prevX, prevY)); //unblock previous position
                //Set new previous to current coordinates
                prevX = (int)this.x;
                prevY = (int)this.y;
                g.BlockCell(new RoyT.AStar.Position((int)this.x,(int)this.y)); //block current position
            }
        }

        //New a* path finding algorithm!
        public void updateDir(RoyT.AStar.Grid g)
        {
            //update dir
            Unit u = this;
            if (u.xToGo <0 || u.yToGo < 0) { return; }

            if (xNextStop >= 0 && yNextStop >= 0)
            {
                if (Math.Abs(u.x - xNextStop)+ Math.Abs(u.y - yNextStop) < 0.1f)
                {
                    u.x = xNextStop;
                    u.y = yNextStop;
                    //do the path finding job and find the xNextStop and yNextStop
                }
                else {
                    updateNextDir();
                    return;
                }
            }

            RoyT.AStar.Position[] path = g.GetPath(new RoyT.AStar.Position((int)u.x, (int)u.y), new RoyT.AStar.Position(u.xToGo, u.yToGo));
            if (path != null && path.Length > 0)
            {
                RoyT.AStar.Position nextPath = path[0];
                int dx = nextPath.X - (int)u.x;
                int dy = nextPath.Y - (int)u.y;
                if (dy == 0 && dy == 0 && path.Length > 1)
                {
                    nextPath = path[1];
                }
                /*if (dx == 0 && dy ==-1) { u.setDirection("N"); }
                if (dx == 1 && dy == -1) { u.setDirection("NE"); }
                if (dx == 1 && dy == 0) { u.setDirection("E"); }
                if (dx == 1 && dy == 1) { u.setDirection("SE"); }
                if (dx == 0 && dy == 1) { u.setDirection("S"); }
                if (dx == -1 && dy == 1) { u.setDirection("SW"); }
                if (dx == -1 && dy == 0) { u.setDirection("W"); }
                if (dx == -1 && dy == -1) { u.setDirection("NW"); }*/
                xNextStop = nextPath.X;
                yNextStop = nextPath.Y;
                updateNextDir();
            }
            else {
                //When click a unpathable place... Week10 by AZ
                //No action to avoid animation crashes
                xNextStop = u.xToGo;
                yNextStop = u.yToGo;
                this.updateNextDir();
                this.onCommand = false;
            }

        }

        private void playSound(tempMap gameData)
        {//function to play dynamic sound (currently private as we don't really need to call sound playing outside)
            //need to pass in viewport data and gameData (where onscreen function is held) (name corresponds with names in XNA)
            System.IO.FileStream fs = null;
            if (gameData.OnScreen(gameData.viewW, gameData.viewH, gameData.screenWidth, gameData.screenHeight, this.x, this.y, this.objImg2D.Width, this.objImg2D.Height))
            { //we are on screen, so play sound
                if (this.unitType == 0) //peasant sounds
                {
                    if (this.actionTranslation("attack") == this.currentAction) //unit is attacking
                    {//implies we have target of some sort
                        if (this.target.index == -2) //we are chopping tree
                        {
                            int choose = new Random().Next(0, 3);

                            fs = new System.IO.FileStream(@"data\snd\misc\tree" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);
                        }
                    }
                }
                else if (meleeUnit() && this.battling == true)
                {//melee unit battle sound
                    int choose = new Random().Next(0, 2);

                    fs = new System.IO.FileStream(@"data\snd\misc\sword" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);
                }
                else if (this.usable == 0 && this.getAction() != "death" && this.getAction() != "decay")  //need different condition to check this
                {//building construction sound
                    fs = new System.IO.FileStream(@"data\snd\misc\construct.wav", System.IO.FileMode.Open);
                }
                else if (this.getAction() == "decay" && this.unitType >= SplashScreen.numUnits)
                {//building explode sound
                    int choose = new Random().Next(0, 2);

                    fs = new System.IO.FileStream(@"data\snd\misc\building-explode" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);
                }
                else if (this.usable == 0 && this.unitType >= SplashScreen.numUnits)
                {//done construction 
                    fs = new System.IO.FileStream(@"data\snd\basic\work-completed.wav", System.IO.FileMode.Open);
                }
                else if (this.unitType == nameTranslation("Arrow") && this.invisible == 1)
                {//archer arrow hit sound
                    fs = new System.IO.FileStream(@"data\snd\misc\bowhit.wav", System.IO.FileMode.Open);
                }
                else if (rangingUnit() && this.unitType == nameTranslation("Archer"))
                {//archer arrow shooting sound
                    fs = new System.IO.FileStream(@"data\snd\misc\bowfire.wav", System.IO.FileMode.Open);
                }
                else if (this.unitType == nameTranslation("Cannonball") && this.invisible == 1)
                {//cannon ball shooting sound
                    fs = new System.IO.FileStream(@"data\snd\misc\cannonhit.wav", System.IO.FileMode.Open);
                }
                else if (rangingUnit() && this.unitType == nameTranslation("Cannonball"))
                {//archer arrow shooting sound
                    fs = new System.IO.FileStream(@"data\snd\misc\cannonfire.wav", System.IO.FileMode.Open);
                }
                /*else if (this.battling == true && repairing == false)
                {//building help
                    if (this.battling == true && playsoundflag == false)
                    {
                        fs = new System.IO.FileStream(@"data\snd\basic\building-help.wav", System.IO.FileMode.Open);
                        playsoundflag = true;
                    }
                    if (playsoundflag == true && this.battling == false) playsoundflag = false;
                }*/
            }
            if (fs != null)
            {
                SoundEffect mysound = SoundEffect.FromStream(fs);
                fs.Dispose();
                mysound.Play(volume: 0.01f * SoundOptionsMenu.pubVolSFX, pitch: 0.0f, pan: 0.0f);
            }
        }

        public void playSound()
        { //moved sound playing here, can add to this function for each unit if different sounds            

            System.IO.FileStream fs = null;
            if (this.unitType == nameTranslation("Archer"))
            {
                int choose = new Random().Next(0, 3);

                fs = new System.IO.FileStream(@"data\snd\archer\acknowledge" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);
            }
            else if (this.unitType == nameTranslation("Peasant"))
            {
                int choose = new Random().Next(0, 3);

                fs = new System.IO.FileStream(@"data\snd\peasant\acknowledge" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);
            }

            else if (this.unitType < SplashScreen.numUnits) //for units
            {
                //random sound command (moved inside unit movement only instead of all terrain)
                //need to fix so that if unit already playing sound, don't play another (as that could lead to crash)
                //we probably don't want multiple units playing same sound at same time either
                int choose = new Random().Next(0, 3);
                fs = new System.IO.FileStream(@"data\snd\basic\acknowledge" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);                
            }
            else //for buildings, or add other cases if needed
            {

            }

            if (fs != null)
            {
                SoundEffect mysound = SoundEffect.FromStream(fs);
                fs.Dispose();
                mysound.Play(volume: 0.01f * SoundOptionsMenu.pubVolSFX, pitch: 0.0f, pan: 0.0f);
            }
        }

        public void setDirection(string dir)
        {
            /*if (this.unitType < -1)  //a projectile
            {
                if (this.direction == -1) //only set direction first time and never change again
                {
                    this.direction = dirTranslation(dir);
                }
            }
            else this.direction = dirTranslation(dir); //a true unit*/
            this.direction = dirTranslation(dir);
            updateImg();
        }

        public void setAction(string action)
        { //call this when actually doing the action (lumber action is after starting to chop tree and got wood)
            this.currentAction = actionTranslation(action);
            updateImg();
        }

        public void animate(tempMap gameData)
        { //repeatedly call this function over timer (in addition to getting image constantly) for animation
            if (this.getAction() == "death") //killed off
            {
                if (this.currentFrame < 2) //only 3 frame for death animation
                {
                    this.currentFrame++;
                }
            }
            else if (this.getAction() == "decay")
            {
                if (this.unitType < SplashScreen.numUnits && this.unitType > -1)
                { //unit decay
                    if (this.currentFrame < 3) //only 3 frame for death animation
                    {
                        this.currentFrame++;
                    }
                }
                else if (this.unitType >= SplashScreen.numUnits)
                { //building decay
                    if (this.currentFrame < 15) //only 16 frame for death animation
                    {
                        this.currentFrame++;
                    }
                    if (this.currentFrame == 1) this.playSound(gameData);
                }
                else
                { //cannonball decay
                    if (this.currentFrame < 3) //4 frames max (0-3 index)
                    {
                        this.currentFrame++;
                    }
                }
            }
            else if (this.unitType < SplashScreen.numUnits && this.unitType > -1) //for unit only
            {
                this.currentFrame++; //(5 frames total 0-4)
                if (rangingUnit())
                {
                    if (this.currentFrame > 1) //for ranging unit only, if hit frame 2, restart
                    { //cycle
                        this.currentFrame = 0;
                    }
                    //if (this.currentFrame == 0) this.playSound(gameData); //for playing animation sound if any (edit playSound function for other sounds)
                }
                else
                {
                    if (this.currentFrame > 4) //for unit only, if hit frame 5, restart
                    { //cycle
                        this.currentFrame = 0;
                    }
                    //if (this.currentFrame == 0) this.playSound(gameData); //for playing animation sound if any (edit playSound function for other sounds)
                }
            }
            else if (this.unitType < -1) //projectiles (not cannonball decay)
            {
                if (this.unitType == -2) //arrow
                {
                    this.currentFrame++;
                    if (this.currentFrame > 1) //only 2 frames for arrow
                    {
                        this.currentFrame = 0; //set back to zero for arrow to rotate animation
                    }
                }
                else //cannonball
                {
                    if (this.currentFrame < 3) //only 4 frames max for cannonball and it doesn't repeat
                    {
                        this.currentFrame++;
                    }
                }
            }
            else //for building animation (only call to update animation once instantly)
            {
                this.currentFrame = nextBuildingIndex();
            }
            updateImg();
        }

        private void updateImg()
        {
            //(8 different directions, 5 frames per direction) for each action
            this.invisible = 0;
            if (this.unitType < SplashScreen.numUnits && this.unitType > -1)
            {
                //int imageIndex = currentAction * 40 + direction * 5 + currentFrame; //*5 no *4
                int imageIndex;
                if (this.getAction() == "death")
                { //NOTE: There is no building death image (I can't find them, only decay image which is called building death)
                    int offset = 0;
                    offset = (this.projectileImgDir + 1) / 2 - 1;
                    if (rangingUnit())
                    {
                        imageIndex = 40 + 2*8 + offset * 3 + currentFrame;
                    }
                    else imageIndex = currentAction * 40 + offset * 3 + currentFrame;
                }
                else if (this.getAction() == "decay")
                {
                        int offset = 0;
                        offset = (this.projectileImgDir + 1) / 2 - 1;
                        imageIndex = offset * 4;
                }
                else if (rangingUnit())
                { //only 2 frames for attack for ranging units (hardcode)
                    if (this.getAction() == "attack")
                    {
                        imageIndex = 40 + direction * 2 + currentFrame; //for attack frames
                    }
                    else imageIndex = currentAction * 40 + direction * 5 + currentFrame; //for normal walk
                }
                else imageIndex = currentAction * 40 + direction * 5 + currentFrame;

                if (this.getAction() == "decay" && this.unitType < SplashScreen.numUnits) this.objectImage = SplashScreen.mapUnits.decayTiles[0][imageIndex + currentFrame];
                else this.objectImage = SplashScreen.mapUnits.allUnitTiles[this.unitType][imageIndex];
                


                //if texture 2d converted
                if (SplashScreen.renderColor == 1)
                {
                    if (this.owner > 0)
                    {
                        //int colorChoice = SplashScreen.playerColor[this.owner - 1];
                        int colorChoice = this.owner - 1;
                        if (this.decaying == true) this.objImg2D = SplashScreen.mapUnits.decayTiles2D[imageIndex + currentFrame];
                        else this.objImg2D = SplashScreen.coloredUnits.allUnitTilesT2dColored[colorChoice][this.unitType][imageIndex];
                    }
                    else
                    {
                        this.objImg2D = SplashScreen.mapUnits.allUnitTilesT2d[this.unitType][imageIndex];
                    }
                }
                else if (SplashScreen.mapUnits.allUnitTilesT2d.Length > 0)
                {
                    this.objImg2D = SplashScreen.mapUnits.allUnitTilesT2d[this.unitType][imageIndex];
                }
            }
            else if (this.unitType >= SplashScreen.numUnits)
            { ///A building case.
                int imageIndex = currentFrame;
                if (decaying == false) this.objectImage = SplashScreen.mapUnits.allUnitTiles[this.unitType][imageIndex];
                this.unitWidth = this.objectImage.Width;
                this.unitHeight = this.objectImage.Height;
                this.unitTileWidth = (int)Math.Round(1.0f * this.unitWidth / SplashScreen.tileObject.tileWidth);
                this.unitTileHeight = (int)Math.Round(1.0f * this.unitHeight / SplashScreen.tileObject.tileHeight);

                //if t2d have already been loaded
                if (SplashScreen.renderColor == 1)
                {
                    if (this.owner > 0)
                    {
                        //int colorChoice = SplashScreen.playerColor[this.owner - 1];
                        int colorChoice = this.owner - 1;
                        if (this.decaying == true) this.objImg2D = SplashScreen.mapUnits.buildingDeathTiles2D[this.unitType-SplashScreen.numUnits][imageIndex];
                        else this.objImg2D = SplashScreen.coloredUnits.allUnitTilesT2dColored[colorChoice][this.unitType][imageIndex];
                    }
                    else
                    {
                        if (this.decaying == true) this.objImg2D = SplashScreen.mapUnits.buildingDeathTiles2D[this.unitType - SplashScreen.numUnits][imageIndex];
                        else this.objImg2D = SplashScreen.mapUnits.allUnitTilesT2d[this.unitType][imageIndex];
                    }

                    //set preview image for building
                    this.objPreviewImg2D = SplashScreen.mapUnits.allUnitTilesT2d[this.unitType][SplashScreen.mapUnits.allUnitTilesT2d[this.unitType].Length-1];


                    //Update the image size so that it can be selected.
                    this.unitWidth = this.objImg2D.Width;
                    this.unitHeight = this.objImg2D.Height;
                    this.unitTileWidth = (int)Math.Round(1.0f * this.unitWidth / SplashScreen.tileObject.tileWidth);
                    this.unitTileHeight = (int)Math.Round(1.0f * this.unitHeight / SplashScreen.tileObject.tileHeight);
                }
                else if (SplashScreen.mapUnits.allUnitTilesT2d.Length > 0)
                {
                    if (this.decaying == true) this.objImg2D = SplashScreen.mapUnits.buildingDeathTiles2D[this.unitType - SplashScreen.numUnits][imageIndex];
                    else this.objImg2D = SplashScreen.mapUnits.allUnitTilesT2d[this.unitType][imageIndex];

                    this.objPreviewImg2D = SplashScreen.mapUnits.allUnitTilesT2d[this.unitType][SplashScreen.mapUnits.allUnitTilesT2d[this.unitType].Length - 1];

                    //Update the image size so that it can be selected.
                    this.unitWidth = this.objImg2D.Width;
                    this.unitHeight = this.objImg2D.Height;
                    this.unitTileWidth = (int)Math.Round(1.0f * this.unitWidth / SplashScreen.tileObject.tileWidth);
                    this.unitTileHeight = (int)Math.Round(1.0f * this.unitHeight / SplashScreen.tileObject.tileHeight);
                }
            }
            else //projectile
            {
                if (this.unitType == -2) //arrow case
                {
                    int imageIndex = projectileImgDir + currentFrame * 8; //arrow has 16 frames total, 8 frames per set of direction
                    this.objectImage = SplashScreen.mapUnits.allProjectiles[0][imageIndex];
                    this.objImg2D = SplashScreen.mapUnits.arrowTiles2D[imageIndex];
                }
                else if (this.unitType == -3) //cannnonball
                {
                    int imageIndex = currentFrame; //4 frames total for each: alive and death
                    this.objectImage = SplashScreen.mapUnits.allProjectiles[1][imageIndex]; //don't have to worry about decay
                    if (decaying == true) this.objImg2D = SplashScreen.mapUnits.cannonBallTiles2D[1][imageIndex];
                    else this.objImg2D = SplashScreen.mapUnits.cannonBallTiles2D[0][imageIndex];
                }
                else if (this.unitType == nameTranslation("gold")) //gold case
                {
                    int imageIndex = currentFrame; //dropped resource has 3 frames each
                    this.objectImage = SplashScreen.mapUnits.droppedResource[imageIndex];
                    this.objImg2D = SplashScreen.mapUnits.droppedResource2D[imageIndex];
                }
                else //if (this.unitType == nameTranslation("lumber")) //gold case
                {
                    int imageIndex = 3 + currentFrame; //dropped resource has 3 frames each, so start at offset of index 3
                    this.objectImage = SplashScreen.mapUnits.droppedResource[imageIndex];
                    this.objImg2D = SplashScreen.mapUnits.droppedResource2D[imageIndex];
                }

                this.unitWidth = this.objImg2D.Width;
                this.unitHeight = this.objImg2D.Height;
                this.unitTileWidth = (int)Math.Round(1.0f * this.unitWidth / SplashScreen.tileObject.tileWidth);
                this.unitTileHeight = (int)Math.Round(1.0f * this.unitHeight / SplashScreen.tileObject.tileHeight);
            }
        }

        private int dirTranslation(string dir)
        { //function to return index in array of asset type (basically a map)
            string[] directionstrings = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
            int index = this.direction; //default direction

            for (int i = 0; i < 8; i++)
            {
                if (directionstrings[i] == dir)
                {
                    index = i;
                    return index;
                }
            }

            return index;
        }

        private string dirTranslation(int dir)
        { //function to return index in array of asset type (basically a map)
            string[] directionstrings = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };

            return directionstrings[dir];
        }

        private void findInactive()
        { //finds the inactive building index and set our current frame to it 
            //this allows us to spawn building at start correctly
            //also can allow for instabuild cheat
            //NOTE: some buildings don't have inactive string which will cause error in this function
            int frame = this.currentFrame;
            string temp = SplashScreen.mapUnits.allUnitLines[this.unitType][frame + 5];
            while (temp != "inactive" && temp != "inactive-0") //inactive-0 is for walls I think
            {
                frame++;
                temp = SplashScreen.mapUnits.allUnitLines[this.unitType][frame + 5];
            }
            this.currentFrame = frame;

        }

        public void setBuildFrame(int mode)
        { //set default building frame depending on mode: mode 0 = placing frame, mode 1 = construct frame
            string[] Nextstrings = { "construct-0", "construct-1", "inactive", "active", "place" };
            int index = 0;
            int flag = 0;
            while(index < this.possibleAnimation && flag == 0)
            {
                if (SplashScreen.mapUnits.allUnitLines[this.unitType][index + 5] == "place" && mode == 0) //found place frame
                {
                    this.currentFrame = index;
                    flag = 1;
                }
                if (SplashScreen.mapUnits.allUnitLines[this.unitType][index + 5] == "construct-0" && mode == 1) //found construct frame
                {
                    this.currentFrame = index;
                    //this.currentFrame = 0; //usually the build frame, if not, then we refactor this code
                    this.usable = 0; //set it so building is not usable until done building
                    if (this.curHitPoint == this.hitPoint) //default build
                    {
                        this.curHitPoint = 0; //set to zero as we started construction
                    }
                    //otherwise, it will be building upgrade where we keep the old HP until finished upgrading
                    flag = 1;
                }
                index++;

            }

           
            if (flag == 0) this.currentFrame = 0; //set to inactive frame instead
            this.updateImg(); //always update the image whether we found frame or not

        }

        private int nextBuildingIndex()
        { //function to return index in array of asset type (basically a map)
            string[] Nextstrings = { "construct-0", "construct-1", "inactive", "active", "place" };
            int index = this.currentFrame; //default direction
            int frame = this.currentFrame;
            if (frame < this.possibleAnimation)
            {
                if (SplashScreen.mapUnits.allUnitLines[this.unitType][frame + 5] == "inactive") //on inactive animation
                {
                    if ((frame+5+1) < SplashScreen.mapUnits.allUnitLines[this.unitType].Length && SplashScreen.mapUnits.allUnitLines[this.unitType][frame + 5 + 1] == "active") //one of the animation buildings
                    {
                        return index + 1;
                    }
                    else return index; //no animation
                }
                else if (SplashScreen.mapUnits.allUnitLines[this.unitType][frame + 5] == "active") //on active animation
                {
                    return index - 1; //go back to inactive animation (doesn't work for walls)
                }
                else if (SplashScreen.mapUnits.allUnitLines[this.unitType][frame + 5] == "place")
                {
                    return 0; //return the construction index (as we just placed it for building)
                }
                else return index + 1;
            }

            return index; //returns same index if at last animation because loop won't run
        }

        private int actionTranslation(string action)
        { //function to return index in array of asset type (basically a map)
            string[] actionstrings = { "walk", "attack", "gold", "lumber", "death" };
            int index = this.currentAction; //default direction

            if (action == "decay") return -1; //return index -1 for decay
            {
                for (int i = 0; i < 5; i++)
                {
                    if (actionstrings[i] == action)
                    {
                        index = i;
                        if (action == "death" && this.unitType != nameTranslation("Peasant")) index -= 2; //only peasant has gold and lumber
                        return index;
                    }
                }
            }

            return index;
        }

        private int nameTranslation(string name)
        { //function to return index in array of asset type (basically a map)
            string[] assetTypes = SplashScreen.allUnitNames;
            string[] projectileTypes = SplashScreen.allProjectilesNames;
            string[] droppedRes = SplashScreen.droppedResourceNames;
            int index = -1; //index of asset name type, -1 = none

            for (int i = 0; i < assetTypes.Length; i++)
            {
                if (assetTypes[i] == name)
                {
                    index = i;
                    return index;
                }
            }

            if (index == -1) //passed in value is not assetType, so check projectile
            {
                for (int i = 0; i < projectileTypes.Length; i++)
                {
                    if (projectileTypes[i] == name)
                    {
                        index = 0 - 2 - i; //negative index, starting at -2 for arrow, -3 for cannonball
                        return index;
                    }
                }
            }

            if (index == -1) //not projectile or unit so check if resource drop
            {
                for (int i = 0; i < droppedRes.Length; i++)
                {
                    if (droppedRes[i] == name)
                    {
                        index = 0 - 4 - i; //negative index, starting at -4 for gold, -5 for lumber
                        return index;
                    }
                }
            }
            return index;
        }
    }
}
