using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
    public partial class UnitDatas : Form
    {
        public int buttonX = 45;  //width
        public int buttonY = 38;  //height

        Label unitName = new Label();
        Label unitHealth = new Label();
        Label armor = new Label();
        Label damage = new Label();
        Label range = new Label();
        Label sight = new Label();
        Label speed = new Label();
        Label progress = new Label(); //for showing progress, goldmine data, etc.
        PictureBox unitPic = new PictureBox();
        PictureBox healthBar = new PictureBox();
        PictureBox fullHealthBar = new PictureBox();

        bool displayMulti = false; // flag to hold if we 'were' displaying multi unit

        //multiple unit selection variables
        ImageList imageList = new ImageList();
        ImageList healthBarList = new ImageList();
        string[] dummy = new string[] { "peasant", "footman", "footman", "footman", "footman", "footman", "footman", "footman" };
        int[] dummyIndex = new int[] { 0, 1,1,1,1,1,1,1 }; //will unittype of all dummy units
        string[] dummyHealthPoint = new string[] { "30 / 30", "60 / 60", "60 / 60", "60 / 60", "60 / 60", "60 / 60", "60 / 60", "60 / 60" };
        Label[] dummyUnitHealth = new Label[9];
        //PictureBox[] dummyHealthBar = new PictureBox[9];
        Rectangle[] dummyHealthBar = new Rectangle[9];
        int index, x = 0, y = 0;
        Image img;
        Graphics gPtr;
        SolidBrush redSBrush; //for drawing red rectangles
        SolidBrush greenSBrush; //for drawing green rectangles
        SolidBrush whiteSBrush;

        int[] allUnitIcons;


        public UnitDatas()
        {
            InitializeComponent();
            this.BackgroundImage = new Bitmap(@"data\img\Texture.png");
            redSBrush = new SolidBrush(Color.Red);
            greenSBrush = new SolidBrush(Color.Green);
            whiteSBrush = new SolidBrush(Color.White);
            //this.gPtr = Graphics.FromHwnd(this.Handle);
            //g = Graphics.FromHwnd(this.Handle);
            loadUnitIcons();
            unitData();
            hide_data();
        }

        private void loadUnitIcons()
        { //to find all unit icon img index before hand (so less access later)
            int maxLength = SplashScreen.allUnitNames.Length;
            this.allUnitIcons = new int[maxLength];
            for (int i = 0; i < maxLength; i++)
            {
                this.allUnitIcons[i] = SplashScreen.icons.findIcon(lowerCaseTranslation(i));
            }
        }

        public void hide_data()
        {
            unitName.Hide();
            unitHealth.Hide();
            healthBar.Hide();
            fullHealthBar.Hide();
            armor.Hide();
            damage.Hide();
            range.Hide();
            sight.Hide();
            speed.Hide(); 
            unitPic.Hide();
            progress.Hide();
            hide_multiData();
            
            //not sure how to hide show_multiData()
        }

        public void show_data(Unit curUnit)
        {
            refresh_data(curUnit); //refresh the data before showing

            if (this.displayMulti) //we were displaying multidata
            { //must hide old data as we are switching from multi to single
                hide_data();
            }
            this.displayMulti = false;
            //Not sure if the showing logic is correct, might have to do with the owner of the unit instead

            //Always show name and picture for all units
            unitName.Show();
            unitPic.Show();
            
            if (nameTranslation(curUnit.unitType) == "GoldMine" || curUnit.inUse == 1) //unit is a gold mine
            {//display gold
                //TODO
                progress.Show();
            }
            else
            { //other units always show hp
                unitHealth.Show();
                fullHealthBar.Show(); // show first so other health bar overlaps this one
                healthBar.Show();
                healthBar.BringToFront(); //bring to front so it overlaps other
            }
            //Only show battle data if it's an unit that can fight
            if ((curUnit.basicDamage != 0 || curUnit.pierceDamage != 0) && curUnit.owner == SplashScreen.mapUnits.playerChoice) //a unit that can fight and it is our unit
            {
                armor.Show();
                damage.Show();
                range.Show();
                sight.Show();
                speed.Show();
            }  
        }

        public void load_multiData(tempMap gameData, List<Unit>[] allUnits)
        {
            //TODO, load all selected unit data into the dummy string
            //this.imageList = new ImageList();
            hide_data(); //hide all old data

            this.imageList.Images.Clear();
            int numElements = gameData.chosenUnits.Count();
            this.dummy = new string[numElements];
            this.dummyHealthPoint = new string[numElements];
            this.dummyUnitHealth = new Label[numElements];
            //this.dummyHealthBar = new PictureBox[numElements];
            this.dummyHealthBar = new Rectangle[numElements];
            this.dummyIndex = new int[numElements];
            int index = 0;
            //for (int i = 0; i < gameData.chosenUnits.Count(); i++)
            foreach (unitPtr i in gameData.chosenUnits) //iterate through all chosen units
            {
                Unit temp = allUnits[i.owner][i.index];
                string tempStr = lowerCaseTranslation(temp.unitType);
                string healthStr;
                if (temp.curHitPoint <= 0) healthStr = "0 / " + temp.hitPoint.ToString();
                else healthStr = temp.curHitPoint.ToString() + " / " + temp.hitPoint.ToString();
                this.dummy[index] = tempStr;
                this.dummyIndex[index] = temp.unitType;
                this.dummyHealthPoint[index] = healthStr;
                //this.dummyHealthBar[index] = new PictureBox();
                this.dummyHealthBar[index] = new Rectangle();
                this.dummyHealthBar[index].Size = new Size((int)(((float)temp.curHitPoint / (float)temp.hitPoint) * (float)(buttonX-5)), 5);
                //this.dummyHealthBar[index].BackColor = Color.Green;
                index++;
            }
        }

        public void hide_multiData()
        {
            //if (this.displayMulti == true) this.gPtr.Dispose();
            this.Refresh();
            this.displayMulti = false;
            this.imageList.Images.Clear();
            //g.Clear(Color.Transparent);
            //g = Graphics.FromHwnd(this.Handle);
            foreach (Control i in this.Controls) //hide all controls
            {
                i.Hide();
            }

            for (int i = 0; i < this.dummy.Length; i++)
            {
                this.Controls.Remove(dummyUnitHealth[i]);
                //this.Controls.Remove(dummyHealthBar[i]);
            }
        }

        public void show_multiData()
        {
            //refresh_data(curUnit);
            this.Refresh(); //refresh before drawing again

            Graphics g = Graphics.FromHwnd(this.Handle);
            //g.Clear(Color.Transparent); //temp
            this.gPtr = g;

            this.displayMulti = true;

            for (int i = 0; i < dummy.Length; i++)
            {
                //unit images
                //index = SplashScreen.icons.findIcon(dummy[i]);
                index = allUnitIcons[dummyIndex[i]];
                img = SplashScreen.icons.tiles[index];
                imageList.Images.Add(dummy[i], img);
                imageList.ImageSize = new Size(buttonX - 5, buttonY);

                //unit health point
                /*dummyUnitHealth[i] = new Label();
                dummyUnitHealth[i].Size = new Size(buttonX, 15);
                dummyUnitHealth[i].Text = dummyHealthPoint[i];
                dummyUnitHealth[i].BackColor = Color.Transparent;
                dummyUnitHealth[i].ForeColor = Color.White;*/

                //unit health bar
                //dummyHealthBar[i] = new PictureBox();
                //dummyHealthBar[i].BackColor = Color.Green;
                //dummyHealthBar[i].BackColor = Color.Red; 
                //dummyHealthBar[i].Size = new Size(buttonX - 5, 5);  //already updated in loadmulti 
                //PictureBox redHealthBar = new PictureBox();
                //redHealthBar.BackColor = Color.Red;
                //redHealthBar.Size = new Size(buttonX - 5, 5);
                

                //Set location of each unit 
                if (i == 0 || i == 3 || i == 6) { x = 0; }
                else { x = x + 45; }
                if (i < 3) { y = 0; }
                else if (i < 6) { y = 65; }
                else { y = 130; }
                
                //ummyUnitHealth[i].Location = new Point(x, y + buttonY + 5);

                Point strLocation = new Point(x, y + buttonY + 5);
                g.DrawString(dummyHealthPoint[i], this.Font, this.whiteSBrush, strLocation);

                //redHealthBar.Location = new Point(x, y + buttonY);
                dummyHealthBar[i].Location = new Point(x, y + buttonY);
                imageList.Draw(g, x, y, i);

                Rectangle redHealthBarRect = new Rectangle(new Point(x,y+buttonY),new Size(buttonX - 5, 5));
                g.FillRectangle(redSBrush, redHealthBarRect); //draw red bar first

                g.FillRectangle(greenSBrush, dummyHealthBar[i]); //draw green bar over red bar

                //this.Controls.Add(redHealthBar);
                //this.Controls.Add(dummyUnitHealth[i]);
                //this.Controls.Add(dummyHealthBar[i]);

                //dummyUnitHealth[i].Show();
                //dummyHealthBar[i].Show();
                //dummyHealthBar[i].BringToFront();
                //dummyUnitHealth[i].BringToFront();
            }
        }

        public void refresh_health(Unit curUnit)
        {//function for refreshing selected unit's health during battle
            if (this.displayMulti == false)
            { //if previous mode is single display mode, simply refresh hp
                if (curUnit.curHitPoint > 0) unitHealth.Text = curUnit.curHitPoint.ToString() + " / " + curUnit.hitPoint.ToString();
                else unitHealth.Text = "0 / " + curUnit.hitPoint.ToString();
                healthBar.Size = new Size((int)(((float)curUnit.curHitPoint / (float)curUnit.hitPoint) * (float)buttonX), 5); //get ratio of depleted health
                unitHealth.Show();
                fullHealthBar.Show(); // show first so other health bar overlaps this one
                healthBar.Show();
                healthBar.BringToFront(); //bring to front so it overlaps other
            }
            else //previous mode was multi display mode
            {//so need to call entire function
                show_data(curUnit);
            }
        }

        public void refresh_healthAll(ref tempMap gameData, List<Unit>[] allUnits)
        {//function for refreshing selected unit's health during battle

            
            if (gameData.chosenUnits.Count() == this.dummy.Length) //if we still have same amount of selected units (meaning none died)
            {
                int curUnitIndex = 0;
                foreach (unitPtr i in gameData.chosenUnits)
                {
                    Unit curUnit = allUnits[i.owner][i.index];
                    if (curUnit.curHitPoint > 0) dummyHealthPoint[curUnitIndex] = curUnit.curHitPoint.ToString() + " / " + curUnit.hitPoint.ToString();
                    else dummyHealthPoint[curUnitIndex] = "0 / " + curUnit.hitPoint.ToString();
                    dummyHealthBar[curUnitIndex].Size = new Size((int)(((float)curUnit.curHitPoint / (float)curUnit.hitPoint) * (float)(buttonX - 5)), 5); //get ratio of depleted health
                    curUnitIndex++;
                }
                show_multiData();
            }
            else //number of selected units changed, meaning a unit probably died
            {
                hide_multiData(); //hide the old data
                load_multiData(gameData, allUnits); //reload data
                show_multiData(); //show new data
            }
            
        }

        public void refreshProgress(Unit curUnit)
        {
            if (nameTranslation(curUnit.unitType) == "GoldMine")
            {
                if (curUnit.gold > 0)
                {
                    progress.Text = "Gold: " + curUnit.gold.ToString();
                }
                else
                {
                    progress.Text = "Gold: 0";
                }
            }
            else 
            {
                int ret = curUnit.getProgress();
                if (ret != -1) //we have progress
                {
                    progress.Text = "Progress: " + ret.ToString() +"%";
                }
                else
                {
                    progress.Text = "";
                }
            }
        }

        private void refresh_data(Unit curUnit)
        { //refresh data based on passed in unit

            unitName.Text = this.nameTranslation(curUnit.unitType);
            if (curUnit.curHitPoint > 0) unitHealth.Text = curUnit.curHitPoint.ToString() + " / " + curUnit.hitPoint.ToString();
            else unitHealth.Text = "0 / " + curUnit.hitPoint.ToString();
            healthBar.Size = new Size((int)(((float)curUnit.curHitPoint / (float)curUnit.hitPoint) * (float)buttonX), 5); //get ratio of depleted health
            armor.Text = "Armor: " + curUnit.armor.ToString();
            if (curUnit.basicDamage != 0 || curUnit.pierceDamage != 0) //a unit that can fight
            { //Show everything
                //Not sure how these values are calculated so defaulted to these
                damage.Text = "Damage: 1-" + (curUnit.basicDamage + curUnit.pierceDamage).ToString();
            }
            else damage.Text = "Damage: 0";
            range.Text = "Range: " + curUnit.range.ToString();
            sight.Text = "Sight: " + curUnit.sight.ToString();
            speed.Text = "Speed: " + curUnit.defaultSpeed.ToString();

            //int index = SplashScreen.icons.findIcon(lowerCaseTranslation(curUnit.unitType));
            int index = this.allUnitIcons[curUnit.unitType];
            unitPic.Image = SplashScreen.icons.tiles[index];
            unitPic.Refresh();

            refreshProgress(curUnit);
            
        }

        public void unitData()
        {
            /*unitName.Text = "Peasant";
            unitHealth.Text = "30 / 30";
            armor.Text = "Armor: 0";
            damage.Text = "Damage: 1-5";
            range.Text = "Range: 1";
            sight.Text = "Sight: 4";
            speed.Text = "Speed: 10";*/

            unitName.Size = new Size(60, 15);
            //unitHealth.Size = new Size(50, 15);
            unitHealth.Size = new Size(100, 15); //changed size so it shows long (building) HP (goldmine HP ~ 25k)
            armor.Size = new Size(60, 15);
            //damage.Size = new Size(70, 15);
            damage.Size = new Size(100, 15); //made longer
            range.Size = new Size(60, 15);
            sight.Size = new Size(60, 15);
            speed.Size = new Size(60, 15);
            progress.Size = new Size(100, 15);

            unitName.ForeColor = Color.White;
            unitHealth.ForeColor = Color.White;
            armor.ForeColor = Color.White;
            damage.ForeColor = Color.White;
            range.ForeColor = Color.White;
            sight.ForeColor = Color.White;
            speed.ForeColor = Color.White;
            progress.ForeColor = Color.White;

            unitName.BackColor = Color.Transparent;
            unitHealth.BackColor = Color.Transparent;
            armor.BackColor = Color.Transparent;
            damage.BackColor = Color.Transparent;
            range.BackColor = Color.Transparent;
            sight.BackColor = Color.Transparent;
            speed.BackColor = Color.Transparent;
            progress.BackColor = Color.Transparent;

            unitName.Location = new Point(unitPic.Width - 30, unitPic.Height / 4);
            unitHealth.Location = new Point(0, unitPic.Height);
            //Shifted all over by 10 as damage is too long to be shown
            armor.Location = new Point(this.Width / 3 - 10, this.Height / 3 - 15);
            damage.Location = new Point(this.Width / 3 - 10, this.Height / 3);
            range.Location = new Point(this.Width / 3 - 10, this.Height / 3 + 15);
            sight.Location = new Point(this.Width / 3 - 10, this.Height / 3 + 30);
            speed.Location = new Point(this.Width / 3 - 10, this.Height / 3 + 45);
            progress.Location = new Point(unitPic.Width - 45, unitPic.Height / 2);

            /*int index = SplashScreen.icons.findIcon("peasant");
            unitPic.Image = SplashScreen.icons.tiles[index];*/

            unitPic.Size = new Size(buttonX, buttonY);
            unitPic.Location = new Point(0, 0);

            //FUll health bar is the bar that shows when health gets decreased
            fullHealthBar.BackColor = Color.Red;
            fullHealthBar.Size = new Size(buttonX, 5);
            fullHealthBar.Location = new Point(0, unitPic.Height + 2);

            healthBar.BackColor = Color.Green;
            healthBar.Size = new Size(buttonX, 5);
            healthBar.Location = new Point(0, unitPic.Height + 2);

            this.Controls.Add(unitName);
            this.Controls.Add(unitHealth);
            this.Controls.Add(armor);
            this.Controls.Add(damage);
            this.Controls.Add(range);
            this.Controls.Add(sight);
            this.Controls.Add(speed);
            this.Controls.Add(unitPic);
            this.Controls.Add(fullHealthBar); //this should be shown underneath healthbar
            this.Controls.Add(healthBar);
            this.Controls.Add(progress);
        }

        public string nameTranslation(int unitType)
        { //function to return unitType as string
            string[] assetTypes = SplashScreen.allUnitNames;

            if (unitType < assetTypes.Length) //we actually have unit string corresponding to unitType
            {
                return assetTypes[unitType];
            }
            return null;
        }

        private string lowerCaseTranslation(int unitType)
        { //function to return unitType as string
            string[] assetTypes = { "peasant", "footman", "archer", "ranger", "gold-mine", "town-hall", "keep", "castle", "chicken-farm", "human-barracks", "human-lumber-mill", "human-blacksmith", "scout-tower", "human-guard-tower", "human-cannon-tower" };

            if (unitType < assetTypes.Length) //we actually have unit string corresponding to unitType
            {
                return assetTypes[unitType];
            }
            return null;
        }
    }
}
