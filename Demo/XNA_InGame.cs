using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RoyT.AStar;



/* Note to Alan:
 * 
 * Changed chosenUnits selection list to list of unitPtr class (so we don't instantiate another list of objects? - not sure if it helped with memory)
 * 
 * Cloned arrays (but unsuccessfuly probably) in AssetDecoratedMap.cs, but not sure how to clone int values in Unit.cs (not even sure if we need to, maybe we should test first)
 * 
 * Colors can be turned off in SplashScreen.cs with renderColor flag.
 * Fog of war can be turned off in AssetDecoratedMap.cs with renderFogFlag flag.
 * 
 */

namespace Demo
{
    public class XNA_InGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        static Texture2D _pointTexture;
        //public int endGameFlag = 0;

        bool clickFlag = false;
        string clickUnit0;
        string clickUnit1;

        //Don't need theMap any more.
        Texture2D theMap;
        int theMapWidth;
        int theMapHeight;
        EntireMapsIndex theMapIndex;
        int screenWidth;
        int screenHeight;
        EventHandler<EventArgs> winResize;
        // How large the minimap is relative to the size of the actual map; Initialized in LoadContent()
        double minimapScaleW;
        double minimapScaleH;
        int minimapWidth;
        int minimapHeight;
        // The position of the minimap relative to the game's form; Initialized in LoadContent()
        int minimapPosX;
        int minimapPosY;
        int viewW = 0; // These values will help determine the thiccness of our viewport on the minimap
        int viewH = 0; // These values will help determine the thiccness of our viewport on the minimap

        double focusTimer = 0;//periodically refocus to the game's form to prevent any input block.
        Texture2D vpRecGlobal;
        Texture2D mouseImg,chosenImg;
        public List<Unit>[] currentUnits;

        
        //List<Unit> chosenUnits = new List<Unit>(); //did something for this in AssetDecoratedMap variables
        //Unit chosenUnit = null; //turn this into a list of unit objects?


        //For input detection
        KeyboardState prevState = new KeyboardState();
        MouseState prevMState = new MouseState();

        int mapChoice = SplashScreen.mapObject.mapChoice;
        public int playerChoice = SplashScreen.mapUnits.playerChoice;

        //Class to hold in game data declared in Splashscreen.cs
        public tempMap thisMap;

        Grid thisMapPathGrid;//Store the map's path finding grid.

        float scrollSpeed = 300;

        float mapX, mapY;
        //float minimapX, minimapY; // Stores the coordinate of the top left of our window relative to the map on our minimap
        int mouseX, mouseY;
        int lastClickX, lastClickY;//the click position on map
        bool isClicking = false;//if is clicking, then draw selection line(?)
        public WMPLib.WindowsMediaPlayer gameMusic; //removed static
        int mapChoiceIndex = 0; //moved to global so that it can be accessed

        float animateTimer = 0.2f;//
        float maxAnimateTimer = 0.2f;//unit's animate timer

        // JD: variables for UI
        Form f, miniMap_menu, button_menu, data_menu;
        public inGame_menu inGame_form;
        RenderTarget2D miniMapRenderTarget;
        double miniMapRenderTime = 0.0;
        PictureBox miniMapbox;
        //MemoryStream memoryStream;

        inGameButtons buttons; //so we can call in other functions
        UnitDatas data; //call unit data

        BasicAI[] AIController; //to hold the AI for each AI player
        int numAIPlayers = 0; 

        public XNA_InGame()
        {
            f = new System.Windows.Forms.Form();
            inGame_form = new inGame_menu(this);
            miniMap_menu = new System.Windows.Forms.Form();

            button_menu = new Form();
            data_menu = new Form();
            //memoryStream = new MemoryStream(); //init once

            //f.Focus();
            Window.AllowUserResizing = true;

            //These two lines of code to disable the maximize button to avoid buggy maximize action by AZ
            f = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this.Window.Handle);
            f.MaximizeBox = false;

            //set up the window resize effect
            winResize = new EventHandler<EventArgs>(Window_ClientSizeChanged);
            Window.ClientSizeChanged += winResize;

            //Use this line to show cursor 
            // this.IsMouseVisible = true;

            //debug map choose
            //mapChoice = 3;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // string path = @"data\img\27.png";
            //test = Content.Load<Texture2D>(@"data\img\27.png");
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // mapChoiceIndex starts at index 1
            // don't know how to stop the music after close the map
            /*mapChoiceIndex = mapChoice + 1;
            gameMusic = new WMPLib.WindowsMediaPlayer();         
            gameMusic.URL = @"data\snd\music\game" + mapChoiceIndex + ".mp3";
            gameMusic.settings.volume = SoundOptionsMenu.pubVolBGM;
            gameMusic.settings.playCount = 999; //set to big number for repeat infinite (doesn't work, still crashes)
            gameMusic.controls.play();*/

            
            //Init thisMap with local copy of all data (this will be accessed/changed/updated during game)
            //I know this is long, blame it on Splashscreen.mapObject
            //thisMap = new tempMap(SplashScreen.mapObject.mapData[mapChoice], SplashScreen.mapObject.mapDataIndices[mapChoice], SplashScreen.mapObject.mapPartials[mapChoice], SplashScreen.mapObject.allMapStrings[mapChoice], SplashScreen.mapUnits.startGold[mapChoice], SplashScreen.mapUnits.startLumber[mapChoice], SplashScreen.mapObject.mapX[mapChoice], SplashScreen.mapObject.mapY[mapChoice], SplashScreen.mapUnits.DLumber[mapChoice], SplashScreen.mapUnits.numPlayer[mapChoice]);

            //inGameMenuAjust(); // JD: set the menu setting, at the bottom

            base.Initialize();            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            mapChoiceIndex = mapChoice + 1;
            gameMusic = new WMPLib.WindowsMediaPlayer();
            gameMusic.URL = @"data\snd\music\game" + mapChoiceIndex + ".mp3";
            gameMusic.settings.volume = SoundOptionsMenu.pubVolBGM;
            gameMusic.settings.playCount = 999; //set to big number for repeat infinite (doesn't work, still crashes)

            //gameMusic.controls.play(); //commenting music out allows game to NOT crash!!!

            int tileX = SplashScreen.tileObject.tileWidth;
            int tileY = SplashScreen.tileObject.tileHeight;

            //Reset these (or it will cause seg fault accessing map value outside of map size)
            mapX = 0;
            mapY = 0;

            playerChoice = SplashScreen.mapUnits.playerChoice; //reload player choice (load before init thisMap)
            thisMap = new tempMap(SplashScreen.mapObject.mapData[mapChoice], SplashScreen.mapObject.mapDataIndices[mapChoice], SplashScreen.mapObject.mapPartials[mapChoice], SplashScreen.mapObject.allMapStrings[mapChoice], SplashScreen.mapUnits.startGold[mapChoice], SplashScreen.mapUnits.startLumber[mapChoice], SplashScreen.mapObject.mapX[mapChoice], SplashScreen.mapObject.mapY[mapChoice], SplashScreen.mapUnits.DLumber[mapChoice], SplashScreen.mapUnits.numPlayer[mapChoice], graphics.GraphicsDevice, playerChoice);

            inGameMenuAjust(); //moved here


            //Get resource for testing here (can now be done with chatbox cheatcode)
            //thisMap.hackResource("all", 50000, playerChoice);


            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //string path = @"data\img\27.png";

            //To optimize
            //System.Drawing.Bitmap btt = SplashScreen.mapObject.entireMaps[mapChoice];

            //System.Drawing.Bitmap btt = new System.Drawing.Bitmap(path);
            //theMap = Texture2D.FromStream(this.GraphicsDevice, File.OpenRead(path));
            //theMap = getTextureFromBitmap(btt, this.GraphicsDevice);


            //AZ: clone this data to allow terrain change
            theMapIndex = null; //maybe put this in unload?
            theMapIndex = SplashScreen.mapObject.entireMapsIndex[mapChoice].clone();
            thisMap.theMapIndex = theMapIndex;//used to modify Terrain in decoratedMap.


            theMapWidth = SplashScreen.mapObject.entireMapsIndex[mapChoice].data[0].Length * tileX;
            theMapHeight = SplashScreen.mapObject.entireMapsIndex[mapChoice].data.Length * tileY;

            //initialize the grid. byAZ
            int mapW = SplashScreen.mapObject.entireMapsIndex[mapChoice].data[0].Length;
            int mapH = SplashScreen.mapObject.entireMapsIndex[mapChoice].data.Length;
            thisMapPathGrid = new Grid(mapW, mapH);

            for (int y = 0; y < mapH; y++)
            {
                for (int x = 0; x < mapW; x++)
                {
                    if (thisMap.isTraversible(x, y) == false)
                    {
                        thisMapPathGrid.BlockCell(new Position(x, y));
                    }
                }
            }
            thisMap.pathFindingGriod = thisMapPathGrid;

            screenWidth = GraphicsDevice.Viewport.Width;
            screenHeight = GraphicsDevice.Viewport.Height;

            minimapScaleW = .05;
            minimapScaleH = .05;
            minimapWidth = (int)(minimapScaleW * theMapWidth);
            minimapHeight = (int)(minimapScaleH * theMapHeight);
            minimapPosX = 0; //minimapPosX = 20;
            minimapPosY = 0; //minimapPosY = 20;

            mouseImg = getTextureFromBitmap(CursorLoader.curPointer, this.GraphicsDevice);
            chosenImg = getTextureFromBitmap(CursorLoader.curGrn, this.GraphicsDevice);

            //convert asset to texture2D
            SplashScreen.mapUnits.convertBitmapsToTextures(GraphicsDevice);
            SplashScreen.tileObject.convertBitmapsToTextures(GraphicsDevice);

            if (SplashScreen.renderColor == 1)
            {
                SplashScreen.coloredUnits = new ColorsLoader(GraphicsDevice, thisMap.numPlayers); //init for calling :must come after SplashScreen.mapUnits.convertBitmapsToTextures(GraphicsDevice);
                //SplashScreen.coloredUnits.loadAllColors(GraphicsDevice); //init color
                //SplashScreen.coloredUnits.loadDefault(GraphicsDevice);
            }


