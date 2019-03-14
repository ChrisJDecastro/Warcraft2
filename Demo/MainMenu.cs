using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace Demo
{
    public partial class MainMenu : Form
    {
        public int clientx = 1024;
        public int clienty = 768;
        public int buttonX = 150;
        public int buttonY = 40;

        //private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button playGame; // Single Player
        private System.Windows.Forms.Button multiMenu; // Multiplayer Menu
        //private System.Windows.Forms.Button cursorTest;
        //private System.Windows.Forms.Button mapSelect;

        private System.Windows.Forms.Button options;
        private System.Windows.Forms.Button exitButton;
        private Login multiplayerMenu;
        private MapList mapListMenu;
        private Options optionsMenu;
        public static WindowsMediaPlayer menuMusic; //removed static
        public static WindowsMediaPlayer buttonClickSnd;

        public MainMenu()
        {
            InitializeComponent();
            this.Size = new Size(clientx, clienty);
            ReallyCenterToScreen();
            Bitmap backGroundImage = new Bitmap(@"data\img\Texture.png");
            this.BackgroundImage = backGroundImage;
            Bitmap color = new Bitmap(@"data\img\Colors.png");
            // because the Colors size is 4 * 8, the bule is at 1, 1
            Color color_blue = color.GetPixel(1, 1);


            //Load menu music
            menuMusic = new WMPLib.WindowsMediaPlayer();
            menuMusic.URL = @"data\snd\music\menu.mp3";
            menuMusic.settings.playCount = 999; //1 for testing, set to 999
            //menuMusic.controls.play();

            //Load button sound
            buttonClickSnd = new WindowsMediaPlayer();
            buttonClickSnd.URL = @"data/snd/misc/thunk.wav";

            multiplayerMenu = new Login();

            mapListMenu = new MapList();

            optionsMenu = new Options();

            this.SuspendLayout();

            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainMenu_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();



            //To handle window resize event
            this.Resize += new EventHandler(MainMenu_Resize);
            //Set background of form (But can't conenct/find path to texture for some reason)
            //this.BackgroundImage = Image.FromFile(@"data\img\Texture.png");
            /*Bitmap textureA = new Bitmap(@"data\img\Texture.png");
            this.BackgroundImage = textureA;*/


            //Play game button
            this.playGame = new Button();
            playGame.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 4);
            playGame.Height = buttonY;
            playGame.Width = buttonX;
            playGame.BackColor = color_blue;
            playGame.ForeColor = Color.Gold;
            playGame.Text = "Single Player Game";
            //playGame.Click += new EventHandler(playGame_Click);
            playGame.Click += new EventHandler(mapSelect_Click);
            this.Controls.Add(playGame);

            //Multiplayer Menu button
            this.multiMenu = new Button();
            multiMenu.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 3);
            multiMenu.Height = buttonY;
            multiMenu.Width = buttonX;
            multiMenu.BackColor = color_blue;
            multiMenu.ForeColor = Color.Gold;
            multiMenu.Text = "Multiplayer";
            multiMenu.Click += new EventHandler(multiMenu_Click);
            this.Controls.Add(multiMenu);

            //Map Select button
            /*
            this.mapSelect = new Button();
            mapSelect.Location = new Point(this.Width / 2 - buttonX / 2, this.Height /8 * 3);
            mapSelect.Height = buttonY;
            mapSelect.Width = buttonX;
            mapSelect.BackColor = color_blue;
            mapSelect.ForeColor = Color.Gold;
            mapSelect.Text = "Select Map";
            mapSelect.Click += new EventHandler(mapSelect_Click);
            this.Controls.Add(mapSelect);
            */

            //Options
            this.options = new Button();
            options.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 2);
            options.Height = buttonY;
            options.Width = buttonX;
            options.BackColor = color_blue;
            options.ForeColor = Color.Gold;
            options.Text = "Options";
            options.Click += new EventHandler(optionsSelect_Click);
            this.Controls.Add(options);

            //Exit button
            this.exitButton = new Button();
            exitButton.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 5);
            exitButton.Height = buttonY;
            exitButton.Width = buttonX;
            exitButton.BackColor = color_blue;
            exitButton.ForeColor = Color.Gold;
            exitButton.Text = "Exit Game";
            exitButton.Click += new EventHandler(exitButton_Click);
            this.Controls.Add(exitButton);

            Label menuName = new Label();
            menuName.Text = "Main Menu";
            menuName.AutoSize = true;
            menuName.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            menuName.BackgroundImage = backGroundImage;
            menuName.ForeColor = Color.Gold;
            menuName.Anchor = (AnchorStyles.Top );
            menuName.Location = new Point(this.Width / 2 - 80, 10);
            this.Controls.Add(menuName);
        }

        private void buttonSnd()
        {
            buttonClickSnd.controls.play();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {

        }
        private void playGame_Click(object sender, EventArgs e)
        {
            //Need to boot game here
            // new inGame().Show();

            //Modified by AZ: we need to generate a XNA window here to display the game window with hardware boost:
           // XNA_InGame g = new XNA_InGame();
            buttonSnd();
            menuMusic.controls.stop();
            
           // g.Run();
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                XNA_InGame g = new XNA_InGame();
                g.Run();
                //Console.WriteLine("Hello, world");
            }).Start();
            this.Hide();
            
            //new MiniMap().Show();

            //this.Hide(); //Don't hide form so can do multiple
        }

        private void cursorTest_Click(object sender, EventArgs e)
        {
            //Need to boot game here
            new FormMouseCR().Show();
            buttonSnd();
            //Application.Run(new FormMouseCR());
            //this.Hide(); //Don't hide form so can do multiple
        }

        private void multiMenu_Click(object sender, EventArgs e)
        {
            //Need to boot game here
            multiplayerMenu.Show();
            //multiplayerMenu.Location = this.Location;
            buttonSnd();
            //Application.Run(new FormMouseCR());
            this.Hide(); //Don't hide form so can do multiple

            // JD: 2/24, set the next showed window size, location, WindowState  
            if (this.WindowState == FormWindowState.Normal)
            {
                multiplayerMenu.Location = this.Location;
                multiplayerMenu.Size = this.Size;
            }
            else
            {
                multiplayerMenu.Size = this.RestoreBounds.Size;
                multiplayerMenu.Location = this.RestoreBounds.Location;
            }
            multiplayerMenu.WindowState = this.WindowState;
        }

        private void mapSelect_Click(object sender, EventArgs e)
        {
            //Need to boot game here
            mapListMenu.Show();
            buttonSnd();
            //Application.Run(new FormMouseCR());
            this.Hide(); //Don't hide form so can do multiple

            // JD: 2/24, set the next showed window size, location, WindowState
            if (this.WindowState == FormWindowState.Normal)
            {
                mapListMenu.Size = this.Size;
                mapListMenu.Location = this.Location;
            }
            else
            {
                mapListMenu.Size = this.RestoreBounds.Size;
                mapListMenu.Location = this.RestoreBounds.Location;
            }
            mapListMenu.WindowState = this.WindowState;
        }

        private void optionsSelect_Click(object sender, EventArgs e)
        {
            //Need to boot game here
            optionsMenu.Show();
            //optionsMenu.Location = this.Location;
            buttonSnd();
            //Application.Run(new FormMouseCR());
            this.Hide(); //Don't hide form so can do multiple

            // JD: 2/24, set the next showed window size, location, WindowState
            if (this.WindowState == FormWindowState.Normal)
            {
                optionsMenu.Location = this.Location;
                optionsMenu.Size = this.Size;
            }
            else
            {
                optionsMenu.Size = this.RestoreBounds.Size;
                optionsMenu.Location = this.RestoreBounds.Location;
            }
            optionsMenu.WindowState = this.WindowState;
        }

        /*
        private void sndOptionsMenu_Click(object sender, EventArgs e)
        {
            sndOptions.Show();
            buttonSnd();
            //this.Hide(); //Don't hide form so can do multiple
        }
        */

        private void exitButton_Click(object sender, EventArgs e)
        {
            buttonSnd();
            Application.Exit();
        }

        //Currently moves button around as window resizes but may be a bit off due to math that needs to be checked
        private void MainMenu_Resize(object sender, System.EventArgs e)
        {
            playGame.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 4);
            multiMenu.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 3);
            //cursorTest.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 3);
            //mapSelect.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 3);
            options.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 2);
            exitButton.Location = new Point(this.Width / 2 - buttonX / 2, this.Height /8 * 5);
            clientx = this.Width;
            clienty = this.Height;
        }

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        { //Lionel's Formclosing code
            /*const string message = "Are you sure that you would like to close the form?";
            const string caption = "Form Closing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            // If the no button was pressed ...
            if (result == DialogResult.No)
            {
                // cancel the closure of the form.
                e.Cancel = true;
            }
            else Application.Exit();*/
            Application.Exit(); //just have this, or we will have double form closing
        }

        // JD 2/1/2019 Reference from https://stackoverflow.com/questions/6837463/how-come-centertoscreen-method-centers-the-form-on-the-screen-where-the-cursor-i
        protected void ReallyCenterToScreen()
        {
            Screen screen = Screen.FromControl(this);

            Rectangle workingArea = screen.WorkingArea;
            this.Location = new Point()
            {
                X = Math.Max(workingArea.X, workingArea.X + (workingArea.Width - this.Width) / 2),
                Y = Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - this.Height) / 2)
            };
        }
    }
}
