using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class BasicAI //your typical baby useless AI just for looks
    {
        //Variables needed

        private int playerNum = 0; //nature by default (nature has no AI)
        private int[] workingUnit; //keeps track of how many units of each type we have
        private int numUnits = SplashScreen.allUnitNames.Length;
        List<Unit>[] allUnits; //to hold list (a reference to it) for quick easy access during calculations
        tempMap gameData;
        Microsoft.Xna.Framework.Graphics.GraphicsDevice g; //for graphics during unit building
        private int numPlayers;  //hold number of players so AI knows which player to fight later on
        public BuildCapabilities builder; //AI has their own builders
        private bool buildingHouse = false; //flag to check whether to build more houses 
        static int buildRange = 15; //range to search for building new buildings
        static int patrolRange = 5; //range for units to patrol after they spawn
        int patrolCounter = 0; //uses this counter to generate pseduo random space
        private bool currentlyBuilding = false;

        //Personal AI variables

        private int waitTimer = 10;
        static int optimalPeasants = 5;
        static int optimalArmy = 10;
        private int waitAndTryAgainP = 0; //for peasant creation
        private int waitAndTryAgainG= 0; //for peasant creation
        private int waitAndTryAgainH = 0; //for farms/house
        private int waitAndTryAgainB = 0; //for barracks
        private int waitAndTryAgainS = 0; //for footman
        private int waitAndTryAgainPS = 0; //for footman patrolling
        private int waitAndTryAgainI = 0; //for invading

        private bool invading = false;
        private bool followed = false;
        Unit leader;

        //PlayerChoice is the AI player number (not our playerchoice)
        public BasicAI(int playerChoice, ref List<Unit>[] allUnits, ref tempMap gameData, int numPlayers, Microsoft.Xna.Framework.Graphics.GraphicsDevice g)
        {
            this.playerNum = playerChoice;
            this.workingUnit = new int[this.numUnits];
            this.gameData = gameData;
            this.numPlayers = numPlayers;
            this.allUnits = allUnits;
            this.builder = new BuildCapabilities();
            this.g = g;
            this.waitTimer = (gameData.DConstructTime + 1) * 10; //a default wait and try again timer


            reCalcUnits(); //init calculations

        }

        public void assignActions()
        { //call this function repeatedly to assign actions to AI units (if any)
            if (checkTownHall()) //checks and builds townhall
            { //if we have a townhall, we can start creating peasants

                if (waitAndTryAgainH <= 0)
                {
                    if(checkHouses()) //if we have enough housing space
                    {
                        if (waitAndTryAgainB <= 0)
                        {
                            if (checkBarrack())
                            { //we do have barracks
                                if (waitAndTryAgainS <= 0 && this.invading == false)
                                {
                                    checkFootman(); //checks and creates peasants if needed
                                }
                                else if (waitAndTryAgainS > 0) waitAndTryAgainS -= 1;

                                if (waitAndTryAgainI <= 0)
                                {
                                    invade(); //checks and creates peasants if needed
                                }
                                else if (waitAndTryAgainI > 0) waitAndTryAgainI -= 1;
                            }
                        }
                        else if (waitAndTryAgainB > 0) waitAndTryAgainB -= 1;

                    }
                }
                else if (waitAndTryAgainH > 0) waitAndTryAgainH -= 1;

                if (waitAndTryAgainP <= 0) checkPeasants(); //checks and creates peasants if needed
                else if (waitAndTryAgainP > 0) waitAndTryAgainP -= 1;

                if (waitAndTryAgainG <= 0) gatherResources(); //send peasants to gather resources only if townhall is available
                else if (waitAndTryAgainG > 0) waitAndTryAgainG -= 1;

                if (waitAndTryAgainPS <= 0 && this.invading == false) patrolFootmans(); //send peasants to gather resources only if townhall is available
                else if (waitAndTryAgainPS > 0) waitAndTryAgainPS -= 1;

            }
        }

        private void invade()
        {
            reCalcUnits();
            int pID = nameTranslation("Footman");
            if (this.workingUnit[pID] >= optimalArmy && invading == false) //we are ready to invade
            {
                int weakestID = findWeakestPlayer();
                if (weakestID != -1) //we actually found a player
                {
                    Tuple<int, int> ret = invadeLocation(weakestID);
                    if (ret.Item1 != -1 && ret.Item2 != -1) //found target location
                    {
                        int firstID = findFirstUnit("Footman");
                        if (firstID != -1)
                        {
                            this.invading = true;
                            this.leader = allUnits[playerNum][firstID];
                            allUnits[playerNum][firstID].patrolling = false;
                            allUnits[playerNum][firstID].onCommand = false;
                            allUnits[playerNum][firstID].xToGo = (int)allUnits[playerNum][firstID].x;
                            allUnits[playerNum][firstID].yToGo = (int)allUnits[playerNum][firstID].y;
                            allUnits[playerNum][firstID].target = gameData.target(allUnits, ret.Item1, ret.Item2, allUnits[playerNum][firstID], 0);
                            allUnits[playerNum][firstID].findNearestOpenSpot(allUnits, ref gameData);
                            allUnits[playerNum][firstID].onCommand = true;
                            if (this.waitAndTryAgainI <= 0) this.waitAndTryAgainI += this.waitTimer;

                        }
                    }

                }

            }
            else if (this.invading == true && notDead(leader) && this.followed == false && leader.startedBattle == false)
            {
                foreach (Unit u in allUnits[playerNum]) //control all soldiers to invade
                {
                    if (u.unitType == nameTranslation("Footman") && u.id != leader.id)
                    {
                        u.patrolling = false;
                        u.onCommand = false;
                        u.xToGo = leader.xToGo;
                        u.yToGo = leader.yToGo;
                        u.target = gameData.target(allUnits, u.xToGo, u.yToGo, u, 0);
                        if (u.target.owner != -1) u.findNearestOpenSpot(allUnits, ref gameData);
                        //u.target.reset(); //commented this out at late night
                        u.onCommand = true;
                    }
                }
                this.followed = true;
                if (this.waitAndTryAgainI <= 0) this.waitAndTryAgainI += this.waitTimer;
            }
            else if (this.invading == true && this.followed == true && ((notDead(leader) && ((int)leader.x == leader.xToGo && (int)leader.y == leader.yToGo)) || !notDead(leader))) //leader is dead or reached destination
            {
                this.followed = false;
                foreach (Unit u in allUnits[playerNum]) //control all soldiers to stop and find nearest unit (auto?)
                {
                    if (u.unitType == nameTranslation("Footman") && u.id != leader.id && u.startedBattle == false) //stop all unit movements
                    {
                        u.patrolling = false;
                        u.xToGo = (int)u.x;
                        u.yToGo = (int)u.y;
                        //u.resetAnimation();
                        //u.onCommand = false;
                        
                        u.findBattleTargetMelee(allUnits, gameData);
                        //u.onCommand = true;
                    }
                }
                if (this.waitAndTryAgainI <= 0) this.waitAndTryAgainI += this.waitTimer;
            }
            else if (this.workingUnit[pID] <= optimalArmy / 2)
            {
                this.invading = false;
            }
            else if (this.waitAndTryAgainI <= 0) this.waitAndTryAgainI += this.waitTimer;
        }

        private Tuple<int, int> invadeLocation(int player)
        { //helper function to find weakest player's location (basically first nondead unit found will be target)
            foreach(Unit u in allUnits[player])
            {
                if (notDead(u)) //found a nondead unit that we can reach  && fightable(u)
                {
                    
                    int x = (int)u.x;
                    int y = (int)u.y;
                    return Tuple.Create(x, y);
                    
                }
            }
            return Tuple.Create(-1, -1);

        }

        private bool fightable(Unit u)
        {
            int x = (int)u.x;
            int y = (int)u.y;
            Unit current = new Unit("Footman", 0, 0, playerNum, 0, ref gameData);
            if (gameData.isTraversible(x, y - 1) && gameData.target(allUnits, x, y - 1, current, 0).owner == -1) return true; //check north && target(allUnits, x, y, current, 0).owner == -1
            if (gameData.isTraversible(x, y + 1) && gameData.target(allUnits, x, y + 1, current, 0).owner == -1) return true; //check south && target(allUnits, x, y, current, 0).owner == -1
            if (gameData.isTraversible(x - 1, y) && gameData.target(allUnits, x - 1, y, current, 0).owner == -1) return true; //check west && target(allUnits, x, y, current, 0).owner == -1
            if (gameData.isTraversible(x + 1, y) && gameData.target(allUnits, x + 1, y, current, 0).owner == -1) return true; //check east && target(allUnits, x, y, current, 0).owner == -1

            return false;
        }

        private bool checkBarrack()
        { //we only need one barracks for basic AI
            reCalcUnits(); //recalculate units before check
            int BIndex = nameTranslation("Barracks");
            int THId = findFirstUnit("TownHall");
            if (this.workingUnit[BIndex] != 0)
            {
                int id = findFirstUnit("Barracks");
                if (id != -1)
                {
                    Unit temp = allUnits[playerNum][id];
                    if (temp.usable == 1) return true; //we have a usable townhall so return true
                }
                //else townhall is currently being built so return false by default
            }
            else if (currentlyBuilding == false)//we don't have one so we have to build one
            {
                currentlyBuilding = true;
                int peasantID = findFirstUnitInterrupt("Peasant"); //first first open peasant (as TH is important, allows interrrupt)
                if (peasantID != -1 && THId != -1) //we actually found an open peasant
                {//so start build mode
                    Unit temp = allUnits[this.playerNum][THId];
                    Tuple<int, int> ret = startBuild((int)temp.x, (int)temp.y, "Barracks"); //if we successfully placed building down
                    if (ret.Item1 != -1 && ret.Item2 != -1)
                    { //we have to send our peasant over to build it
                        moveUnit(peasantID, ret.Item1, ret.Item2);
                        Unit tempBuild = new Unit("Barracks", 0, 0, playerNum, 0, ref gameData);
                        this.waitAndTryAgainS += tempBuild.buildTime * gameData.DConstructTime + waitTimer; //units must wait for barracks to be done
                        //return true;
                    }
                    else //we failed to start build, so wait a bit of time before starting build again (as we probably don't have enough resources)
                    {
                        Unit tempBuild = new Unit("Barracks", 0, 0, playerNum, 0, ref gameData);
                        if (this.waitAndTryAgainB <= 0) this.waitAndTryAgainB += tempBuild.buildTime * gameData.DConstructTime;
                    }
                }
                currentlyBuilding = false;

            }
            return false;
        }

        private bool checkFootman()
        {
            reCalcUnits(); //recalculate units before check
            int THIndex = nameTranslation("Barracks");
            int PIndex = nameTranslation("Footman");
            if (this.workingUnit[PIndex] >= optimalArmy) return true; //there is already enough peasants
            else if (this.workingUnit[THIndex] != 0)
            {
                int idTH = findFirstUnit("Barracks");
                if (idTH != -1) //we actually found a townhall
                {
                    Unit tempTH = allUnits[playerNum][idTH];
                    if (tempTH.inUse == 0 && tempTH.usable == 1) //if townhall has been built and is not already in use
                    {
                        allUnits[playerNum][idTH].QueueUnit("Footman", ref gameData); //queue the peasant
                        Unit temp = new Unit("Footman", 0, 0, playerNum, 0, ref gameData);
                        this.waitAndTryAgainS = gameData.DConstructTime * temp.buildTime; //we must wait at least this long before we recreate peasant
                        //this.waitAndTryAgainPS = gameData.DConstructTime * temp.buildTime + waitTimer; //patrol must wait for unit to be 
                    }
                }

            }


            return false;
        }

        private bool checkHouses()
        { //check if open housing space, if not, then build more farms
            reCalcUnits(); //recalculate units before check
            int FIndex = nameTranslation("Farm");
            int THId = findFirstUnit("TownHall");
            if (gameData.food[playerNum] < gameData.foodMax[playerNum])
            {
                int PIndex = nameTranslation("Peasant");
                if (this.workingUnit[PIndex] >= optimalPeasants) //check if we have reached optimal peasants
                { //after we have enough peasants, THEN we can allow for other unit creations
                    this.buildingHouse = false; //we won't be building anymore house
                    return true; //we still have open food slots so don't do anything
                }
            }
            else if (this.buildingHouse == false && currentlyBuilding == false) //only build more houses if we don't already have houses building
            {
                this.currentlyBuilding = true;
                int peasantID = findFirstUnitInterrupt("Peasant"); //first first open peasant (as TH is important, allows interrrupt)
                if (peasantID != -1 && THId != -1) //we actually found an open peasant
                {//so start build mode
                    Unit temp = allUnits[this.playerNum][THId];
                    allUnits[this.playerNum][peasantID].onCommand = false;
                    Tuple<int, int> ret = startBuild((int)temp.x, (int)temp.y, "Farm"); //if we successfully placed building down
                    if (ret.Item1 != -1 && ret.Item2 != -1)
                    { //we have to send our peasant over to build it
                        this.buildingHouse = true;
                        moveUnit(peasantID, ret.Item1, ret.Item2);
                        Unit tempBuild1 = new Unit("Farm", 0, 0, playerNum, 0, ref gameData);
                        if (this.waitAndTryAgainS <= 0) this.waitAndTryAgainS = gameData.DConstructTime * tempBuild1.buildTime + waitTimer; //we must wait at least this long for house to be done
                        if (this.waitAndTryAgainP <= 0) this.waitAndTryAgainP = gameData.DConstructTime * tempBuild1.buildTime + waitTimer; //we must wait at least this long for house to be done
                        this.waitAndTryAgainH += tempBuild1.buildTime * gameData.DConstructTime + waitTimer;

                    }
                }
                this.currentlyBuilding = false;
            }
            else if (this.buildingHouse == true)
            {
                int countBuilder = 0;
                foreach(Unit u in allUnits[playerNum])
                {
                    if (u.unitType == nameTranslation("Peasant") && u.constructing == true) countBuilder += 1;
                }
                if (countBuilder == 0) //if we don't have a builder
                {
                    int peasantID = findFirstUnitInterrupt("Peasant"); //first first open peasant (as TH is important, allows interrrupt)
                    int farmID = findFirstUnitToBuild("Farm");
                    if (peasantID != -1 &&  farmID != -1) //we actually found an open peasant
                    {//so send peasant to build
                        allUnits[this.playerNum][peasantID].onCommand = false;
                        this.buildingHouse = true;
                        moveUnit(peasantID, (int)allUnits[this.playerNum][farmID].x, (int)allUnits[this.playerNum][farmID].y);
                        Unit tempBuild1 = new Unit("Farm", 0, 0, playerNum, 0, ref gameData);
                        if (this.waitAndTryAgainS <= 0) this.waitAndTryAgainS = gameData.DConstructTime * tempBuild1.buildTime + waitTimer; //we must wait at least this long for house to be done
                        if (this.waitAndTryAgainP <= 0) this.waitAndTryAgainP = gameData.DConstructTime * tempBuild1.buildTime + waitTimer; //we must wait at least this long for house to be done
                        this.waitAndTryAgainH += tempBuild1.buildTime * gameData.DConstructTime + waitTimer;
                    }
                }
            }

            
            if (this.waitAndTryAgainH <= 0)
            {
                //Unit tempBuild = new Unit("Farm", 0, 0, playerNum, 0, ref gameData);
                //this.waitAndTryAgainH += tempBuild.buildTime * gameData.DConstructTime;
                this.waitAndTryAgainH += waitTimer;
            }
            return false;
        }

        private bool checkPeasants()
        {
            reCalcUnits(); //recalculate units before check
            int THIndex = nameTranslation("TownHall");
            int PIndex = nameTranslation("Peasant");
            if (this.workingUnit[PIndex] >= optimalPeasants) return true; //there is already enough peasants
            else if (this.workingUnit[THIndex] != 0)
            {
                int idTH = findFirstUnit("TownHall");
                if (idTH != -1) //we actually found a townhall
                {
                    Unit tempTH = allUnits[playerNum][idTH];
                    if (tempTH.inUse == 0 && tempTH.usable == 1) //if townhall has been built and is not already in use
                    {
                        allUnits[playerNum][idTH].QueueUnit("Peasant", ref gameData); //queue the peasant
                        Unit temp = new Unit("Peasant", 0, 0, playerNum, 0, ref gameData);
                        this.waitAndTryAgainP = gameData.DConstructTime * temp.buildTime; //we must wait at least this long before we recreate peasant
                    }
                }

            }


            return false;
        }

        private bool checkTownHall()
        { //this function checks if there is a town hall, if not then AI shall build one
            reCalcUnits(); //recalculate units before check
            int THIndex = nameTranslation("TownHall");
            if (this.workingUnit[THIndex] != 0)
            {
                int id = findFirstUnit("TownHall");
                if (id != -1)
                {
                    Unit temp = allUnits[playerNum][id];
                    if (temp.usable == 1) return true; //we have a usable townhall so return true
                }
                //else townhall is currently being built so return false by default
            }
            else if (this.currentlyBuilding == false) //we don't have one so we have to build one
            {
                this.currentlyBuilding = true;
                int peasantID = findFirstUnitInterrupt("Peasant"); //first first open peasant (as TH is important, allows interrrupt)
                if (peasantID != -1) //we actually found an open peasant
                {//so start build mode
                    Unit temp = allUnits[this.playerNum][peasantID];
                    Tuple<int, int> ret = startBuild((int)temp.x, (int)temp.y, "TownHall"); //if we successfully placed building down
                    if (ret.Item1 != -1 && ret.Item2 != -1)
                    { //we have to send our peasant over to build it
                        moveUnit(peasantID, ret.Item1, ret.Item2);
                        Unit tempBuild = new Unit("TownHall", 0, 0, playerNum, 0, ref gameData);
                        this.waitAndTryAgainH += tempBuild.buildTime * gameData.DConstructTime + waitTimer; //houses must wait for townhall complete done
                        //return true;
                    }
                }
                this.currentlyBuilding = false;

            }
            return false;

        }

        private void gatherResources()
        { //this function will send idle peasants to gather resource
            int id = findFirstUnit("Peasant");
            if (id != -1) //while we still have an idle peasant
            {
                Unit temp = allUnits[playerNum][id];
                if (temp.onCommand == false && (temp.gold <= 0 && temp.lumber <= 0)) //only use peasant if the peasant not already on duty (and not carrying resources)
                {
                    //Check which resource to gather (send peasant gathering the resource that AI has less of at the moment)
                    if (gameData.gold[playerNum] > gameData.lumber[playerNum]) //we have more gold
                    { //so gather lumber
                        unitPtr tempTarget = new unitPtr();
                        tempTarget.x = (int)temp.x;
                        tempTarget.y = (int)temp.y;
                        unitPtr ret = gameData.findNearestTree(tempTarget, temp, 10, allUnits); //search a range of 10 for tree
                        /*if (ret.owner == -1) //we did not find a tree
                        {//there was no tree in range (hence expand search distance once)
                            ret = gameData.findNearestTree(tempTarget, temp, 10); //search a range of 25 (more extensive search)
                        }*/
                        if (ret.owner == 0) //after all search, if we had found a tree
                        { //move peasant to chop it
                            moveUnit(id, ret.x, ret.y);
                            //this.waitAndTryAgainG += waitTimer;
                        }
                        else //we gather gold as there is usually always gold
                        {
                            unitPtr retG = gameData.findNearestBuilding(allUnits, temp, "gather"); //find a goldMine
                            if (retG.owner != -1) //if there is an existing goldMine
                            {
                                Unit tempGM = allUnits[0][retG.index];
                                moveUnit(id, (int)tempGM.x, (int)tempGM.y); //send peasant to goldMine
                                //this.waitAndTryAgainG += waitTimer;
                            }
                        }
                        //else AI will do nothing as most likely trees ran out around AI

                    }
                    else //we have more lumber or equal so gather gold (priority on gold for army)
                    {
                        unitPtr ret = gameData.findNearestBuilding(allUnits, temp, "gather"); //find a goldMine
                        if (ret.owner != -1) //if there is an existing goldMine
                        {
                            Unit tempGM = allUnits[0][ret.index];
                            moveUnit(id, (int)tempGM.x, (int)tempGM.y); //send peasant to goldMine
                            //this.waitAndTryAgainG += waitTimer;
                        }
                    }

                }
                else if ((temp.gold > 0 || temp.lumber > 0)) //peasant carrying resources, so store resource first
                {
                    if (temp.gold > 0)
                    {
                        unitPtr goTo = gameData.findNearestBuilding(allUnits, temp, "gold");
                        if (goTo.owner != -1) //we actually found a  building
                        {
                            Unit tempBuilding = allUnits[goTo.owner][goTo.index];
                            moveUnit(id, (int)tempBuilding.x, (int)tempBuilding.y);
                        }
                    }
                    else
                    {
                        unitPtr goTo = gameData.findNearestBuilding(allUnits, temp, "lumber");
                        if (goTo.owner != -1) //we actually found a  building
                        {
                            Unit tempBuilding = allUnits[goTo.owner][goTo.index];
                            moveUnit(id, (int)tempBuilding.x, (int)tempBuilding.y);
                        }
                    }
                }
            }
            else this.waitAndTryAgainG += waitTimer;

        }

        private void patrolFootmans()
        { //this function will send idle peasants to gather resource
            reCalcUnits(); //recalculate units before check
            int unitType = nameTranslation("Footman");
            if (this.workingUnit[unitType] >= 1 && this.invading == false) //if we actually have footman available
            {
                int id = findFirstUnit("Footman");
                if (id != -1) //while we still have an idle peasant
                {
                    Unit temp = allUnits[playerNum][id];
                    if (temp.onCommand == false) //only use footman that is not already on duty
                    {
                        Tuple<int, int> ret = findOpenRandSpace((int)temp.x, (int)temp.y);
                        if (ret.Item1 != -1 && ret.Item2 != -1) //if we found a spot
                        {
                            allUnits[playerNum][id].patrolX2 = (int)allUnits[playerNum][id].x;
                            allUnits[playerNum][id].patrolY2 = (int)allUnits[playerNum][id].y;
                            allUnits[playerNum][id].xToGo = ret.Item1;
                            allUnits[playerNum][id].yToGo = ret.Item2;
                            allUnits[playerNum][id].target = gameData.target(allUnits, ret.Item1, ret.Item2, allUnits[playerNum][id], 0);
                            if (allUnits[playerNum][id].target.owner != -1) allUnits[playerNum][id].findNearestOpenSpot(allUnits, ref gameData);
                            allUnits[playerNum][id].target.reset();
                            allUnits[playerNum][id].patrolX1 = allUnits[playerNum][id].xToGo;
                            allUnits[playerNum][id].patrolY1 = allUnits[playerNum][id].yToGo;
                            allUnits[playerNum][id].patrolling = true;
                            allUnits[playerNum][id].onCommand = true;
                        }

                    }
                }
            }
            this.waitAndTryAgainPS += waitTimer; //always wait a bit before commanding next

        }

        private Tuple<int,int> findOpenRandSpace(int x, int y)
        { //find a pseudo random open space around current spot
            int range = patrolRange;
            if (this.patrolCounter >= optimalArmy) this.patrolCounter = 0; //we only need random number up to army count
            else
            {
                this.patrolCounter += 1;
                for (int i = 0; i < range; i++)
                {
                    for (int j = 0; j < range; j++)
                    {
                        if (i != 0 || j != 0) //so we don't search (x,y) positon
                        {
                            if ((x - i) % this.patrolCounter == 0 && (y - j) % this.patrolCounter == 0)
                            {
                                if (gameData.isTraversible(x - i, y - j))
                                {
                                    
                                    return Tuple.Create(x - i, y - j); //don't worry about target at location
                                }
                            }
                            if ((x - i) % this.patrolCounter == 0 && (y + j) % this.patrolCounter == 0)
                            {
                                if (gameData.isTraversible(x - i, y + j))
                                {
                                    return Tuple.Create(x - i, y + j); //don't worry about target at location
                                }
                            }
                            if ((x + i) % this.patrolCounter == 0 && (y - j) % this.patrolCounter == 0)
                            {
                                if (gameData.isTraversible(x + i, y - j))
                                {
                                    return Tuple.Create(x + i, y - j); //don't worry about target at location
                                }
                            }
                            if ((x + i) % this.patrolCounter == 0 && (y + j) % this.patrolCounter == 0)
                            {
                                if (gameData.isTraversible(x + i, y + j))
                                {
                                    return Tuple.Create(x + i, y + j); //don't worry about target at location
                                }
                            }
                        }
                    }
                }
                
            }
            return Tuple.Create(-1, -1);
        }

        private void moveUnit(int id, int x, int y)
        { //basically like the right click function
            //moves unit of unit id to location
            if (x >= 0 && x < gameData.mapW && y >= 0 && y < gameData.mapH) //we are within bounds
            {

                if (gameData.isTree(x, y)) //we came into this if case because peasanto on tree
                {//so set tree as target
                    allUnits[playerNum][id].target.owner = 0; //set tree owner to nature
                    allUnits[playerNum][id].target.index = -1; //set to tree index
                    allUnits[playerNum][id].target.x = x;
                    allUnits[playerNum][id].target.y = y;
                }
                else allUnits[playerNum][id].target = gameData.target(allUnits, x, y, allUnits[playerNum][id], 0); //set the (normal) target (if any)
                allUnits[playerNum][id].xToGo = x;
                allUnits[playerNum][id].xToGo = y;
                allUnits[playerNum][id].findNearestOpenSpot(allUnits, ref gameData);

                allUnits[playerNum][id].onCommand = true;
            }
        }

        private Tuple<int,int> startBuild(int x, int y, string curBuilding)
        { //start building a building
            //As this is AI, we simply have to keep finding open spot until it works
            //this.builder.buildCancel();
            //if (this.builder.getMode() == 0) //only build if builder actually is open
            {
                this.builder.Build(curBuilding, this.playerNum, ref gameData);
                Unit temp = new Unit(curBuilding, 0, 0, playerNum, 0, ref gameData); //this is temporary
                int buildingW = temp.defaultUnitTileW;
                int buildingH = temp.defaultUnitTileH;
                int range = buildRange + buildingW; //search in range of 10 for building spot
                for (int i = 0; i < range; i++)
                {
                    for (int j = 0; j < range; j++)
                    {
                        if (awayFrom(x - i, y - j, temp) && this.builder.Register(ref allUnits, gameData, (x - i), (y - j), g, playerNum, 0))
                        {
                            //this.builder.buildCancel();
                            return Tuple.Create(x - i, y - j); //return true if we registered building
                        }
                        else if (awayFrom(x - i, y + j, temp) && this.builder.Register(ref allUnits, gameData, (x - i), (y + j), g, playerNum, 0))
                        {
                            //this.builder.buildCancel();
                            return Tuple.Create(x - i, y + j);
                        }
                        else if (awayFrom(x + i, y - j, temp) && this.builder.Register(ref allUnits, gameData, (x + i), (y - j), g, playerNum, 0))
                        {
                            //this.builder.buildCancel();
                            return Tuple.Create(x + i, y - j);
                        }
                        else if (awayFrom(x + i, y + j, temp) && this.builder.Register(ref allUnits, gameData, (x + i), (y + j), g, playerNum, 0))
                        {
                            //this.builder.buildCancel();
                            return Tuple.Create(x + i, y + j);
                        }
                    }
                }
            }


            return Tuple.Create(-1,-1);
        }

        public void reCalcUnits()
        { //function to recalculate number of each units this player has
            //this should be constantly called before assigning action to AI units (basically so our AI understands its own capability)
            Array.Clear(this.workingUnit, 0, this.numUnits); //clear array to zero
            foreach (Unit u in allUnits[this.playerNum])
            {
                if (notDead(u)) //first check to make sure unit is not dead
                {
                    if (u.unitType == nameTranslation("Peasant") && ((u.constructing || u.goldMining) || (u.constructing == false && u.goldMining == false))) this.workingUnit[u.unitType] += 1; //it's a peasant that was invisible due to construction or goldmining
                    else if (gameData.checkInvisible(u)) //if unit is not invisible
                    {
                        this.workingUnit[u.unitType] += 1;
                    }
                }
            }
        }

        private bool awayFrom(int x, int y, Unit building)
        { //checks to make sure our building spot leaves at least a one spot gap away from other buildings
            //Only checks against nature and AI ownered buildings for now

            int bW = building.defaultUnitTileW-1;
            int bH = building.defaultUnitTileH-1;

            if (x < 0 || y < 0 || x > gameData.mapW || y > gameData.mapH) return false; //exceed boundary of map
            if (!gameData.isTraversible(x, y) || !gameData.isTraversible(x + bW, y) || !gameData.isTraversible(x, y + bH) || !gameData.isTraversible(x + bW, y + bH)) return false;

            foreach (Unit u in allUnits[0])
            { //only need to check goldMines for nature
                if (u.unitType == nameTranslation("GoldMine"))
                {
                    int uW = u.defaultUnitTileW;
                    int uH = u.defaultUnitTileH;
                    if (u.x - 1 <= x && x < u.x+uW+1 && u.y - 1 <= y && y < u.y+uH+1) return false; //if within passed in x value is within one tile of other building
                    if (u.x - 1 <= x+bW && x+bW < u.x + uW + 1 && u.y - 1 <= y+bH && y+bH < u.y + uH + 1) return false; //if bottom right corner within one tile of other
                    if (u.x - 1 <= x + bW && x + bW < u.x + uW + 1 && u.y - 1 <= y && y < u.y + uH + 1) return false; //top right
                    if (u.x - 1 <= x && x< u.x + uW + 1 && u.y - 1 <= y + bH && y + bH < u.y + uH + 1) return false; //bottom left
                }

            }

            foreach (Unit u in allUnits[this.playerNum]) //check all units for this AI player
            { //only need to check goldMines for nature
               
                int uW = u.defaultUnitTileW;
                int uH = u.defaultUnitTileH;
                if (u.x - 1 <= x && x < u.x + uW + 1 && u.y - 1 <= y && y < u.y + uH + 1) return false; //if within passed in x value is within one tile of other building
                if (u.x - 1 <= x + bW && x + bW < u.x + uW + 1 && u.y - 1 <= y + bH && y + bH < u.y + uH + 1) return false; //if bottom right corner within one tile of other
                if (u.x - 1 <= x + bW && x + bW < u.x + uW + 1 && u.y - 1 <= y && y < u.y + uH + 1) return false; //top right
                if (u.x - 1 <= x && x < u.x + uW + 1 && u.y - 1 <= y + bH && y + bH < u.y + uH + 1) return false; //bottom left

            }


            return true; //by default as that means we passed all cases and can build

        }

        private int findFirstUnit(string type)
        { //helper function to find and return id of the first unit of AI that is ready but can't interrupt
            foreach(Unit u in this.allUnits[this.playerNum])
            {
                if (u.unitType == nameTranslation(type) && ready(u) && u.onCommand == false && u.patrolling == false) return u.id; //buildings can't have onCommand... so doesn't affect buildings
            }

            return -1;
        }

        private int findFirstUnitToBuild(string type)
        { //helper function to find and return id of the first unit of AI that is ready but can't interrupt
            foreach (Unit u in this.allUnits[this.playerNum])
            {
                if (u.unitType == nameTranslation(type) && ready(u) && u.usable == 0) return u.id; //find a non-built buildingf
            }

            return -1;
        }

        private int findFirstUnitInterrupt(string type)
        { //helper function to find and return id of the first unit of AI that is ready and can interrupt the unit if it has an action already
            foreach (Unit u in this.allUnits[this.playerNum])
            {
                if (u.unitType == nameTranslation(type) && ready(u) && u.constructing == false && u.goldMining == false && u.startedBattle == false) return u.id;
            }

            return -1;
        }

        private bool ready(Unit u)
        { //helper function to check if unit is not dead
            if (notDead(u) && gameData.checkInvisible(u)) return true; //&& u.onCommand == false
            return false;
        }

        private bool notDead(Unit u)
        { //helper function to check if a unit is not dead
            if (u.getAction() != "death" && u.getAction() != "decay") return true;
            return false;
        }

        public int findWeakestPlayer()
        { //helper function to return weakest player on map (or basically enemy)
            //this function basically uses unit count to judge player strength
            int retPlayer = -1; //we should not target nature
            int count = 0;
            for (int i = 1; i <= this.numPlayers; i++)
            {
                if (i != this.playerNum) //only can return if we are not only player
                {
                    int tempCount = allUnits[i].Count();
                    if (tempCount > 0) //only can target player with units (should always hold true, maybe check invisibility instead?)
                    {
                        if (retPlayer == -1)
                        {
                            count = tempCount;
                            retPlayer = i; //first player found
                        }
                        else
                        {
                            if (tempCount > count) //we actually have more units
                            {
                                count = tempCount;
                                retPlayer = i;
                            }
                        }
                    }
                }
            }

            return retPlayer;

        }


        public string nameTranslation(int type)
        { //opposite of nametranslation for easy use
            string[] assetTypes = SplashScreen.allUnitNames;
            if (type >= 0 && type < assetTypes.Length)
            {
                return assetTypes[type];
            }
            else return null; //can return '0' string if needed, it should never return this anyways
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


    }
}