            //AZ:A better way of rendering the mini map...
            //theMap = SplashScreen.tileObject.tiles2D[0];
            theMap = getTextureFromBitmap(new CGraphicTileset().generateMiniMap(mapChoice, SplashScreen.tileObject, SplashScreen.mapObject), this.GraphicsDevice);
            // TODO: use this.Content to load your game content here

            //Load all units = creating them
            //Load objects

            currentUnits = new List<Unit>[SplashScreen.mapUnits.numPlayer[mapChoice] + 1]; //init (+1 for nature player 0)

            for (int i = 0; i <= SplashScreen.mapUnits.numPlayer[mapChoice]; i++) //iterate through all players
            {
                currentUnits[i] = new List<Unit>();
                int index = 0; //used for id of unit
                Color[] uCol = new Color[1]; // Unit Color

                uCol[0] = Color.Yellow;
                //uCol[0] = chosenPlayerColor;
                foreach (Asset u in SplashScreen.mapUnits.mapAssets[mapChoice][i]) //iterate through all assets of a player
                {
                    if (i == playerChoice && index == 0) //if it's the our first unit
                    { //we use this to get our starting position on map
                        //Offsets to recenter screen if needed (makes it look nicer at start)
                        int offsetX = 0;
                        int offsetY = 0;
                        //offsetX = screenWidth / 2;
                        //offsetY = screenHeight / 2;
                        mapX -= (u.x * tileX + offsetX);
                        mapY -= (u.y * tileY + offsetY);
                    }
                    Unit tempUnit = new Unit(u.type, u.x, u.y, i, index, ref thisMap); //create unit (only need to do once)
                    //Texture2D tempImg = getTextureFromBitmap(tempUnit.objectImage, this.GraphicsDevice);

                    //tempUnit.objImg2D = tempImg; //store the img
                    Texture2D uRec = new Texture2D(graphics.GraphicsDevice, 1, 1);
                    uRec.SetData(uCol);
                    tempUnit.uRec = uRec;
                    currentUnits[i].Add(tempUnit);

                    index++;
                }

            }

