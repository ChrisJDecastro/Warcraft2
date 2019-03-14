using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework.Graphics;
/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;*/


namespace Demo
{
    public class UpgradeData
    {
        public int[] armorImprovement;
        public int[] sightImprovement;
        public int[] speedImprovement;
        public int[] basicDamageImprovement;
        public int[] piercingDamageImprovement;
        public int[] rangeImprovement;
        public int[] goldCost;
        public int[] lumberCost;
        public int[] time;
        public int[] numberOfAssets;
        public List<string>[] assetNames;
        public int numUpgrades = 0;

        string[] allUpgradeNames = SplashScreen.allUpgradeNames;

        public int upgradeTranslation(string upgradeStr)
        {
            for (int i = 0; i < this.allUpgradeNames.Length; i++)
            {
                if (upgradeStr == this.allUpgradeNames[i]) return i;
            }
            return -1; //unfound
        }

        public UpgradeData()
        { //init all
            this.numUpgrades = allUpgradeNames.Length;
            this.armorImprovement = new int[allUpgradeNames.Length];
            this.sightImprovement = new int[allUpgradeNames.Length];
            this.speedImprovement = new int[allUpgradeNames.Length];
            this.basicDamageImprovement = new int[allUpgradeNames.Length];
            this.piercingDamageImprovement = new int[allUpgradeNames.Length];
            this.rangeImprovement = new int[allUpgradeNames.Length];
            this.goldCost = new int[allUpgradeNames.Length];
            this.lumberCost = new int[allUpgradeNames.Length];
            this.time = new int[allUpgradeNames.Length];
            this.numberOfAssets = new int[allUpgradeNames.Length];
            this.assetNames = new List<string>[allUpgradeNames.Length];
            for (int i = 0; i < allUpgradeNames.Length; i++)
            {
                this.assetNames[i] = new List<string>();
            }
        }
    }

    public class AssetData
    { //to hold all asset data
        //# Resource Name (Same order as loading list) (basically use unitType to access)
        //Hit Points
        public int[] hitPoints;
        // Armor
        public int[] armor;
        // Sight
        public int[] sight;
        // Sight during construction
        public int[] sightConstruction;
        // Size
        public int[] size; //I think this is tile size? Not really used for us?
        // Speed
        public int[] speed; //0 for buildings
        // Gold Cost
        public int[] goldCost;
        //Lumber Cost
        public int[] lumberCost;
        //Food Consumption (Negative Production)
        public int[] foodConsumption;
        //Build Time (s)
        public int[] buildTime;
        //Attack Steps
        public int[] attackSteps; //0 for buildings (might be the number of time it takes for doing damage?)
        //Reload Steps
        public int[] reloadSteps; //arching units only?
        //Basic Damage
        public int[] basicDamage; //0 if unit can't fight
        //Piercing Damage
        public int[] piercingDamage;
        //Range
        public int[] range; //pretty straight forward
        //Capability Count
        public int[] capabilityCount; //basically number of buttons it has
        //Name of Capabilities
        public List<string>[] capabilitiesNames; //list of button names for each unit
        //Requirement Count
        public int[] requirementCount; //requirement (of other units) in order to build unit
        //List of requirement
        public List<string>[] requirementList;

