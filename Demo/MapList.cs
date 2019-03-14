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

using System.IO;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
//using Microsoft.Xna.Framework.Input;


namespace Demo
{
    public partial class MapList : Form
    {
        // set this true in Multiplayer.cs, then set it back to false in MultiplayerSelect
        public static bool isMultiplayerHost; 
        //public static int isMultiplayerJoin;
        int buttonX = 150;
        int buttonY = 40;
        PictureBox tilebox;
        playerSelectMenu playerSelectM;

        private ListBox mapbox;
        Label textLabel4, textLabel3;
        Bitmap unitsonMap;
        int mapX, mapY;

        Bitmap listBoxBackground;

        public MapList()
        {
            InitializeComponent();

            //Reference: http://yacsharpblog.blogspot.com/2008/07/listbox-flicker.html
            //Attempt at fixing flicker doesn't work
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.DoubleBuffered = true;

            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MapList_FormClosing);
            Bitmap backGroundImage = new Bitmap(@"data\img\Texture.png");
            this.BackgroundImage = backGroundImage;

            //this.listBoxBackground = SplashScreen.mapListBG;
            

            Bitmap color = new Bitmap(@"data\img\Colors.png");
            // because the Colors size is 4 * 8, the bule is at 1, 1
            System.Drawing.Color color_blue = color.GetPixel(1, 1);

            mapbox = new ListBox();
            
            mapbox.Location = new System.Drawing.Point(20, 50);
            mapbox.Name = "Maps";
            mapbox.Size = new System.Drawing.Size(this.Width / 4 * 3 - 40, this.Height - 100);
            mapbox.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left);

            this.listBoxBackground = new Bitmap(this.Width*2, this.Height*2);
            using (Graphics g = Graphics.FromImage(this.listBoxBackground))
            {
                int i = 0;
                while (i * SplashScreen.mapListBG.Width < this.Width*2)
                {
                    int j = 0;
                    while (j * SplashScreen.mapListBG.Height < this.Height*2)
                    {

                        g.DrawImage(SplashScreen.mapListBG, new Rectangle(i * SplashScreen.mapListBG.Width, j * SplashScreen.mapListBG.Height, SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height), 0.0f, 0.0f, SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height, GraphicsUnit.Pixel);
                        j++;
                    }
                    i++;
                }
                
                //g.DrawImage(SplashScreen.mapListBG, new Rectangle(0, 0, SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height), 0.0f, 0.0f, SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height, GraphicsUnit.Pixel);
                //g.DrawImage(SplashScreen.mapListBG, new Rectangle(SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height, SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height), 0.0f, 0.0f, SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height, GraphicsUnit.Pixel);
            }

            //Reference: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.listbox.drawitem?view=netframework-4.7.2
            mapbox.DrawMode = DrawMode.OwnerDrawFixed;
            mapbox.DrawItem += new DrawItemEventHandler(listBox_BG);
            mapbox.Resize += new EventHandler(listBoxResize);

            mapbox.BeginUpdate();
            for (int i = 0; i < SplashScreen.mapObject.mapNames.Length; i++) //changed numMaps
            {
                mapbox.Items.Add(SplashScreen.mapObject.mapNames[i]);
            }
            mapbox.EndUpdate();

            SplashScreen.mapObject.mapChoice = mapbox.SelectedIndex;


            mapbox.SelectedIndexChanged += new EventHandler(SelectChange);
            this.Controls.Add(mapbox);

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

            // JD 2/3/2019: function from XNA_InGame
            if (!(1 < mapbox.SelectedIndex && mapbox.SelectedIndex < SplashScreen.mapObject.mapNames.Length))
            {
                SplashScreen.mapObject.mapChoice = 2;
            }

            //int mapX = SplashScreen.mapObject.mapX[SplashScreen.mapObject.mapChoice]; //minus one because we have -1 row/col for tiles
            //int mapY = SplashScreen.mapObject.mapY[SplashScreen.mapObject.mapChoice];

            tilebox = new PictureBox();
            tilebox.BackColor = Color.Black;
            tilebox.Location = new Point(this.Width - buttonX - 40, 50);
            tilebox.Size = new Size(150, 150);
            unitsonMap = SplashScreen.mapObject.entireMaps[SplashScreen.mapObject.mapChoice];
            renderUnitonMiniMap(unitsonMap);
            tilebox.Image = unitsonMap;
            tilebox.SizeMode = PictureBoxSizeMode.Zoom;//to prevent image not sized correctly
            tilebox.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
            base.Controls.Add(tilebox);

