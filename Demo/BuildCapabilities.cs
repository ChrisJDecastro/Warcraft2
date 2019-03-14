using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Demo
{
    public class ButtonImage
    {
        public Bitmap[] tiles;
        public string[] lines;
        public int numTiles, tileWidth, tileHeight;

        public int findIcon(string type)
        { //return index of image in tiles
            int length = this.lines.Length;
            int index = 5;
            for (int i = 5; i < length; i++)
            {
                if (this.lines[i] == type) index = i;
                
            }
            return index - 5;
        }

    }

    public class BuildCapabilities
    {

        public Unit building; // needed to be accessed by XNA in game.
        string buildStr;
        private int buildMode;
        Unit newUnit; //for unit building (separate so it doesn't collide when we build buildings at same time)
        string newUnitStr;
        string[] allUnitNames;


        public BuildCapabilities()
        {
            this.buildMode = 0; //build mode off by default
            this.allUnitNames = SplashScreen.allUnitNames.Clone() as string[];
        }

        public int getMode()
        { //return build mode
            return this.buildMode;

        }

        public bool BuildUnit(string unitType, int owner, Unit curBuilding, ref List<Unit>[] allUnits, tempMap gameData)
        { //called by a building for building a unit (call when unit needs to be added)
            //conflicts may happen if two units needs to be created at exact same time
            int type = nameTranslation(unitType);
            bool success = true;
            if (type < SplashScreen.numUnits)
            {
                Tuple<int, int> temp = findOpenSpace((int)curBuilding.x, (int)curBuilding.y, curBuilding.unitTileWidth, curBuilding.unitTileHeight, allUnits, gameData);
                //Tuple<int, int> temp = findOpenSpace((int)curBuilding.x, (int)curBuilding.y, curBuilding.defaultUnitTileW, curBuilding.defaultUnitTileH, allUnits, gameData);
                int newX = temp.Item1;
                int newY = temp.Item2;
                if (newX != -1 && newY != -1) //we actually found a new position
                {
                    this.newUnitStr = unitType;
                    this.newUnit = new Unit(unitType, 0, 0, owner, 0, ref gameData);
                    //this.buildMode = 2;
                    if (Register(ref allUnits, gameData, newX, newY, gameData.curDevice, owner, 1)) success = true; //call register immediately to clear queue
                }
            }

            return success;

        }

        public void Build(string unitType, int owner, ref tempMap gameData)
        { //function to be called when build button is clicked
            int type = nameTranslation(unitType);
            if (type >= SplashScreen.numUnits) //an actual building (units use other build call)
            {
                this.buildStr = unitType;

                building = new Unit(unitType, 0, 0, owner, 0, ref gameData); //this is temporary
                building.setBuildFrame(0);
                this.buildMode = 1;
            }

        }

        /*public void setMode(int num)
        {
            if (num == 0 || num == 1) this.buildMode = num;
        }*/
        private Tuple<int,int> findOpenSpace(int x, int y, int w, int h, List<Unit>[] allUnits, tempMap gameData)
        {//function to find first open space around current building
            //not fully functioning (probably bug somewhere, unknown yet)
            //(x,y) and (w,h) are data of the building that we are creating unit from
            int openX = -1;
            int openY = -1;

            int top = y - 1; //row above building
            int bottom = y + h; //not sure if +1 is needed yet
            int left = x - 1;
            int right = x + w; //not sure like above

            int found = 0;
            //Not sure about the ordering of these checks (can move around to change order)
            //Check bottom row
            for (int i = x; i < x+w; i++)
            {
                if (openSpace(i, bottom, w, h, allUnits, gameData,1) && found == 0)
                {
                    openX = i;
                    openY = bottom;
                    found = 1;
                }
            }
            //Check right column
            for (int i = y; i < y+h; i++)
            {
                if (openSpace(right, i, w, h, allUnits, gameData,1) && found == 0)
                {
                    openX = right;
                    openY = i;
                    found = 1;
                }
            }
            //Check top row
            for (int i = x; i < x+w; i++)
            {
                if (openSpace(i, top, w, h, allUnits, gameData, 1) && found == 0)
                {
                    openX = i;
                    openY = top;
                    found = 1;
                }
            }
            //Check left column
            for (int i = y; i < y+h; i++)
            {
                if (openSpace(left, i, w, h, allUnits, gameData, 1) && found == 0)
                {
                    openX = left;
                    openY = i;
                    found = 1;
                }
            }
            

            return Tuple.Create(openX, openY);
        }

        private bool openSpace(int x, int y, int w, int h, List<Unit>[] allUnits, tempMap gameData, int mode)
        { //check if there is already a unit at that position (even if unit is movable, we can't let build over; maybe in future we can)
            //also checks if there is collision between images
            //different checks for unit creation vs building construction
            for (int i = 0; i <= gameData.numPlayers; i++)
            {
                foreach (Unit u in allUnits[i])
                {
                    if (gameData.checkInvisible(u) && Math.Abs(u.x-x) < 10 && Math.Abs(u.y-y) < 10) //only check units nearby to selected coordinates and only if unit is not invisble //invisible == 0 default
                    {
                        int unitW = u.unitTileWidth;
                        int unitH = u.unitTileHeight;
                        //int unitW = u.defaultUnitTileW;
                        //int unitH = u.defaultUnitTileH;

                        if (!gameData.canPlaceOn(x,y)) return false; //check if it's traversible terrain

                        //One case for this unit being inside other unit, other case for other unit being inside this unit
                        /*if (x >= u.x && x < u.x + unitW && y >= u.y && y < u.y + unitH) //check top left corner
                        { //touching another unit
                            return false;
                        }
                        else if (x + w > u.x && x + w < u.x + unitW && y + h > u.y && y + h < u.y + unitH) //check bottom right corner
                        { //touching another unit (w/0 = sign)
                            return false;
                        }*/

                        if (mode == 0)
                        { //strict restriction for building (so no overlap)
                            //currently prevents building right next to each other (all buildings have gap)
                            if (x >= u.x && x <= u.x + unitW - 1 && y >= u.y && y <= u.y + unitH - 1) //check top left corner
                            { //touching another unit
                                return false;
                            }
                            else if (x + w-1 >= u.x && x + w-1 <= u.x + unitW - 1 && y + h-1 >= u.y && y + h-1 <= u.y + unitH - 1) //check bottom right corner
                            { //touching another unit
                                return false;
                            }
                            else if (x >= u.x && x <= u.x + unitW - 1 && y + h - 1 >= u.y && y + h - 1 <= u.y + unitH - 1) //check bottom left corner
                            { //touching another unit
                                return false;
                            }
                            else if (x + w - 1 >= u.x && x + w - 1 <= u.x + unitW - 1 && y >= u.y && y <= u.y + unitH - 1) //check top right corner
                            { //touching another unit
                                return false;
                            }
                            else if (x <= u.x && x + w - 1 >= u.x + unitW - 1 && y <= u.y && y + h - 1 >=  u.y + unitH - 1) //check if smaller unit is at the spot (so we don't build on top of them)
                            { //touching another unit
                                return false;
                            }
                        }
                        else if (mode == 1)
                        { //less restrictive so can spawn unit next to building
                            if (x >= u.x && x < u.x + unitW && y >= u.y && y < u.y + unitH) //check top left corner
                            { //touching another unit
                                return false;
                            }
                            else if (x + w > u.x && x + w < u.x + unitW && y + h > u.y && y + h < u.y + unitH) //check bottom right corner
                            { //touching another unit (w/0 = sign)
                                return false;
                            }
                        }



                    }

                }

            }

            return true;
        }

        public bool Register(ref List<Unit>[] allUnits, tempMap gameData, int x, int y, Microsoft.Xna.Framework.Graphics.GraphicsDevice g, int owner, int mode)
        { //adds building to list (at mouse position (x,y))
            //mode 0 for building, mode 1 for unit creation (so they don't conflict)
            bool success = false;
            int goldRequired, lumberRequired, foodRequired;
            //int owner = this.building.owner; //default 
            //Set resource for each building here (I don't know yet)

            
            if (this.buildStr != null || this.newUnitStr != null) //we actually have stuff to build
            {
                int typeIndex;
                if (mode == 0) typeIndex = nameTranslation(this.buildStr); //get index
                else typeIndex = nameTranslation(this.newUnitStr);
                goldRequired = SplashScreen.unitData.goldCost[typeIndex];
                lumberRequired = SplashScreen.unitData.lumberCost[typeIndex];
                foodRequired = SplashScreen.unitData.foodConsumption[typeIndex];
                int checkMode = 0;

                string curBuildStr;
                int unitW, unitH;
                if (this.buildStr != null && mode == 0)
                {
                    curBuildStr = this.buildStr;
                    unitW = this.building.unitTileWidth;
                    unitH = this.building.unitTileHeight;
                    //unitW = this.building.defaultUnitTileW;
                    //unitH = this.building.defaultUnitTileH;
                    checkMode = 0;
                }
                else
                {
                    curBuildStr = this.newUnitStr;
                    unitW = this.newUnit.unitTileWidth;
                    unitH = this.newUnit.unitTileHeight;
                    //unitW = this.newUnit.defaultUnitTileW;
                    //unitH = this.newUnit.defaultUnitTileH;
                    checkMode = 1;
                }
                
                if (gameData.canBuildOn(x, y, curBuildStr) && openSpace(x, y, unitW, unitH, allUnits, gameData, checkMode))
                {
                    if (mode == 0 && gameData.gold[owner] >= goldRequired && gameData.lumber[owner] >= lumberRequired && this.building.unitType >= SplashScreen.numUnits)
                    { //checks that we have enough resource
                        Unit temp = new Unit(curBuildStr, x, y, owner, this.findLastID(allUnits[owner]) + 1, ref gameData); //+1 for new id
                        if (temp.unitType >= SplashScreen.numUnits) temp.setBuildFrame(1); //set to construction frame for building

                        //Init minimap data for unit
                        Microsoft.Xna.Framework.Color[] uCol = new Microsoft.Xna.Framework.Color[1]; // Unit Color
                        uCol[0] = Microsoft.Xna.Framework.Color.Yellow;
                        //Microsoft.Xna.Framework.Graphics.Texture2D tempImg = getTextureFromBitmap(temp.objectImage, g);
                        //temp.objImg2D = tempImg; //store the img
                        Microsoft.Xna.Framework.Graphics.Texture2D uRec = new Microsoft.Xna.Framework.Graphics.Texture2D(g, 1, 1);
                        uRec.SetData(uCol);
                        temp.uRec = uRec;

                        allUnits[temp.owner].Add(temp); //add unit

                        gameData.gold[owner] -= goldRequired;
                        gameData.lumber[owner] -= lumberRequired;

                        //don't need to subtract/add food as that will be auto updated when unit added to screen

                        this.building = null;
                        this.buildStr = null;
                        success = true;
                        buildCancel(); //finished building so cancel build mode
                    }
                    //else if (mode == 1 && gameData.gold[owner] >= goldRequired && gameData.lumber[owner] >= lumberRequired && (gameData.foodMax[owner] >= foodRequired + gameData.food[owner])) //a unit
                    else if (mode == 1 && (gameData.foodMax[owner] >= foodRequired + gameData.food[owner])) //a unit so only check if enough open population in order to register unit (if not the buffer will make it so we wait for enough population)
                    { //unit creation probably
                        Unit temp = new Unit(curBuildStr, x, y, owner, this.findLastID(allUnits[owner]) + 1, ref gameData); //+1 for new id

                        //Init minimap data for unit
                        Microsoft.Xna.Framework.Color[] uCol = new Microsoft.Xna.Framework.Color[1]; // Unit Color
                        uCol[0] = Microsoft.Xna.Framework.Color.Yellow;
                        //Microsoft.Xna.Framework.Graphics.Texture2D tempImg = getTextureFromBitmap(temp.objectImage, g);
                        //temp.objImg2D = tempImg; //store the img
                        Microsoft.Xna.Framework.Graphics.Texture2D uRec = new Microsoft.Xna.Framework.Graphics.Texture2D(g, 1, 1);
                        uRec.SetData(uCol);
                        temp.uRec = uRec;

                        allUnits[temp.owner].Add(temp); //add unit

                        //Don't subtract resource here, subtracted when queueing
                        //gameData.gold[owner] -= goldRequired;
                        //gameData.lumber[owner] -= lumberRequired;

                        //don't need to subtract/add food as that will be auto updated when unit added to screen

                        this.newUnit = null;
                        this.newUnitStr = null;
                        success = true;
                    }
                    else
                    {
                        if (mode == 0)
                        {
                            this.buildMode = 0;
                            this.buildStr = null;
                            this.building = null;
                        }
                    }

                }
                else
                {
                    if (mode == 0)
                    {
                        this.buildMode = 0;
                        this.buildStr = null;
                        this.building = null;
                    }
                }

            }

            return success; //if build fails, we have to change menus back (like calling cancel)
        }

        public Microsoft.Xna.Framework.Graphics.Texture2D getImage()
        {
            return this.building.objImg2D;
        }

        private int findLastID(List<Unit> currentUnits)
        {
            int length = currentUnits.Count();
            int lastID = currentUnits.ElementAt(length - 1).id;
            return lastID;
        }

        public void buildCancel()
        { //cancel building mode

            this.buildMode = 0;
            this.building = null;
            this.buildStr = null;

        }


        private int nameTranslation(string name)
        { //function to return index in array of asset type (basically a map)
            //string[] assetTypes = SplashScreen.allUnitNames;
            string[] assetTypes = this.allUnitNames;
            int index = -1; //index of asset name type, -1 = none

            for (int i = 0; i < assetTypes.Length; i++)
            {
                if (assetTypes[i] == name)
                {
                    index = i;
                    return index;
                }
            }

            return index;
        }

        public static Microsoft.Xna.Framework.Graphics.Texture2D getTextureFromBitmap(System.Drawing.Bitmap b, Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice)
        { //copied from XNA_InGame
            Microsoft.Xna.Framework.Graphics.Texture2D tx = null;
            using (System.IO.MemoryStream s = new System.IO.MemoryStream())
            {
                b.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                s.Seek(0, System.IO.SeekOrigin.Begin);
                tx = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(graphicsDevice, s);
            }
            return tx;
        }
    }

    
}
