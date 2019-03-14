using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using WMPLib;

/* Info (List Bugs founded here):
 * 
 * Anyone can try to fix any of these (and list big changes that were made to files like deleting them):
 * 
 * The Cgraphictileset class is an object that can be instantiated to call the functions that can load images.
 * It can be used for splash image, cursor image, etc., but this uses a lot of memory
 *  Usage of this function in Splash and Cursor may still need to be implemented
 * 
 * The game currently has extreme amount of memory usage due to all the global variables.
 * Currently no way to fix this as we need global variables to access the values from other forms.
 * 
 * Deleted Form2
 * 
 * Many parts of the codes need to be broken down into smaller functions (and they can be, just that we all lazy).
 * 
 * 
 * All functions needed to be documented still~ Please use comments (at least a comment on what the function does if it's not obvious) to make life easier on the documenter.
 * 
 * Note: numMap variable was removed, can call mapObject.mapNames.Length instead
 * 
 * 
*/


namespace Demo
{

    public class loader
    {
        //All tile variables
        public Bitmap[] tiles;
        public Microsoft.Xna.Framework.Graphics.Texture2D[] tiles2D = null;
        public string[] lines;
        public int numTiles, tileWidth, tileHeight;


        //AZ: convert all tile bitmaps to tex2Ds
        public void convertBitmapsToTextures(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice)
        {
            tiles2D = new Microsoft.Xna.Framework.Graphics.Texture2D[tiles.Length];
                for (int l = 0; l < tiles.Length; l++)
                {
                    tiles2D[l] = XNA_InGame.getTextureFromBitmap(tiles[l], graphicsDevice);

                }


        }
    }
    


    public partial class SplashScreen : Form
    {
        public PictureBox pbAbove;
        public float timer = 10;
        public float opacity = 1.0f;
        Bitmap lower;
        Bitmap upper;
        WMPLib.WindowsMediaPlayer mp;

        //Flags for checking if loading is finished
        public static int tileFinished = 0;

        //Objects of each loaded data
        public static loader tileObject;
        public static maps mapObject;
        public static Bitmap[] cursors;
        public static AssetsOnMap mapUnits;
        public static ButtonImage icons;
        public static AssetData unitData;
        public static UpgradeData unitUpgradeData;
        public static ColorsLoader coloredUnits;
        public static Bitmap mapListBG;
        //public static Form mainMenuForm; //holds pointer to mainMenu that can be accessed elsewhere

        //Placed all unit names global so only need to edit here to edit all
        public static int numUnits = 4; //holds a default index variable for the where the number of units end in allUnitNames
        public static string[] allUnitNames = { "Peasant", "Footman", "Archer", "Ranger", "GoldMine", "TownHall", "Keep", "Castle", "Farm", "Barracks", "LumberMill", "Blacksmith", "ScoutTower", "GuardTower", "CannonTower" };
        public static string[] allUpgradeNames = { "Armor2", "Armor3", "Arrow2", "Arrow3", "Longbow", "Marksmanship", "Ranger", "Scouting", "Weapon2", "Weapon3" };
        public static string[] allProjectilesNames = { "Arrow", "Cannonball" };
        public static string[] droppedResourceNames = { "gold", "lumber" };
        public static int newGameFeatures = 1; //global variable to whether include new game features or not during runtime (currently not in use, but wrap all new features with this variable in future if any)
        public static int renderProjectiles = 1; //whether to render projectiles or not in game (maybe more lag if we do? 1 for render, 0 for off)

        //Player color data (moved here)
        public static int[] playerColor;// Player Color Here!!!!!
                                        //Color variable
        public static int renderColor = 1; //1 for turning different colors on (longer load time for now); 0 to turn off
        public static int bootUpAI = 1; //1 to have AI for non-player activated (AI is buggy and may cause crashes)
        public static Bitmap color = new Bitmap(@"data\img\Colors.png"); //stored globally (so can access in any form)
        

        // Player AI difficulty, player2 is playerAI[0], the value stored inside, 0 = low AI, 1 = Medium, 2 = High.
        public static int[] playerAI;
        