        public AssetData clone()
        { //to clone all game data
            AssetData temp = new AssetData();
            temp.hitPoints = this.hitPoints.Clone() as int[];
            temp.armor = this.armor.Clone() as int[];
            temp.sight = this.sight.Clone() as int[];
            temp.sightConstruction = this.sightConstruction.Clone() as int[];
            temp.size = this.size.Clone() as int[];
            temp.speed = this.speed.Clone() as int[];
            temp.goldCost = this.goldCost.Clone() as int[];
            temp.lumberCost = this.lumberCost.Clone() as int[];
            temp.foodConsumption = this.foodConsumption.Clone() as int[];
            temp.buildTime = this.buildTime.Clone() as int[];
            temp.attackSteps = this.attackSteps.Clone() as int[];
            temp.reloadSteps = this.reloadSteps.Clone() as int[];
            temp.basicDamage = this.basicDamage.Clone() as int[];
            temp.piercingDamage = this.piercingDamage.Clone() as int[];
            temp.range = this.range.Clone() as int[];
            temp.capabilityCount = this.capabilityCount.Clone() as int[];
            temp.capabilitiesNames = this.capabilitiesNames; //don't have to clone?
            temp.requirementCount = this.requirementCount.Clone() as int[];
            temp.requirementList = this.requirementList;

            return temp;
        }

    }
    public class AssetsOnMap
    {
        //Map Assets
        public List<Asset>[][] mapAssets; //hold lists of players' assets (player 0 = nature) for each map
        public int playerChoice; //holds the chosen player to play as, starts as default nature
        public int[] numPlayer; //holds the number of player for each map
        public int[][] startGold;
        public int[][] startLumber;
        public int[][] DLumber; //global lumber for each position of map for each map
        public Bitmap[][] allUnitTiles; //hold image tiles for each unit

        public Texture2D[][] allUnitTilesT2d;
        public string[][] allUnitLines; //hold parsed .dat file lines for each unit
        public int[] allUnitTileW;
        public int[] allUnitTileH;

        public Bitmap[][] decayTiles; //hold unit decay tiles (for both unit and building)
        public Texture2D[] decayTiles2D; //only hold for unit as building has another one

        public Bitmap[][] buildingDeathTiles; //hold building death tiles for each building
        public Texture2D[][] buildingDeathTiles2D;

        public Bitmap[][] allProjectiles; //holds all projectiles tiles
        public Texture2D[][] cannonBallTiles2D; //holds cannonball tile and death tiles
        public Texture2D[] arrowTiles2D;

        public Bitmap[] miniBevel; //holds the minibevel tiles
        public Texture2D[] miniBevel2D; //minibevel in 2D
        //Map asset tilesets

        public Bitmap[] droppedResource;
        public Texture2D[] droppedResource2D;

        //Convert all unitTiles into allUnitTilesT2d
        public void convertBitmapsToTextures(GraphicsDevice graphicsDevice)
        {

            allUnitTilesT2d = new Texture2D[allUnitTiles.Length][];
            for (int k = 0; k < allUnitTiles.Length; k++)
            {
                allUnitTilesT2d[k] = new Texture2D[allUnitTiles[k].Length];
                for (int l = 0; l < allUnitTiles[k].Length; l++)
                {
                    allUnitTilesT2d[k][l]= XNA_InGame.getTextureFromBitmap(allUnitTiles[k][l],graphicsDevice);
                    
                }
            }

            //Convert decay tiles to texture2D
            decayTiles2D = new Texture2D[decayTiles[0].Length];
            for (int k = 0; k < decayTiles[0].Length; k++)
            {
                decayTiles2D[k] = XNA_InGame.getTextureFromBitmap(decayTiles[0][k], graphicsDevice);
            }


            //Convert building death tiles to texture2D
            buildingDeathTiles2D = new Texture2D[buildingDeathTiles.Length][];
            for (int i = 0; i < buildingDeathTiles.Length; i++)
            {
                buildingDeathTiles2D[i] = new Texture2D[buildingDeathTiles[i].Length];
                for (int k = 0; k < buildingDeathTiles[i].Length; k++)
                {
                    buildingDeathTiles2D[i][k] = XNA_InGame.getTextureFromBitmap(buildingDeathTiles[i][k], graphicsDevice);
                }
            }

            //Convert cannonball tiles to texture2D
            cannonBallTiles2D = new Texture2D[2][]; //one for alive tiles, other for death tiles
            for (int i = 0; i < 2; i++)
            {
                cannonBallTiles2D[i] = new Texture2D[allProjectiles[i + 1].Length]; //+1 to skip arrow data
                for (int k = 0; k < allProjectiles[i + 1].Length; k++)
                {
                    cannonBallTiles2D[i][k] = XNA_InGame.getTextureFromBitmap(allProjectiles[i + 1][k], graphicsDevice);
                }
            }

            //Convert arrow tiles to texture2D
            arrowTiles2D = new Texture2D[allProjectiles[0].Length];
            for (int i = 0; i < allProjectiles[0].Length; i++)
            {
                arrowTiles2D[i] = XNA_InGame.getTextureFromBitmap(allProjectiles[0][i], graphicsDevice);
            }

            miniBevel2D = new Texture2D[miniBevel.Length];
            for (int i = 0; i < miniBevel.Length; i++)
            {
                miniBevel2D[i] = XNA_InGame.getTextureFromBitmap(miniBevel[i], graphicsDevice);
            }

            droppedResource2D = new Texture2D[droppedResource.Length];
            for (int i = 0; i < droppedResource.Length; i++)
            {
                droppedResource2D[i] = XNA_InGame.getTextureFromBitmap(droppedResource[i], graphicsDevice);
            }
        }

    }

    

