using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
//using System.Windows.Forms; //for testing only on this at least

namespace Demo
{
    class AssetDecoratedMap
    { //filler class for name of file?
    }

    public class unitCreateData
    {
        public string unitType;
        public int owner;
        public Unit curBuilding;
    }

    public class unitPtr //class for unit pointer data cause it was used so often
    {//NOTE: not all parts implemented this yet so some still needs to be changed
        public int owner; //using index -1 for no pointer
        public int index; //will be used index -1 for tree

        //use for storing location of targets that are not units (like tree) (might not need)
        public int x;
        public int y;
        
        public unitPtr()
        { //init
            this.index = 0;
            this.owner = -1; //default no owner
        }

        public void reset()
        {
            this.index = 0;
            this.owner = -1;
        }
    }
    
    public class tempMap //class to hold in game data
    {
        public EntireMapsIndex theMapIndex;//AZ: use this to change terrain, when remove the forest, this variable's value will be changed.
        public RoyT.AStar.Grid pathFindingGriod;
        //public int updateMapFlag = 1; //flag used to limit drawthemap() function, default 1 to draw once at start
        public GraphicsDevice curDevice;
        public List<unitCreateData> unitsToCreate; //list that we push units to so less conflicts for creation
        public int updateFlag = 1; //flag used to limit updates on resource labels (start with 1 for default update on load)
        public int updateHealth = 0;
        public int updateProgress = 0;
        public int reShowSingleButton = 0;
        public int reShowData = 0;
        public int reShowMultiData = 0;
        public int reShowMultiButtons = 0;

        public int playerChoice = 0;


        //For unit upgrade
        public AssetData unitDataCopy;
        public int[] allUpgrades; //holds flag for all *unit* upgrades (if it was already upgraded) : 3 flags: 0=no;1=upgrading;2=done
        public int updateUnitFlag = 0; //to determinue when to update all unit data (for after upgrades)
        public int[][] unitsToUpdate; //holds flag for which units to update

        //Fog of War Variables
        public int renderFogFlag = 1;
        public int[] fogMap; //default all 0 when init
        public int[] exploredMap; //holds all explored position of map

        public int[] tempMapData; //holds temporary copy of our current map (one we can edit in game);
        public int[] tempMapDataIndices; //holds temp indices
        public int mapW;
        public int mapH;
        private int[] DLumberAvailable;
        private int[] tempMapPartials; //hold partial map data
        private string[] tempMapLines; //hold map string data
        public int numPlayers; //holds the number of players

        //Resource data holding for each player
        public int[] gold; //hold ingame gold for each player
        public int[] lumber; //hold ingame lumber for each player
        public int[] food; //holds food (number of units probably? for now at least)
        public int[] foodMax; //holds maximum food for each player (basically based on building)
        

        //Harvest Variables (can be set to constant, but we can create cheat codes if it's not) :)
        //DO NOT change these as it is from Professor's code!!!
        public int DLumberPerHarvest = 100; //should be passed in as amount for RemoveLumber function
        public int DGoldPerMining = 100;
        public int DHarvestTime = 5; //timer for harvesting animation time before calling removeLumber function
        public int DMiningTime = 5;
        public int DDeathTime = 1;
        public int DDecayTime = 4;

        //Build Variables
        public int DConstructTime = 20; 
        public int DConstructTimeDefault;//1 for testing; (default 100 for slow), 20 works pretty well for normal
        public int DDecayTimer = 20; //100 for slower animation (default 100 for slower)
        public int DAttackSteps = 5; //attack steps to multiply by (lower number = faster atk speed (default to 5 as it works well)

        public int DChopConstant = 1; //1 for default chop, 4+ for instant chop

        //Viewport values (stored here for easy access in unit functions)
        //Updated in updateTimer function
        public int screenWidth;
        public int screenHeight;
        public float viewW = 0; // These values will help determine the thiccness of our viewport on the minimap
        public float viewH = 0; // These values will help determine the thiccness of our viewport on the minimap

        //Chosen units variable
        public List<unitPtr> chosenUnits; //holds index to chosen unit in the original units list (like a pointer) [owner][pointer to index in original]
                                          //We want to sort selected units by player group because that can easily allow us to tell groups of enemies apart

        public List<unitPtr>[] groupedUnits; //for unit grouping
        public List<unitPtr> cleanUpUnits; //list of units to delete

        public tempMap(int[] data, int[] dataIndices, int[] partials, string[] lines, int[] startgold, int[] startlumber, int x, int y, int[] mapLumber, int players, GraphicsDevice initDevice, int playerChoice)
        {
            //we need to somehow localize these data ... it will use more memory but we need it so default values don't get changed
            this.playerChoice = playerChoice;
            this.unitsToCreate = new List<unitCreateData>();
            this.cleanUpUnits = new List<unitPtr>();
            this.curDevice = initDevice; //not clone (pointer to the graphcis device)
            this.tempMapData = data.Clone() as int[];
            this.tempMapDataIndices = dataIndices.Clone() as int[];
            this.tempMapPartials = partials.Clone() as int[];
            this.tempMapLines = lines.Clone() as string[];
            this.gold = startgold.Clone() as int[];
            this.lumber = startlumber.Clone() as int[];
            this.mapW = x;
            this.mapH = y;
            this.DLumberAvailable = mapLumber.Clone() as int[];
            this.numPlayers = players;
            this.chosenUnits = new List<unitPtr>(); //holds pointer to chosen units

            this.food = new int[numPlayers+1];
            this.foodMax = new int[numPlayers+1];

            this.fogMap = new int[mapW * mapH]; //init to map size
            this.exploredMap = new int[mapW * mapH]; //we don't need to clear this

            this.unitDataCopy = SplashScreen.unitData.clone(); //copy this because we have upgrades in game
            this.allUpgrades = new int[SplashScreen.unitUpgradeData.numUpgrades]; //don't need to copy upgrades, they don't change
            this.DConstructTimeDefault = DConstructTime; //set once at start

            this.unitsToUpdate = new int[this.numPlayers][];
            for (int i =0; i < this.numPlayers; i++)
            {
                this.unitsToUpdate[i] = new int[SplashScreen.allUnitNames.Length]; //will init to 0
            }

            this.groupedUnits = new List<unitPtr>[10]; //unit groupings 0-9
            for (int i = 0; i < 10; i++) //init all groups
            {
                this.groupedUnits[i] = new List<unitPtr>();
            }
            

        }

