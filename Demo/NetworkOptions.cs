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
    public partial class NetworkOptions : Form
    {
        public int buttonX = 150;
        public int buttonY = 40;
        public int clientx = 800;
        public int clienty = 600;

        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Button select;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label username;
        private System.Windows.Forms.Label remoteHostname;
        private System.Windows.Forms.Label remoteHostnamePort;
        private System.Windows.Forms.Label password;
        private System.Windows.Forms.TextBox usernameBox;
        private System.Windows.Forms.TextBox remoteHostnameBox;
        private System.Windows.Forms.TextBox remoteHostnamePortBox;
        private System.Windows.Forms.TextBox passwordBox;
        public static WindowsMediaPlayer buttonClickSnd;

        public NetworkOptions()
        {
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NetworkOptions_FormClosing);
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



            buttonClickSnd = new WindowsMediaPlayer();
            buttonClickSnd.URL = @"data/snd/misc/thunk.wav";

            this.Resize += new EventHandler(NetworkOptionsMenu_Resize);

            this.title = new Label();
            title.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 16);
            title.Text = "Network Options";
            title.AutoSize = true;
            title.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title.BackColor = Color.Transparent;
            title.ForeColor = Color.White;
            this.Controls.Add(title);

            this.username = new Label();
            username.Location = new Point(this.Width / 3 - buttonX / 16, this.Height / 6);
            username.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            username.Width = buttonX -15 ;
            this.AutoSize = false;
            username.BackColor = Color.Transparent;
            username.ForeColor = Color.Gold;
            username.Text = "User Name:";
            this.Controls.Add(username);

            this.usernameBox = new TextBox();
            usernameBox.Location = new Point(this.Width / 2 - buttonX / 16, this.Height / 6);
            usernameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            usernameBox.Width = buttonX;
            this.AutoSize = false;
            usernameBox.BackColor = color_blue;
            usernameBox.ForeColor = Color.Gold;
            usernameBox.Text = "";
            this.Controls.Add(usernameBox);

         
            this.remoteHostname = new Label();
            remoteHostname.Location = new Point((this.Width / 4 - buttonX / 16) - 4, this.Height / 12 * 3);
            remoteHostname.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            remoteHostname.Width = buttonX + 53;
            this.AutoSize = false;
            remoteHostname.BackColor = Color.Transparent;
            remoteHostname.ForeColor = Color.Gold;
            remoteHostname.Text = "Remote Hostname:";
            this.Controls.Add(remoteHostname);
         

            this.remoteHostnameBox = new TextBox();
            remoteHostnameBox.Location = new Point(this.Width / 2 - buttonX / 16, this.Height / 12 * 3);
            remoteHostnameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            remoteHostnameBox.Width = buttonX;
            remoteHostnameBox.Height = buttonX;
            this.AutoSize = false;
            remoteHostnameBox.BackColor = color_blue;
            remoteHostnameBox.ForeColor = Color.Gold;
            remoteHostnameBox.Text = "";
            this.Controls.Add(remoteHostnameBox);
           
            this.remoteHostnamePort = new Label();
            remoteHostnamePort.Location = new Point((this.Width / 4 - buttonX / 16) - 4, this.Height / 3);
            remoteHostnamePort.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            remoteHostnamePort.Width = buttonX + 53; 
            this.AutoSize = false;
            remoteHostnamePort.BackColor = Color.Transparent;
            remoteHostnamePort.ForeColor = Color.Gold;
            remoteHostnamePort.Text = "Remote Hostname:";
            this.Controls.Add(remoteHostnamePort);
           
            this.remoteHostnamePortBox = new TextBox();
            remoteHostnamePortBox.Location = new Point(this.Width / 2 - buttonX / 16, this.Height / 3);
            remoteHostnamePortBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            remoteHostnamePortBox.Width = buttonX;
            this.AutoSize = false;
            remoteHostnamePortBox.BackColor = color_blue;
            remoteHostnamePortBox.ForeColor = Color.Gold;
            remoteHostnamePortBox.Text = "";
            this.Controls.Add(remoteHostnamePortBox);
            
            this.password = new Label();
            password.Location = new Point(this.Width / 3 - buttonX / 16, this.Height / 12 * 5);
            password.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            password.Width = buttonX-15;
            this.AutoSize = false;
            password.BackColor = Color.Transparent;
            password.ForeColor = Color.Gold;
            password.Text = "   Password:";
            this.Controls.Add(password);
            
            this.passwordBox = new TextBox();
            passwordBox.Location = new Point(this.Width / 2 - buttonX / 16, this.Height / 12 * 5);
            passwordBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            passwordBox.Width = buttonX;
            this.AutoSize = false;
            passwordBox.BackColor = color_blue;
            passwordBox.ForeColor = Color.Gold;
            passwordBox.Text = "";
            this.Controls.Add(passwordBox);



            this.select = new Button();
            select.Location = new System.Drawing.Point(this.Width - buttonX - 40, this.Height / 10 * 8 - buttonY - 20);
            select.Height = buttonY;
            select.Width = buttonX;
            select.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            select.BackColor = color_blue;
            select.ForeColor = Color.Gold;
            select.Text = "OK";
            select.Click += new EventHandler(cancel_Click);
            this.Controls.Add(select);

            this.cancel = new Button();
            cancel.Location = new System.Drawing.Point(this.Width - buttonX - 40, this.Height / 10 * 9 - buttonY - 20);
            cancel.Height = buttonY;
            cancel.Width = buttonX;
            cancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            cancel.BackColor = color_blue;
            cancel.ForeColor = Color.Gold;
            cancel.Text = "Cancel";
            cancel.Click += new EventHandler(cancel_Click);
            this.Controls.Add(cancel);
        }
        private void NetworkOptionsMenu_Resize(object sender, System.EventArgs e)
        {
            title.Location = new Point(this.Width / 2 - buttonX / 2, this.Height / 16);
            username.Location = new Point(this.Width / 3 - buttonX / 16, this.Height / 6);
            usernameBox.Location = new Point(this.Width / 2 - buttonX / 16, this.Height / 6);
            remoteHostname.Location = new Point((this.Width / 4 - buttonX / 16) - 4, this.Height / 12 * 3);
            remoteHostnameBox.Location = new Point(this.Width / 2 - buttonX / 16, this.Height / 12 * 3);
            remoteHostnamePort.Location = new Point((this.Width / 4 - buttonX / 16) - 4, this.Height / 3);
            remoteHostnamePortBox.Location = new Point(this.Width / 2 - buttonX / 16, this.Height / 3);
            password.Location = new Point(this.Width / 3 - buttonX / 16, this.Height / 12 * 5);
            passwordBox.Location = new Point(this.Width / 2 - buttonX / 16, this.Height / 12 * 5);
            clientx = this.Width;
            clienty = this.Height;
        }
        private void cancel_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();

            Application.OpenForms.OfType<Options>().First().Show();
            this.Hide();

        }
        private void NetworkOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }


    }

}