        static void initAssets()
        {
            //Load background for maplist menu
            //6=black, 1 = blue
            Bitmap backGroundImage = new Bitmap(@"data\img\Texture.png");
            Color[] blueData = new Color[4];
            Color[] blackData = new Color[4];
            Color[] blackData2 = new Color[4];

            for (int i = 0; i < 4; i++)
            {
                blueData[i] = color.GetPixel(i, 1);
                blackData[i] = color.GetPixel(i, 6);
                blackData2[i] = backGroundImage.GetPixel(i, 0); //using this actually shows that we can't change color, but it's not changing from black (so the provided colors.dat black pixels are wrong?)
            }
            //Iterate through all pixels and change black to blue
            for (int j = 0; j < backGroundImage.Height; j++)
            {
                for (int i = 0; i < backGroundImage.Width; i++)
                {
                    if (backGroundImage.GetPixel(i, j) == blackData[0]) backGroundImage.SetPixel(i, j, blueData[0]);
                    if (backGroundImage.GetPixel(i, j) == blackData[1]) backGroundImage.SetPixel(i, j, blueData[1]);
                    if (backGroundImage.GetPixel(i, j) == blackData[2]) backGroundImage.SetPixel(i, j, blueData[2]);
                    if (backGroundImage.GetPixel(i, j) == blackData[3]) backGroundImage.SetPixel(i, j, blueData[3]);
                }
            }

            mapListBG = backGroundImage;

            int numFiles = mapObject.mapNames.Length;

            //Asset stuff
            mapUnits = new AssetsOnMap(); //class to hold all loaded data
            PlayerAsset tempAsset = new PlayerAsset(); //the loader
            mapUnits.playerChoice = 1; //default as player1 (we can only play as that it seems) (set to nature = 0 for testing)
            mapUnits.numPlayer = new int[numFiles];
            mapUnits.startGold = new int[numFiles][];
            mapUnits.startLumber = new int[numFiles][];
            mapUnits.mapAssets = new List<Asset>[numFiles][];
            mapUnits.DLumber = new int[numFiles][];

            for (int i = 0; i < numFiles; i ++)
            {
                int mapY = mapObject.mapY[i];
                int mapX = mapObject.mapX[i];
                string[] curLines = mapObject.mapLines[i];
                Tuple<List<Asset>[], int[], int[], int> tempRet = tempAsset.LoadAsset(curLines, mapY);
                mapUnits.mapAssets[i] = tempRet.Item1;
                mapUnits.startGold[i] = tempRet.Item2;
                mapUnits.startLumber[i] = tempRet.Item3;
                mapUnits.numPlayer[i] = tempRet.Item4;
                //mapObject.mapLines[i] = null; //free data?

                //init global lumber for all maps
                mapUnits.DLumber[i] = new int[(mapX + 1) * (mapY + 1)]; //same size as Dpartials or mapString
                for (int y = 0; y < mapY + 1; y++)
                {
                    for (int x = 0; x < mapX + 1; x++)
                    {
                        if (mapObject.allMapStrings[i][y*(mapX+1)+x] == "forest")
                        {//is forest so set starting lumber of a tree at a spot to nature's lumber value (should be 400 by default)
                            mapUnits.DLumber[i][y * (mapX + 1) + x] = mapUnits.startLumber[i][0];
                        }
                        else mapUnits.DLumber[i][y * (mapX + 1) + x] = 0; //no lumber as it's not forest tile

                    }
                }
            }
            Tuple<Bitmap[][], string[][], int[], int[]> tempUnit = tempAsset.LoadAssetTiles();
            mapUnits.allUnitTiles = tempUnit.Item1;
            mapUnits.allUnitLines = tempUnit.Item2;
            mapUnits.allUnitTileW = tempUnit.Item3;
            mapUnits.allUnitTileH = tempUnit.Item4;
            mapUnits.decayTiles = tempAsset.LoadCorpse();
            mapUnits.buildingDeathTiles = tempAsset.ScaleBuildingDeath(mapUnits.allUnitTiles, mapUnits.decayTiles);
            mapUnits.allProjectiles = tempAsset.LoadProjectiles();

            unitData = tempAsset.LoadAssetData(); //load asset .dat
            unitUpgradeData = tempAsset.LoadUpgradeData(); //load upgrade .dat

            mapUnits.miniBevel = tempAsset.loadMiniBevel();
            mapUnits.droppedResource = tempAsset.loadDroppedResource();

            //free the inited temp stuff?
            initButtons();

        }