        public bool checkSameSelected(List<Unit>[] allUnits)
        { //checks all units in chosenUnits and return true if same type
            //we use this function decide whether to show buttons for multiple unit selection
            int type = -1; //negative one type for none (not eprojectiles should not be able to be selected
            int owner = -2;
            foreach(unitPtr i in this.chosenUnits)
            {
                Unit temp = allUnits[i.owner][i.index];
                if (type == -1) //first unit
                {
                    type = temp.unitType;
                    owner = temp.owner;
                }
                else if (type == nameTranslation("Ranger") || type == nameTranslation("Footman") || type == nameTranslation("Archer"))
                { //army units that have same buttons
                    if (temp.owner != owner || (temp.unitType != nameTranslation("Ranger") && temp.unitType != nameTranslation("Footman") && temp.unitType != nameTranslation("Archer")))
                    { //check if selected unit is an army unit
                        return false;
                    }

                }
                else
                {
                    if (temp.unitType != type || temp.owner != owner) return false;
                }
            }

            return true;

        }

        public void unitGroup(int key, List<Unit>[] allUnits)
        { //function to group unit based on number key
            if (key >= 0 && key < 10) //check if valid group key
            {
                if (compareToChosen())
                {
                    this.groupedUnits[key].Clear(); //first clear the list before creating a new group
                    foreach (unitPtr i in this.chosenUnits) //iterate through all current chosen units
                    {
                        this.groupedUnits[key].Add(i); //add the pointer to selected unit to group
                    }
                }
                else unitSelectGroup(key, allUnits);
            }
        }

        private bool compareToChosen()
        {//helper function to compare currently selected units to groups and makes sure that we don't already have a group with exact same chosen units
            /*for (int i = 0; i < 10; i++)
            {
                if (this.chosenUnits == this.groupedUnits[i]) return false;
            }
            return true;*/

            for (int i = 0; i < 10; i++)
            {
                int diffCount = 0;
                if (this.chosenUnits.Count() == this.groupedUnits[i].Count()) //same number of units so have to check
                {
                    for (int j = 0; j < this.chosenUnits.Count(); j++)
                    {
                        
                        unitPtr p1 = this.groupedUnits[i].ElementAt(j);
                        unitPtr p2 = this.chosenUnits.ElementAt(j);
                        if (p1.owner == p2.owner && p1.index == p2.index) diffCount++; //found same so increment diff count
                        
                    }
                }
                if (diffCount == this.chosenUnits.Count()) return false; //basically exact same so return false (so we don't group as there is already a same group)
            }

            return true;
        }

        public void unitSelectGroup(int key, List<Unit>[] allUnits)
        { //sets the selected group as our current selected unit

            if (key >= 0 && key < 10) //valid key
            {
                if (this.groupedUnits[key].Count() != 0) //if we actually have a group at that key
                {
                    int newChosen = 0; //flag to keep track of whether we cleared chosen units yet (only clear it if  we actually have units to select)

                    foreach(unitPtr i in this.groupedUnits[key])
                    {
                        Unit temp = allUnits[i.owner][i.index];
                        if (temp.getAction() != "death" && temp.getAction() != "decay" && temp.invisible == 0) //only select nondead units
                        {
                            if (newChosen == 0)
                            {
                                newChosen = 1;
                                this.chosenUnits.Clear(); //first clear chosen units
                                                          //deselect all units here
                            }

                            this.chosenUnits.Add(i);
                        }
                    }

                    if (newChosen == 0) //if we still didn't add a unit after iterating list
                    { //that means our group of unit is all dead so reset that group 
                        this.groupedUnits[key].Clear();
                    }
                    else //we did switch groups
                    {
                        this.reShowMultiData = 1; //so reshow data labels
                        this.reShowMultiButtons = 1;
                    }
                }


            }
        }

        public void hackResource(string resource, int amount, int playerChoice)
        { //call this function to give current player more resource
            //NOTE: we are usually defaulted to player 1
            if (resource == "gold")
            {
                this.gold[playerChoice] += amount;
            }
            else if (resource == "lumber")
            {
                this.lumber[playerChoice] += amount;
            }
            else if (resource == "all")
            {
                this.gold[playerChoice] += amount;
                this.lumber[playerChoice] += amount;
            }

        }


        public bool finishUpgrade(int upgradeIndex, int playerChoice)
        { //call to finish upgrade
            if (this.allUpgrades[upgradeIndex] == 1)
            {
                this.allUpgrades[upgradeIndex] = 2;

                //int numIndex = SplashScreen.unitUpgradeData.numberOfAssets[upgradeIndex]; //number of units affected
                foreach (string unitType in SplashScreen.unitUpgradeData.assetNames[upgradeIndex])
                { //iterate through all units that the upgrade affects
                    int unitIndex = this.nameTranslation(unitType);

                    //if (unitIndex != -1) //it's an unit we have implemented
                    if (unitIndex >= 0) //it's an unit we have implemented
                    {
                        this.unitsToUpdate[playerChoice][unitIndex] = 1; //set flag for unit to update it later
                        int armorImprovement = SplashScreen.unitUpgradeData.armorImprovement[upgradeIndex];
                        int sightImprovement = SplashScreen.unitUpgradeData.sightImprovement[upgradeIndex];
                        int speedImprovement = SplashScreen.unitUpgradeData.speedImprovement[upgradeIndex];
                        int basicDamageImprovement = SplashScreen.unitUpgradeData.basicDamageImprovement[upgradeIndex];
                        int piercingDamageImprovement = SplashScreen.unitUpgradeData.piercingDamageImprovement[upgradeIndex];
                        int rangeImprovement = SplashScreen.unitUpgradeData.rangeImprovement[upgradeIndex];
                        //Update all upgraded values
                        if (armorImprovement != 0)
                        {
                            this.unitDataCopy.armor[unitIndex] += armorImprovement;
                        }
                        if (sightImprovement != 0)
                        {
                            this.unitDataCopy.sight[unitIndex] += sightImprovement;
                        }
                        if (speedImprovement != 0)
                        {
                            this.unitDataCopy.speed[unitIndex] += speedImprovement;
                        }
                        if (basicDamageImprovement != 0)
                        {
                            this.unitDataCopy.basicDamage[unitIndex] += basicDamageImprovement;
                        }
                        if (piercingDamageImprovement != 0)
                        {
                            this.unitDataCopy.piercingDamage[unitIndex] += piercingDamageImprovement;
                        }
                        if (rangeImprovement != 0)
                        {
                            this.unitDataCopy.range[unitIndex] += rangeImprovement;
                        }
                    }

                }

                return true; //success
            }
            return false;
        }

        public bool upgradeUnit(string upgradeStr, int playerChoice)
        {
            //Currently, upgrades can't be canceled when started
            int upgradeIndex = SplashScreen.unitUpgradeData.upgradeTranslation(upgradeStr);
            if (upgradeIndex == -1) return false; //upgrade doesn't exist
            if (this.allUpgrades[upgradeIndex] == 1) return false; //we currently upgrading(only one call at a time), so return false
            else if (this.allUpgrades[upgradeIndex] == 2) return false; //done upgrading
            else //actually upgrade if possible
            {
                int goldRequired = SplashScreen.unitUpgradeData.goldCost[upgradeIndex];
                int lumberRequired = SplashScreen.unitUpgradeData.lumberCost[upgradeIndex];
                if (this.gold[playerChoice] >= goldRequired && this.lumber[playerChoice] >= lumberRequired)
                { //we have enough resources
                    this.allUpgrades[upgradeIndex] = 1;
                    this.gold[playerChoice] -= goldRequired;
                    this.lumber[playerChoice] -= lumberRequired;
                }
                else return false;

            }


            return true; //upgrade success throughout

        }

