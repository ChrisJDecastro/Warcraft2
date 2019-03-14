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
    public partial class inGame_menu : Form
    {
        // the key state will be past to XNA_Game
        public static bool[] arrowKeysState = new bool[4];
        Label goldLabel = new Label();
        Label treeLabel = new Label();
        Label meatLabel = new Label();
        Form small_inGame_menu;
        public int buttonX = 150;
        public int buttonY = 40;
        public bool console = false;

        Bitmap pic;
        PictureBox goldPic = new PictureBox();
        PictureBox treePic = new PictureBox();
        PictureBox meatPic = new PictureBox();

        public Button mainMenuButton;
        public int endGameFlag = 0;
        XNA_InGame game;

        Form consoleForm;
        public TextBox inputBox;
        TextBox chatBox;
        bool inputEnter = false;
        bool chatShowing = false; //flag for whether chatbox is showing (aka allow input)

        public inGame_menu(XNA_InGame game)
        {
            InitializeComponent();
            this.game = game;
            this.Load += inGame_menu_Load;
            this.FormClosing += new FormClosingEventHandler(this.inGame_menu_FormClosing);

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(inGame_menu_KeyDown);
            this.KeyUp += new KeyEventHandler(inGame_menu_KeyUp);
            Bitmap backGround = new Bitmap(@"data\img\Texture.png");
            this.BackgroundImage = backGround;
            Bitmap color = new Bitmap(@"data\img\Colors.png");
            System.Drawing.Color color_blue = color.GetPixel(1, 1);

            mainMenuButton = new Button();
            mainMenuButton.Location = new Point(35, 10);
            mainMenuButton.Height = 30;
            mainMenuButton.Width = 60;
            mainMenuButton.BackColor = color_blue;
            mainMenuButton.ForeColor = Color.Gold;
            mainMenuButton.Text = "Main Menu";
            mainMenuButton.MouseClick += new MouseEventHandler(mainMenuButton_Click);
            mainMenuButton.TabStop = false;

            // Resource

            //gold icon
            Bitmap miniIcon = new Bitmap(@"data/img/MiniIcons.png");
            pic = miniIcon.Clone(new Rectangle(0, 0, miniIcon.Size.Width, miniIcon.Size.Height / 4), miniIcon.PixelFormat);
            goldPic.Image = pic;
            goldPic.BackColor = Color.Transparent;
            goldPic.Location = new Point(this.Width / 5 * 2 - 20, 10);
            goldPic.SizeMode = PictureBoxSizeMode.StretchImage;
            goldPic.Size = new Size(20, 20);
            this.Controls.Add(goldPic);

            goldLabel.Location = new Point(this.Width / 5 * 2, 10);
            goldLabel.Text = "Gold = 300"; // how to assign value: goldLabel.Text = "Gold  = "  goldNumber;
            goldLabel.AutoSize = true;
            goldLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            goldLabel.BackgroundImage = backGround;
            goldLabel.ForeColor = Color.Gold;
            //goldLabel.Anchor = (AnchorStyles.Right); // this will change label location
            this.Controls.Add(goldLabel);

            //treePic icon
            pic = miniIcon.Clone(new Rectangle(0, miniIcon.Size.Height / 4, miniIcon.Size.Width, miniIcon.Size.Height / 4), miniIcon.PixelFormat);
            treePic.Image = pic;
            treePic.BackColor = Color.Transparent;
            treePic.Location = new Point(this.Width / 5 * 3 - 20, 10);
            treePic.SizeMode = PictureBoxSizeMode.StretchImage;
            treePic.Size = new Size(20, 20);
            this.Controls.Add(treePic);

            treeLabel.Location = new Point(this.Width / 5 * 3, 10);
            treeLabel.Text = "Tree  = 300";
            treeLabel.AutoSize = true;
            treeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            treeLabel.BackgroundImage = backGround;
            treeLabel.ForeColor = Color.Gold;
            //treeLabel.Anchor = (AnchorStyles.Right);
            this.Controls.Add(treeLabel);

            //meatPic icon
            pic = miniIcon.Clone(new Rectangle(0, miniIcon.Size.Height / 2, miniIcon.Size.Width, miniIcon.Size.Height / 4), miniIcon.PixelFormat);
            meatPic.Image = pic;
            meatPic.BackColor = Color.Transparent;
            meatPic.Location = new Point(this.Width / 5 * 4 - 20, 10);
            meatPic.SizeMode = PictureBoxSizeMode.StretchImage;
            meatPic.Size = new Size(20, 20);
            this.Controls.Add(meatPic);

            meatLabel.Location = new Point(this.Width / 5 * 4, 10);
            meatLabel.Text = "Meat  = 300/300";
            meatLabel.AutoSize = true;
            meatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            meatLabel.BackgroundImage = backGround;
            meatLabel.ForeColor = Color.Gold;
            //meatLabel.Anchor = (AnchorStyles.Right);
            this.Controls.Add(meatLabel);

            small_inGame_menu = new Form();
            small_inGame_menu.Location = new Point(this.Width/4, this.Height/4);
            small_inGame_menu.Size = new Size(this.Width / 2, this.Height / 2);
            small_inGame_menu.BackgroundImage = backGround;

            Button backButton = new Button();
            backButton.Location = new Point(small_inGame_menu.Width / 2 - buttonX / 2, small_inGame_menu.Height / 4 - buttonY /2);
            backButton.Height = buttonY;
            backButton.Width = buttonX;
            backButton.BackColor = color_blue;
            backButton.ForeColor = Color.Gold;
            backButton.Text = "Back";
            backButton.Click += new EventHandler(backButton_Click);
            small_inGame_menu.Controls.Add(backButton);

            //go back to mainMenu button
            Button goBackMainMenuButton = new Button();
            goBackMainMenuButton.Location = new Point(small_inGame_menu.Width / 2 - buttonX / 2, small_inGame_menu.Height / 2 - buttonY / 2);
            goBackMainMenuButton.Height = buttonY;
            goBackMainMenuButton.Width = buttonX;
            goBackMainMenuButton.BackColor = color_blue;
            goBackMainMenuButton.ForeColor = Color.Gold;
            goBackMainMenuButton.Text = "Main Menu";
            goBackMainMenuButton.Click += new EventHandler(goBackMainMenuButton_Click);
            small_inGame_menu.Controls.Add(goBackMainMenuButton);

            //Exit button
            Button exitButton = new Button();
            exitButton.Location = new Point(small_inGame_menu.Width / 2 - buttonX / 2, small_inGame_menu.Height / 4 * 3 - buttonY / 2);
            exitButton.Height = buttonY;
            exitButton.Width = buttonX;
            exitButton.BackColor = color_blue;
            exitButton.ForeColor = Color.Gold;
            exitButton.Text = "Exit";
            exitButton.Click += new EventHandler(exitButton_Click);
            small_inGame_menu.Controls.Add(exitButton);

            small_inGame_menu.TopLevel = false;
            small_inGame_menu.FormBorderStyle = FormBorderStyle.None;
            this.Controls.Add(small_inGame_menu);
            small_inGame_menu.Show();
            small_inGame_menu.Hide();



            consoleForm = new Form();
            //consoleForm.g = this.game;
            consoleForm.TopLevel = false;
            //consoleForm.Parent = this;
            consoleForm.Location = new Point(this.Width / 4, this.Height / 4);
            consoleForm.Size = new Size(this.Width / 2, this.Height / 2);
            consoleForm.BackgroundImage = backGround;

            this.Controls.Add(consoleForm);

            consoleForm.Show();
            consoleForm.Hide();

            chatBox = new TextBox();
            this.Controls.Add(chatBox);
            chatBox.Location = new System.Drawing.Point(10, 390 + 125 + 10);
            chatBox.Size = new System.Drawing.Size(125, 125);
            //inputBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            chatBox.BackColor = Color.Black;
            chatBox.ForeColor = Color.Gold;
            chatBox.BorderStyle = BorderStyle.Fixed3D;
            chatBox.Multiline = true;
            chatBox.ScrollBars = ScrollBars.Both;
            chatBox.ReadOnly = true;
            //chatBox.TextChanged += new EventHandler(chatBox_TextChanged);

            inputBox = new TextBox();
            this.Controls.Add(inputBox);
            //inputBox.Location = new System.Drawing.Point(10, 390 + 125 + 125 + 10 + 10);
            //inputBox.Location = new System.Drawing.Point(10, 10); // JD: 3/11: change inputBox location to the top
            
            inputBox.Size = new System.Drawing.Size(125, 18);
            inputBox.Location = new System.Drawing.Point(this.Width + 25 - inputBox.Width, 10); // JD: 3/11: change inputBox location to the top right
            inputBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            //inputBox.AutoSize = true;
            //inputBox.Size = new System.Drawing.Size(this.Width - (150+20), 20);
            inputBox.BackColor = Color.Black;
            inputBox.ForeColor = Color.Gold;
            inputBox.BorderStyle = BorderStyle.Fixed3D;
            inputBox.TextChanged += new EventHandler(inputBox_TextChanged);
            inputBox.Hide(); //hide by default

            this.Controls.Add(mainMenuButton); // JD: 3/11: put it here, so the button doesn't block the inputbox
        }
        
        public void disableButton()
        {
            /*foreach(Control ctrl in this.Controls)
            { 
                b.Enabled = false;
            }*/
            mainMenuButton.Enabled = false;
        }

        public void enableButton()
        {
            /*foreach(Button b in this.Controls)
            {
                b.Enabled = true;
            }*/
            mainMenuButton.Enabled = true;
        }

        public void labelUpdate(string type, int amount, int amount2)
        { //function for updating labels
            //amount2 is the max value of food, should be 0 for all other cases (as it is unused)
            if (type == "gold")
            {
                this.goldLabel.Text = amount.ToString();
            }
            else if (type == "lumber")
            {
                this.treeLabel.Text = amount.ToString();
            }
            else if (type == "food")
            {
                this.meatLabel.Text = amount.ToString() + "/" + amount2.ToString();
            }

        }

        private void inGame_menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Application.Exit();
            game.Exit();
        }
        private void inGame_menu_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            foreach (Control control in this.Controls)
            {
                control.PreviewKeyDown += new PreviewKeyDownEventHandler(control_PreviewKeyDown);
            }

        }
        public void keyPreview_Load()
        {//reference from load function above, created to be called anytime later after load
            foreach (Control control in this.Controls)
            {
                control.PreviewKeyDown += new PreviewKeyDownEventHandler(control_PreviewKeyDown);
                foreach (Control innerCtrl in control.Controls)
                { //add for inner controls aka buttons as well
                    control.PreviewKeyDown += new PreviewKeyDownEventHandler(control_PreviewKeyDown);
                }
            }
        } //Doesn't work

        //reference from https://stackoverflow.com/questions/1318236/how-to-disable-navigation-on-winform-with-arrows-in-c
        //this function prevents the arrow keys to control the selection of buttons
        void control_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.C)
            {
                e.IsInputKey = true;
                //e.IsInputKey = false; //tried this, doesn't work
            }
        }

        private void inGame_menu_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Up:
                    //MessageBox.Show("up");
                    arrowKeysState[0] = false;
                    break;
                case System.Windows.Forms.Keys.Down:
                    //MessageBox.Show("right");
                    arrowKeysState[1] = false;
                    break;
                case System.Windows.Forms.Keys.Left:
                    //MessageBox.Show("d");
                    arrowKeysState[2] = false;
                    break;
                case System.Windows.Forms.Keys.Right:
                    //MessageBox.Show("u");
                    arrowKeysState[3] = false;
                    break;
                case System.Windows.Forms.Keys.Enter:
                    {
                        if (chatShowing == false)
                        {
                            chatShowing = true; //set to true so we can type input
                            inputBox.Clear(); //clear any run on text that was typed when inputbox is not shown
                            inputBox.Show(); //show input box
                            inputBox.Focus(); //focus onto inputBox control

                        }
                        else chatShowing = false;
                    }
                    break;
                case System.Windows.Forms.Keys.C:
                    if (console == true && chatShowing == false) {
                        DebugConsole.createDebugConsole(game,this);
                    }
                    console = false;
                    break;
                case System.Windows.Forms.Keys.D0:
                    if (chatShowing == false)
                    {
                        if (game.thisMap.chosenUnits.Count() == 0) //no unit selected so call selected group
                        {
                            game.thisMap.unitSelectGroup(0, game.currentUnits);
                        }
                        else //if (game.thisMap.chosenUnits.Count() <= 9) //we do have units selected so group them (only if less than 9 units)
                        {
                            game.thisMap.unitGroup(0, game.currentUnits);
                        }
                    }
                    break;
                case System.Windows.Forms.Keys.D1:
                    if (chatShowing == false)
                    {
                        if (game.thisMap.chosenUnits.Count() == 0) //no unit selected so call selected group
                        {
                            game.thisMap.unitSelectGroup(1, game.currentUnits);
                        }
                        else //if (game.thisMap.chosenUnits.Count() <= 9) //we do have units selected so group them (only if less than 9 units)
                        {
                            game.thisMap.unitGroup(1, game.currentUnits);
                        }
                    }
                    break;
                case System.Windows.Forms.Keys.D2:
                    if (chatShowing == false)
                    {
                        if (game.thisMap.chosenUnits.Count() == 0) //no unit selected so call selected group
                        {
                            game.thisMap.unitSelectGroup(2, game.currentUnits);
                        }
                        else //if (game.thisMap.chosenUnits.Count() <= 9) //we do have units selected so group them (only if less than 9 units)
                        {
                            game.thisMap.unitGroup(2, game.currentUnits);
                        }
                    }
                    break;
                case System.Windows.Forms.Keys.D3:
                    if (chatShowing == false)
                    {
                        if (game.thisMap.chosenUnits.Count() == 0) //no unit selected so call selected group
                        {
                            game.thisMap.unitSelectGroup(3, game.currentUnits);
                        }
                        else //if (game.thisMap.chosenUnits.Count() <= 9) //we do have units selected so group them (only if less than 9 units)
                        {
                            game.thisMap.unitGroup(3, game.currentUnits);
                        }
                    }
                    break;
                case System.Windows.Forms.Keys.D4:
                    if (chatShowing == false)
                    {
                        if (game.thisMap.chosenUnits.Count() == 0) //no unit selected so call selected group
                        {
                            game.thisMap.unitSelectGroup(4, game.currentUnits);
                        }
                        else //if (game.thisMap.chosenUnits.Count() <= 9) //we do have units selected so group them (only if less than 9 units)
                        {
                            game.thisMap.unitGroup(4, game.currentUnits);
                        }
                    }
                    break;
                case System.Windows.Forms.Keys.D5:
                    if (chatShowing == false)
                    {
                        if (game.thisMap.chosenUnits.Count() == 0) //no unit selected so call selected group
                        {
                            game.thisMap.unitSelectGroup(5, game.currentUnits);
                        }
                        else //if (game.thisMap.chosenUnits.Count() <= 9) //we do have units selected so group them (only if less than 9 units)
                        {
                            game.thisMap.unitGroup(5, game.currentUnits);
                        }
                    }
                    break;
                case System.Windows.Forms.Keys.D6:
                    if (chatShowing == false)
                    {
                        if (game.thisMap.chosenUnits.Count() == 0) //no unit selected so call selected group
                        {
                            game.thisMap.unitSelectGroup(6, game.currentUnits);
                        }
                        else //if (game.thisMap.chosenUnits.Count() <= 9) //we do have units selected so group them (only if less than 9 units)
                        {
                            game.thisMap.unitGroup(6, game.currentUnits);
                        }
                    }
                    break;
                case System.Windows.Forms.Keys.D7:
                    if (chatShowing == false)
                    {
                        if (game.thisMap.chosenUnits.Count() == 0) //no unit selected so call selected group
                        {
                            game.thisMap.unitSelectGroup(7, game.currentUnits);
                        }
                        else //if (game.thisMap.chosenUnits.Count() <= 9) //we do have units selected so group them (only if less than 9 units)
                        {
                            game.thisMap.unitGroup(7, game.currentUnits);
                        }
                    }
                    break;
                case System.Windows.Forms.Keys.D8:
                    if (chatShowing == false)
                    {
                        if (game.thisMap.chosenUnits.Count() == 0) //no unit selected so call selected group
                        {
                            game.thisMap.unitSelectGroup(8, game.currentUnits);
                        }
                        else //if (game.thisMap.chosenUnits.Count() <= 9) //we do have units selected so group them (only if less than 9 units)
                        {
                            game.thisMap.unitGroup(8, game.currentUnits);
                        }
                    }
                    break;
                case System.Windows.Forms.Keys.D9:
                    if (chatShowing == false)
                    {
                        if (game.thisMap.chosenUnits.Count() == 0) //no unit selected so call selected group
                        {
                            game.thisMap.unitSelectGroup(9, game.currentUnits);
                        }
                        else //if (game.thisMap.chosenUnits.Count() <= 9) //we do have units selected so group them (only if less than 9 units)
                        {
                            game.thisMap.unitGroup(9, game.currentUnits);
                        }
                    }
                    break;
            }
            e.Handled = true; //prevents passing keyboard strokes to other controls in form

        }
        //reference from https://stackoverflow.com/questions/14971174/use-of-arrow-keys
        private void inGame_menu_KeyDown(object sender, KeyEventArgs e)
        {
            //arrowKeysState[0] = arrowKeysState[1] = arrowKeysState[2] = arrowKeysState[3] = false;
            //removing above made it a bit smoother?
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Up:
                    //MessageBox.Show("up");
                    arrowKeysState[0] = true;
                    break;
                case System.Windows.Forms.Keys.Down:
                    //MessageBox.Show("right");
                    arrowKeysState[1] = true;
                    break;
                case System.Windows.Forms.Keys.Left:
                    //MessageBox.Show("d");
                    arrowKeysState[2] = true;
                    break;
                case System.Windows.Forms.Keys.Right:
                    //MessageBox.Show("u");
                    arrowKeysState[3] = true;
                    break;
                case System.Windows.Forms.Keys.C:
                    if (chatShowing == false) console = true;
                    break;
                case System.Windows.Forms.Keys.Enter:
                    {
                        if (chatShowing)
                        {
                            inputEnter = true;
                            //chatShowing = false;
                            chatBox_TextChanged(inputEnter);
                            inputBox.Hide(); //hide input box after using
                            this.ActiveControl = null; //set to null to return control to game
                            inputBox.Size = new System.Drawing.Size(125, 18);
                        }
                    }
                    
                    break;
            }

            e.Handled = true; //prevents passing keyboard strokes to other controls in form

        }

        public bool[] getArrowKeysState()
        {
            return arrowKeysState;
        }


        private void mainMenuButton_Click(object sender, MouseEventArgs e)
        {
            // JD 2/3/2019: add default window position and size into Demo.Properties to save current window position and size, reference from https://www.codeproject.com/Articles/15013/Windows-Forms-User-Settings-in-C
            /*Properties.Settings.Default.WindowLocation = this.Location;
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.WindowSize = this.Size;
                Properties.Settings.Default.WindowMax = false;
            }
            else
            {
                Properties.Settings.Default.WindowMax = true;
                Properties.Settings.Default.WindowSize = this.RestoreBounds.Size;
            }
            Properties.Settings.Default.Save();

            WindowsMediaPlayer buttonClick = new WindowsMediaPlayer();
            buttonClick.URL = @"data/snd/misc/thunk.wav";
            buttonClick.controls.play();

            //new MainMenu().Show();
            //this.Hide();
            */
            
            //this.Close();

            //SplashScreen.mainMenuForm.Show();
            MainMenu.buttonClickSnd.controls.play();
            small_inGame_menu.Show();
            this.ActiveControl = null;//prevent buttons block the input
        }


        // need to stop the game here, so it can go back to Main Menu to start a new game
        private void goBackMainMenuButton_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();

            //Application.Run(new MainMenu());
            //Application.OpenForms.OfType<MainMenu>().First().Show();

            //new MainMenu().Show();
            //SplashScreen.mainMenuForm.Show();

            //this.Hide();
            //this.Dispose();
            //this.Close();
            //this.endGameFlag = 1;
            //Thread.CurrentThread.Interrupt();
            this.Close(); //stops the game

        }
        private void exitButton_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();
            Application.Exit();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();
            small_inGame_menu.Hide();
            this.ActiveControl = null;//prevent buttons block the input
        }
        private void chatBox_TextChanged(bool inputEnter)
        {
            if (inputEnter)
            {
                if (!string.IsNullOrWhiteSpace(inputBox.Text)) //if we didn't enter null aka empty string with spaces
                {
                    //chatBox.Text = chatBox.Text + "\nYou: " + inputBox.Text + Environment.NewLine;
                    /*if (inputBox.Text == "Hello" | inputBox.Text == "hello" | inputBox.Text == "hi" | inputBox.Text == "Hi")
                    {
                        for (int i = 2; i <= SplashScreen.mapUnits.numPlayer[SplashScreen.mapObject.mapChoice]; i++)
                        {
                            chatBox.Text = chatBox.Text + "\r\nPlayer" + i + ": Hello" + Environment.NewLine;
                        }
                    }*/

                    //Cheat Codes
                    if (inputBox.Text == "Glittering prizes") //10k gold, 5k lumber for ALL players
                    {
                        for (int i = 1; i <= game.thisMap.numPlayers; i++) //iterate through all players and give gold
                        {
                            game.thisMap.hackResource("lumber", 5000, i);
                            game.thisMap.hackResource("gold", 10000, i);
                        }
                    }
                    else if (inputBox.Text == "On screen") //removes fog of war (turn back on also implemented)
                    {
                        if (game.thisMap.renderFogFlag == 1)
                        {
                            game.thisMap.renderFogFlag = 0;
                            game.thisMap.turnOffFog(game.currentUnits);
                        }
                        else //turn fog of war on
                        {
                            game.thisMap.renderFogFlag = 1;
                        }
                    }
                    else if (inputBox.Text == "Make it so") //instant building on (NOTE: instant does not affect buildings already in construction)
                    {
                        if (game.thisMap.DConstructTime == game.thisMap.DConstructTimeDefault) //init construct time
                        { //turn to instant
                            game.thisMap.DConstructTime = 1;
                        }
                        else game.thisMap.DConstructTime = game.thisMap.DConstructTimeDefault; //turn off instant
                    }
                    else if (inputBox.Text == "Hatchet") //one tree chop to fell tree
                    {
                        if (game.thisMap.DChopConstant == 1) //default chop
                        { //turn to instant
                            game.thisMap.DChopConstant = 4; //set to instant fell tree after chop
                        }
                        else game.thisMap.DChopConstant = 1; //set back to default chop
                    }
                    else chatBox.Text = chatBox.Text + "\nYou: " + inputBox.Text + Environment.NewLine; //only show text if not cheatcode

                    inputBox.Clear();
                    inputEnter = false;
                    this.ActiveControl = this.mainMenuButton;
                    chatBox.Refresh();
                }
            }

        }
        private void inputBox_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(inputBox.Text, inputBox.Font);
            if(size.Width > 125)
                inputBox.Width = size.Width;
            inputBox.Height = size.Height;
        }
    }

}