        static void initButtons()
        {
            icons = new ButtonImage();
            CGraphicTileset generator = new CGraphicTileset();
            Tuple<string[], Bitmap[], int, int, int> tileData = generator.LoadTileset("Icons.dat", "img");
            icons.lines = tileData.Item1;
            icons.tiles = tileData.Item2;
            icons.numTiles = tileData.Item3;
            icons.tileWidth = tileData.Item4;
            icons.tileHeight = tileData.Item5;

            initDone(); //we are done loading stuff
        }
        static void initMap()
        {
            //Get data for maps
            CGraphicTileset generator = new CGraphicTileset();
            mapObject = new maps();
            mapObject.DTileIndices = generator.OrganizeTiles(tileObject.lines);
            Tuple<int[][], int[][], string[], int[][], string[][], int[][], string[][]> tempMapData = generator.LoadMap("map", tileObject.lines, mapObject.DTileIndices);
            mapObject.mapData = tempMapData.Item1;
            mapObject.mapDataIndices = tempMapData.Item2;
            mapObject.mapNames = tempMapData.Item3;
            int[][] tempMapXY = tempMapData.Item4;
            mapObject.mapX = tempMapXY[0];
            mapObject.mapY = tempMapXY[1];
            mapObject.allMapStrings = tempMapData.Item5;
            mapObject.mapPartials = tempMapData.Item6;
            mapObject.mapLines = tempMapData.Item7;

            int numMaps = mapObject.mapNames.Length;
            mapObject.entireMaps = new Bitmap[numMaps];
            mapObject.entireMapsIndex = new EntireMapsIndex[numMaps];

            for (int i = 0; i < numMaps; i++)
            {
                //mapObject.entireMaps[i] = new Bitmap(mapObject.mapX[i], mapObject.mapY[i]);

                //AZ: Optimized this step, no longer need to generate big bitmaps..
                mapObject.entireMaps[i] = generator.generateMiniMap(i, SplashScreen.tileObject, mapObject);
                //Just need a 2D int array:
                mapObject.entireMapsIndex[i] = generator.generateMapIndex(i, SplashScreen.tileObject, mapObject);
            }

        }
        static void initTile()
        {
            //Get data for tile
            CGraphicTileset generator = new CGraphicTileset();
            tileObject = new loader();
            Tuple<string[], Bitmap[], int, int, int> tileData = generator.LoadTileset("Terrain.dat", "img");
            tileObject.lines = tileData.Item1;
            tileObject.tiles = tileData.Item2;
            tileObject.numTiles = tileData.Item3;
            tileObject.tileWidth = tileData.Item4;
            tileObject.tileHeight = tileData.Item5;
        }


        static void initAll() //calls all init functions
        {
            initTile(); //must be called before initMap
            initMap();
            initAssets();
            CursorLoader.initialize();

            //Initialize all the cursors
        }

        static void initDone() //for calling memory clear functions
        { //clear all object data we don't need here
            mapObject.mapLines = null; //free parsed data as we already read/load them
            tileObject.lines = null; //we don't need tile lines either as they are stored in index form
            tileFinished = 1; //turn flag on because we are done

        }

