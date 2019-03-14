using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using System.Threading;

namespace Demo
{

    //Note: 

    public partial class playerSelectMenu : Form
    {
        /*public static int[] playerColor = new int[SplashScreen.mapUnits.numPlayer[SplashScreen.mapObject.mapChoice]];// Player Color Here!!!!!
        //public static int defaultColor = 0; //stores the default red color
        // Player AI difficulty, player2 is playerAI[0], the value stored inside, 0 = low AI, 1 = Medium, 2 = High.
        public static int[] playerAI = new int[SplashScreen.mapUnits.numPlayer[SplashScreen.mapObject.mapChoice]];*/ //Moved to SplashScreen.cs

        public int[] playerColor;// Player Color Here!!!!!
        
        public int[] playerAI;

        public int buttonX = 150;
        public int buttonY = 40;
        private List<Label> playerList;
        private List<Button> colorList;
        private List<PictureBox> selectedColorList;

        int mapChoice = SplashScreen.mapObject.mapChoice;
        int playerChoice = SplashScreen.mapUnits.playerChoice;
        int numPlayer = SplashScreen.mapUnits.numPlayer[SplashScreen.mapObject.mapChoice];
        PictureBox colorBox;
        //public Bitmap color = new Bitmap(@"data\img\Colors.png");
        public Bitmap color;
        Bitmap unitsonMap;
        PictureBox tilebox;
        int mapX, mapY;

        public playerSelectMenu()
        {
            InitializeComponent();

            SplashScreen.playerColor = new int[SplashScreen.mapUnits.numPlayer[SplashScreen.mapObject.mapChoice]];// Player Color Here!!!!!
            SplashScreen.playerAI = new int[SplashScreen.mapUnits.numPlayer[SplashScreen.mapObject.mapChoice]];
            this.playerAI = SplashScreen.playerAI; //pointers over to SplashScreen.cs
            this.playerColor = SplashScreen.playerColor;
            this.color = SplashScreen.color;
            

            // JD:set Data->ApplicationSettings->Location to be WindowLocation in the property of MapList.cs[Design]. reference from https://www.codeproject.com/Articles/15013/Windows-Forms-User-Settings-in-C
            // set this window position and size base on the previous window.
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.playerSelection_FormClosing);
            Bitmap backGroundImage = new Bitmap(@"data\img\Texture.png");
            this.BackgroundImage = backGroundImage;

            // because the Colors size is 4 * 8, the bule is at 1, 1
            System.Drawing.Color color_blue = color.GetPixel(1, 1);

            this.Load += new EventHandler(playerSelection_Load);

            colorList = new List<Button>();
            playerList = new List<Label>();
            selectedColorList = new List<PictureBox>();



            for (int i = 0; i < numPlayer; i++) //changed numMaps
            {
                // textLabel 1 is player number, and player 1 is "You".
                Label textLabel1 = new Label();
                textLabel1.Location = new Point(50, buttonY * i + 100);
                if (i == 0)
                {
                    textLabel1.Text = "You       ";
                }
                else
                {
                    textLabel1.Text = "Player " + (i + 1);
                }
                textLabel1.AutoSize = true;
                textLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                textLabel1.BackgroundImage = backGroundImage;
                textLabel1.ForeColor = Color.Gold;
                playerList.Add(textLabel1);
                this.Controls.Add(textLabel1);

                colorBox = new PictureBox();
                colorBox.Size = new System.Drawing.Size(60, 20);
                colorBox.Location = new System.Drawing.Point(textLabel1.Location.X + textLabel1.Width * 3 / 2 + 30 * 8, textLabel1.Location.Y);
                colorBox.SizeMode = PictureBoxSizeMode.StretchImage;
                colorBox.BackColor = color.GetPixel(1, i);
                colorBox.Name = String.Format("player" + i);
                selectedColorList.Add(colorBox);
                this.Controls.Add(colorBox);

                // textLabel 2 is the description of colorBox
                Label textLabel2 = new Label();
                textLabel2.Location = new Point(textLabel1.Location.X + textLabel1.Width * 3 / 2 + 30 * 8, textLabel1.Location.Y - 20);
                if (i == 0)
                {
                    textLabel2.Text = "Your color ";
                }
                else
                {
                    textLabel2.Text = "Player " + (i + 1) + " color ";
                }
                textLabel2.AutoSize = true;
                textLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                textLabel2.BackgroundImage = backGroundImage;
                textLabel2.ForeColor = Color.Gold;
                this.Controls.Add(textLabel2);

                if (i != 0) // for AI
                {
                    ComboBox aiBox = new ComboBox();
                    aiBox.Size = new System.Drawing.Size(100, 20);
                    aiBox.Location = new System.Drawing.Point(colorBox.Location.X + colorBox.Width + 10, colorBox.Location.Y);
                    aiBox.BackColor = System.Drawing.Color.Black;
                    aiBox.ForeColor = System.Drawing.Color.Gold;
                    aiBox.Name = String.Format("player" + i);
                    aiBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    aiBox.Items.Add("Low AI");
                    aiBox.Items.Add("Medium AI");
                    aiBox.Items.Add("High AI");
                    aiBox.SelectedItem = "Low AI";
                    aiBox.SelectedIndexChanged += new EventHandler(aiBox_Click);
                    this.Controls.Add(aiBox);
                    playerAI[i - 1] = 0; // add the default AI difficulty 
                }


                for (int f = 0; f < 8; f++) //changed numMaps
                {
                    Button colorButton1 = new Button();
                    colorButton1.Width = buttonY / 2;
                    colorButton1.Height = buttonY / 2;
                    colorButton1.Location = new Point(textLabel1.Location.X + textLabel1.Width * 3 / 2 + 30 * f, textLabel1.Location.Y);
                    colorButton1.BackColor = color.GetPixel(1, f);
                    colorButton1.Click += new EventHandler(colorButton1_Click);
                    colorButton1.Name = String.Format("player" + i + "color" + f); // the button should be "player2
                    colorList.Add(colorButton1);
                    this.Controls.Add(colorButton1);
                }

                //selectedColorList[i].Refresh();

            }

