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


namespace Demo
{
    public partial class Options : Form
    {
        public int buttonX = 150;
        public int buttonY = 40;
        public int clientx = 800;
        public int clienty = 600;

        private System.Windows.Forms.Button sndOptionsMenu;
        private System.Windows.Forms.Button networkOptions;
        private System.Windows.Forms.Button back;
        private System.Windows.Forms.Label title;


        private SoundOptionsMenu sndOptions;
        private NetworkOptions networkOptionsMenu;
        public static WindowsMediaPlayer buttonClickSnd;

        public Options()
        {
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Options_FormClosing);
            this.BackgroundImage = new Bitmap(@"data\img\Texture.png");
            Bitmap color = new Bitmap(@"data\img\Colors.png");
            System.Drawing.Color color_blue = color.GetPixel(1, 1);

            sndOptions = new SoundOptionsMenu();
            networkOptionsMenu = new NetworkOptions();

            buttonClickSnd = new WindowsMediaPlayer();
            buttonClickSnd.URL = @"data/snd/misc/thunk.wav";

            this.Resize += new EventHandler(OptionsMenu_Resize);

            this.title = new Label();
            title.Location = new Point(this.Width / 2 - buttonX/4, this.Height / 16);
            title.Text = "Options";
            title.AutoSize = true;
            title.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title.BackColor = Color.Transparent;
            title.ForeColor = Color.White;
            this.Controls.Add(title);


            this.sndOptionsMenu = new Button();
            sndOptionsMenu.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 4);
            sndOptionsMenu.Height = buttonY;
            sndOptionsMenu.Width = buttonX;
            sndOptionsMenu.BackColor = color_blue;
            sndOptionsMenu.ForeColor = Color.Gold;
            sndOptionsMenu.Text = "Sound Options";
            sndOptionsMenu.Click += new EventHandler(sndOptionsSelect_Click);
            this.Controls.Add(sndOptionsMenu);



            this.networkOptions = new Button();
            networkOptions.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 3);
            networkOptions.Height = buttonY;
            networkOptions.Width = buttonX;
            networkOptions.BackColor = color_blue;
            networkOptions.ForeColor = Color.Gold;
            networkOptions.Text = "Network Options";
            networkOptions.Click += new EventHandler(networkOptions_Click);
            this.Controls.Add(networkOptions);


            this.back = new Button();
            back.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 5);
            back.Height = buttonY;
            back.Width = buttonX;
            back.BackColor = color_blue;
            back.ForeColor = Color.Gold;
            back.Text = "Back";
            back.Click += new EventHandler(cancel_Click);
            this.Controls.Add(back);
        }
        private void OptionsMenu_Resize(object sender, System.EventArgs e)
        {
            title.Location = new Point(this.Width / 2 - buttonX / 4, this.Height / 16);
            sndOptionsMenu.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 4);
            networkOptions.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 3);
            back.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 8 * 5);
            clientx = this.Width;
            clienty = this.Height;
        }
        private void buttonSnd()
        {
            buttonClickSnd.controls.play();
        }
        private void sndOptionsSelect_Click(object sender, EventArgs e)
        {
            //Need to boot game here
            sndOptions.Show();
            //sndOptions.Location = this.Location;
            buttonSnd();
            //Application.Run(new FormMouseCR());
            this.Hide(); //Don't hide form so can do multiple

            // JD: 2/24, set the next showed window size, location, WindowState  
            if (this.WindowState == FormWindowState.Normal)
            {
                sndOptions.Location = this.Location;
                sndOptions.Size = this.Size;
            }
            else
            {
                sndOptions.Size = this.RestoreBounds.Size;
                sndOptions.Location = this.RestoreBounds.Location;
            }
            sndOptions.WindowState = this.WindowState;
        }
        private void networkOptions_Click(object sender, EventArgs e)
        {
            //Need to boot game here
            networkOptionsMenu.Show();
            //networkOptionsMenu.Location = this.Location;
            buttonSnd();
            //Application.Run(new FormMouseCR());
            this.Hide(); //Don't hide form so can do multiple

            // JD: 2/24, set the next showed window size, location, WindowState  
            if (this.WindowState == FormWindowState.Normal)
            {
                networkOptionsMenu.Location = this.Location;
                networkOptionsMenu.Size = this.Size;
            }
            else
            {
                networkOptionsMenu.Size = this.RestoreBounds.Size;
                networkOptionsMenu.Location = this.RestoreBounds.Location;
            }
            networkOptionsMenu.WindowState = this.WindowState;
        }
        private void cancel_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();

            Application.OpenForms.OfType<MainMenu>().First().Show();
            this.Hide();

            // JD: 2/24, set the next showed window size, location, WindowState  
            if (this.WindowState == FormWindowState.Normal)
            {
                Application.OpenForms.OfType<MainMenu>().First().Location = this.Location;
                Application.OpenForms.OfType<MainMenu>().First().Size = this.Size;
            }
            else
            {
                Application.OpenForms.OfType<MainMenu>().First().Size = this.RestoreBounds.Size;
                Application.OpenForms.OfType<MainMenu>().First().Location = this.RestoreBounds.Location;
            }
            Application.OpenForms.OfType<MainMenu>().First().WindowState = this.WindowState;
        }
        private void Options_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

    }
}