    public class Asset
    {
        public string type;
        public int x;
        public int y;
    }
    //class asset needed to hold each asset and its location, type, etc.
    public class PlayerAsset
    {
        static string[] assetTypes = SplashScreen.allUnitNames;
        static string[] allUpgradeNames = SplashScreen.allUpgradeNames;

        public UpgradeData LoadUpgradeData()
        {
            string folder = "upg"; //resource folder
            UpgradeData temp = new UpgradeData();

            for (int i = 0; i < allUpgradeNames.Length; i++) //iterate through all 
            {
                string curFile = allUpgradeNames[i] + ".dat"; //generate dat file name
                string imgpath = @"data\" + folder + @"\" + curFile; //generate full image path
                string[] lines = System.IO.File.ReadAllLines(imgpath); //read all the lines
                int index = 3; //start at index 3 to skip first 3 comment line basically
                //Load all data into data structure for easy readability
        
                Int32.TryParse(lines[index], out temp.armorImprovement[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.sightImprovement[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.speedImprovement[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.basicDamageImprovement[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.piercingDamageImprovement[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.rangeImprovement[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.goldCost[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.lumberCost[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.time[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.numberOfAssets[i]);
                index += 2;
                for (int j = 0; j < temp.numberOfAssets[i]; j++) //load list of capabilities
                {
                    temp.assetNames[i].Add(lines[index]); //add to list
                    index++;
                }
            }

            return temp;
        }

        public AssetData LoadAssetData()
        {
            string folder = "res"; //resource folder
            AssetData temp = new AssetData();
            //Init all storage memory
            temp.hitPoints = new int[assetTypes.Length];
            temp.armor = new int[assetTypes.Length];
            temp.sight = new int[assetTypes.Length];
            temp.sightConstruction = new int[assetTypes.Length];
            temp.size = new int[assetTypes.Length];
            temp.speed = new int[assetTypes.Length];
            temp.goldCost = new int[assetTypes.Length];
            temp.lumberCost = new int[assetTypes.Length];
            temp.foodConsumption = new int[assetTypes.Length];
            temp.buildTime = new int[assetTypes.Length];
            temp.attackSteps = new int[assetTypes.Length];
            temp.reloadSteps = new int[assetTypes.Length];
            temp.basicDamage = new int[assetTypes.Length];
            temp.piercingDamage = new int[assetTypes.Length];
            temp.range = new int[assetTypes.Length];
            temp.capabilityCount = new int[assetTypes.Length];
            temp.capabilitiesNames = new List<string>[assetTypes.Length];
            temp.requirementCount = new int[assetTypes.Length];
            temp.requirementList = new List<string>[assetTypes.Length];


            for (int i = 0; i < assetTypes.Length; i++) //iterate through all assetTypes in above array
            {
                string curFile = assetTypes[i] + ".dat"; //generate dat file name
                string imgpath = @"data\" + folder + @"\" + curFile; //generate full image path
                string[] lines = System.IO.File.ReadAllLines(imgpath); //read all the lines
                int index = 3; //start at index 3 to skip first 3 comment line basically
                //Load all data into data structure for easy readability
                Int32.TryParse(lines[index], out temp.hitPoints[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.armor[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.sight[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.sightConstruction[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.size[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.speed[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.goldCost[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.lumberCost[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.foodConsumption[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.buildTime[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.attackSteps[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.reloadSteps[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.basicDamage[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.piercingDamage[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.range[i]);
                index += 2;
                Int32.TryParse(lines[index], out temp.capabilityCount[i]);
                index += 2;
                temp.capabilitiesNames[i] = new List<string>();
                for (int j = 0; j < temp.capabilityCount[i]; j++) //load list of capabilities
                {
                    temp.capabilitiesNames[i].Add(lines[index]); //add to list
                    index++;
                }
                index++; //to skip comment line
                Int32.TryParse(lines[index], out temp.requirementCount[i]);
                index += 2;
                temp.requirementList[i] = new List<string>();
                for (int j = 0; j < temp.requirementCount[i]; j++) //load list of requirements
                {
                    temp.requirementList[i].Add(lines[index]); //add to list
                    index++;
                }
            }

            return temp;
        }
        public Tuple<Bitmap[][],string[][], int[], int[]> LoadAssetTiles()
        {
            Bitmap[][] allUnitTiles = new Bitmap[assetTypes.Length][];
            string[][] allUnitLines = new string[assetTypes.Length][];
            int[] allUnitWidth = new int[assetTypes.Length];
            int[] allUnitHeight = new int[assetTypes.Length];
            //int[][] allUnitIndex = new int[assetTypes.Length][];
            string folder = "img";
            CGraphicTileset loader = new CGraphicTileset(); //use for loading data/img/ files

            for (int i = 0; i < assetTypes.Length; i++) //iterate through all assetTypes in above array
            {
                string curFile = assetTypes[i] + ".dat"; //generate dat file name
                Tuple<string[], Bitmap[], int, int, int> temp = loader.LoadTileset(curFile, folder);
                allUnitTiles[i] = temp.Item2; //image

         
                /*if (i < 3) //resize unit images only if unit
                {
                    allUnitTiles[i] = resizeImage(allUnitTiles[i]);
                }*/
                allUnitLines[i] = temp.Item1;
                allUnitWidth[i] = temp.Item4;
                allUnitHeight[i] = temp.Item5;
            }


            return Tuple.Create(allUnitTiles, allUnitLines, allUnitWidth, allUnitHeight);
        }

        public Bitmap[][] LoadCorpse()
        { //Load unit corpse and building death explosion
            string[] fileToLoad = { "Corpse.dat", "BuildingDeath.dat"};
            CGraphicTileset tempLoader = new CGraphicTileset();
            Bitmap[][] temp = new Bitmap[fileToLoad.Length][]; //hardcoded
            for (int i = 0; i < fileToLoad.Length; i++)
            {
                //temp[i] = new Bitmap[16];
                Tuple<string[], Bitmap[], int, int, int> tempRet = tempLoader.LoadTileset(fileToLoad[i], "img");
                temp[i] = tempRet.Item2;
            }
            return temp;
        }

        public Bitmap[][] LoadProjectiles()
        {
            string[] filesToLoad = new string[SplashScreen.allProjectilesNames.Length+1]; //plus 1 for cannonballdeath
            for (int i = 0; i < SplashScreen.allProjectilesNames.Length; i++)
            {
                filesToLoad[i] = SplashScreen.allProjectilesNames[i] + ".dat";
            }
            filesToLoad[filesToLoad.Length - 1] = "CannonballDeath.dat";
            CGraphicTileset tempLoader = new CGraphicTileset();
            Bitmap[][] temp = new Bitmap[filesToLoad.Length][]; //hardcoded
            for (int i = 0; i < filesToLoad.Length; i++)
            {
                Tuple<string[], Bitmap[], int, int, int> tempRet = tempLoader.LoadTileset(filesToLoad[i], "img");
                temp[i] = tempRet.Item2;
            }
            return temp;
        }

        public Bitmap[][] ScaleBuildingDeath(Bitmap[][] allTiles, Bitmap[][] decayTiles)
        { //must be called after LoadCorpse and after loading all building tiles (allTiles is all unit + building tiles)
            int numUnits = SplashScreen.numUnits;
            int numBuildings = allTiles.Length - numUnits; //numBuilding = total # of unit/building tiles - # unit tiles
            Bitmap[][] temp = new Bitmap[numBuildings][];
            for (int i = 0; i < numBuildings; i++) //loop through all buildings
            {
                temp[i] = new Bitmap[decayTiles[1].Length];  //hardcode only to choose building decay tiles (1)
                for (int k = 0; k < decayTiles[1].Length; k++)
                {
                    int buildingW = allTiles[i + numUnits][0].Width;
                    int buildingH = allTiles[i + numUnits][0].Height;
                    temp[i][k] = new Bitmap(buildingW, buildingH);
                    using (Graphics g = Graphics.FromImage(temp[i][k]))
                    {//scale the explosion image to the size of our building
                        //Reference: SplashScreen.cs
                        g.DrawImage(decayTiles[1][k],new Rectangle(0, 0, buildingW, buildingH),0.0f, 0.0f,decayTiles[1][k].Width,decayTiles[1][k].Height, GraphicsUnit.Pixel);
                    }
                }
            }


            return temp;
        }
        
        public Tuple<List<Asset>[], int[], int[], int> LoadAsset(string[] mapList, int skipLines)
        { //Load all assets for ONE map
            const int skip = 5; //skip this amount at start of file offset
            
            int startIndex = skip + (skipLines * 2 + 3 + 1); //to start on number of players line

            string[] tempStr = mapList[startIndex].Split();
            
            int[] tempNumPlayers = Array.ConvertAll(tempStr, int.Parse);
            int numPlayers = tempNumPlayers[0]; //get number of players
            List<Asset>[] mapStuff;
            mapStuff = new List<Asset>[numPlayers+1];
            int[] startGold = new int[numPlayers + 1];
            int[] startLumber = new int[numPlayers + 1];

            startIndex += 2; //to skip #Comment line for starting resources

            for (int i = 0; i <= numPlayers; i++) //<= because we have player 0 as nature
            {
                mapStuff[i] = new List<Asset>();
                string[] dimToken = mapList[startIndex+i].Split(); //split the two dimensions apart
                int[] dimNum = Array.ConvertAll(dimToken, int.Parse); //converet numbers into 
                //Use dimNum[0] which is player number assuming that it is not ordered
                startGold[dimNum[0]] = dimNum[1];
                startLumber[dimNum[0]] = dimNum[2];
            }
            startIndex += (numPlayers + 1) + 1; //+1 for nature player 0, +1 for skipping comment line
            int numAssets = Convert.ToInt32(mapList[startIndex]); //get numAsset line for loading
            
            startIndex += 2; //to skip comment assets

            for (int i = 0; i < numAssets; i++)
            {
                string[] Token = mapList[startIndex + i].Split(); //split into 4 string <type owner x y>
                Asset temp = new Asset();
                temp.type = Token[0];
                string[] Token2 = new string[3];
                Token2[0] = Token[1]; //owner
                Token2[1] = Token[2]; //x
                Token2[2] = Token[3]; //y
                int[] tempNum = Array.ConvertAll(Token2, int.Parse);
                temp.x = tempNum[1];
                temp.y = tempNum[2];
                mapStuff[tempNum[0]].Add(temp); //add asset to its corresponding player
                //Error on hedges.map (7 instead of 6 players?!)
                //temp = null; //clear memory?
            }
            return Tuple.Create(mapStuff, startGold, startLumber, numPlayers);
        }

        public Bitmap[] loadMiniBevel()
        {
            string filesToLoad = "MiniBevel.dat";
            
            CGraphicTileset tempLoader = new CGraphicTileset();
            Bitmap[] temp;
            Tuple<string[], Bitmap[], int, int, int> tempRet = tempLoader.LoadTileset(filesToLoad, "img");
            temp = tempRet.Item2;
            return temp;
        }

        public Bitmap[] loadDroppedResource()
        {
            string filesToLoad = "DroppedResources.dat";

            CGraphicTileset tempLoader = new CGraphicTileset();
            Bitmap[] temp;
            Tuple<string[], Bitmap[], int, int, int> tempRet = tempLoader.LoadTileset(filesToLoad, "img");
            temp = tempRet.Item2;
            return temp;
        }

    }
}