        public void checkHealthDisplay(Unit updatedUnit)
        {// checks if currently selected unit is the passed in unit (if so, set the update flag)
            if (this.chosenUnits.Count() != 0)
            {
                if (this.chosenUnits.Count() > 1)
                {
                    for (int i = 0; i < this.chosenUnits.Count(); i++) //serach through entire list of units
                    {
                        unitPtr temp = this.chosenUnits.ElementAt(i);
                        if (updatedUnit.owner == temp.owner && updatedUnit.id == temp.index) //same unit
                        {
                            this.updateHealth = 1; //set flag to one to tell display to update
                        }
                    }
                }
                else
                {
                    unitPtr temp = this.chosenUnits.ElementAt(0);
                    if (updatedUnit.owner == temp.owner && updatedUnit.id == temp.index) //same unit
                    {
                        this.updateHealth = 1; //set flag to one to tell display to update
                    }
                }
            }
        }

        public bool checkCurrentUnit(string unitName, List<Unit>[] allUnits)
        { //checks to see if currently select unit is a certain unit
            int indexInput = nameTranslation(unitName);
            unitPtr temp = this.chosenUnits.ElementAt(0);
            int selectedIndex = allUnits[temp.owner][temp.index].unitType;
            if (selectedIndex == indexInput) return true;

            return false;
        }
        public bool checkRequirements(string unitName, List<Unit>[] allUnits, int playerChoice)
        { //checks if all requirements are met for a button to show up
            int unitType = nameTranslation(unitName);
            foreach(string req in SplashScreen.unitData.requirementList[unitType])
            {
                if (req == "Keep") //keep special case
                {
                    if (!findUnit(req, allUnits, playerChoice))
                    { //if can't find keep
                        if (!findUnit("Castle", allUnits, playerChoice)) return false; //find castle, if can't then return false
                    }
                }
                else if (!findUnit(req, allUnits, playerChoice)) return false; //unit not existing yet
            }

            return true; //by default so button doesn't show up
        }

        public void turnOffFog(List<Unit>[] allUnits)
        { //turns unit back to visible 
            for (int i = 0; i <= this.numPlayers; i++)
            {
                
                    foreach (Unit u in allUnits[i])
                    {
                        u.tintGrey = 0; //turn off tinting
                        if (u.getAction() != "death" && u.getAction() != "decay" && u.unitType > -1 && u.constructing == false && u.goldMining == false) //if unit is not dead
                        {
                            u.invisible = 0;
                        }
                    }
                
            }
        }