        public SplashScreen()
        {
            //Cathy's music code
            
            mp = new WMPLib.WindowsMediaPlayer();
            mp.URL = @"data\snd\music\load.mp3";
            mp.controls.play();

            InitializeComponent();
            //Thread tilethread = new Thread(new ThreadStart(InitTiles(ref lines, ref tiles, ref numTiles, ref tileWidth, ref tileHeight)));
            //Starts thread 
            Thread tileThread = new Thread(new ThreadStart(SplashScreen.initAll));
            tileThread.IsBackground = true;
            tileThread.Start();
            //  this.BackgroundImage = new Image();
            //Uri uri = new Uri(@"data\img\Splash.png", UriKind.Relative);
            //PngBitmapDecoder decoder2 = new PngBitmapDecoder(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            Bitmap a = new Bitmap(@"data\img\Splash.png");
            upper = a.Clone(new Rectangle(0, 0, a.Size.Width, a.Size.Height / 2), a.PixelFormat);
            lower = a.Clone(new Rectangle(0, a.Size.Height / 2, a.Size.Width, a.Size.Height / 2), a.PixelFormat);
            a = upper;
            pbAbove = new PictureBox();
            pbAbove.Image = lower;

            // JD 2/1/2019: reset the splash image size to 800, 600
            //pbAbove.Size = new Size(a.Size.Width / 2, a.Size.Height / 2);
            pbAbove.Size = new Size(800, 600);
            pbAbove.SizeMode = PictureBoxSizeMode.StretchImage;
            pbAbove.BackColor = Color.Transparent;

            //this.Size = new Size(a.Size.Width / 2, a.Size.Height / 2);
            this.Size = new Size(800, 600);

            this.Controls.Add(pbAbove);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            opacity -= 0.00001f;
            if (opacity > 0)
            {

                // float opacity = alpha / 255.0f;

                Image img = pbAbove.Image;
                using (Graphics g = Graphics.FromImage(img))
                {
                    //Pen pen = new Pen(Color.FromArgb(opacity, 255, 255, 255), img.Width);
                    // g.DrawRectangle(pen, new Rectangle(0, 0, img.Width, img.Height));
                    g.DrawImage(
        lower,
 new Rectangle(0, 0, img.Width, img.Height),  // destination rectangle
 0.0f,                          // source rectangle x 
 0.0f,                          // source rectangle y
 upper.Width,                        // source rectangle width
 upper.Height,                       // source rectangle height
 GraphicsUnit.Pixel);

                    /* g.DrawLine(
                        new Pen(Color.Black, 25),
                        new Point(10, 35),
                        new Point(200, 35));*/

                    float[][] matrixItems ={
                                new float[] {1, 0, 0, 0, 0},
                                new float[] {0, 1, 0, 0, 0},
                                new float[] {0, 0, 1, 0, 0},
                                new float[] {0, 0, 0, 0.15f, 0},
                                new float[] {0, 0, 0, 0, 1}};

                    ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
                    ImageAttributes imageAttributes = new ImageAttributes();
                    imageAttributes.SetColorMatrix(
                        colorMatrix,
                        ColorMatrixFlag.Default,
                        ColorAdjustType.Bitmap);
                    //g.DrawLine(pen, -1, -1, img.Width, img.Height);


                    g.DrawImage(
          upper,
   new Rectangle(0, 0, img.Width, img.Height),  // destination rectangle
   0.0f,                          // source rectangle x 
   0.0f,                          // source rectangle y
   upper.Width,                        // source rectangle width
   upper.Height,                       // source rectangle height
   GraphicsUnit.Pixel,
  imageAttributes);
                    g.Save();
                }
                pbAbove.Image = img;

            }
            if (timer > 0) //if we're done loading we don't need to wait for timer
            {
                timer -= 1;


            }

            //delete the else to reduce loading time
             if (tileFinished == 1)
            {
                timer1.Stop();
                //this.mainMenuForm = new MainMenu();
                //this.mainMenuForm.Show();
                new MainMenu().Show();
                mp.controls.stop();
                this.Hide();

                //Free up resource
               this.pbAbove = null; //free picturebox
               this.lower = null;
               this.upper = null;
               //this.mp = null;
                }
            else timer += 1; //add more to timer as we aren't done loading
        }

        private void SplashScreen_Load_1(object sender, EventArgs e)
        {

            timer = 60; //lowered default timer so faster computers can load faster
            timer1.Start();
        }
    }
}