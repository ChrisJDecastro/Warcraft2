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

namespace Demo
{
    public partial class Multiplayer : Form
    {
        public int buttonX = 150;
        public int buttonY = 40;
        public int clientx = 800;
        public int clienty = 600;
        private System.Windows.Forms.Button host;
        private System.Windows.Forms.Button join;
        private System.Windows.Forms.Button joinDirect;
        private System.Windows.Forms.Button joinServer;
        private System.Windows.Forms.Button back;
        private System.Windows.Forms.Label title;

        private MapList mapListMenu;

        // Server Table
        private Label serverTitle;
        private Label serverGame;
        private Label serverOwner;
        private Label serverMap;
        private Label serverPlayers;
        private System.Windows.Forms.Button serverBack;

        public static WindowsMediaPlayer buttonClickSnd;

        public Multiplayer()
        {
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.multiplayer_FormClosing);
            if (Properties.Settings.Default.WindowMax)
            {
                WindowState = FormWindowState.Maximized;
            }
            if (Properties.Settings.Default.WindowLocation != null)
            {
                this.Location = Properties.Settings.Default.WindowLocation;
            }
            if (Properties.Settings.Default.WindowSize != null)
            {
                this.Size = Properties.Settings.Default.WindowSize;
            }
            this.BackgroundImage = new Bitmap(@"data\img\Texture.png");
            Bitmap color = new Bitmap(@"data\img\Colors.png");
            System.Drawing.Color color_blue = color.GetPixel(1, 1);

            mapListMenu = new MapList();

            buttonClickSnd = new WindowsMediaPlayer();
            buttonClickSnd.URL = @"data/snd/misc/thunk.wav";

            this.Resize += new EventHandler(multiplayer_Resize);

            this.title = new Label();
            title.Location = new Point(this.Width / 3 - buttonX / 48, this.Height / 16);
            title.Text = "Multi Player Game Options";
            title.AutoSize = true;
            title.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title.BackColor = Color.Transparent;
            title.ForeColor = Color.White;
            this.Controls.Add(title);