            // JD: stuff for rendering the miniMap seperately
            //Get the current presentation parameters
            var pp = graphics.GraphicsDevice.PresentationParameters;
            //Create our new render target
            miniMapRenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                                 minimapWidth,//pp.BackBufferWidth, //Same width as backbuffer
                                 minimapHeight,//pp.BackBufferHeight, //Same height
                                 false, //No mip-mapping
                                 pp.BackBufferFormat, //Same colour format
                                 pp.DepthStencilFormat); //Same depth stencil


            //initialize miniMap
            miniMapbox = new PictureBox();
            miniMapbox.Size = new System.Drawing.Size(miniMap_menu.Width, miniMap_menu.Height); //set once here
            miniMapbox.SizeMode = PictureBoxSizeMode.StretchImage;
            miniMapbox.Click += new EventHandler(this.minimap_Click);//Added a click event handler to track the minimap click event
            miniMap_menu.Controls.Add(miniMapbox);

            drawWinForm(miniMap_menu, miniMapRenderTarget, true);

            thisMap.updateUnitFood(currentUnits); //can be called here (update once at start after unit creation)
            //inGameMenuAjust(); //moved here
            buttons.reloadBuilder(ref thisMap, ref currentUnits);
            if (thisMap.renderFogFlag == 1) thisMap.updateFogOfWar(playerChoice, currentUnits); //load fog of war


            //Then finally load the AI at end
            if (SplashScreen.bootUpAI == 1)
            {
                numAIPlayers = SplashScreen.mapUnits.numPlayer[mapChoice] - 1; //-1 for the main player
                AIController = new BasicAI[numAIPlayers];
                for (int i = 0; i < numAIPlayers; i++)
                {
                    int playerNum = i + 2; //+1 for main player at index 1, and +1 for nature player
                    AIController[i] = new BasicAI(playerNum, ref currentUnits, ref thisMap, SplashScreen.mapUnits.numPlayer[mapChoice], graphics.GraphicsDevice);
                }
            }
            else numAIPlayers = 0;
            
        }



        //Refer to the code at http://florianblock.blogspot.com/2008/06/copying-dynamically-created-bitmap-to.html
        //Also refer to https://gamedev.stackexchange.com/questions/6440/bitmap-to-texture2d-problem-with-colors
        //Modied by Alan Zhang
        //Used to generate useable Texture2D file from a system Bitmap file.
        public static Texture2D getTextureFromBitmap(System.Drawing.Bitmap b, GraphicsDevice graphicsDevice)
        {
            Texture2D tx = null;
            using (MemoryStream s = new MemoryStream())
            {
                b.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                s.Seek(0, SeekOrigin.Begin);
                tx = Texture2D.FromStream(graphicsDevice, s);
            }
            return tx;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            //spriteBatch = null;
            //theMap = null;
            //currentUnits = null;
            //thisMap = null;
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //f.Focus();
            //inGame_form.Focus(); //always refocus to lock keyboard input, works not so well...
            KeyboardState state = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            int tileX = SplashScreen.tileObject.tileWidth;
            int tileY = SplashScreen.tileObject.tileHeight;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Application.Exit();

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            
            //JD: arrowKeysState[0] is up, 1 is down, 2 is left, 3 is right
            if (inGame_menu.arrowKeysState[3])
            //if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                mapX -= (scrollSpeed * delta);
            }

            if (inGame_menu.arrowKeysState[2])
            //if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                mapX += (scrollSpeed * delta);
            }

            if (inGame_menu.arrowKeysState[0])
            //if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                mapY += (scrollSpeed * delta);
            }

            if (inGame_menu.arrowKeysState[1])
            //if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                mapY -= (scrollSpeed * delta);
            }
            //inGame_menu.arrowKeysState[0] = inGame_menu.arrowKeysState[1] = inGame_menu.arrowKeysState[2] = inGame_menu.arrowKeysState[3] = false;

            if (mapX > 0) { mapX = 0; }

            if (mapY > 0) { mapY = 0; }

            if (mapX + theMapWidth < Window.ClientBounds.Width)
            {
                mapX = Window.ClientBounds.Width - theMapWidth;
            }

            if (mapY + theMapHeight < Window.ClientBounds.Height)
            {
                mapY = Window.ClientBounds.Height - theMapHeight;
            }
            /*
            if (mapY + this.GraphicsDevice.Viewport.Height < theMapHeight)
            {
                mapY = theMapHeight - this.GraphicsDevice.Viewport.Height;
            }*/

            mouseX = Mouse.GetState().Position.X;
            mouseY = Mouse.GetState().Position.Y;


            //

            //If the mouse just pressed but not released
            if (mState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && prevMState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && isClicking == false)
            {
                if (0 <= mouseX && 0 <= mouseY) // && mouseY < Window.ClientBounds.Height && mouseX < Window.ClientBounds.Width)
                { //we need extra test case to make sure we are clicking within game window
                    lastClickX = mouseX - (int)mapX;
                    lastClickY = mouseY - (int)mapY;
                    isClicking = true;
                }
            }

                //Choose unit effect
             if (mState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && prevMState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && isClicking == true)
             {
                


                if (buttons.mode == 0) //default mode (0); we don't have human move (mode 2) here because that is movement only
                {
                    //chosenUnit = null;
                    isClicking = false;
                    //chosenUnits.Clear();
                    //if (thisMap.chosenUnits.Count() == 0) buttons.hideButtons(); //hide all buttons (only if selected units is empty)
                    buttons.hideButtons();
                    data.hide_data();
                    thisMap.chosenUnits.Clear();
                    

                    //single choosing mode
                    if (Math.Abs(mouseX - (int)mapX - lastClickX) + Math.Abs(mouseY - (int)mapY - lastClickY) < 5)
                    {
                        for (int i = 0; i <= SplashScreen.mapUnits.numPlayer[mapChoice]; i++) //iterate through all players
                        {
                            foreach (Unit u in currentUnits[i]) //iterate through all assets of a player
                            {
                                if (u.invisible == 0 && u.getAction() != "death" && u.getAction() != "decay" && u.unitType >= 0) //only allow unit selection if it's not invisible and a true unit
                                { //we need to edit this for building selection!  take into account building image size!
                                  //Roger that
                                    if (Math.Abs(u.x * tileX + 0.5f * tileX * u.unitTileWidth  + mapX - mouseX) < tileX * 0.5f * u.unitTileWidth)
                                    {
                                        if (Math.Abs(u.y * tileY + 0.5f * tileY * u.unitTileHeight + mapY - mouseY) < tileY * 0.5f * u.unitTileHeight)
                                        {
                                            unitPtr temp = new unitPtr();
                                            temp.owner = u.owner;
                                            temp.index = u.id;
                                            thisMap.chosenUnits.Add(temp);
                                            data.show_data(u); //show the unit data as we had selected it
                                            singleShowButton(u);
                                            /*if (u.unitType == 0 && u.owner == playerChoice)
                                            {
                                                buttons.peasantButtons(); //show peasant buttons only if first select
                                               // buttons.playerChoice = u.owner;//the peasant can only build its owner's building.
                                            }
                                            if (u.unitType == 4 && u.owner == playerChoice)
                                            {
                                                buttons.townHallButtons();
                                            }*/
                                            //System.Windows.Forms.MessageBox.Show("Selected unit type:" + u.unitType + "TileX:" + u.unitTileWidth + "TileY:" + u.unitTileHeight);

                                           /* if (inGame_form.console)
                                            {
                                                System.Windows.Forms.MessageBox.Show("Console triggered");

                                                ConsoleForm cf = new ConsoleForm();
                                                cf.g = this;
                                                cf.Parent = null;
                                                cf.Show();
                                            }*/

                                            //chosenUnits.Add(u);
                                            //chosenUnit = u;
                                            break;
                                        }
                                    }
                                }
                                //spriteBatch.Draw(u.objImg2D, new Rectangle(u.x * tileX + (int)mapX, u.y * tileY + (int)mapY, u.objImg2D.Width, u.objImg2D.Height), Color.White);
                            }
                            //if (chosenUnits.Count() > 0)
                            if (thisMap.chosenUnits.Count() > 0)
                            {
                                break;
                            }
                            /* if (chosenUnit!= null)
                             {
                                 break;
                             }*/
                        }
                    }
                    else
                    {
                        //multiple choosing mode
                        int leftUpperX = (int)Math.Min(lastClickX, mouseX - mapX);
                        int leftUpperY = (int)Math.Min(lastClickY, mouseY - mapY);

                        int rightLowerX = (int)Math.Max(lastClickX, mouseX - mapX);
                        int rightLowerY = (int)Math.Max(lastClickY, mouseY - mapY);

                        for (int i = 0; i <= SplashScreen.mapUnits.numPlayer[mapChoice]; i++) //iterate through all players
                        {
                            foreach (Unit u in currentUnits[i]) //iterate through all assets of a player
                            {
                                if (u.invisible == 0 && u.getAction() != "death" && u.getAction() != "decay" && u.unitType > -1) //only allow unit selection if it's not invisible
                                { //we need to edit this for building selection!  take into account building image size!
                                  //Roger that
                                    if (leftUpperX < u.x * tileX + 0.5f * tileX && u.x * tileX + 0.5f * tileX < rightLowerX)
                                    {
                                        if (leftUpperY < u.y * tileY + 0.5f * tileY && u.y * tileY + 0.5f * tileY < rightLowerY)
                                        {
                                            unitPtr temp = new unitPtr();
                                            temp.owner = u.owner;
                                            temp.index = u.id;
                                            thisMap.chosenUnits.Add(temp);
                                            //chosenUnits.Add(u);
                                            //chosenUnit = u;
                                            //break;

                                            
                                            
                                        }
                                    }
                                }
                                //spriteBatch.Draw(u.objImg2D, new Rectangle(u.x * tileX + (int)mapX, u.y * tileY + (int)mapY, u.objImg2D.Width, u.objImg2D.Height), Color.White);
                            }
                            //if (chosenUnits.Count() > 0)
                            if (thisMap.chosenUnits.Count() > 1)
                            {
                                data.load_multiData(thisMap, currentUnits);
                                data.show_multiData();
                            }
                            else if (thisMap.chosenUnits.Count == 1) //only one unit chosen
                            {
                                unitPtr temp = thisMap.chosenUnits.ElementAt(0);
                                data.show_data(currentUnits[temp.owner][temp.index]); //show the unit data as we had selected it
                                singleShowButton(currentUnits[temp.owner][temp.index]);
                            }
                            showMultiUnitsButtons();
                        }


                    }
                }
                else if (buttons.mode == 1) //build mode
                {
                    isClicking = false;
                    //int x = ((int)mapX + mouseX) / tileX;
                    //int y = ((int)mapY + mouseY) / tileY;
                    int x = (mouseX - (int)mapX) / tileX;
                    int y = (mouseY - (int)mapY) / tileY;
                    int builderOwner = -1;
                    if (thisMap.chosenUnits.Count() != 0) builderOwner = thisMap.chosenUnits.ElementAt(0).owner;
                    else builderOwner = 0; //for nature
                    if (buttons.builder.Register(ref currentUnits, thisMap, x, y, graphics.GraphicsDevice, builderOwner,0))
                    {
                        buttons.cancelBuild(); //System.Windows.Forms.MessageBox.Show("Building succeed");
                        thisMap.updateFlag = 1; //we added unit so need to update food (don't know if needed)
                        RightClickMoveFunc(); //set so that unit will move to where we placed building automatically
                    }
                    else buttons.cancelBuild();
                }
                else if (buttons.mode == 2) //human Move button mode
                {
                    isClicking = false;
                    RightClickMoveFunc();
                }
                else if (buttons.mode == 3) //resource gathering mode
                {
                    isClicking = false;
                    if (thisMap.chosenUnits.Count != 0) //if we have units chosen
                    {
                        unitPtr tempCur = thisMap.chosenUnits.ElementAt(0);
                        Unit tempUnit = currentUnits[tempCur.owner][tempCur.index];
                        int xToGo = (int)((mouseX - mapX) / tileX);
                        int yToGo = (int)((mouseY - mapY) / tileY);

                        unitPtr temp = thisMap.target(currentUnits, xToGo, yToGo, tempUnit, 0);
                        if (temp.owner != -1) //meaning there was a target
                        {
                            if (temp.index >= 0 && currentUnits[temp.owner][temp.index].unitType == thisMap.nameTranslation("GoldMine")) RightClickMoveFunc(); //allow for move function to be called as we clicked on goldmine
                        }
                        else if (thisMap.isTree(xToGo, yToGo)) RightClickMoveFunc(); //we targeted a tree so allow for movement

                    }
                }
                else if (buttons.mode == 4) //patrol mode
                {
                    isClicking = false;
                    if (thisMap.chosenUnits.Count != 0) //if we have units chosen
                    {
                        unitPtr tempCur = thisMap.chosenUnits.ElementAt(0);
                        Unit tempUnit = currentUnits[tempCur.owner][tempCur.index];
                        int xToGo = (int)((mouseX - mapX) / tileX);
                        int yToGo = (int)((mouseY - mapY) / tileY);

                        unitPtr temp = thisMap.target(currentUnits, xToGo, yToGo, tempUnit, 0);
                        if (temp.owner == -1 && thisMap.isTraversible(xToGo,yToGo)) //meaning there was no target and selected spot is traversible
                        {
                            RightClickMoveFunc(); //call right click move function
                        }

                    }

                }
                else if (buttons.mode == 5) //attack mode
                {
                    isClicking = false;
                    if (thisMap.chosenUnits.Count != 0) //if we have units chosen
                    {
                        unitPtr tempCur = thisMap.chosenUnits.ElementAt(0);
                        Unit tempUnit = currentUnits[tempCur.owner][tempCur.index];
                        int xToGo = (int)((mouseX - mapX) / tileX);
                        int yToGo = (int)((mouseY - mapY) / tileY);

                        unitPtr temp = thisMap.target(currentUnits, xToGo, yToGo, tempUnit, 0);
                        if (temp.owner >= 1 && temp.index >= 0) //meaning there was a target (not nature as we can't attack nature)
                        {
                            RightClickMoveFunc(); //call right click move function
                        }

                    }
                }
                else if (buttons.mode == 6) //repair mode
                {
                    isClicking = false;
                    if (thisMap.chosenUnits.Count != 0) //if we have units chosen
                    {
                        unitPtr tempCur = thisMap.chosenUnits.ElementAt(0);
                        Unit tempUnit = currentUnits[tempCur.owner][tempCur.index];
                        int xToGo = (int)((mouseX - mapX) / tileX);
                        int yToGo = (int)((mouseY - mapY) / tileY);

                        unitPtr temp = thisMap.target(currentUnits, xToGo, yToGo, tempUnit, 0);
                        if (temp.owner == playerChoice && temp.index >= 0) //meaning there was a target and the target is our building
                        {
                            Unit tempOther = currentUnits[temp.owner][temp.index];
                            if (tempOther.unitType >= SplashScreen.numUnits) //target unit is a building
                            {
                                RightClickMoveFunc(); //call right click move function
                            }
                        }

                    }
                }

                //if (thisMap.chosenUnits.Count() == 0) buttons.hideButtons(); //hide all buttons (if no units selected)
            }

            //click ground sound effect
            if (mState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && prevMState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            {
                //Functions moved into function below so can be called elsewhere
                if (buttons.mode == 2) buttons.cancelHumanMove();
                else if (buttons.mode == 1) buttons.cancelBuild();
                else if (buttons.mode == 3) buttons.cancelHumanMove();
                else if (buttons.mode == 4) buttons.cancelHumanMoveArmy(); //cancel selection
                else if (buttons.mode == 5) buttons.cancelHumanMoveArmy();
                else if (buttons.mode == 6) buttons.cancelHumanMove();
                else RightClickMoveFunc(); //this is Alan's move function
            }


            // TODO: Add your update logic here
            thisMap.updateTimers(ref currentUnits, ref thisMap, mapX, mapY, screenWidth, screenHeight, ref buttons, ref data); //updates garrisoned unit in buildings (like goldMine)

            
            for (int i = 0; i < numAIPlayers; i++) //constantly call assign actions for all AI players
            {
                AIController[i].assignActions();
            }
            
            
            if (thisMap.updateFlag == 1)
            {
                thisMap.updateUnitFood(currentUnits); //update food counts (maybe called elsewhere for less compute time? Can't find spot yet as food can be changed randomly?)
                //updateFlag = 0; //turn off update so we only update once
                if (thisMap.renderFogFlag == 1) thisMap.updateFogOfWar(playerChoice, currentUnits); //call here for adding new units
            }           
            

            if (thisMap.updateUnitFlag != 0) //update created units' data
            {
                foreach(Unit u in currentUnits[thisMap.updateUnitFlag]) //only update units of the player that upgraded
                {
                    if (thisMap.unitsToUpdate[thisMap.updateUnitFlag][u.unitType] == 1) //if unit needs to be updated for player
                    {
                        u.initLoadedData(ref thisMap);
                    }
                }

                thisMap.updateUnitFlag = 0; //reset flag
            }

            if (thisMap.updateHealth == 1)
            {
                if (thisMap.chosenUnits.Count() != 0)
                { //if we actually have a unit selected
                    if (thisMap.chosenUnits.Count() > 1) //multiple units selected
                    {
                        data.refresh_healthAll(ref thisMap, currentUnits);
                    }
                    else //only one unit selected
                    {
                        unitPtr temp = thisMap.chosenUnits.ElementAt(0);
                        if (temp.owner >= 0 && temp.index >= 0) //we have valid selected unit
                        {
                            data.refresh_health(currentUnits[temp.owner][temp.index]);
                        }
                    }
                }
                thisMap.updateHealth = 0; //always turn off even if we did not update
            }

            if (thisMap.updateProgress == 1)
            {
                if (thisMap.chosenUnits.Count() != 0)
                { //if we actually have a unit selected
                    
                    unitPtr temp = thisMap.chosenUnits.ElementAt(0);
                    if (temp.owner >= 0 && temp.index >= 0) //we have valid selected unit
                    {
                        data.refreshProgress(currentUnits[temp.owner][temp.index]);
                    }
                    
                }
                thisMap.updateProgress = 0; //always turn off even if we did not update
            }

            if (thisMap.reShowData == 1)
            {
                if (thisMap.chosenUnits.Count() != 0)
                {
                    unitPtr temp = thisMap.chosenUnits.ElementAt(0);
                    if (temp.owner >= 0 && temp.index >= 0)
                    {
                        data.show_data(currentUnits[temp.owner][temp.index]);
                    }
                }
                thisMap.reShowData = 0;
            }

            if (thisMap.reShowMultiData == 1)
            {
                if (thisMap.chosenUnits.Count() != 0)
                {
                    if (thisMap.chosenUnits.Count() > 1)
                    {
                        data.load_multiData(thisMap, currentUnits);
                        data.show_multiData();
                    }
                    else //if (thisMap.chosenUnits.Count == 1) //only one unit chosen
                    {
                        unitPtr temp = thisMap.chosenUnits.ElementAt(0);
                        data.show_data(currentUnits[temp.owner][temp.index]); //show the unit data as we had selected it
                        singleShowButton(currentUnits[temp.owner][temp.index]);
                    }
                }
                thisMap.reShowMultiData = 0;
            }

            if (thisMap.reShowSingleButton == 1)
            {
                if (thisMap.chosenUnits.Count() != 0)
                {
                    unitPtr temp = thisMap.chosenUnits.ElementAt(0);
                    if (temp.owner >= 0 && temp.index >= 0)
                    {
                        singleShowButton(currentUnits[temp.owner][temp.index]);
                    }
                }
                thisMap.reShowSingleButton = 0;
            }

            if (thisMap.reShowMultiButtons == 1)
            {
                if (thisMap.chosenUnits.Count() != 0)
                {
                    showMultiUnitsButtons();
                }
                thisMap.reShowMultiButtons = 0;
            }

            if (thisMap.unitsToCreate.Count() != 0) //we have units queued (call once each update so that we don't have units clashing)
            {
                unitCreateData temp = thisMap.unitsToCreate.ElementAt(0); //get first unit in list
                if (temp.owner != -1)
                {
                    if (temp.owner == playerChoice)
                    {
                        if (buttons.builder.BuildUnit(temp.unitType, temp.owner, temp.curBuilding, ref currentUnits, thisMap))
                        {
                            temp.curBuilding.creating = null; //reset building create flag so we can queue a new unit
                            thisMap.unitsToCreate.RemoveAt(0);//we successfully created unit so remove from list
                            thisMap.updateFlag = 1; //call update as we added unit
                        }
                        else
                        { //remove unit and add back to end of queue
                            thisMap.unitsToCreate.RemoveAt(0);
                            thisMap.unitsToCreate.Add(temp);
                        }
                    }
                    else if (numAIPlayers > 0) //AI is building (only if AI is enabled)
                    {
                        if (AIController[temp.owner - 2].builder.BuildUnit(temp.unitType, temp.owner, temp.curBuilding, ref currentUnits, thisMap)) //-2 for offset on AI controller
                        {
                            temp.curBuilding.creating = null; //reset building create flag so we can queue a new unit
                            thisMap.unitsToCreate.RemoveAt(0);//we successfully created unit so remove from list
                            thisMap.updateFlag = 1; //call update as we added unit
                        }
                        else
                        { //remove unit and add back to end of queue
                            thisMap.unitsToCreate.RemoveAt(0);
                            thisMap.unitsToCreate.Add(temp);
                        }
                    }
                }
            }

            /*if (thisMap.cleanUpUnits.Count() != 0) //we have units to clean up (aka units had died)
            {
                unitPtr temp = thisMap.cleanUpUnits.ElementAt(0); //get first unit that needs to be cleaned up
                currentUnits[temp.owner].RemoveAt(temp.index); //remove the unit from our current units
                thisMap.cleanUpUnits.RemoveAt(0); //remove the pointer to unit from the cleanup list
            }*/ //currently can't clean up units or will crash


            //button_menu.ActiveControl = null;//prevent the focus become the buttons.


            //Periodically regain focus
            if (inGame_form.ActiveControl != inGame_form.mainMenuButton && inGame_form == Form.ActiveForm)
            {
                focusTimer += delta;
                // JD: 3/10 add inGame_form.ActiveControl != inGame_form.inputBox to prevent the focus move from inputBox to mainMenuButton
                if (focusTimer > 0.55f && inGame_form.ActiveControl != inGame_form.inputBox) 
                {
                    focusTimer = 0;
                    inGame_form.ActiveControl = inGame_form.mainMenuButton;
                }
            }

            //inGame_form.Activate();
            //if(inGame_form.Visible && inGame_form.Enabled)
            //inGame_form.ActiveControl = inGame_form;

            //Update resources display
            inGame_form.labelUpdate("gold", thisMap.gold[playerChoice],0);
            inGame_form.labelUpdate("lumber", thisMap.lumber[playerChoice],0);
            inGame_form.labelUpdate("food", thisMap.food[playerChoice], thisMap.foodMax[playerChoice]);


            animateTimer -= delta;
            if (animateTimer < 0)
            {
                animateTimer = maxAnimateTimer;
                for (int i = 0; i <= SplashScreen.mapUnits.numPlayer[mapChoice]; i++) //iterate through all players
                {
                    foreach (Unit u in currentUnits[i]) //iterate through all assets of a player
                    {

                       
                        if (u.onCommand && u.unitType > -1 && u.battling == false) //only need path finding if unit is not projectile and unit is not fighting
                        {
                            u.animate(thisMap);
                            u.updateDir(thisMapPathGrid);
                            //Update dir now moved to another place
                            //u.updateDir(thisMapPathGrid);
                        }
                        else if (u.onCommand) //animate is always called on command
                        {
                            u.animate(thisMap);
                        }
                       
                    }

                }
            }


            //on Command operation
            for (int i = 0; i <= SplashScreen.mapUnits.numPlayer[mapChoice]; i++) //iterate through all players
            {
                foreach (Unit u in currentUnits[i]) //iterate through all assets of a player
                {
                    if (u.onCommand && u.target.index != -2 && u.battling == false && u.getAction() != "death" && u.getAction() != "decay") //-2 means animate but don't move, battling false means not fighting animation
                    {
                        

                        /*if (1 <= u.target.owner && u.unitType < SplashScreen.numUnits && u.unitType > -1 && u.rangingUnit() && u.battling == false)
                        { //we want to act on target check because we might be ranging unit
                            if (((u.target.owner != -1 && u.invisible == 0) || (u.target.index == -1)) && u.chasing == false) //we reach target destination hence we stopped
                            {
                                if (u.getAction() == "attack" || u.battling == true || u.chasing == true) u.resetAnimation(); //interrupting
                                u.actOnTarget(ref currentUnits, ref thisMap, ref buttons, ref data);
                            }
                        }*/
                        if (checkCloseEnough(u)) //so we can act on target once we are near enough
                        {
                            if (u.getAction() == "attack" || u.battling == true || u.chasing == true || u.startedBattle == true || u.patrolling == true) u.resetAnimation(); //interrupting
                            u.actOnTarget(ref currentUnits, ref thisMap, ref buttons, ref data);
                        }
                        else if (Math.Abs(u.x - u.xToGo) < 0.1f && Math.Abs(u.y - u.yToGo) < 0.1f)
                        {
                            u.onCommand = false;
                            u.x = u.xToGo;
                            u.y = u.yToGo;
                            u.stopAnimation();

                            
                            if (((u.target.owner != -1 && thisMap.checkInvisible(u)) || (u.target.index == -1)) && u.chasing == false && u.unitType > -1) //we reach target destination hence we stopped
                            {
                                //MessageBox.Show("Acted on target");
                                u.onCommand = false;
                                //if (u.getAction() == "attack" || u.battling == true || u.chasing == true) u.resetAnimation(); //interrupting
                                //if (u.getAction() == "attack" || u.battling == true) u.resetAnimation(); //interrupting
                                u.actOnTarget(ref currentUnits, ref thisMap, ref buttons, ref data);
                            }
                            else if (u.patrolling == true && u.unitType > -1) //a patrolling unit
                            {
                                u.onCommand = false;
                                u.actOnTarget(ref currentUnits, ref thisMap, ref buttons, ref data);
                            }
                            
                            
                        }
                        else { //added test traversible
                           
                            string dir = u.getDirection();
                            
                            if (dir == "N" && thisMap.isTraversible(u.x, u.y, u.speed, delta, "N", currentUnits,u))
                            {
                                u.x -= 0 * u.speed * delta;
                                u.y -= u.speed * delta;
                            }
                            else if (dir == "NE" && thisMap.isTraversible(u.x, u.y, u.speed, delta, "NE", currentUnits, u))
                            {
                                u.x += 0.5f * u.speed * delta;
                                u.y -= 0.5f * u.speed * delta;
                            }
                            else if (dir == "E" && thisMap.isTraversible(u.x, u.y, u.speed, delta, "E", currentUnits, u))
                            {
                                u.x += 1f * u.speed * delta;
                                u.y -= 0 * u.speed * delta;
                            }
                            else if (dir == "SE" && thisMap.isTraversible(u.x, u.y, u.speed, delta, "SE", currentUnits, u))
                            {
                                u.x += 0.5f * u.speed * delta;
                                u.y += 0.5f * u.speed * delta;
                            }
                            else if (dir == "S" && thisMap.isTraversible(u.x, u.y, u.speed, delta, "S", currentUnits, u))
                            {
                                u.y += 1 * u.speed * delta;
                            }
                            else if (dir == "SW" && thisMap.isTraversible(u.x, u.y, u.speed, delta, "SW", currentUnits, u))
                            {
                                u.x -= 0.5f * u.speed * delta;
                                u.y += 0.5f * u.speed * delta;
                            }
                            else if (dir == "W" && thisMap.isTraversible(u.x, u.y, u.speed, delta, "W", currentUnits, u))
                            {
                                u.x -= 1f * u.speed * delta;
                            }
                            else if (dir == "NW" && thisMap.isTraversible(u.x, u.y, u.speed, delta, "NW", currentUnits, u))
                            {
                                u.x -= 0.5f * u.speed * delta;
                                u.y -= 0.5f * u.speed * delta;
                            }
                            else //not traversible terrain ahead so stop but don't jump to position
                            {
                                //commented this line to yield other units and continue the path.
                                //u.onCommand = false;
                                
                                u.stopAnimation();

                                
                                if (((u.target.owner != -1 && thisMap.checkInvisible(u)) || (u.target.index == -1)) && u.chasing == false && u.unitType > -1)  //we have a target hence we stopped
                                {
                                    //MessageBox.Show("Acted on target");
                                    //if (u.getAction() == "attack" || u.battling == true || u.chasing == true) u.resetAnimation(); //interrupting
                                    //if (u.getAction() == "attack" || u.battling == true) u.resetAnimation(); //interrupting
                                    u.onCommand = false; //already acted so don't act again
                                    u.actOnTarget(ref currentUnits, ref thisMap, ref buttons, ref data);

                                }
                                else if (u.patrolling == true && u.unitType > -1) //a patrolling unit
                                {
                                    u.onCommand = false;
                                    u.actOnTarget(ref currentUnits, ref thisMap, ref buttons, ref data);
                                }


                            }

                            //try to razterize the movement.
                            if (u.onCommand && u.unitType > -1 && u.battling == false) {

                                if (Math.Abs(u.x - u.xNextStop) < 0.1f && Math.Abs(u.y - u.yNextStop) < 0.1f)
                                {
                                    u.x = u.xNextStop;
                                    u.y = u.yNextStop;

                                    //u.xNextStop = -1;
                                    //u.yNextStop = -1;
                                    u.updateDir(thisMapPathGrid);
                                    //do the path finding job and find the xNextStop and yNextStop
                                }
                                else {
                                    u.updateNextDir();
                                }
                            }

                        }
                      if (thisMap.renderFogFlag == 1) thisMap.updateFogOfWar(playerChoice, currentUnits); //update fog after movement
                    }
                   
                }

            }

            prevMState = mState;
            prevState = state;

            // JD: update the refresh time for miniMap
            miniMapRenderTime += delta;

            base.Update(gameTime);
        }

        private bool checkCloseEnough(Unit u)
        { //helper function to check if ranger is close enough to target it chooses to attack
            if (u.rangingUnit() && u.unitType < SplashScreen.numUnits && u.target.owner >= 1 && u.target.index >= 0 && u.startedBattle == false) //so we can act on target once we are near enough
            {
                Unit temp = currentUnits[u.target.owner][u.target.index];
                if (u.nearEnough(temp))
                {
                    if (thisMap.checkInvisible(u) && u.chasing == false && u.unitType > -1) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //For object location on map
            int tileX = SplashScreen.tileObject.tileWidth;
            int tileY = SplashScreen.tileObject.tileHeight;

            // JD: print the renderTarget to correct window, if the gameTime has past more than 1 sec, miniMap will render itmelf.
            /*if (miniMapRenderTime > 1)
            {
                //drawWinForm(miniMap_menu, miniMapRenderTarget, false);
                miniMapRenderTime = 0.0;
            }*/

            drawWinForm(miniMap_menu, miniMapRenderTarget, false); //moved out of test case so we have no delay


            //GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(Color.Black); //clear to black for fog of war?
            spriteBatch.Begin();

            // TODO: Add your drawing code here

            DrawTheMap();



            /* if (chosenUnit!= null)
             {
                 spriteBatch.Draw(chosenImg, new Rectangle( (int)(chosenUnit.x * tileX + (int)mapX), (int)(chosenUnit.y * tileY + (int)mapY), chosenImg.Width, chosenImg.Height), Color.White);
             }*/
            
            foreach (unitPtr cur in thisMap.chosenUnits)
            {
                //u.unitWidth
                Unit u = currentUnits[cur.owner][cur.index];
                if (u.invisible == 0)
                {
                    //if the unit is not a building, then

                    DrawRectangle(spriteBatch, new Rectangle((int)(u.x * tileX + mapX), (int)(u.y * tileY + (int)mapY),
                    tileX * u.unitTileWidth, tileY * u.unitTileHeight), Color.LightGreen, 2);



                }
                //spriteBatch.Draw(chosenImg, new Rectangle((int)(currentUnits[cur.owner][cur.index].x * tileX + (int)mapX), (int)(currentUnits[cur.owner][cur.index].y * tileY + (int)mapY),  tileX, tileY), Color.White);
            }
            


            //draw the units on the map.
            DrawUnits();


            //Draw the virtual building if on the building mode.
            if (buttons.mode == 1) //build mode
            {

                int x = (mouseX - (int)mapX) / tileX;
                int y = (mouseY - (int)mapY) / tileY;
                int w = 1;
                int h = 1;
                if (buttons.builder.building != null)
                {
                    w = buttons.builder.building.defaultUnitTileW;
                    h = buttons.builder.building.defaultUnitTileH;
                    Unit u = buttons.builder.building;
                    spriteBatch.Draw(u.objPreviewImg2D, new Rectangle((int)(x * tileX + (int)mapX ), (int)(y * tileY + (int)mapY ), u.objPreviewImg2D.Width, u.objPreviewImg2D.Height), new Color(Color.White,0.5f));

                }
                
                DrawRectangle(spriteBatch, new Rectangle((int)(x * tileX + mapX), (int)(y * tileY + (int)mapY),
                   tileX * w, tileY * h), Color.LightGreen, 2);

            }



            //draw the minimap and its thumb nail.
            //DrawMinimap();

            if (isClicking && buttons.mode == 0) //only draw box if we are in normal select mode
            {
                if (Math.Abs(mouseX - (int)mapX - lastClickX) + Math.Abs(mouseY - (int)mapY - lastClickY) >= 5)
                {
                    int leftUpperX = (int)Math.Min(lastClickX, mouseX - mapX);
                    int leftUpperY = (int)Math.Min(lastClickY, mouseY - mapY);

                    int rightLowerX = (int)Math.Max(lastClickX, mouseX - mapX);
                    int rightLowerY = (int)Math.Max(lastClickY, mouseY - mapY);
                    DrawRectangle(spriteBatch, new Rectangle((int)(leftUpperX+mapX), (int)(leftUpperY+mapY),(int)(rightLowerX-leftUpperX), (int)(rightLowerY-leftUpperY)), Color.Green, 1);
                }
            }
            //draw cursor image with offset (becasue upper left corner is not the actual hot point) by AZ
            spriteBatch.Draw(mouseImg, new Rectangle((int)mouseX - 5, (int)mouseY - 1, mouseImg.Width, mouseImg.Height), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void RightClickMoveFunc()
        { //AZ code wrapped in function so can be called elsewhere (like human move button event)
            int tileX = SplashScreen.tileObject.tileWidth;
            int tileY = SplashScreen.tileObject.tileHeight;
            int xToGo = (int)((mouseX - mapX) / tileX);
            int yToGo = (int)((mouseY - mapY) / tileY);
            int unitN = 0;//the unit's order number of the whole chosen units list, used to arrange the formation

            if (xToGo >= 0 && yToGo >= 0 && xToGo <= thisMap.mapW && yToGo <= thisMap.mapH)
            { //wrap with this check to make sure our click is within bound of map (as we can't move to negative coordinates or outside of map - causes game to crash)

                int startPatrol = 0;
                if (buttons.mode == 4)
                {
                    buttons.cancelHumanMoveArmy(); //cancel selection
                    startPatrol = 1;
                }

                int battleMode = 0;
                if (buttons.mode == 5)
                {
                    buttons.cancelHumanMoveArmy(); //cancel selection
                    battleMode = 1;
                }

                int resourceMode = 0;
                if (buttons.mode == 3)
                {
                    buttons.cancelHumanMove(); //cancel selection
                    resourceMode = 1;
                }

                int repairMode = 0;
                if (buttons.mode == 6)
                {
                    buttons.cancelHumanMove();
                    repairMode = 1;
                }
                //foreach (Unit chosenUnit in chosenUnits)
                foreach (unitPtr cur in thisMap.chosenUnits)
                {
                    //currentUnits[cur.owner][cur.index] gives pointer to unit
                    //Added check traversibiliy, will ignore command if not
                    if (currentUnits[cur.owner][cur.index] != null && currentUnits[cur.owner][cur.index].unitType < SplashScreen.numUnits && (currentUnits[cur.owner][cur.index].owner == playerChoice || playerChoice == 0))
                    { //added extra condition so if a certain player (besides nature 0), then must be owner of object to select object

                        //temparay commented this line to test group working
                        //if (thisMap.isTraversible(xToGo, yToGo) || (thisMap.isTree(xToGo, yToGo) && thisMap.isChoppable(xToGo, yToGo) && chosenUnit.unitType == 0)) //comment out this line for easier movement
                        if (currentUnits[cur.owner][cur.index].getAction() != "death" && currentUnits[cur.owner][cur.index].getAction() != "decay" && currentUnits[cur.owner][cur.index].usable == 1) //can't move if dead or not usable // && currentUnits[cur.owner][cur.index].battling == false
                        { //only moveable if check if traversible or if peasant and tree
                            currentUnits[cur.owner][cur.index].xToGo = xToGo;
                            currentUnits[cur.owner][cur.index].yToGo = yToGo;

                            //use this switch to handle multipe units' formation, (to prevent all units go to the same destination tile)
                            switch (unitN)
                            {
                                case 0:
                                    break;
                                case 1:
                                    currentUnits[cur.owner][cur.index].xToGo += 1;
                                    currentUnits[cur.owner][cur.index].yToGo += 0;
                                    break;
                                case 2:
                                    currentUnits[cur.owner][cur.index].xToGo += 1;
                                    currentUnits[cur.owner][cur.index].yToGo += 1;
                                    break;
                                case 3:
                                    currentUnits[cur.owner][cur.index].xToGo += 0;
                                    currentUnits[cur.owner][cur.index].yToGo += 1;
                                    break;
                                case 4:
                                    currentUnits[cur.owner][cur.index].xToGo += -1;
                                    currentUnits[cur.owner][cur.index].yToGo += 1;
                                    break;
                                case 5:
                                    currentUnits[cur.owner][cur.index].xToGo += -1;
                                    currentUnits[cur.owner][cur.index].yToGo += 0;
                                    break;
                                case 6:
                                    currentUnits[cur.owner][cur.index].xToGo += -1;
                                    currentUnits[cur.owner][cur.index].yToGo += -1;
                                    break;
                                case 7:
                                    currentUnits[cur.owner][cur.index].xToGo += 0;
                                    currentUnits[cur.owner][cur.index].yToGo += -1;
                                    break;
                                case 8:
                                    currentUnits[cur.owner][cur.index].xToGo += 1;
                                    currentUnits[cur.owner][cur.index].yToGo += -1;
                                    break;

                                default:
                                    break;
                            }

                            // currentUnits[cur.owner][cur.index].

                            unitN++;


                            //Only set target if normal movement right click (human move button doesn't set target)
                            if (thisMap.isTree(xToGo, yToGo) && (buttons.mode == 0 || resourceMode == 1)) //we came into this if case because peasanto on tree
                            {//so set tree as target
                                currentUnits[cur.owner][cur.index].target.owner = 0; //set tree owner to nature
                                currentUnits[cur.owner][cur.index].target.index = -1; //set to tree index
                                currentUnits[cur.owner][cur.index].target.x = xToGo;
                                currentUnits[cur.owner][cur.index].target.y = yToGo;
                                currentUnits[cur.owner][cur.index].findNearestOpenSpot(currentUnits, ref thisMap);
                            }
                            else if (buttons.mode == 0 || resourceMode == 1) //normal click so check for target unit
                            {
                                currentUnits[cur.owner][cur.index].target = thisMap.target(currentUnits, xToGo, yToGo, currentUnits[cur.owner][cur.index], 0); //set target if any
                                currentUnits[cur.owner][cur.index].findNearestOpenSpot(currentUnits, ref thisMap);
                                
                            }
                            if (currentUnits[cur.owner][cur.index].getAction() == "attack" || currentUnits[cur.owner][cur.index].battling == true || currentUnits[cur.owner][cur.index].startedBattle == true || currentUnits[cur.owner][cur.index].chasing == true || currentUnits[cur.owner][cur.index].patrolling == true) currentUnits[cur.owner][cur.index].resetAnimation(); //interrupting


                            //chosenUnit.prevTarget.reset();
                            //chosenUnit.target = thisMap.target(currentUnits, xToGo, yToGo, chosenUnit); //set target if any
                            currentUnits[cur.owner][cur.index].prevTarget.reset();

                            if (startPatrol == 1) //send units on patrol
                            {
                                if ((int)currentUnits[cur.owner][cur.index].x != xToGo || (int)currentUnits[cur.owner][cur.index].y != yToGo) //we are not moving to same spot (basically y or x value changed)
                                {
                                    currentUnits[cur.owner][cur.index].patrolX2 = (int)currentUnits[cur.owner][cur.index].x;
                                    currentUnits[cur.owner][cur.index].patrolY2 = (int)currentUnits[cur.owner][cur.index].y;
                                    currentUnits[cur.owner][cur.index].patrolX1 = xToGo;
                                    currentUnits[cur.owner][cur.index].patrolY1 = yToGo;
                                    currentUnits[cur.owner][cur.index].patrolling = true;
                                }
                            }

                            if (battleMode == 1) //battle mode for all units
                            { //set selfdestroy to true for unit so it can attack friendly unit
                                currentUnits[cur.owner][cur.index].selfDestroy = true;
                            }

                            if (repairMode == 1) //repair mode for all peasant units
                            {
                                currentUnits[cur.owner][cur.index].repairing = true;
                            }

                            float dx = currentUnits[cur.owner][cur.index].xToGo - currentUnits[cur.owner][cur.index].x;
                            float dy = currentUnits[cur.owner][cur.index].yToGo - currentUnits[cur.owner][cur.index].y;
                            if (dy == 0) { dy = 0.0001f; }
                            if (dx == 0) { dx = 0.0001f; }
                            if (dy < 0)//if facing north
                            {
                                if (Math.Abs(dx / dy) < 0.5)
                                {
                                    currentUnits[cur.owner][cur.index].setDirection("N");
                                }
                                else if (dx < 0)
                                { // to west

                                    if (Math.Abs(dy / dx) < 0.5)//purely west
                                    {
                                        currentUnits[cur.owner][cur.index].setDirection("W");
                                    }
                                    else
                                    {
                                        currentUnits[cur.owner][cur.index].setDirection("NW");
                                    }

                                }
                                else
                                { // to east
                                    if (Math.Abs(dy / dx) < 0.5)
                                    {
                                        currentUnits[cur.owner][cur.index].setDirection("E");
                                    }
                                    else
                                    {
                                        currentUnits[cur.owner][cur.index].setDirection("NE");
                                    }
                                }

                            }
                            else
                            {
                                if (Math.Abs(dx / dy) < 0.5)
                                {
                                    currentUnits[cur.owner][cur.index].setDirection("S");
                                }
                                else if (dx < 0)
                                { // to west

                                    if (Math.Abs(dy / dx) < 0.5)// purely west
                                    {
                                        currentUnits[cur.owner][cur.index].setDirection("W");
                                    }
                                    else
                                    {
                                        currentUnits[cur.owner][cur.index].setDirection("SW");
                                    }

                                }
                                else
                                { // to east
                                    if (Math.Abs(dy / dx) < 0.5)
                                    {
                                        currentUnits[cur.owner][cur.index].setDirection("E");
                                    }
                                    else
                                    {
                                        currentUnits[cur.owner][cur.index].setDirection("SE");
                                    }
                                }

                            }

                            //random sound command (moved inside unit movement only instead of all terrain)
                            currentUnits[cur.owner][cur.index].playSound(); //function moved into Unit.cs

                            currentUnits[cur.owner][cur.index].onCommand = true;
                            if (currentUnits[cur.owner][cur.index].unitType > -1 && currentUnits[cur.owner][cur.index].battling == false) //if it's a unit and not projectile
                            {
                                currentUnits[cur.owner][cur.index].updateDir(thisMapPathGrid);
                                currentUnits[cur.owner][cur.index].updateNextDir();
                            }

                        }
                    }
                }
            }

        }
        //separate the function of drawing mini map and view port to make ecode easier to read. by AZ
        //Problem fixed: when resize the view port
        private void DrawMinimap()
        {
            //Draw a mini map, moved drawing of the minimap here, so I could draw units on top of it
            spriteBatch.Draw(theMap, new Rectangle(minimapPosX, minimapPosY, minimapWidth, minimapHeight), Color.White);
            //Draw viewport
            int newViewW = (int)(screenWidth * .05); // These values will help determine the thiccness of our viewport on the minimap
            int newViewH = (int)(screenHeight * .05); // These values will help determine the thiccness of our viewport on the minimap
            


            /*int viewW = (int)(screenWidth * .05); // These values will help determine the thiccness of our viewport on the minimap
            int viewH = (int)(screenHeight * .05); // These values will help determine the thiccness of our viewport on the minimap
            Texture2D vpRec = new Texture2D(graphics.GraphicsDevice, viewW, viewH);
            Color[] vpCol = new Color[viewW * viewH]; // Unit Color*/



            //draw unit on the minimap:
            int tileX = SplashScreen.tileObject.tileWidth;
            int tileY = SplashScreen.tileObject.tileHeight;
            for (int i = 0; i <= SplashScreen.mapUnits.numPlayer[mapChoice]; i++) //iterate through all players
            {
                foreach (Unit u in currentUnits[i]) //iterate through all assets of a player
                {
                    if (u.invisible == 0 && u.unitType > -1) //not a projectile
                    {
                    
                        // Draw unit on the minimap
                        //Texture2D uRec = new Texture2D(graphics.GraphicsDevice, 1, 1);
                        //Use stored pixel because uses less memory?
                        Color[] uCol = new Color[1]; // Unit Color

                        uCol[0] = Color.Yellow;
                        //if is player, set the mini map's data. Because the uCol[0] is reserved by public asset, and the Color.Brown is very close to Red.
                        if (i != 0)
                        {
                            if (SplashScreen.playerColor[i - 1] == 0) { uCol[0] = Color.Red; } // if (i == 1) { uCol[0] = Color.Red; }
                            if (SplashScreen.playerColor[i - 1] == 1) { uCol[0] = Color.Blue; } // if (i == 2) { uCol[0] = Color.Blue; }
                            if (SplashScreen.playerColor[i - 1] == 2) { uCol[0] = Color.Green; } // if (i == 3) { uCol[0] = Color.Green; }
                            if (SplashScreen.playerColor[i - 1] == 3) { uCol[0] = Color.Purple; } // if (i == 4) { uCol[0] = Color.Purple; }
                            if (SplashScreen.playerColor[i - 1] == 4) { uCol[0] = Color.Orange; } // if (i == 5) { uCol[0] = Color.Orange; }
                            if (SplashScreen.playerColor[i - 1] == 5) { uCol[0] = Color.Black; } // if (i == 6) { uCol[0] = Color.Black; }
                            if (SplashScreen.playerColor[i - 1] == 6) { uCol[0] = Color.Gray; } // if (i == 7) { uCol[0] = Color.Gray; }
                            if (SplashScreen.playerColor[i - 1] == 7) { uCol[0] = Color.Brown; } // if (i == 8) { uCol[0] = Color.Brown; }
                        }

                        // if (i == SplashScreen.mapUnits.playerChoice) { uCol[0] = Color.Green; }
                        u.uRec.SetData(uCol);
                        
                        spriteBatch.Draw(
                            u.uRec,
                            new Rectangle(
                                (int)(u.x * tileX * minimapScaleW) + minimapPosX,
                                (int)(u.y * tileY * minimapScaleH) + minimapPosY,
                                (int)(u.objImg2D.Width * minimapScaleW),
                                (int)(u.objImg2D.Height * minimapScaleH)
                                ),
                            Color.White
                            );
                        
                    }

                }

            }
            //Draw the fog of war. by AZ
            int height = theMapIndex.data.Length;
            int width = theMapIndex.data[0].Length;
            Texture2D _pointTexture2 = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pointTexture2.SetData<Color>(new Color[] { Color.Black });

            int pixelWidth = (int)(Math.Ceiling(1.0f  / width * minimapWidth));
            int pixelHeight = (int)(Math.Ceiling(1.0f / height * minimapHeight));
            int[,] greyArea = new int[minimapHeight+1,minimapWidth+1];
            for (int i = 0; i < pixelWidth; i++)
            {
                for (int i2 = 0; i2 < pixelHeight; i2++)
                {
                    greyArea[i2, i] = 0;
                   
                }
            }

            if (thisMap.renderFogFlag != 0)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (thisMap.exploredMap[(y) * thisMap.mapW + (x)] == 0) //if it is not explored) { }
                        {
                            //(minimapPosX, minimapPosY, minimapWidth, minimapHeight
                           
                            spriteBatch.Draw(_pointTexture2, new Rectangle((int)(1.0f * x / width * minimapWidth + minimapPosX), (int)(1.0f * y / height * minimapHeight + minimapPosY), pixelWidth, pixelHeight), Color.White);
                        }
                        else if (thisMap.fogMap[(y) * thisMap.mapW + (x)] == 0) //if it is in fog) { }
                        {
                            //(minimapPosX, minimapPosY, minimapWidth, minimapHeight
                            //draw the sem-opaque fog
                            for (int i =0;i < pixelWidth;i++)
                            {
                                for (int i2 = 0; i2 < pixelHeight; i2++)
                                {
                                    int targetX = (int)(1.0f * x / width * minimapWidth + minimapPosX + i);
                                    int targetY = (int)(1.0f * y / height * minimapHeight + minimapPosY + i2);
                                    if (greyArea[targetY,targetX] == 0)
                                    {
                                        spriteBatch.Draw(_pointTexture2, new Rectangle(targetX,targetY ,1,1), new Color(Color.White, 0.5f));
                                        greyArea[targetY, targetX] = 1;
                                    }
                                }
                            }
                            //spriteBatch.Draw(_pointTexture2, new Rectangle((int)(1.0f * x / width * minimapWidth + minimapPosX), (int)(1.0f * y / height * minimapHeight + minimapPosY), pixelWidth, pixelHeight), new Color(Color.White, 0.5f));
                        }

                    }

                }
            }


            //Draw the view port:
            //for (int i = 0; i < vpCol.Length; i++)*/
            if (newViewH != viewH || newViewW != viewW) //basically only call if viewpoint position changed
            {
                viewW = newViewW;
                viewH = newViewH;
                //if viewport size is 0 then return to prevent critical bug
                if (viewH <= 1 || viewW <= 1) { return; }

                Texture2D vpRec = new Texture2D(graphics.GraphicsDevice, viewW, viewH);
               
                Color[] vpCol = new Color[viewW * viewH]; // Unit Color
                for (int i = 0; i < vpCol.Length; i++)
                {
                    if ((i >= 0 && i < viewW) // If top of screen
                        || (i % viewW == 0) // If left of screen
                        || (i % viewW == viewW - 1) // If right of screen
                        || (i >= (viewW * viewH) - viewW && i < (viewW * viewH)) // If bottom of screen
                        )
                    {
                        vpCol[i] = Color.White;
                    }
                }
                vpRec.SetData(vpCol);
                vpRecGlobal = vpRec;

            }
            spriteBatch.Draw(
                vpRecGlobal,
                new Rectangle(
                    (int)(-mapX * minimapScaleW) + minimapPosX,
                    (int)(-mapY * minimapScaleH) + minimapPosY,
                    (int)(minimapScaleW * screenWidth),
                    (int)(minimapScaleH * screenHeight)
                    ),
                Color.White
                );
         
        }


        private void DrawTheMap()
        {
            int tileX = SplashScreen.tileObject.tileWidth;
            int tileY = SplashScreen.tileObject.tileHeight;

            int renderStartX = (int)(-mapX / tileX) - 1;
            int renderStartY = (int)(-mapY / tileY) - 1;

            if (renderStartX < 0) { renderStartX = 0; }
            if (renderStartX >= theMapIndex.data[0].Length) { renderStartX = theMapIndex.data[0].Length -1; }
            if (renderStartY < 0) { renderStartY = 0; }
            if (renderStartY >= theMapIndex.data.Length) { renderStartY = theMapIndex.data.Length - 1; }

            int width = screenWidth / tileX + 3;
            int height = screenHeight / tileY + 3;

            //Rectangle screenRectangle = new Rectangle((int)mapX, (int)mapY, theMapWidth, theMapHeight);
            //spriteBatch.Draw(theMap, screenRectangle, Color.White);


            if (thisMap.renderFogFlag == 0)
            {
                //New rendering method by AZ week 7
                for (int y = 0;y < height;y++)
                {
                    for (int x = 0;x < width;x++)
                    {
                        if (renderStartX + x < theMapIndex.data[0].Length && renderStartY + y < theMapIndex.data.Length)
                        {
                            spriteBatch.Draw(SplashScreen.tileObject.tiles2D[theMapIndex.data[renderStartY + y][renderStartX + x]], new Rectangle((int)mapX + (renderStartX +x) * tileX, (int)mapY + (renderStartY +y) * tileY, tileX, tileY), Color.White);
                        }

                    }

                }


    
            }
            else //render fog of war
            {

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (renderStartX + x < theMapIndex.data[0].Length && renderStartY + y < theMapIndex.data.Length)
                        {
                            if (thisMap.fogMap[(renderStartY +y) * thisMap.mapW + (renderStartX+ x)] != 0) //if it is in view
                            {
                                
                                spriteBatch.Draw(SplashScreen.tileObject.tiles2D[theMapIndex.data[renderStartY + y][renderStartX + x]], new Rectangle((int)mapX + (renderStartX + x) * tileX, (int)mapY + (renderStartY + y) * tileY, tileX, tileY), Color.White);
                            } else if (thisMap.exploredMap[(renderStartY + y) * thisMap.mapW + (renderStartX + x)] != 0) //if it is explored
                            {
                                spriteBatch.Draw(SplashScreen.tileObject.tiles2D[theMapIndex.data[renderStartY + y][renderStartX + x]], new Rectangle((int)mapX + (renderStartX + x) * tileX, (int)mapY + (renderStartY + y) * tileY, tileX, tileY), Color.Gray);
                            }

                        }

                    }

                }


            }

        }

        // sperate the drawing units function to make code clearer. by AZ
        private void DrawUnits()
        {
            int tileX = SplashScreen.tileObject.tileWidth;
            int tileY = SplashScreen.tileObject.tileHeight;
            for (int i = 0; i <= SplashScreen.mapUnits.numPlayer[mapChoice]; i++) //iterate through all players
            {
                foreach (Unit u in currentUnits[i]) //iterate through all assets of a player
                {
                    if (u.invisible == 0)
                    {
                        //if is a unit, then neet to correct its center
                        int offsetX = 0;
                        int offsetY = 0;
                        if (u.unitType < SplashScreen.numUnits)//if it is a unit, then place the center of the picture to the center of the tile.
                        {
                            offsetX = -u.objImg2D.Width / 2 + tileX / 2;
                            offsetY = -u.objImg2D.Height / 2 + tileY / 2;
                        }
                        if (u.tintGrey == 1) spriteBatch.Draw(u.objImg2D, new Rectangle((int)(u.x * tileX + (int)mapX + offsetX), (int)(u.y * tileY + (int)mapY + offsetY), u.objImg2D.Width, u.objImg2D.Height), Color.Gray);
                        else spriteBatch.Draw(u.objImg2D, new Rectangle((int)(u.x * tileX + (int)mapX + offsetX), (int)(u.y * tileY + (int)mapY + offsetY), u.objImg2D.Width, u.objImg2D.Height), Color.White);

                    }

                }

            }
        }


  
        //A handy function to draw a rectage,
        //Came from https://stackoverflow.com/questions/13893959/how-to-draw-the-border-of-a-square
        //Because it is a very basic function so that I could not modify it too much and cited it directly. by AZ
        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int lineWidth)
        {
            if (_pointTexture == null)
            {
                _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pointTexture.SetData<Color>(new Color[] { Color.White });
            }

            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

            // to update the viewport on the minimap's data
            screenWidth = Window.ClientBounds.Width;
            screenHeight = Window.ClientBounds.Height;


            graphics.ApplyChanges();
        }

        //Override the on exiting method to stop BGM while exiting by AZ
        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            //stop music
            gameMusic.controls.stop();
            Window.ClientSizeChanged -= winResize;
            //Set to null to free memory (not working yet)
            graphics = null;
            spriteBatch = null;
            theMap = null;
            currentUnits = null;
            thisMap = null;

            // Stop the threads
        }

        // draw things to win form
        private void drawWinForm(Form targetScreen, RenderTarget2D renderTarget, bool isLoadContent)
        {
            if (!isLoadContent)
            {
                //release memory by disposing
                //miniMapbox.Dispose();
            }
            //JD draw the miniMap, then later will put it into miniMap winForm
            graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            //spriteBatch.Draw(theMap, new Rectangle(0, 0, miniMap_menu.Width, miniMap_menu.Height), Color.White);
            DrawMinimap();
            spriteBatch.End();
            graphics.GraphicsDevice.SetRenderTarget(null); // go back to game screen
            // translate Texture2D to Bitmap, reference from https://stackoverflow.com/questions/12495172/i-want-to-use-a-texture2d-as-a-system-drawing-bitmap

            MemoryStream memoryStream = new MemoryStream();
            renderTarget.SaveAsPng(memoryStream, renderTarget.Width, renderTarget.Height);

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(memoryStream);

            //Reference here: https://stackoverflow.com/questions/24413517/picturebox-image-wont-update-refresh
            if (miniMapbox.Image != null) miniMapbox.Image.Dispose();
            miniMapbox.Image = bmp;
            //foreach (Control ctrl in targetScreen.Controls) ctrl.Refresh();
            miniMapbox.Refresh();
            //miniMapbox.Size = new System.Drawing.Size(targetScreen.Width, targetScreen.Height);
            //miniMapbox.SizeMode = PictureBoxSizeMode.StretchImage;
            //miniMapbox.BackColor = System.Drawing.Color.Transparent;
            //targetScreen.Controls.Add(miniMapbox);
        }

        private void inGameButtons()
        {
            
        }

        //AZ: New Functionality: added click the minimap to move view port
        private void minimap_Click(object sender, System.EventArgs e)
        {
            //the the mouse's corrdinate on the minimap component
           System.Drawing.Point point = miniMap_menu.PointToClient(Cursor.Position);
            int idealMapX =(int)( -1.0f * point.X / miniMap_menu.Width * theMapWidth) + (int)(Window.ClientBounds.Width * 0.5f);
            int idealMapY =(int)( -1.0f * point.Y / miniMap_menu.Height * theMapHeight) + (int)(Window.ClientBounds.Height * 0.5f);

            mapX = idealMapX;
            mapY = idealMapY;
            if (mapX > 0) { mapX = 0; }

            if (mapY > 0) { mapY = 0; }

            if (mapX + theMapWidth < Window.ClientBounds.Width)
            {
                mapX = Window.ClientBounds.Width - theMapWidth;
            }

            if (mapY + theMapHeight < Window.ClientBounds.Height)
            {
                mapY = Window.ClientBounds.Height - theMapHeight;
            }

        }

        private void inGameMenuAjust()
        {

            // JD 2/9/2019
            if (Properties.Settings.Default.WindowSize != null)
            {
                inGame_form.Size = Properties.Settings.Default.WindowSize;
            }
            if (Properties.Settings.Default.WindowLocation != null)
            {
                inGame_form.Location = Properties.Settings.Default.WindowLocation;
            }
            f.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            f.Location = new System.Drawing.Point(150, 50);
            f.Size = new System.Drawing.Size(inGame_form.Width - 150, inGame_form.Height - 50);
            f.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left);

            miniMap_menu.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            miniMap_menu.Location = new System.Drawing.Point(10, 50);
            miniMap_menu.Size = new System.Drawing.Size(130, 130);

            //Buttons
            buttons = new inGameButtons(inGame_form, ref thisMap, ref currentUnits);
            buttons.FormBorderStyle = FormBorderStyle.None;
            buttons.TopLevel = false;
            button_menu.Controls.Add(buttons);
            button_menu.FormBorderStyle = FormBorderStyle.None;
            button_menu.Location = new System.Drawing.Point(10, 390);
            button_menu.Size = new System.Drawing.Size(125, 125);
            button_menu.BackgroundImage = new System.Drawing.Bitmap(@"data\img\Texture.png");
            buttons.Show();

            //Unit Datas
            data = new UnitDatas();
            data.FormBorderStyle = FormBorderStyle.None;
            data.TopLevel = false;
            data_menu.Controls.Add(data);
            data_menu.FormBorderStyle = FormBorderStyle.None;
            data_menu.Location = new System.Drawing.Point(10, 190);
            data_menu.Size = new System.Drawing.Size(130, 190);
            data_menu.BackgroundImage = new System.Drawing.Bitmap(@"data\img\Texture.png");
            data.Show();

            //Cursor
            inGame_form.Cursor = new Cursor(CursorLoader.curPointer.GetHicon());

            // JD: add menu and embedded menu
            inGame_form.Show();
            f.TopLevel = false;
            //f.TopLevel = true;
            inGame_form.Controls.Add(f);
            f.Show();

            miniMap_menu.TopLevel = false;
            inGame_form.Controls.Add(miniMap_menu);
            miniMap_menu.Show();

            button_menu.TopLevel = false;
            inGame_form.Controls.Add(button_menu);
            button_menu.Show();

            data_menu.TopLevel = false;
            inGame_form.Controls.Add(data_menu);
            data_menu.Show();

            inGame_form.keyPreview_Load();

            if (Properties.Settings.Default.WindowMax)
            {
                inGame_form.WindowState = FormWindowState.Maximized;
            }
        }

        private void singleShowButton(Unit u)  //play building sound here
        {
            System.IO.FileStream fs = null;
            int choose;
            clickUnit1 = clickUnit0;

            if (u.owner == playerChoice && u.inUse == 0 && u.usable == 1) //we are selecting a unit that is ours; hence menu
            {
                buttons.hideButtons(); //hide old buttons before showing new ones
                switch (data.nameTranslation(u.unitType))
                {
                    case "Peasant": //peasant
                        buttons.peasantButtons(); 
                        clickUnit0 = "Peasant";
                        if (clickUnit0 == clickUnit1)
                        {
                            choose = new Random().Next(0, 6);
                            fs = new System.IO.FileStream(@"data\snd\peasant\annoyed" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);
                        }
                        else
                        {
                            choose = new Random().Next(0, 3);
                            fs = new System.IO.FileStream(@"data\snd\peasant\selected" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);
                        }
                        break;
                    case "Footman": //footman
                                    //buttons.hideButtons();
                        buttons.footmanButtons();
                        clickUnit0 = "Footman";
                        if (clickUnit0 == clickUnit1)
                        {
                            choose = new Random().Next(0, 6);
                            fs = new System.IO.FileStream(@"data\snd\basic\annoyed" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);
                        }
                        else
                        {
                            choose = new Random().Next(0, 5);
                            fs = new System.IO.FileStream(@"data\snd\basic\selected" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);
                        }
                            break;
                    case "Archer": //archer
                        buttons.archerButtons();
                        clickUnit0 = "Archer";
                        if (clickUnit0 == clickUnit1)
                        {
                            choose = new Random().Next(0, 2);
                            fs = new System.IO.FileStream(@"data\snd\archer\annoyed" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);
                        }
                        else
                        {
                            choose = new Random().Next(0, 3);
                            fs = new System.IO.FileStream(@"data\snd\archer\selected" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);
                        }
                        break;
                    case "Ranger": //Ranger shows same as archer
                        buttons.archerButtons();
                        choose = new Random().Next(0, 3);
                        fs = new System.IO.FileStream(@"data\snd\archer\selected" + (choose + 1).ToString() + ".wav", System.IO.FileMode.Open);
                        break;
                    case "TownHall": //townhall
                        buttons.townHallButtons();
                        break;
                    case "Keep": //keep
                        buttons.townHallButtons();
                        break;
                    case "Castle": //castle
                        buttons.townHallButtons();
                        break;
                    case "Barracks": //barracks
                                     //buttons.hideButtons();
                        buttons.barracksButtons();
                        break;
                    case "Blacksmith": //barracks
                                       //buttons.hideButtons();
                        buttons.blackSmithButtons();
                        fs = new System.IO.FileStream(@"data\snd\buildings\blacksmith.wav", System.IO.FileMode.Open);
                        break;
                    case "LumberMill": //lumbermill
                        buttons.lumbermillButtons();
                        fs = new System.IO.FileStream(@"data\snd\buildings\lumber-mill.wav", System.IO.FileMode.Open);
                        break;
                    case "ScoutTower":
                        buttons.scouttowerButtons();
                        break;
                    case "Farm":
                        fs = new System.IO.FileStream(@"data\snd\buildings\farm.wav", System.IO.FileMode.Open);
                        break;
                    case "GoldMine":  //not working?
                        fs = new System.IO.FileStream(@"data\snd\buildings\gold-mine.wav", System.IO.FileMode.Open);
                        break;
                    default: //nothing happens
                        buttons.hideButtons();
                        break;
                }
                if (fs != null)
                {
                    SoundEffect mysound = SoundEffect.FromStream(fs);
                    fs.Dispose();
                    mysound.Play(volume: 0.01f * SoundOptionsMenu.pubVolSFX, pitch: 0.0f, pan: 0.0f);
                }
            }
        }

        private void showMultiUnitsButtons()
        {
            if (thisMap.chosenUnits.Count() > 0 && thisMap.checkSameSelected(currentUnits))
            { //check to make sure if all units selected are of same type before showing the buttons
                buttons.hideButtons(); //hide old buttons before showing new ones
                unitPtr temp = thisMap.chosenUnits.ElementAt(0);
                Unit u = currentUnits[temp.owner][temp.index]; //get first unit in list
                if (u.owner == playerChoice && u.inUse == 0 && u.usable == 1 && thisMap.chosenUnits.Count() > 1) //we are selecting multiple units so don't display building buttons (we don't have multi building button clicks implemented at same time)
                {
                    switch (data.nameTranslation(u.unitType))
                    {
                        case "Peasant": //peasant
                            buttons.peasantButtons();
                            break;
                        case "Footman": //footman
                                        //buttons.hideButtons();
                            buttons.footmanButtons();
                            break;
                        case "Archer": //archer
                            buttons.archerButtons();
                            break;
                        case "Ranger": //Ranger shows same as archer
                            buttons.archerButtons();
                            break;
                        default: //nothing happens
                            buttons.hideButtons();
                            break;
                    }
                }
                else singleShowButton(u); //show single button


            }
        }

    }
}