            // textLabel 3 and 4 are the map description
            textLabel3 = new Label();
            textLabel3.Location = new Point(tilebox.Location.X + 25, tilebox.Location.Y + 160);
            textLabel3.Text = "Size: " + SplashScreen.mapObject.mapX[SplashScreen.mapObject.mapChoice] + " * " + SplashScreen.mapObject.mapY[SplashScreen.mapObject.mapChoice];
            textLabel3.AutoSize = true;
            textLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            textLabel3.BackgroundImage = backGroundImage;
            textLabel3.ForeColor = Color.Gold;
            textLabel3.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
            this.Controls.Add(textLabel3);

            textLabel4 = new Label();
            textLabel4.Location = new Point(tilebox.Location.X + 15, tilebox.Location.Y + 180);
            textLabel4.Text = "num of player: " + SplashScreen.mapUnits.numPlayer[SplashScreen.mapObject.mapChoice];
            textLabel4.AutoSize = true;
            textLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            textLabel4.BackgroundImage = backGroundImage;
            textLabel4.ForeColor = Color.Gold;
            textLabel4.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
            this.Controls.Add(textLabel4);

            Label menuName = new Label();
            menuName.Text = "Map List";
            menuName.AutoSize = true;
            menuName.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            menuName.BackgroundImage = backGroundImage;
            menuName.ForeColor = Color.Gold;
            menuName.Anchor = (AnchorStyles.Top);
            menuName.Location = new Point(this.Width / 2 - 80, 10);
            this.Controls.Add(menuName);

        }

        private void listBoxResize(object sender, EventArgs e)
        {
            //mapbox.BeginUpdate();
            using (Graphics g = Graphics.FromImage(this.listBoxBackground))
            {
                int i = 0;
                while (i * SplashScreen.mapListBG.Width < this.Width)
                {
                    int j = 0;
                    while (j * SplashScreen.mapListBG.Height < this.Height)
                    {

                        g.DrawImage(SplashScreen.mapListBG, new Rectangle(i * SplashScreen.mapListBG.Width, j * SplashScreen.mapListBG.Height, SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height), 0.0f, 0.0f, SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height, GraphicsUnit.Pixel);
                        j++;
                    }
                    i++;
                }

                //g.DrawImage(SplashScreen.mapListBG, new Rectangle(0, 0, SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height), 0.0f, 0.0f, SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height, GraphicsUnit.Pixel);
                //g.DrawImage(SplashScreen.mapListBG, new Rectangle(SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height, SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height), 0.0f, 0.0f, SplashScreen.mapListBG.Width, SplashScreen.mapListBG.Height, GraphicsUnit.Pixel);
            }
            //mapbox.EndUpdate();
        }

        private void listBox_BG(object sender, DrawItemEventArgs e)
        { //Reference: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.listbox.drawitem?view=netframework-4.7.2

            e.Graphics.DrawImage(this.listBoxBackground,new Point(0,0));
            //e.DrawBackground();

            Brush fontColor = Brushes.Gold;
            Brush selectedColor = Brushes.White;
            int index = 0;
            int itemHeight = mapbox.GetItemHeight(e.Index);
            int selected = mapbox.SelectedIndex;
            foreach (var i in this.mapbox.Items)
            {
                if (index == selected) e.Graphics.DrawString(i.ToString(), e.Font, selectedColor, 0, itemHeight * index);
                else e.Graphics.DrawString(i.ToString(), e.Font, fontColor, 0, itemHeight * index);
                index++;
            }
            //e.Graphics.DrawString(this.mapbox.Items[e.Index].ToString(), e.Font, fontColor, 0, (float)(e.Bounds.Top + mapbox.GetItemHeight(e.Index)));

            //e.Graphics.DrawString(this.mapbox.Items[e.Index].ToString(), e.Font, fontColor, e.Bounds.Top, StringFormat.GenericDefault);

            //e.DrawFocusRectangle();
            //e.Graphics.Dispose();
        }

        private void SelectChange(object sender, System.EventArgs e)
        {
            //mapbox.BeginUpdate();
            SplashScreen.mapObject.mapChoice = mapbox.SelectedIndex;
            if (0 <= SplashScreen.mapObject.mapChoice && SplashScreen.mapObject.mapChoice < SplashScreen.mapObject.mapNames.Length)
            { //make sure we selected valid map
              //tilebox.Image = SplashScreen.mapObject.entireMaps[SplashScreen.mapObject.mapChoice];
                unitsonMap = SplashScreen.mapObject.entireMaps[SplashScreen.mapObject.mapChoice];
                renderUnitonMiniMap(unitsonMap);
                tilebox.Image = unitsonMap;
            }
            //JD :2/24
            if (!(0 <= mapbox.SelectedIndex && mapbox.SelectedIndex < SplashScreen.mapObject.mapNames.Length))
            {
                SplashScreen.mapObject.mapChoice = 2;
            }
            textLabel3.Text = "Size: " + SplashScreen.mapObject.mapX[SplashScreen.mapObject.mapChoice] + " * " + SplashScreen.mapObject.mapY[SplashScreen.mapObject.mapChoice];
            textLabel4.Text = "num of player: " + SplashScreen.mapUnits.numPlayer[SplashScreen.mapObject.mapChoice];
            //mapbox.EndUpdate();
        }

        private void MapList_Load(object sender, EventArgs e)
        {

        }

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

        private void select_Click(object sender, EventArgs e)
        {
            //this.Close();
            //Modified by AZ: we need to generate a XNA window here to display the game window with hardware boost:
            //WindowsMediaPlayer buttonClick = new WindowsMediaPlayer();
            //buttonClick.URL = @"data/snd/misc/thunk.wav";
            //buttonClick.controls.play();

            //deprecated method:
            //XNA_InGame g = new XNA_InGame();
            //g.Run();

            /*
            //New method of starting a game: // JD move to playerSelection
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                XNA_InGame g = new XNA_InGame();
                g.Run();
                //Console.WriteLine("Hello, world");
            }).Start();
            */
            MainMenu.buttonClickSnd.controls.play();
            if (isMultiplayerHost)
            {
                MultiplayerSelect playerSelectM;
                playerSelectM = new MultiplayerSelect(); // JD 3/11
                playerSelectM.Show();
                this.Hide(); //Don't hide form so can do multiple

                // JD: 2/24, set the next showed window size, location, WindowState
                playerSelectM.WindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    playerSelectM.Location = this.Location;
                    playerSelectM.Size = this.Size;
                }
                else
                {
                    playerSelectM.Size = this.RestoreBounds.Size;
                    playerSelectM.Location = this.RestoreBounds.Location;
                }
            }
            else
            {
                playerSelectM = new playerSelectMenu(); //JD: because it needs to update the selectIndex. If put this above, the selectIdex is always 2.
                playerSelectM.Show();
                this.Hide(); //Don't hide form so can do multiple

                // JD: 2/24, set the next showed window size, location, WindowState
                playerSelectM.WindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    playerSelectM.Location = this.Location;
                    playerSelectM.Size = this.Size;
                }
                else
                {
                    playerSelectM.Size = this.RestoreBounds.Size;
                    playerSelectM.Location = this.RestoreBounds.Location;
                }
            }



        }
        private void cancel_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();

            Application.OpenForms.OfType<MainMenu>().First().Show();
            this.Hide();

            // JD: 2/24, set the next showed window size, location, WindowState
            Application.OpenForms.OfType<MainMenu>().First().WindowState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal)
            {
                Application.OpenForms.OfType<MainMenu>().First().Size = this.Size;
                Application.OpenForms.OfType<MainMenu>().First().Location = this.Location;

            }
            else
            {
                Application.OpenForms.OfType<MainMenu>().First().Size = this.RestoreBounds.Size;
                Application.OpenForms.OfType<MainMenu>().First().Location = this.RestoreBounds.Location;
            }
            

        }
        private void MapList_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        private void renderUnitonMiniMap(Bitmap miniMap)
        {
            mapX = SplashScreen.mapObject.mapX[SplashScreen.mapObject.mapChoice]; //minus one because we have -1 row/col for tiles
            mapY = SplashScreen.mapObject.mapY[SplashScreen.mapObject.mapChoice];

            for (int i = 0; i <= SplashScreen.mapUnits.numPlayer[SplashScreen.mapObject.mapChoice]; i++) //iterate through all players
            {
                Brush[] uCol = new Brush[1]; // Unit Color
                if (i == 0) { uCol[0] = Brushes.Yellow; }
                if (i == 1) { uCol[0] = Brushes.Red; }
                if (i == 2) { uCol[0] = Brushes.Blue; }
                if (i == 3) { uCol[0] = Brushes.Green; }
                if (i == 4) { uCol[0] = Brushes.Purple; }
                if (i == 5) { uCol[0] = Brushes.Orange; }
                if (i == 6) { uCol[0] = Brushes.Black; }
                if (i == 7) { uCol[0] = Brushes.Gray; }
                if (i == 8) { uCol[0] = Brushes.Brown; }

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