            Button select = new Button();
            select.Location = new System.Drawing.Point(this.Width - buttonX - 40, this.Height / 10 * 8 - buttonY - 20);
            select.Height = buttonY;
            select.Width = buttonX;
            select.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            select.BackColor = color_blue;
            select.ForeColor = Color.Gold;
            select.Text = "Select";
            select.Click += new EventHandler(select_Click);
            this.Controls.Add(select);

            Button cancel = new Button();
            cancel.Location = new System.Drawing.Point(this.Width - buttonX - 40, this.Height / 10 * 9 - buttonY - 20);
            cancel.Height = buttonY;
            cancel.Width = buttonX;
            cancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            cancel.BackColor = color_blue;
            cancel.ForeColor = Color.Gold;
            cancel.Text = "Cancel";
            cancel.Click += new EventHandler(cancel_Click);
            this.Controls.Add(cancel);

            mapX = SplashScreen.mapObject.mapX[mapChoice]; //minus one because we have -1 row/col for tiles
            mapY = SplashScreen.mapObject.mapY[mapChoice];

            tilebox = new PictureBox();
            tilebox.Location = new Point(this.Width - buttonX - 40, 50);
            tilebox.Size = new Size(150, 150);
            tilebox.BackColor = Color.Black;//AZ week 7: fix the white border issue
            tilebox.Image = SplashScreen.mapObject.entireMaps[mapChoice];
            tilebox.SizeMode = PictureBoxSizeMode.Zoom;
            tilebox.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
            base.Controls.Add(tilebox);

            // textLabel 3 and 4 are the map description
            Label textLabel3 = new Label();
            textLabel3.Location = new Point(tilebox.Location.X + 25, tilebox.Location.Y + 160);
            textLabel3.Text = "Size: " + mapX + " * " + mapY;
            textLabel3.AutoSize = true;
            textLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            textLabel3.BackgroundImage = backGroundImage;
            textLabel3.ForeColor = Color.Gold;
            textLabel3.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
            this.Controls.Add(textLabel3);

            Label textLabel4 = new Label();
            textLabel4.Location = new Point(tilebox.Location.X + 15, tilebox.Location.Y + 180);
            textLabel4.Text = "num of player: " + numPlayer;
            textLabel4.AutoSize = true;
            textLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            textLabel4.BackgroundImage = backGroundImage;
            textLabel4.ForeColor = Color.Gold;
            textLabel4.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
            this.Controls.Add(textLabel4);

            Label menuName = new Label();
            menuName.Text = "Select Player";
            menuName.AutoSize = true;
            menuName.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            menuName.BackgroundImage = backGroundImage;
            menuName.ForeColor = Color.Gold;
            menuName.Anchor = (AnchorStyles.Top);
            menuName.Location = new Point(this.Width / 2 - 80, 10);
            this.Controls.Add(menuName);

        }

        private void select_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();
            MainMenu.menuMusic.controls.stop();//stop the menu music.