        public void updateFogOfWar(int playerChoice, List<Unit>[] allUnits)
        {
            //Uses a LOT of resources for update
            //int playerChoice = SplashScreen.mapUnits.playerChoice; //get player choice
            Array.Clear(fogMap,0,mapW*mapH); //clear array to zero
            foreach(Unit u in allUnits[playerChoice]) //only check current player for fog of war
            {
                if (u.invisible == 0) //not invisible and a usable unit
                {
                    int curSight = 0;
                    if (u.usable == 1) curSight = u.sight;
                    else curSight = u.sightConstruction;
                    if (curSight != 0)
                    {
                        int xMin = (int)u.x - u.sight;
                        int xMax = (int)u.x + curSight + u.unitTileWidth;
                        int yMin = (int)u.y - u.sight;
                        int yMax = (int)u.y + curSight + u.unitTileHeight;

                        for (int i = yMin; i < yMax; i++) //iterate all lines of sight
                        {
                            if (0 <= i && yMin < this.mapH)
                            {
                                for (int j = xMin; j < xMax; j++) //iterate through each line of sight
                                {
                                    if (0 <= j && j < this.mapW)
                                    {
                                       

                                        //"crop" the sight, it is not a squre. It is a squre with no corner.
                                        if ((i == yMin|| i == yMax-1) && (j == xMin || j == xMax - 1)) { continue; }

                                        this.fogMap[i * this.mapW + j] = 1;
                                        this.exploredMap[i * this.mapW + j] = 1; //also set explored to 1
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i <= this.numPlayers; i++)
            {
                if (i != playerChoice) //we don't have to iterate player choice
                {
                    foreach (Unit u in allUnits[i])
                    {
                        if (this.exploredMap[(int)u.y * this.mapW + (int)u.x] == 1 && this.fogMap[(int)u.y * this.mapW + (int)u.x] == 0)
                        { //a building in explored section but outside of view
                            if (u.unitType < SplashScreen.numUnits) u.invisible = 1; //turn unit invisible always as it's a unit
                            else if (u.unitType > -1) //do not change projectile invisibility
                            {
                                u.invisible = 0; //building stays visible
                                u.tintGrey = 1; //but we tint it grey (as it is outside current vission)
                            }
                        }
                        else if (this.fogMap[(int)u.y * this.mapW + (int)u.x] == 0) //other unit is in fog of war and isn't explored
                        {
                            u.invisible = 1; //just make invisible as it's not explored yet
                        }
                        else if (u.unitType > -1) //fog of war not affected by projectiles
                        {
                            u.invisible = 0; //make unit visible
                            u.tintGrey = 0; //don't tint unit (as it is in view)
                        }
                    }
                }
            }
        }

        public void updateUnitFood(List<Unit>[] allUnits)
        { //function to update food values based on units for all players
            for (int i = 0; i <= this.numPlayers; i++)
            {
                int tempCountUnits = 0;
                int tempCountMax = 0;
                foreach (Unit u in allUnits[i])
                {
                    //other buildings that gives population count needs to be added here
                    if (u.usable == 1) //only count units that are created and are usable
                    {
                        if (0 < u.foodConsumption) tempCountUnits += u.foodConsumption;
                        else if (0 > u.foodConsumption) tempCountMax += u.foodConsumption; //building
                    }
                    
                }
                this.food[i] = tempCountUnits;
                this.foodMax[i] = Math.Abs(tempCountMax); //because default values are negative for max food
            }

        }

        public void deselectUnit(unitPtr u, ref inGameButtons curButtons, ref UnitDatas curLabel)
        {
            int foundIndex = -1;
            int index = 0;
            while (index < this.chosenUnits.Count() && foundIndex == -1) //iterate through list
            {
                if (this.chosenUnits[index].index == u.index && this.chosenUnits[index].owner == u.owner)
                {
                    foundIndex = index;
                }
                index++;
            }
            if (foundIndex != -1) //actually found unit in chosen list
            {
                this.chosenUnits.RemoveAt(foundIndex); //remove unit pointer at that spot

                if (this.chosenUnits.Count() == 0) //if after deselecting, we no longer have selected units
                {
                    curButtons.hideButtons(); //hide buttons because buttons showed because of selected unit
                    curLabel.hide_data();
                }
                else
                {
                    this.updateHealth = 1;
                }
            }
        }

        public bool findUnit(string unitName, List<Unit>[] allUnits, int playerChoice)
        {//checks if a unit exist in current units of player
            //use for upgrade requirements
            foreach(Unit u in allUnits[playerChoice])
            {
                if (u.usable == 1 && u.unitType == nameTranslation(unitName)) return true; //unit has to be built
            }
            return false;
        }

        public bool isTraversible(int x, int y)
        { //takes in x and y to output true or false
            //light-grass, dark-grass, light-dirt, dark-dirt, rock, forest, shallow-water, deep-water, stump, rubble
            if (0 <= x && 0 <= y && x < this.mapW && y < this.mapH) //make sure tile is on map
            {
                int tIndex = y * this.mapW + x;
                int terrainType = this.tempMapData[tIndex];
                switch (terrainType) //how professor coded his
                {
                    case -1: //None
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 8:
                    case 9: return true;
                    default:
                        return false;
                }
            }
            else return false; //tiles is off map so obviously not travesible aka none
        }

        public bool isTraversible(float x, float y, float speed, float delta, string dir, List<Unit>[] allUnits, Unit current)
        { //to check if one step in a direction is traversible relative to current position
            //can be used to prempt stopping unit when they walk up to tree, rock, etc.
            //logic from Alan's movement code in XNA_ingame
            //Not completely working yet as it bugs out on some movement where it still walks on tree like god
            //Also, some tiles can't be walked on even though its looks like you should be able to (maybe need AI pathing?)
            //allows for unrestricted movement when invisible! mwahahahaha!
            
            if (current.unitType < -1) return true; //projectile can travel through everything mwahahahah

            float move = speed * delta;

            return true;
            /*
            //move = 0;
            if (dir == "N")
            {
                unitPtr temp = target(allUnits, (int)x, (int)(y - move), current, 1);
                //if (temp.Item1 != -1) return false; //there is building at location so just return
                if ((isTraversible((int)x, (int)(y - move)) && temp.owner == -1) || current.invisible == 1) return true; //maybe (or maybe not) minus extra one because img center point top left
            }
            else if (dir == "NE")
            {
                unitPtr temp = target(allUnits, (int)(x + 0.5f * move), (int)(y - 0.5f * move), current, 1);
                //unitPtr temp = target(allUnits, (int)(1f + x + 0.5f * move), (int)(y - 0.5f * move), current);
                //if (temp.Item1 != -1) return false; //there is building at location so just return
                if ((isTraversible((int)(x + 0.5f * move), (int)(y - 0.5f * move)) && temp.owner == -1) || current.invisible == 1) return true;
            }
            else if (dir == "E")
            {
                unitPtr temp = target(allUnits, (int)(1f + x + move), (int)y, current, 1);
                //unitPtr temp = target(allUnits, (int)(x + move), (int)y, current);
                //if (temp.Item1 != -1) return false; //there is building at location so just return
                if ((isTraversible((int)(1f + x + move), (int)y) && temp.owner == -1) || current.invisible == 1) return true; //add an extra one because our image center point is top left
            }
            else if (dir == "SE")
            {
                unitPtr temp = target(allUnits, (int)(x + 0.5f * move), (int)(y + 0.5f * move), current, 1);
                //unitPtr temp = target(allUnits, (int)(1f + x + 0.5f * move), (int)(1f + y + 0.5f * move), current);
                //if (temp.Item1 != -1) return false; //there is building at location so just return
                if ((isTraversible((int)(x + 0.5f * move), (int)(y + 0.5f * move)) && temp.owner == -1) || current.invisible == 1) return true;
            }
            else if (dir == "S")
            {
                unitPtr temp = target(allUnits, (int)x, (int)(1f + y + move), current, 1); //+1f because top left corner is center of unit image
                //unitPtr temp = target(allUnits, (int)x, (int)(y + move), current);
                //if (temp.Item1 != -1) return false; //there is building at location so just return
                if ((isTraversible((int)x, (int)(1f + y + move)) && temp.owner == -1) || current.invisible == 1) return true;
            }
            else if (dir == "SW")
            {
                unitPtr temp = target(allUnits, (int)(x - 0.5f * move), (int)(y + 0.5f * move), current, 1);
                //unitPtr temp = target(allUnits, (int)(x - 0.5f * move), (int)(1f + y + 0.5f * move), current);
                //if (temp.Item1 != -1) return false; //there is building at location so just return
                if ((isTraversible((int)(x - 0.5f * move), (int)(y + 0.5f * move)) && temp.owner == -1) || current.invisible == 1) return true;
            }
            else if (dir == "W")
            {
                unitPtr temp = target(allUnits, (int)(x - move), (int)y, current, 1);
                //if (temp != null) return false; //there is building at location so just return
                if ((isTraversible((int)(x - move), (int)y) && temp.owner == -1) || current.invisible == 1) return true;
            }
            else if (dir == "NW")
            {
                unitPtr temp = target(allUnits, (int)(x - 0.5f * move), (int)(y - 0.5f * move), current, 1);
                //if (temp.Item1 != -1) return false; //there is building at location so just return
                if ((isTraversible((int)(x - 0.5f * move), (int)(y - 0.5f * move)) && temp.owner == -1) || current.invisible == 1) return true;
            }
            return false;*/
        }

        public void updateTimers(ref List<Unit>[] allUnits, ref tempMap gameData, float mapX, float mapY, int screenW, int screenH, ref inGameButtons curButtons, ref UnitDatas curLabel)
        { //update all unit timers if they have any
            for (int i = 0; i <= this.numPlayers; i++)
            {
                foreach(Unit u in allUnits[i])
                {
                    if (checkInvisible(u)) //u.invisible == 0
                    {
                        if (u.inUse == 1) this.updateProgress = 1;
                        u.decTimer(ref allUnits, ref gameData, ref curButtons, ref curLabel); //this is this object
                    }
                }
            }
            this.viewH = mapY;
            this.viewW = mapX;
            this.screenHeight = screenH;
            this.screenWidth = screenW;

        }

        public unitPtr target(List<Unit>[] allUnits, int x, int y, Unit current, int mode)
        {//function that returns the pointer/index of unit target clicked on (used for right click) and owner
            //(x,y) is click coordinates aka xToGo, yToGo ; can also be used for static checking (not mouse coordinates)
            //coordinates of clicking is not perfect due to differing image sizes

            unitPtr temp = new unitPtr();
            int tileW = SplashScreen.tileObject.tileWidth;
            int tileH = SplashScreen.tileObject.tileHeight;
            
            for (int i = 0; i <= this.numPlayers; i++)
            {
                //temp.index = 0; //should change this to u.id for all cases for more accuracy
                foreach(Unit u in allUnits[i])
                {
                    temp.index = u.id;
                    if (checkInvisible(u) && u.getAction() != "decay" && (u.unitType > -1 || u.unitType < -3) && ((u.owner != current.owner) || (current.owner == u.owner && current.id != u.id))) //only check non-invisible units and non-decaying units (we can walk on decaying unit?) and if unit is not self
                    {
                        //if (u.unitType >= 3) //for now only works for building and ignores unit collision
                        //if (u.unitType >= 0)
                        
                        if (mode == 0 || (mode == 1 && (Math.Abs(u.x - x) < 8) && (Math.Abs(u.y - y) < 8))) //mode 0 = select target so check all units
                        { //we want to only check close by units for movement (when we are not selecting target)
                          //the above allows us to skip out on some units during movement, enhancing our performance
                          //if (u.unitType >= SplashScreen.numUnits) //for now only works for building and ignores unit collision
                            if (u.unitType > -1 || (u.unitType < -3)) //a true unit (or resource drop)
                            {
                                //have to divide by tile to get actual coordinates span distance
                                //int unitW = u.objectImage.Width / tileW;
                                //int unitH = u.objectImage.Height / tileH;
                                //int unitW = u.unitTileWidth;
                                //int unitH = u.unitTileHeight;
                                int unitH = u.defaultUnitTileW;
                                int unitW = u.defaultUnitTileH;
                                float startX = u.x;
                                float startY = u.y;
                                float endX = startX + unitW;
                                float endY = startY + unitH;
                                if ((startX <= x) && (x < endX) && (startY <= y) && (y < endY)) //click is within bounds
                                {
                                    temp.owner = i;
                                    return temp; //index, owner
                                }
                            }
                            else if ((u.id != current.id && u.owner == current.owner) || (u.owner != current.owner)) // for units, check if same unit with same owner
                            { //if not, handle unit collision (id has to be different for same owner)
                              //TODO:
                              //float unitW = u.objectImage.Width / tileW;
                              //float unitHextra = (u.objectImage.Height - tileH) / 2; //because unit tile is longer vertically
                              //float unitH = u.objectImage.Height / tileH;
                              //float unitH = u.objectImage.Height - unitHextra;

                                float unitW = u.objImg2D.Width / tileW;
                                float unitH = u.objImg2D.Height / tileH;

                                float startX = u.x;
                                //float startY = u.y + unitHextra;
                                float startY = u.y - 1;
                                float endX = startX + unitW;
                                float endY = startY + unitH - 1;

                                //Takes care of NSWE directions
                                /*if ((startX <= x) && (x + 1 < endX) && (startY + 1 <= y) && (y - 1 < endY)) //takes care S and E direction
                                {
                                    temp.owner = i;
                                    return temp; //index, owner
                                }*/ //original
                                if ((startX <= x || startX <= x + 1) && (x + 1 < endX) && (startY + 1 <= y) && (y + 1 < endY + 1 + 1) && (current.getDirection() == "N" || current.getDirection() == "S")) //takes care S and E direction
                                {
                                    temp.owner = i;
                                    return temp; //index, owner
                                }
                                if ((startX <= x) && (x + 1 < endX) && (startY + 1 <= y || startY + 1 <= y + 1) && (y + 1 < endY + 1 + 1) && (current.getDirection() == "E" || current.getDirection() == "W")) //takes care S and E direction
                                {
                                    temp.owner = i;
                                    return temp; //index, owner
                                }
                                //TODO: Fix edge cases here:
                                //going towards crowd of soldier from S, N, and E direction on top right corner leads tp bug where we have half of out unit walking in half the other

                                //Attempt at diagonal directions
                                if (current.getDirection() == "NE" && (startX <= x + 1) && (x + 1 < endX) && (startY + 1 <= y) && (y <= endY)) //check top right of current unit
                                {
                                    temp.owner = i;
                                    return temp; //index, owner
                                } //correct
                                if (current.getDirection() == "SE" && (startX <= x + 1) && (x + 1 < endX) && (startY + 1 <= y + 1) && (y + 1 <= endY)) //check bottom right of current unit
                                {
                                    temp.owner = i;
                                    return temp; //index, owner
                                } //correct
                                if (current.getDirection() == "NW" && (startX <= x) && (x <= endX) && (startY + 1 < y) && (y < endY)) //check top right of current unit
                                {
                                    temp.owner = i;
                                    return temp; //index, owner
                                } // correct
                                if (current.getDirection() == "SW" && (startX <= x) && (x < endX) && (startY + 1 < y + 1) && (y < endY)) //fixes SW with units to left
                                {
                                    temp.owner = i;
                                    return temp; //index, owner
                                } //wrong
                                if (current.getDirection() == "SW" && (startX <= x) && (x + 1 < endX) && (startY + 1 <= y + 1) && (y < endY)) //fix SW with unit to bottom
                                {
                                    temp.owner = i;
                                    return temp; //index, owner
                                } //wrong
                            }
                        }
                    }
                    //temp.index++;
                }
            }

            //return Tuple.Create(-1, -1); //default -1 for nothing
            return temp;
        }

        /*public unitPtr findUnit(unitPtr unit, ref List<Unit>[] chosenList)
        { //function to find a unit in the selected units list and remove it
            unitPtr temp = new unitPtr();
            for (int i = 0; i <= this.numPlayers; i++)
            {
                if (i == unit.owner)
                {
                    foreach (Unit u in chosenList[i])
                    {
                        if (u.id == unit.id)
                        {
                            temp.id = unit.id;
                        }
                    }
                }
            }

        }*/

        public bool canPlaceOn(int x, int y)
        {
            if (!isTraversible(x, y)) return false; //if NOT traversible, then not placeable
            else return true;

        }

        public int nameTranslation(string name)
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
                        index = -2 - i; //negative index, starting at -2 for arrow, -3 for cannonball
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

        public bool canBuildOn(int x, int y, string buildType)
        {//take in x,y position of current cursor (assuming building top left is at cursour position
            //buildType is the building we selected to build
            //if (this.unitType == 0) //only peasant can build
            //{
                //TODO: 
                //Check x,y,x+1,y+1 combos etc. up to building.x/tile.x building.y/tile.y

                //Calculate the number of spaces we have to check

                int buildingIndex = nameTranslation(buildType);
            //int buildingW = SplashScreen.mapUnits.allUnitTiles[buildingIndex][0].Width;
            //int buildingH = SplashScreen.mapUnits.allUnitTiles[buildingIndex][0].Height;
            //int tileW = SplashScreen.tileObject.tileWidth;
            //int tileH = SplashScreen.tileObject.tileHeight;
            //int calcW = buildingW / tileW; //number to horz. tiles that building span
            //int calcH = buildingH / tileH; //number of vertical tiles that building span
            int calcW = SplashScreen.unitData.size[buildingIndex];
            int calcH = SplashScreen.unitData.size[buildingIndex];

            //uses isTraversible function to calculate (edge case -1 which doesn't pop up in professor's file)
            for (int i = 0; i < calcW; i++) //iterate through all possible tiles that need to be checked
                {
                    for (int j = 0; j < calcH; j++)
                    {
                        if (!canPlaceOn(x + i, y+j)) return false;
                    }
                }
                return true; //all cases passed so return true
            //}
            //return false;

        }

        public bool isTree(int x, int y)
        { //checks if a tile is a tree
            /*int tileX = SplashScreen.tileObject.tileWidth;
            int tileY = SplashScreen.tileObject.tileHeight;
            int x = (int)((mouseX - mapX) / tileX);
            int y= (int)((mouseY - mapY) / tileY);*/
            
            if (y < this.mapH && x < this.mapW && x >= 0 && y >= 0 && this.tempMapData[y*this.mapW +x] == 5) //only need to check if tile type is forest
            { //NOTE: tile type will automatically updated if forest is chopped to grass
                return true; //we clicked on forest, return true, then move there and call other functions
            }
            return false; //by default
        }

        public unitPtr findNearestTree(unitPtr curTree, Unit current, int range, List<Unit>[] allUnits)
        { //function to return pointer to nearest tree
            //call this function once we finish chopping a tree
            int x = curTree.x;
            int y = curTree.y;
            int uX = (int)current.x;
            int uY = (int)current.y;
            int rangeToSearch = range; //max range to search for trees (larger=easier to find tree but may take longer time to search)
            unitPtr tempTree = new unitPtr();
            double curNearest = 999;

            for (int i = 0; i < rangeToSearch; i++) //search horizontally
            {
                for (int j = 0; j < rangeToSearch; j++) //search vertically
                {
                    if ((y-j) >= 0 && (x-i) >= 0 && isTree(x-i,y-j) && isChoppable(x-i,y-j,allUnits,current)) //if we have lumber at a location
                    {
                        if (curNearest == 999) //first tree found
                        {
                            curNearest = Math.Sqrt(Math.Pow(uX - (x-i), 2) + Math.Pow(uY - (y-i), 2));
                            tempTree.owner = 0;
                            tempTree.index = -1;
                            tempTree.x = (x - i);
                            tempTree.y = (y - j);
                        }
                        else
                        {
                            double newNearest = Math.Sqrt(Math.Pow(uX - (x - i), 2) + Math.Pow(uY - (y - i), 2));
                            if (newNearest < curNearest)
                            {
                                curNearest = newNearest;
                                tempTree.owner = 0;
                                tempTree.index = -1;
                                tempTree.x = (x - i);
                                tempTree.y = (y - j);
                            }
                        }
                        return tempTree;
                    }
                    if ((y + j) <= this.mapH && (x + i) <= this.mapW && isTree(x+i,y+j) && isChoppable(x + i, y + j, allUnits, current)) //if we have lumber at a location
                    {
                        if (curNearest == 999) //first tree found
                        {
                            curNearest = Math.Sqrt(Math.Pow(uX - (x+i), 2) + Math.Pow(uY - (y+j), 2));
                            tempTree.owner = 0;
                            tempTree.index = -1;
                            tempTree.x = (x + i);
                            tempTree.y = (y + j);
                        }
                        else
                        {
                            double newNearest = Math.Sqrt(Math.Pow(uX - (x + i), 2) + Math.Pow(uY - (y + j), 2));
                            if (newNearest < curNearest)
                            {
                                curNearest = newNearest;
                                tempTree.owner = 0;
                                tempTree.index = -1;
                                tempTree.x = (x + i);
                                tempTree.y = (y + j);
                                //return tempTree;
                            }
                        }
                        return tempTree;
                    }
                    if ((y - j) >= 0 && (x + i) <= this.mapW && isTree(x+i,y-j) && isChoppable(x + i, y - j, allUnits, current)) //if we have lumber at a location
                    {
                        if (curNearest == 999) //first tree found
                        {
                            curNearest = Math.Sqrt(Math.Pow(uX - (x + i), 2) + Math.Pow(uY - (y - j), 2));
                            tempTree.owner = 0;
                            tempTree.index = -1;
                            tempTree.x = (x + i);
                            tempTree.y = (y - j);
                        }
                        else
                        {
                            double newNearest = Math.Sqrt(Math.Pow(uX - (x + i), 2) + Math.Pow(uY - (y - j), 2));
                            if (newNearest < curNearest)
                            {
                                curNearest = newNearest;
                                tempTree.owner = 0;
                                tempTree.index = -1;
                                tempTree.x = (x + i);
                                tempTree.y = (y - j);
                                //return tempTree;
                            }
                        }
                        return tempTree;
                    }
                    if ((y + j) <= this.mapH && (x - i) >= 0 && isTree(x-i,y+j) && isChoppable(x - i, y + j, allUnits, current)) //if we have lumber at a location
                    {
                        if (curNearest == 999) //first tree found
                        {
                            curNearest = Math.Sqrt(Math.Pow(uX - (x - i), 2) + Math.Pow(uY - (y + j), 2));
                        tempTree.owner = 0;
                        tempTree.index = -1;
                        tempTree.x = (x - i);
                        tempTree.y = (y + j);
                        }
                    
                        else
                        {
                            double newNearest = Math.Sqrt(Math.Pow(uX - (x - i), 2) + Math.Pow(uY - (y + j), 2));
                            if (newNearest < curNearest)
                            {
                                curNearest = newNearest;
                                tempTree.owner = 0;
                                tempTree.index = -1;
                                tempTree.x = (x - i);
                                tempTree.y = (y + j);
                                //return tempTree;
                            }
                        }
                        //return tempTree;
                    }
                }
            }

            return tempTree;

        }

        public unitPtr findNearestBuilding(List<Unit>[] allUnits, Unit current, string type)
        {//find nearest building (that can store resource) to current unit position of a certain type of resource
            //returns index of nearest building
            string[] types = { "lumber", "gold", "gather" }; //types of resources to store (attack is attack more search)
            int index = 0;
            for (int i = 0; i < types.Length; i++)
            {
                if (type == types[i]) index = i;
            }

            string[] goldStorage = { "TownHall", "Keep", "Castle" }; //gold storage
            string[] lumberStorage = { "LumberMill", "TownHall", "Keep", "Castle" }; //not quite sure if lumbermill stores ....

            string[][] storage = new string[2][];
            storage[1] = goldStorage;
            storage[0] = lumberStorage;

            unitPtr temp = new unitPtr(); //we want to know owner too just for future cases (like attacking a building of another player)
            double currentDistance = -1;

            if (index < 2) //storage units
            {
                //we only need to search buildings that are owned by current unit as we can't store in enemy/ally buildings
                foreach (Unit u in allUnits[current.owner])
                {
                    if (u.unitType >= SplashScreen.numUnits) //only look at buildings
                    {
                        for (int i = 0; i < storage[index].Length; i++) //iterate through all possible buildings
                        {
                            if (this.nameTranslation(storage[index][i]) == u.unitType) //found a building
                            {
                                //get Euclidean distance
                                double x = Math.Pow((u.x - current.x), 2);
                                double y = Math.Pow((u.y - current.y), 2);
                                double sqrtXY = Math.Sqrt(x + y);
                                if (currentDistance == -1) //first building found
                                {//so set distance by default
                                    temp.owner = current.owner;
                                    temp.index = u.id;
                                    currentDistance = sqrtXY;
                                }
                                else if (sqrtXY < currentDistance)
                                {
                                    temp.owner = current.owner;
                                    temp.index = u.id;
                                    currentDistance = sqrtXY;
                                }
                            }
                        }
                    }
                }
            }
            else if (index == 2) //gather mode, so search for goldmine
            { //goldMine only owned by nature so only search nature
                foreach (Unit u in allUnits[0])
                {
                    if (u.unitType >= SplashScreen.numUnits) //only look at buildings
                    {
                            if (this.nameTranslation("GoldMine") == u.unitType && u.gold > 0) //found a GoldMine with gold in it
                            {
                                //get Euclidean distance
                                double x = Math.Pow((u.x - current.x), 2);
                                double y = Math.Pow((u.y - current.y), 2);
                                double sqrtXY = Math.Sqrt(x + y);
                                if (currentDistance == -1) //first building found
                                {//so set distance by default
                                    temp.owner = 0; //owner is nature
                                    temp.index = u.id;
                                    currentDistance = sqrtXY;
                                }
                                else if (sqrtXY < currentDistance)
                                {
                                    temp.owner = 0;
                                    temp.index = u.id;
                                    currentDistance = sqrtXY;
                                }
                            
                            }
                    }
                }
            }

            return temp;

        }

        public bool isChoppable(int x, int y, List<Unit>[] allUnits, Unit current)
        {//checks if a tile is actually choppable, aka reachable
            if (isTraversible(x, y - 1) && target(allUnits, x, y-1, current, 0).owner == -1) return true; //check north && target(allUnits, x, y, current, 0).owner == -1
            if (isTraversible(x, y + 1) && target(allUnits, x, y+1, current, 0).owner == -1) return true; //check south && target(allUnits, x, y, current, 0).owner == -1
            if (isTraversible(x - 1, y) && target(allUnits, x-1, y, current, 0).owner == -1) return true; //check west && target(allUnits, x, y, current, 0).owner == -1
            if (isTraversible(x + 1, y) && target(allUnits, x+1, y, current, 0).owner == -1) return true; //check east && target(allUnits, x, y, current, 0).owner == -1

            return false; //by default (all test cases failed meaning no path
        }

        /*public unitPtr findNearestTree(int x, int y)
        {
            unitPtr temp = new unitPtr();
            TODO: algorithm that will find nearest tree after a tree runs out and return pointer (index is tree's index in array)


        }*/

        
            //AZ: Modified this function because now it can directly modify the map's data, and don't need to return the texture2D anymore.
        public void getNewTiles(int x, int y)
        {//This function gets 9 tiles of terrain (8 around (x,y) and (x,y) itself)
            //Returns an array of the 9 changed tile in order (x-1,y-1), (x,y-1), (x+1,y-1), (x-1,y), ... (x+1,y+1)
            //call this function after calling removeLumber and getting return of 1
            //Now returns 9 Texture2D images in an array.
            //Texture2D[] tempImg = new Texture2D[9];
            int index = 0;

           
            for (int j = y-1; j < y+1+1; j++) //iterate from y-1 to y+1
            {
                for (int i = x - 1; i < x + 1 + 1; i++) //iterate from x-1 to x+1
                {
                    //if (0 <= i && i < SplashScreen.mapObject.mapX[mapChoice] && 0 <= j && j < SplashScreen.mapObject.mapY[mapChoice])
                    if (0 <= i && i < theMapIndex.data[0].Length && 0 <= j && j < theMapIndex.data.Length)
                    { //checks to make sure we are inbound
                        int typeIndex = this.tempMapData[j * this.mapW + i];
                        int tileIndex = this.tempMapDataIndices[j * this.mapW + i];
                        //True img is the index of tiles directly from the file.dat (but stored inside an array)
                        int trueImgIndex = SplashScreen.mapObject.DTileIndices[typeIndex][tileIndex];


                        //System.Drawing.Bitmap tmp = SplashScreen.tileObject.tiles[trueImgIndex];
                        //tempImg[index] = XNA_InGame.getTextureFromBitmap(tmp, g);


                        //Directly modify theMapIndex's data, so that it can be shown on the map easily.
                        theMapIndex.data[j][i] = trueImgIndex;

                        //unblock the path finding cell if no tree any more
                        if (isTraversible(i,j)) { pathFindingGriod.UnblockCell(new RoyT.AStar.Position(i, j)); }

                        //AZ: this line is for debugging, from the outpu ,
                        //you can see the "new terrain tile type" doesn't change, although lumber availability is updated and chopped wood's position is correct.
                        System.Diagnostics.Debug.Print("Terrain updated, new Type"+ trueImgIndex +":X[" + i + "] Y:[" + j + "], Lumber available:" + DLumberAvailable[j * (this.mapW + 1) + i] );

                    }
                    else //returns default tile 0 (water?)
                    { 
                    }
                    index++;
                }
            }

            return;

        }

        public void clearSelect() //clears all selected units 
        { 
            this.chosenUnits.Clear();

        }




        public int RemoveLumber(int x, int y, int uX, int uY, int amount)
        { //remove {amount} number of lumber from position (x,y), and if that position is out, then change tile
            //(x,y) is position of tree tile, (ux,uy) is position of unit
            //this code is pretty much from original AssetDecoratedMap.cpp
            //modified to return value so that we know when to call getNewTiles
            //use DLumberPerHarvest variable from top for amount
            //call this function once every DHarvestTimer while chopping wood animation

            //AZ DEBUG: amount *=10

            //amount *= 100;
            int Index = 0;
            for (int YOff = 0; YOff < 2; YOff++)
            {
                for (int XOff = 0; XOff < 2; XOff++)
                {
                    int XPos = x + XOff;
                    int YPos = y + YOff;
                    Index |= (this.tempMapLines[YPos*(this.mapW+1)+XPos] == "forest") && (this.tempMapPartials[YPos * (mapW + 1) + XPos] != 0) ? 1 << (YOff * 2 + XOff) : 0;
                }
            }

            if ((Index != 0) && (0xF != Index))
            {
                switch (Index)
                {
                    case 1:
                        Index = 0;
                        break;
                    case 2:
                        Index = 1;
                        break;
                    case 3:
                        Index = uX > x ? 1 : 0;
                        break;
                    case 4:
                        Index = 2;
                        break;
                    case 5:
                        Index = uY < y ? 0 : 2;
                        break;
                    case 6:
                        Index = uY > y ? 2 : 1;
                        break;
                    case 7:
                        Index = 2;
                        break;
                    case 8:
                        Index = 3;
                        break;
                    case 9:
                        Index = uY > y ? 0 : 3; //this is how it looked like in original
                        break;
                    case 10:
                        Index = uY > y ? 3 : 1; //this is original
                        break;
                    case 11:
                        Index = 0;
                        break;
                    case 12:
                        Index = uX < x ? 2 : 3;
                        break;
                    case 13:
                        Index = 3;
                        break;
                    case 14:
                        Index = 1;
                        break;
                }
            }

            switch(Index)
            {
                case 0:
                    this.DLumberAvailable[y * (this.mapW + 1) + x] -= amount;
                    if (0 >= this.DLumberAvailable[y * (this.mapW + 1) + x])
                    {
                        this.DLumberAvailable[y * (this.mapW + 1) + x] = 0;
                        pathFindingGriod.UnblockCell(new RoyT.AStar.Position(x, y));
                        changeTerrainTilePartial(x, y, 0);
                        return 1;
                    }
                    break;
                case 1:
                    this.DLumberAvailable[y * (this.mapW + 1) + x + 1] -= amount;
                    if (0 >= this.DLumberAvailable[y * (this.mapW + 1) + x + 1])
                    {
                        this.DLumberAvailable[y * (this.mapW + 1) + x + 1] = 0;
                        pathFindingGriod.UnblockCell(new RoyT.AStar.Position(x + 1, y));
                        changeTerrainTilePartial(x+1, y, 0);
                        return 1;
                    }
                    break;
                case 2:
                    this.DLumberAvailable[(y+1) * (this.mapW + 1) + x] -= amount;
                    if (0 >= this.DLumberAvailable[(y+1) * (this.mapW + 1) + x])
                    {
                        this.DLumberAvailable[(y+1) * (this.mapW + 1) + x] = 0;
                        pathFindingGriod.UnblockCell(new RoyT.AStar.Position(x, y+1));
                        changeTerrainTilePartial(x, y+1, 0);
                        return 1;
                    }
                    break;
                case 3:
                    this.DLumberAvailable[(y+1) * (this.mapW + 1) + x + 1] -= amount;
                    if (0 >= this.DLumberAvailable[(y+1) * (this.mapW + 1) + x + 1])
                    {
                        this.DLumberAvailable[(y+1) * (this.mapW + 1) + x + 1] = 0;
                        pathFindingGriod.UnblockCell(new RoyT.AStar.Position(x+1, y+1));
                        changeTerrainTilePartial(x+1, y+1, 0);
                        return 1;
                    }
                    break;
            }

            return 0;

        }

        private void changeTerrainTilePartial(int x, int y, int type)
        {
            //this function will also update the tempMapData and tempMapDataIndices
            //takes in input coordinate (x,y) and the type of the terrain to change it to (type is usually zero for grass)
            //Used in AssetDecoratedMap.cpp and located in Terrainmap.cpp
            CGraphicTileset tempShifter = new CGraphicTileset();
            if ((0 <= x) && (0 <= y))
            {
                //if ((y <= this.mapH) && (x <= this.mapW))
                if ((y <= this.mapH) && (x <= this.mapW))
                {

                    this.tempMapLines[y * (this.mapW + 1) + x] = "stump";
 

                    for (int YOff = -1; YOff < 2; YOff++)
                    {

                        for (int XOff = -1; XOff < 2; XOff++)
                        {
                            //only call this function when in game which is already accounted for
                            int XPos = x + XOff;
                            int YPos = y + YOff;
                            if ((0 <= XPos) && (0 <= YPos))
                            {
                                if ((XPos + 1 < this.mapW) && (YPos + 1 < this.mapH))
                                {
                                    Tuple<int, int> temp = tempShifter.CalculateTileTypeAndIndex(this.mapW, XPos, YPos, this.tempMapPartials, this.tempMapLines);
                                    this.tempMapData[YPos * this.mapW + XPos] = temp.Item1;
                                    this.tempMapDataIndices[YPos * this.mapW + XPos] = temp.Item2;
                                }
                            }
                        }
                    }

                }

            }


        }

        public bool OnScreen(float viewX, float viewY, int viewW, int viewH, float ux, float uy, int uW, int uH)
        { //function that takes in viewport (x,y) and viewport width and height in addition to unit (x,y) and unit width+height location to determine if on screen or not
            //used by unit.cs onscreen function
            //reference: SoundEventRenderer.cpp

            int LeftX, RightX, TopY, BottomY;
            int tileH = SplashScreen.tileObject.tileHeight;
            int tileW = SplashScreen.tileObject.tileWidth;

            int unitWidth = uW / tileW;
            int unitHeight = uW / tileH;

            LeftX = (int)viewX - unitWidth;
            if (ux < LeftX) return false;
            RightX = (int)viewX + viewW + unitWidth - 1;
            if (ux > RightX) return false;
            TopY = (int)viewY - unitHeight;
            if (uy < TopY) return false;
            BottomY = (int)viewY + viewH + unitHeight - 1;
            if (uy > BottomY) return false;

            return true;

        }

        public bool checkInvisible(Unit current)
        { //this helper function handles the invisiblity issue for different players
            //this invisibility function is used where unit movement/action is needed (not graphic rendering as if unit is invisible, it should stay invisible)
            //this function shouldn't be used for unit selection either (but used for unit targetting)
            if (current.owner == this.playerChoice && current.invisible == 0) return true; //the default invisibility condition for chosen player

            //For other players
            if (current.owner != this.playerChoice)
            {
                if (this.renderFogFlag == 0 && current.invisible == 0) return true; //if fog of war is off then obviously it is checked like normal
                else if (this.renderFogFlag == 1) //fog of war is on
                {
                    if (current.invisible == 0) return true; //fog of war on and not invisible
                    if (current.invisible == 1) //fog of war on so unit is invisible by default, now check if it is truly dead
                    {
                        if (current.getAction() != "death" && current.getAction() != "decay") //if unit is not dead 
                        {
                            //if (current.unitType == nameTranslation("Peasant") && current.constructing == false && current.goldMining == false) return true; //basically peasant is only truly not invisible if not constructing or goldmining
                            if (current.unitType == nameTranslation("Peasant") && ((current.constructing || current.goldMining) || (current.constructing == false && current.goldMining == false))) return true;
                            else if (current.unitType != nameTranslation("Peasant")) return true; //only return true if not a peasant as peasant has special check case
                        }
                    }
                }
            }


            return false; //by default return that invisible == 1 basically
        }
    }

    
}