            this.host = new Button();
            host.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 4);
            host.Height = buttonY;
            host.Width = buttonX;
            host.BackColor = color_blue;
            host.ForeColor = Color.Gold;
            host.Text = "Host Multi Player Game";
            host.Click += new EventHandler(mapSelect_Click);
            this.Controls.Add(host);

   
            this.join = new Button();
            join.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 3);
            join.Height = buttonY;
            join.Width = buttonX;
            join.BackColor = color_blue;
            join.ForeColor = Color.Gold;
            join.Text = "Join Multiplayer Game";
            join.Click += new EventHandler(join_Click);
            this.Controls.Add(join);

            this.joinDirect = new Button();
            joinDirect.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 4);
            joinDirect.Height = buttonY;
            joinDirect.Width = buttonX;
            joinDirect.BackColor = color_blue;
            joinDirect.ForeColor = Color.Gold;
            joinDirect.Text = "Direct Connection";
            //joinDirect.Click += new EventHandler(back_Click);
            joinDirect.Hide(); // Hide the join buttons at initialization
            this.Controls.Add(joinDirect);

            this.joinServer = new Button();
            joinServer.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 3);
            joinServer.Height = buttonY;
            joinServer.Width = buttonX;
            joinServer.BackColor = color_blue;
            joinServer.ForeColor = Color.Gold;
            joinServer.Text = "Server Connection";
            joinServer.Click += new EventHandler(joinServer_Click);
            joinServer.Hide(); // Hide the join buttons at initialization
            this.Controls.Add(joinServer);

            initServerTable();

            this.back = new Button();
            back.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 5);
            back.Height = buttonY;
            back.Width = buttonX;
            back.BackColor = color_blue;
            back.ForeColor = Color.Gold;
            back.Text = "Back";
            back.Click += new EventHandler(back_Click);
            this.Controls.Add(back);
        }

        private void initServerTable()
        {
            this.serverTitle = new Label();
            serverTitle.Location = new Point(this.Width / 2 - this.serverTitle.Width / 2, this.Height / 32);
            serverTitle.Text = "Servers";
            serverTitle.Size = title.Size;
            serverTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            serverTitle.BackColor = Color.Transparent;
            serverTitle.ForeColor = Color.White;
            this.serverTitle.Hide();
            this.Controls.Add(serverTitle);

            this.serverGame = new Label();
            serverGame.Location = new Point(this.Width / 16, this.Height / 12);
            serverGame.Text = "Game";
            serverGame.AutoSize = true;
            serverGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            serverGame.BackColor = Color.Transparent;
            serverGame.ForeColor = Color.White;
            this.serverGame.Hide();
            this.Controls.Add(serverGame);

            this.serverOwner = new Label();
            serverOwner.Location = new Point(this.Width / 16 * 4, this.Height / 12);
            serverOwner.Text = "Owner";
            serverOwner.AutoSize = true;
            serverOwner.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            serverOwner.BackColor = Color.Transparent;
            serverOwner.ForeColor = Color.White;
            this.serverOwner.Hide();
            this.Controls.Add(serverOwner);

            this.serverMap = new Label();
            serverMap.Location = new Point(this.Width / 16 * 8, this.Height / 12);
            serverMap.Text = "Map";
            serverMap.AutoSize = true;
            serverMap.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            serverMap.BackColor = Color.Transparent;
            serverMap.ForeColor = Color.White;
            this.serverMap.Hide();
            this.Controls.Add(serverMap);

            this.serverPlayers = new Label();
            serverPlayers.Location = new Point(this.Width / 16 * 12, this.Height / 12);
            serverPlayers.Text = "Players";
            serverPlayers.AutoSize = true;
            serverPlayers.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            serverPlayers.BackColor = Color.Transparent;
            serverPlayers.ForeColor = Color.White;
            this.serverPlayers.Hide();
            this.Controls.Add(serverPlayers);

            Bitmap color = new Bitmap(@"data\img\Colors.png");
            System.Drawing.Color color_blue = color.GetPixel(1, 1);
            this.serverBack = new Button();
            serverBack.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 6);
            serverBack.Height = buttonY;
            serverBack.Width = buttonX;
            serverBack.BackColor = color_blue;
            serverBack.ForeColor = Color.Gold;
            serverBack.Text = "Back";
            serverBack.Click += new EventHandler(serverBack_Click);
            this.serverBack.Hide();
            this.Controls.Add(serverBack);
        }

        private void serverBack_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();

            this.serverTitle.Hide();
            this.serverGame.Hide();
            this.serverOwner.Hide();
            this.serverMap.Hide();
            this.serverPlayers.Hide();
            this.serverBack.Hide();
            this.title.Show();
            this.joinDirect.Show();
            this.joinServer.Show();
            this.back.Show();
        }

        private void mapSelect_Click(object sender, EventArgs e)
        {
            MapList.isMultiplayerHost = true;
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
        private void multiplayer_Resize(object sender, System.EventArgs e)
        {
            title.Location = new Point(this.Width / 3 - buttonX / 48, this.Height / 16);
            host.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 4);
            join.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 3);
            back.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 5);
            clientx = this.Width;
            clienty = this.Height;
        }
        private void buttonSnd()
        {
            buttonClickSnd.controls.play();
        }

        private void join_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();

            this.joinDirect.Show();
            this.joinServer.Show();
            this.host.Hide();
            this.join.Hide();
        }
        private void joinServer_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();

            this.title.Hide();
            this.joinDirect.Hide();
            this.joinServer.Hide();
            this.back.Hide();
            this.serverTitle.Show();
            this.serverGame.Show();
            this.serverOwner.Show();
            this.serverMap.Show();
            this.serverPlayers.Show();
            this.serverBack.Show();
        }
        private void back_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();

            if (this.joinDirect.Visible)
            {
                // If inside join menu
                this.host.Show();
                this.join.Show();
                this.joinDirect.Hide();
                this.joinServer.Hide();
            }
            else
            {
                Application.OpenForms.OfType<MainMenu>().First().Show();
                this.Hide();
            }
        }
        private void multiplayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