            // JD: 2/24, set the next showed window size, location, WindowState
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.WindowLocation = this.Location;
                Properties.Settings.Default.WindowSize = this.Size;
                Properties.Settings.Default.WindowMax = false;
            }
            else
            {
                Properties.Settings.Default.WindowSize = this.RestoreBounds.Size;
                Properties.Settings.Default.WindowLocation = this.RestoreBounds.Location;
                Properties.Settings.Default.WindowMax = true;
            }

            //New method of starting a game:
            /*new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;



                XNA_InGame g = new XNA_InGame();
                g.Run();
                //Console.WriteLine("Hello, world");
            }).Start();*/

            Thread tempGame = new Thread(() =>
            {
                //Thread.CurrentThread.IsBackground = true;
                XNA_InGame g = new XNA_InGame();
                g.Run();
                //Console.WriteLine("Hello, world");
            });

            Thread main = Thread.CurrentThread;
            tempGame.Start();

            //this.Hide(); //hides the form

            if (Thread.CurrentThread == main)
            {
                this.Hide();
                tempGame.Join(); //wait for game thread to end
                //Thread.CurrentThread.IsBackground = false;
                this.Show(); //reshow form

                MainMenu.menuMusic.controls.play();//play the menu music.

                //Not working
                tempGame = null; //need to free thread memory
                SplashScreen.coloredUnits = null; //need to free (as we re-init on new XNA)
                
                //new MainMenu();
            }
        }
        private void cancel_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();

            Application.OpenForms.OfType<MapList>().First().Show();
            this.Hide();

            // JD: 2/24, set the next showed window size, location, WindowState
            Application.OpenForms.OfType<MapList>().First().WindowState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal)
            {
                Application.OpenForms.OfType<MapList>().First().Location = this.Location;
                Application.OpenForms.OfType<MapList>().First().Size = this.Size;
            }
            else
            {
                Application.OpenForms.OfType<MapList>().First().Location = this.RestoreBounds.Location;
                Application.OpenForms.OfType<MapList>().First().Size = this.RestoreBounds.Size;
            } 
        }

        private void playerSelection_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        private void colorButton1_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            bool colorSetted = false;
            int colorIndex = button.Name[12] - 48; // don't know why need to minus 49 and 48... I think because the hex number repersent the int number in string
            int playerIndex = button.Name[6] - 48;

            // check whether  the color is selected by another player, then assign the color
            //MessageBox.Show("playerIndex = " + playerIndex);
            //MessageBox.Show("colorIndex = " + colorIndex);
            for (int i = 0; i < numPlayer; i++)
            {
                if (playerColor[i] == colorIndex)
                {
                    playerColor[i] = playerColor[playerIndex];
                    playerColor[playerIndex] = colorIndex;
                    colorSetted = true;
                }
            }
            if (colorSetted == false)
            {
                playerColor[playerIndex] = colorIndex;

            }
            // refresh the selected color box
            for (int i = 0; i < numPlayer; i++)
            {
                selectedColorList[i].BackColor = color.GetPixel(1, playerColor[i]);
                selectedColorList[i].Refresh();
            }
            unitsonMap = SplashScreen.mapObject.entireMaps[SplashScreen.mapObject.mapChoice];
            renderUnitonMiniMap(unitsonMap);
            tilebox.Image = unitsonMap;
        }
        private void playerSelection_Load(object sender, EventArgs e)
        {
            //int[] playerColor = new int[numPlayer];
            for (int i = 0; i < numPlayer; i++) //changed numMaps
            {
                //if (i == 0) defaultColor = i;
                playerColor[i] = i; // assign the default color
                //MessageBox.Show("playerColor[i] = " + playerColor[i]);
            }

        }

        private void aiBox_Click(object sender, EventArgs e)
        {
            ComboBox aiBox = (ComboBox)sender;
            int playerIndex = aiBox.Name[6] - 48;

            playerAI[playerIndex - 1] = aiBox.SelectedIndex;
            //MessageBox.Show("playerAI" + (playerIndex -1) + " = " + aiBox.SelectedIndex);

        }
        private void renderUnitonMiniMap(Bitmap miniMap)
        {
            // initially i = 1, because 0 is GoldMine that not need color change
            for (int i = 1; i <= SplashScreen.mapUnits.numPlayer[SplashScreen.mapObject.mapChoice]; i++) //iterate through all players
            {
                Brush[] uCol = new Brush[1]; // Unit Color
                //if (playerColor[i - 1] == 0) { uCol[0] = Brushes.Yellow; } // yellow is reserved by public assets
                if (playerColor[i - 1] == 0) { uCol[0] = Brushes.Red; }
                if (playerColor[i - 1] == 1) { uCol[0] = Brushes.Blue; }
                if (playerColor[i - 1] == 2) { uCol[0] = Brushes.Green; }
                if (playerColor[i - 1] == 3) { uCol[0] = Brushes.Purple; }
                if (playerColor[i - 1] == 4) { uCol[0] = Brushes.Orange; }
                if (playerColor[i - 1] == 5) { uCol[0] = Brushes.Black; }
                if (playerColor[i - 1] == 6) { uCol[0] = Brushes.Gray; }
                if (playerColor[i - 1] == 7) { uCol[0] = Brushes.Brown; }

                foreach (Asset u in SplashScreen.mapUnits.mapAssets[SplashScreen.mapObject.mapChoice][i]) //iterate through all assets of a player
                {
                    using (Graphics graphics = Graphics.FromImage(miniMap))
                    {
                        if (u.type == "GoldMine" || u.type == "TownHall")
                            graphics.FillRectangle(uCol[0], new Rectangle(u.x, u.y, 2 , 2 )); 
                        if (u.type == "Peasant" || u.type == "Footman" || u.type == "Archer" || u.type == "Ranger")
                            graphics.FillRectangle(uCol[0], new Rectangle(u.x, u.y, 1 , 1 ));
                        graphics.Save();
                    }
                }
            }
        }

    }
}

