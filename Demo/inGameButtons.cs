using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
    public partial class inGameButtons : Form
    {
        public BuildCapabilities builder; //for building data

        int playerChoice;

        public int mode = 0; //0 for default, 1 for build, 2 for human_move, 3 for mine select, 4 for patrol, 5 for attack button, 6 for repair mode... (list selections here)
        //int showingButtons = 0; //0 for none, 1= peasant, 2= townhall

        private tempMap gameData; //pointer to thisMap in XNA_inGame

        public inGame_menu focus_form;
        public List<Unit>[] allUnits;


        public int buttonX = 40;
        public int buttonY = 40;

        //in game buttons
        public Button humanMove;
        public Button humanArmor1; //what does thsi button do?      //not sure what it does, but it's in the original game.
        public Button humanWeapon1; 
        public Button repair; //no implementation yet (not in old original)
        public Button mine; 
        public Button buildSimple;

        //build-simple buttons
        public Button chickenFarm;
        public Button townHall;
        public Button humanBarracks;
        public Button humanLumberMill;
        public Button humanBlacksmith;
        public Button scoutTower;

        //town hall buttons
        public Button peasant;
        public Button keep; 
        public Button castle;

        //barrack building buttons
        public Button footman;
        public Button archer;
        public Button ranger;  //build ranger

        //footman button
        public Button humanPatrol;

        //blacksmith building button
        public Button humanWeapon2;  //weapon upgrade level 2
        public Button humanWeapon3;  //weapon upgrade level 3
        public Button humanArmor2;  //armor upgrade level 2
        public Button humanArmor3;  //armor upgrade level 3

        //Scout tower building button
        public Button humanGuardTower;  //upgrade scout tower to guard tower
        public Button humanCannonTower;  //upgrade scout tower to cannon tower

        //lumber mill building button
        public Button rangerUpgrade; //upgrade to unlock ranger building
        public Button humanArrow2;  //arrow upgrade level 2
        public Button humanArrow3; //arrow upgrade to level 3
        public Button longbow;  //unit upgrade
        public Button rangerScouting;  //unit upgrade
        public Button marksmanship;  //unit upgrade

        public Button cancel;

        public inGameButtons(inGame_menu form, ref tempMap gameData, ref List<Unit>[] allUnits)
        {
            InitializeComponent();
            this.focus_form = form;
            this.allUnits = allUnits; //do not copy want pointers
            //this.gameData = gameData;
            this.BackgroundImage = new Bitmap(@"data\img\Texture.png");
            HumanMove_Button();
            HumanArmor1_Button();
            HumanWeapon1_Button();
            Repair_Button();
            Mine_Button();
            BuildSimple_Button();

            ChickenFarm_Button();
            TownHall_Button();
            HumanBarracks_Button();
            HumanLumberMill_Button();
            HumanBlacksmith_Button();
            ScoutTower_Button();
            Cancel_Button();

            Peasant_Button();
            TownHall_Button();
            Keep_Button();
            Footman_Button();
            HumanPatrol_Button();
            Archer_Button();

            //Upgrade buttons
            Ranger_Button();
            humanArrow2_Button();
            humanArrow3_Button();
            HumanArmor2_Button();
            HumanArmor3_Button();
            HumanWeapon2_Button();
            HumanWeapon3_Button();
            Keep_Button();
            castle_Button();
            HumanGuardTower_Button();
            HumanCannonTower_Button();
            longbow_Button();
            rangerScouting_Button();
            rangerUpgrade_Button();
            marksmanship_Button();



            hideButtons();

            this.KeyDown += new KeyEventHandler(inGame_menu_KeyDown);
            this.KeyUp += new KeyEventHandler(inGame_menu_KeyDown); //add to key release too
            this.builder = new BuildCapabilities(); //init
            this.playerChoice = SplashScreen.mapUnits.playerChoice;
            this.gameData = gameData;

        }
        private void inGame_menu_KeyDown(object sender, KeyEventArgs e)
        {
            this.ActiveControl = null;
            e.Handled = true; //prevents passing keyboard strokes to other controls in form

        }

        public void reloadBuilder(ref tempMap gameData, ref List<Unit>[] allUnits)
        {//function for reloading building data
            //call this function when we load new map
            this.playerChoice = SplashScreen.mapUnits.playerChoice;
            this.mode = 0; //reset mode
            builder = null;
            builder = new BuildCapabilities(); //init new builder
            this.gameData = gameData;
            this.allUnits = allUnits;
            
        }



        public void peasantButtons() //basic peasant button only (other functions for other units)
        { //currently not spawning buttons for multi unit select (unsure of that logic yet)
            //this.playerChoice = focus_form.
            inGameButton_Show();
        }

        public void townHallButtons() //basic town hall buttosn only
        { //currently not spawning buttons for multi unit select (unsure of that logic yet)
            //this.playerChoice = focus_form.
            townHallButton_Show();


        }

        public void barracksButtons()
        {
            humanBarracksButton_Show();
        }

        public void blackSmithButtons()
        {
            humanBlackSmithButton_Show();
        }

        public void lumbermillButtons()
        {
            lumbermillupgrade_Show();
        }

        public void scouttowerButtons()
        {
            humanScoutTowerButton_Show();
        }

        public void footmanButtons()
        {
            footmanButton_Show();
        }

        public void archerButtons()
        {
            footmanButton_Show();
        }


        public void hideButtons() //hide all buttons (can be called outside)
        {
            inGameButton_Hide();
            BuildSimple_Hide();
            townHallButton_Hide();
            humanBarracksButton_Hide();
            footmanButton_Hide();
            //humanLumberMillButton_Hide();
            humanBlackSmithButton_Hide();
            humanScoutTowerButton_Hide();
            lumbermillupgrade_Hide();
            this.ActiveControl = null;
        }

        public void cancelBuild()
        {
            inGameButton_Hide();
            BuildSimple_Show();
            builder.buildCancel();
            this.mode = builder.getMode();
        }

        public void cancelHumanMove()
        {
            cancel.Hide();
            inGameButton_Show();
            this.mode = 0;
        }

        public void cancelHumanMoveArmy()
        {
            cancel.Hide();
            footmanButton_Show();
            this.mode = 0;
        }

        private void HumanMove_Button()
        {
            //Human_move button
            humanMove = new Button();
            humanMove.Location = new Point(0, 0);
            humanMove.Height = buttonY;
            humanMove.Width = buttonX;
            humanMove.MouseClick += new MouseEventHandler(HumanMove_Click);
            int index = SplashScreen.icons.findIcon("human-move");
            humanMove.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanMove);
        }

        private void HumanMove_Click(object sender, MouseEventArgs e)
        {
            cancel.Show();
            inGameButton_Hide();
            focus_form.Focus();
            this.ActiveControl = null;
            this.mode = 2;
        }

        private void HumanArmor1_Button()
        {
            //HumanArmor1 button
            humanArmor1 = new Button();
            humanArmor1.Location = new Point(42, 0);
            humanArmor1.Height = buttonY;
            humanArmor1.Width = buttonX;
            humanArmor1.MouseClick += new MouseEventHandler(HumanArmor1_Click);
            int index = SplashScreen.icons.findIcon("human-armor-1");
            humanArmor1.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanArmor1);
        }

        private void HumanArmor1_Click(object sender, MouseEventArgs e)
        {
            cancel.Show();
            inGameButton_Hide();
            focus_form.Focus();
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void HumanWeapon1_Button()
        {
            //HumanWeapon1 button
            humanWeapon1 = new Button();
            humanWeapon1.Location = new Point(84, 0);
            humanWeapon1.Height = buttonY;
            humanWeapon1.Width = buttonX;
            humanWeapon1.MouseClick += new MouseEventHandler(HumanWeapon1_Click);
            int index = SplashScreen.icons.findIcon("human-weapon-1");
            humanWeapon1.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanWeapon1);
        }

        private void HumanWeapon1_Click(object sender, MouseEventArgs e)
        {
            cancel.Show();
            inGameButton_Hide();
            focus_form.Focus();
            this.mode = 5; //set to attack mode
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void Repair_Button()
        {
            //Repair button
            repair = new Button();
            repair.Location = new Point(0, 42);
            repair.Height = buttonY;
            repair.Width = buttonX;
            repair.MouseClick += new MouseEventHandler(Repair_Click);
            int index = SplashScreen.icons.findIcon("repair");
            repair.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(repair);
        }

        private void Repair_Click(object sender, MouseEventArgs e)
        {
            cancel.Show();
            inGameButton_Hide();
            focus_form.Focus();
            this.mode = 6; //6 for repair mode
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void Mine_Button()
        {
            //Mine button
            mine = new Button();
            mine.Location = new Point(42, 42);
            mine.Height = buttonY;
            mine.Width = buttonX;
            mine.MouseClick += new MouseEventHandler(Mine_Click);
            int index = SplashScreen.icons.findIcon("mine");
            mine.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(mine);
        }

        private void Mine_Click(object sender, MouseEventArgs e)
        {
            cancel.Show();
            inGameButton_Hide();
            focus_form.Focus();
            this.mode = 3; //3 for resource gather mode
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void BuildSimple_Button()
        {
            //BuildSimple button
            buildSimple = new Button();
            buildSimple.Location = new Point(0, 84);
            buildSimple.Height = buttonY;
            buildSimple.Width = buttonX;
            buildSimple.MouseClick += new MouseEventHandler(BuildSimple_Click);
            int index = SplashScreen.icons.findIcon("build-simple");
            buildSimple.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(buildSimple);
        }

        private void BuildSimple_Click(object sender, MouseEventArgs e)
        {
            cancel.Show();
            BuildSimple_Show();
            inGameButton_Hide();
            focus_form.Focus();
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void ChickenFarm_Button()
        {
            //ChickenFarm button
            chickenFarm = new Button();
            chickenFarm.Location = new Point(0, 0);
            chickenFarm.Height = buttonY;
            chickenFarm.Width = buttonX;
            chickenFarm.MouseClick += new MouseEventHandler(ChickenFarm_Click);
            int index = SplashScreen.icons.findIcon("chicken-farm");
            chickenFarm.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(chickenFarm);
            chickenFarm.Hide();
        }

        private void ChickenFarm_Click(object sender, MouseEventArgs e)
        {
            builder.Build("Farm", this.playerChoice, ref gameData);
            this.mode = builder.getMode();
            BuildSimple_Hide();
            cancel.Show();
            focus_form.Focus();
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void TownHall_Button()
        {
            //TownHall button
            townHall = new Button();
            townHall.Location = new Point(42, 0);
            townHall.Height = buttonY;
            townHall.Width = buttonX;
            townHall.MouseClick += new MouseEventHandler(TownHall_Click);
            int index = SplashScreen.icons.findIcon("town-hall");
            townHall.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(townHall);
            townHall.Hide();
        }

        private void TownHall_Click(object sender, MouseEventArgs e)
        {
            builder.Build("TownHall", this.playerChoice, ref gameData);
            this.mode = builder.getMode();
            BuildSimple_Hide();
            cancel.Show();
            focus_form.Focus();
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void HumanBarracks_Button()
        {
            //HumanBarracks button
            humanBarracks = new Button();
            humanBarracks.Location = new Point(84, 0);
            humanBarracks.Height = buttonY;
            humanBarracks.Width = buttonX;
            humanBarracks.MouseClick += new MouseEventHandler(HumanBarracks_Click);
            int index = SplashScreen.icons.findIcon("human-barracks");
            humanBarracks.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanBarracks);
            humanBarracks.Hide();
        }

        private void HumanBarracks_Click(object sender, MouseEventArgs e)
        {
            builder.Build("Barracks", this.playerChoice, ref gameData);
            this.mode = builder.getMode();
            BuildSimple_Hide();
            cancel.Show();
            focus_form.Focus();
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void HumanLumberMill_Button()
        {
            //humanLumberMill button
            humanLumberMill = new Button();
            humanLumberMill.Location = new Point(0, 42);
            humanLumberMill.Height = buttonY;
            humanLumberMill.Width = buttonX;
            humanLumberMill.MouseClick += new MouseEventHandler(HumanLumberMill_Click);
            int index = SplashScreen.icons.findIcon("human-lumber-mill");
            humanLumberMill.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanLumberMill);
            humanLumberMill.Hide();
        }

        private void HumanLumberMill_Click(object sender, MouseEventArgs e)
        {
            builder.Build("LumberMill", this.playerChoice, ref gameData);
            this.mode = builder.getMode();
            BuildSimple_Hide();
            cancel.Show();
            focus_form.Focus();
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void HumanBlacksmith_Button()
        {
            //HumanBlacksmith button
            humanBlacksmith = new Button();
            humanBlacksmith.Location = new Point(42, 42);
            humanBlacksmith.Height = buttonY;
            humanBlacksmith.Width = buttonX;
            humanBlacksmith.MouseClick += new MouseEventHandler(HumanBlacksmith_Click);
            int index = SplashScreen.icons.findIcon("human-blacksmith");
            humanBlacksmith.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanBlacksmith);
            humanBlacksmith.Hide();
        }

        private void HumanBlacksmith_Click(object sender, MouseEventArgs e)
        {
            builder.Build("Blacksmith", this.playerChoice, ref gameData);
            this.mode = builder.getMode();
            BuildSimple_Hide();
            cancel.Show();
            focus_form.Focus();
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void ScoutTower_Button()
        {
            //ScoutTower button
            scoutTower = new Button();
            scoutTower.Location = new Point(84, 42);
            scoutTower.Height = buttonY;
            scoutTower.Width = buttonX;
            scoutTower.MouseClick += new MouseEventHandler(ScoutTower_Click);
            int index = SplashScreen.icons.findIcon("scout-tower");
            scoutTower.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(scoutTower);
            scoutTower.Hide();
        }

        private void ScoutTower_Click(object sender, MouseEventArgs e)
        {
            BuildSimple_Hide();
            builder.Build("ScoutTower", this.playerChoice, ref gameData);
            this.mode = builder.getMode();
            cancel.Show();
            focus_form.Focus();
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void Cancel_Button()
        {
            //Cancel button
            cancel = new Button();
            cancel.Location = new Point(84, 84);
            cancel.Height = buttonY;
            cancel.Width = buttonX;
            cancel.MouseClick += new MouseEventHandler(Cancel_Click);
            int index = SplashScreen.icons.findIcon("cancel");
            cancel.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(cancel);
            cancel.Hide();
        }

        private void Cancel_Click(object sender, MouseEventArgs e)
        {
            //need to fix cancel button for: footman/archer and all buildings besides townhall (ex: lumber mill, blacksmith, etc...)

            if (this.mode == 1)
            { //building cancel
                cancelBuild();
            }
            else if (this.mode == 5 || this.mode == 4) //attack or patrol mode
            {
                cancelHumanMoveArmy();
            }
            else if (this.mode == 2) //human move button is on both peasant and footman
            {
                unitPtr temp = gameData.chosenUnits.ElementAt(0); //get the first selected unit
                Unit unitTemp = allUnits[temp.owner][temp.index];
                if (unitTemp.unitType == gameData.nameTranslation("Peasant"))
                {
                    cancelHumanMove();
                }
                else //army unit
                {
                    cancelHumanMoveArmy();
                }
            }
            else
            { //normal cancel
                inGameButton_Show();
                BuildSimple_Hide();
                builder.buildCancel();
                this.mode = 0;
            }
            focus_form.Focus();
            this.ActiveControl = null;//prevent buttons block the input

        }

        //TownHall buttons
        private void Peasant_Button()
        {
            peasant = new Button();
            peasant.Location = new Point(0, 0);
            peasant.Height = buttonY;
            peasant.Width = buttonX;
            int index = SplashScreen.icons.findIcon("peasant");
            peasant.MouseClick += new MouseEventHandler(Peasant_Click);
            peasant.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(peasant);
            peasant.Hide();
        }

        private void Peasant_Click(object sender, MouseEventArgs e)
        {
            //townHallButton_Hide();
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].QueueUnit("Peasant", ref this.gameData))
            { //success
                //System.Windows.Forms.MessageBox.Show("Queued unit button click!"); //testing
                hideButtons();
            }
            else //fail
            {
                //play some failed sound here?
                //System.Windows.Forms.MessageBox.Show("Unit queueing failed!"); //testing
            }
            //cancel.Show();
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void Keep_Button()
        {
            keep = new Button();
            keep.Location = new Point(42,0);
            keep.Height = buttonY;
            keep.Width = buttonX;
            keep.MouseClick += new MouseEventHandler(Keep_Click);
            int index = SplashScreen.icons.findIcon("keep");
            keep.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(keep);
            keep.Hide();
        }

        private void Keep_Click(object sender, MouseEventArgs e)
        {
            //townHallButton_Hide();
            //cancel.Show();
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Keep", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void castle_Button()
        {
            castle = new Button();
            castle.Location = new Point(42, 0);
            castle.Height = buttonY;
            castle.Width = buttonX;
            castle.MouseClick += new MouseEventHandler(Castle_Click);
            int index = SplashScreen.icons.findIcon("castle");
            castle.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(castle);
            castle.Hide();
        }

        private void Castle_Click(object sender, MouseEventArgs e)
        {
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Castle", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
            hideButtons();
        }

        //Barrack building buttons
        private void Footman_Button()
        {
            footman = new Button();
            footman.Location = new Point(0, 0);
            footman.Height = buttonY;
            footman.Width = buttonX;
            int index = SplashScreen.icons.findIcon("footman");
            footman.MouseClick += new MouseEventHandler(Footman_Click);
            footman.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(footman);
            footman.Hide();
        }

        private void Footman_Click(object sender, MouseEventArgs e)
        {
            //humanBarracksButton_Hide();
            //cancel.Show();
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].QueueUnit("Footman", ref this.gameData))
            { //success
                //System.Windows.Forms.MessageBox.Show("Queued unit button click!"); //testing
                hideButtons();
            }
            else //fail
            {
                //play some failed sound here?
                //System.Windows.Forms.MessageBox.Show("Unit queueing failed!"); //testing
            }
            //cancel.Show();
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void Archer_Button()
        {
            archer = new Button();
            archer.Location = new Point(42, 0);
            archer.Height = buttonY;
            archer.Width = buttonX;
            int index = SplashScreen.icons.findIcon("archer");
            archer.MouseClick += new MouseEventHandler(Archer_Click);
            archer.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(archer);
            archer.Hide();
        }

        private void Archer_Click(object sender, MouseEventArgs e)
        {
            //humanBarracksButton_Hide();
            //cancel.Show();
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].QueueUnit("Archer", ref this.gameData))
            { //success
                //System.Windows.Forms.MessageBox.Show("Queued unit button click!"); //testing
                hideButtons();
            }
            else //fail
            {
                //play some failed sound here?
                //System.Windows.Forms.MessageBox.Show("Unit queueing failed!"); //testing
            }
            this.ActiveControl = null;//prevent buttons block the input
        }

        //Buttons when click on footman/archer: move, armor, weapon, and patrol
        private void HumanPatrol_Button()
        {
            humanPatrol = new Button();
            humanPatrol.Location = new Point(0, 42);
            humanPatrol.Height = buttonY;
            humanPatrol.Width = buttonX;
            int index = SplashScreen.icons.findIcon("human-patrol");
            humanPatrol.MouseClick += new MouseEventHandler(HumanPatrol_Click);
            humanPatrol.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanPatrol);
            humanPatrol.Hide();
        }

        private void HumanPatrol_Click(object sender, MouseEventArgs e)
        {
            footmanButton_Hide();
            cancel.Show();
            this.mode = 4;
            this.ActiveControl = null;//prevent buttons block the input
        }

        //Scout tower buttons
        private void HumanGuardTower_Button()
        {
            humanGuardTower = new Button();
            humanGuardTower.Location = new Point(0, 0);
            humanGuardTower.Height = buttonY;
            humanGuardTower.Width = buttonX;
            int index = SplashScreen.icons.findIcon("human-guard-tower");
            humanGuardTower.MouseClick += new MouseEventHandler(HumanGuardTower_Click);
            humanGuardTower.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanGuardTower);
            humanGuardTower.Hide();
        }

        private void HumanGuardTower_Click(object sender, MouseEventArgs e)
        {
            //cancel.Show();
            //humanScoutTowerButton_Hide();
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("GuardTower", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void HumanCannonTower_Button()
        {
            humanCannonTower = new Button();
            humanCannonTower.Location = new Point(42, 0);
            humanCannonTower.Height = buttonY;
            humanCannonTower.Width = buttonX;
            int index = SplashScreen.icons.findIcon("human-cannon-tower");
            humanCannonTower.MouseClick += new MouseEventHandler(HumanCannonTower_Click);
            humanCannonTower.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanCannonTower);
            humanCannonTower.Hide();
        }

        private void HumanCannonTower_Click(object sender, MouseEventArgs e)
        {
            //cancel.Show();
            //humanScoutTowerButton_Hide();
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("CannonTower", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
            this.ActiveControl = null;//prevent buttons block the input
        }

        
        private void Ranger_Button()
        { //replaces archer button after upgrade
            ranger = new Button();
            ranger.Location = new Point(42, 0);
            ranger.Height = buttonY;
            ranger.Width = buttonX;
            int index = SplashScreen.icons.findIcon("ranger");
            ranger.MouseClick += new MouseEventHandler(Ranger_Click);
            ranger.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(ranger);
            ranger.Hide();
        }

        private void Ranger_Click(object sender, MouseEventArgs e)
        {
            /*if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Ranger", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }*/ //oops, it's not an upgrade button
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].QueueUnit("Ranger", ref this.gameData))
            { //success
                //System.Windows.Forms.MessageBox.Show("Queued unit button click!"); //testing
                hideButtons();
            }
            else //fail
            {
                //play some failed sound here?
                //System.Windows.Forms.MessageBox.Show("Unit queueing failed!"); //testing
            }


            this.ActiveControl = null;//prevent buttons block the input
        }

        //Lumber mill building button
        private void rangerUpgrade_Button()
        {
            rangerUpgrade = new Button();
            rangerUpgrade.Location = new Point(42, 0);
            rangerUpgrade.Height = buttonY;
            rangerUpgrade.Width = buttonX;
            int index = SplashScreen.icons.findIcon("ranger");
            rangerUpgrade.MouseClick += new MouseEventHandler(rangerUpgrade_Click);
            rangerUpgrade.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(rangerUpgrade);
            rangerUpgrade.Hide();
        }

        private void rangerUpgrade_Click(object sender, MouseEventArgs e)
        {
            //cancel.Show();
            //lumbermillupgrade_Hide();
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Ranger", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void humanArrow2_Button()
        {
            humanArrow2 = new Button();
            humanArrow2.Location = new Point(42, 0);
            humanArrow2.Height = buttonY;
            humanArrow2.Width = buttonX;
            int index = SplashScreen.icons.findIcon("human-arrow-2");
            humanArrow2.MouseClick += new MouseEventHandler(humanArrow2_Click);
            humanArrow2.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanArrow2);
            humanArrow2.Hide();
        }

        private void humanArrow2_Click(object sender, MouseEventArgs e)
        {
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Arrow2", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void humanArrow3_Button()
        {
            humanArrow3 = new Button();
            humanArrow3.Location = new Point(42, 0);
            humanArrow3.Height = buttonY;
            humanArrow3.Width = buttonX;
            int index = SplashScreen.icons.findIcon("human-arrow-3");
            humanArrow3.MouseClick += new MouseEventHandler(humanArrow3_Click);
            humanArrow3.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanArrow2);
            humanArrow3.Hide();
        }

        private void humanArrow3_Click(object sender, MouseEventArgs e)
        {
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Arrow3", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
        }

        //blacksmith building button
        private void HumanWeapon2_Button()
        {
            humanWeapon2 = new Button();
            humanWeapon2.Location = new Point(0, 0);
            humanWeapon2.Height = buttonY;
            humanWeapon2.Width = buttonX;
            int index = SplashScreen.icons.findIcon("human-weapon-2");
            humanWeapon2.MouseClick += new MouseEventHandler(HumanWeapon2_Click);
            humanWeapon2.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanWeapon2);
            humanWeapon2.Hide();
        }

        private void HumanWeapon2_Click(object sender, MouseEventArgs e)
        {
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Weapon2", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void HumanWeapon3_Button()
        {
            humanWeapon3 = new Button();
            humanWeapon3.Location = new Point(0, 0);
            humanWeapon3.Height = buttonY;
            humanWeapon3.Width = buttonX;
            int index = SplashScreen.icons.findIcon("human-weapon-3");
            humanWeapon3.MouseClick += new MouseEventHandler(HumanWeapon3_Click);
            humanWeapon3.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanWeapon3);
            humanWeapon3.Hide();
        }

        private void HumanWeapon3_Click(object sender, MouseEventArgs e)
        {
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Weapon3", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
            //hideButtons();
        }

        private void HumanArmor2_Button()
        {
            humanArmor2 = new Button();
            humanArmor2.Location = new Point(42, 0);
            humanArmor2.Height = buttonY;
            humanArmor2.Width = buttonX;
            int index = SplashScreen.icons.findIcon("human-armor-2");
            humanArmor2.MouseClick += new MouseEventHandler(HumanArmor2_Click);
            humanArmor2.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanArmor2);
            humanArmor2.Hide();
        }

        private void HumanArmor2_Click(object sender, MouseEventArgs e)
        {
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Armor2", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void HumanArmor3_Button()
        {
            humanArmor3 = new Button();
            humanArmor3.Location = new Point(42, 0);
            humanArmor3.Height = buttonY;
            humanArmor3.Width = buttonX;
            int index = SplashScreen.icons.findIcon("human-armor-3");
            humanArmor3.MouseClick += new MouseEventHandler(HumanArmor3_Click);
            humanArmor3.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(humanArmor3);
            humanArmor3.Hide();
        }

        private void HumanArmor3_Click(object sender, MouseEventArgs e)
        {
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Armor3", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
            //hideButtons();
        }

        private void longbow_Button()
        {
            longbow = new Button();
            longbow.Location = new Point(42, 0);
            longbow.Height = buttonY;
            longbow.Width = buttonX;
            int index = SplashScreen.icons.findIcon("longbow");
            longbow.MouseClick += new MouseEventHandler(HumanArmor2_Click);
            longbow.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(longbow);
            longbow.Hide();
        }

        private void longbow_Click(object sender, MouseEventArgs e)
        {
            //cancel.Show();
            //lumbermillupgrade_Hide();
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Longbow", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void rangerScouting_Button()
        {
            rangerScouting = new Button();
            rangerScouting.Location = new Point(84,0);
            rangerScouting.Height = buttonY;
            rangerScouting.Width = buttonX;
            int index = SplashScreen.icons.findIcon("ranger-scouting");
            rangerScouting.MouseClick += new MouseEventHandler(rangerScouting_Click);
            rangerScouting.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(rangerScouting);
            rangerScouting.Hide();
        }

        private void rangerScouting_Click(object sender, MouseEventArgs e)
        {
            //cancel.Show();
            //lumbermillupgrade_Hide();
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Scouting", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
        }

        private void marksmanship_Button()
        {
            marksmanship = new Button();
            marksmanship.Location = new Point(0,42);
            marksmanship.Height = buttonY;
            marksmanship.Width = buttonX;
            int index = SplashScreen.icons.findIcon("marksmanship");
            marksmanship.MouseClick += new MouseEventHandler(marksmanship_Click);
            marksmanship.Image = SplashScreen.icons.tiles[index];
            this.Controls.Add(marksmanship);
            marksmanship.Hide();
        }

        private void marksmanship_Click(object sender, MouseEventArgs e)
        {
            //cancel.Show();
            //lumbermillupgrade_Hide();
            if (this.allUnits[this.gameData.chosenUnits.ElementAt(0).owner][this.gameData.chosenUnits.ElementAt(0).index].startUpgrade("Marksmanship", ref gameData))
            { //success
                hideButtons(); //hide all buttons as we are now upgrading
            }
            this.ActiveControl = null;//prevent buttons block the input
        }

        //show all the building buttons
        private void BuildSimple_Show()
        {
            chickenFarm.Show();
            townHall.Show();
            humanBarracks.Show();
            humanLumberMill.Show();
            humanBlacksmith.Show();
            scoutTower.Show();
            cancel.Show();
        }

        //hide all the building buttons
        private void BuildSimple_Hide()
        {
            chickenFarm.Hide();
            townHall.Hide();
            humanBarracks.Hide();
            humanLumberMill.Hide();
            humanBlacksmith.Hide();
            scoutTower.Hide();
            cancel.Hide();
        }

        //show all the in game buttons
        private void inGameButton_Show()
        {
            humanMove.Show();
            humanArmor1.Show();
            humanWeapon1.Show();
            repair.Show();
            mine.Show();
            buildSimple.Show();
            
        }

        //Hide all the in game button
        private void inGameButton_Hide()
        {
            humanMove.Hide();
            humanArmor1.Hide();
            humanWeapon1.Hide();
            humanPatrol.Hide();
            repair.Hide();
            mine.Hide();
            buildSimple.Hide();
        }

        //Show lumbermill buttons

        /*private void humanLumberMillButton_Show()
        {

        }

        private void humanLumberMillButton_Hide()
        {
            humanArrow2.Hide();
            ranger.Hide();
        }*/

        //Show town hall buttons
        private void townHallButton_Show()
        {
            peasant.Show();
            //Only show keep upgrade if selected building is TownHall and we meet requirement for keep upgrade
            if (gameData.checkRequirements("Keep", allUnits, playerChoice) && gameData.checkCurrentUnit("TownHall",allUnits)) keep.Show();
            //Only show castle upgrade if selected building is Keep and we meet requirement for Castle upgrade
            if (gameData.checkRequirements("Castle", allUnits, playerChoice) && gameData.checkCurrentUnit("Keep", allUnits))
            { 
                castle.Show();
            }
        }

        //Hide town hall buttons
        private void townHallButton_Hide()
        {
            peasant.Hide();
            keep.Hide();
            castle.Hide();
        }
        
        //Show barrack building buttons
        private void humanBarracksButton_Show()
        {
            footman.Show();
            int index = SplashScreen.unitUpgradeData.upgradeTranslation("Ranger");
            if (gameData.checkRequirements("Ranger", allUnits, playerChoice) && gameData.allUpgrades[index] == 2) ranger.Show(); //only show ranger if requirements met (keep) and completed Ranger upgrade
            else if (gameData.checkRequirements("Archer", allUnits, playerChoice)) archer.Show(); //only show archer if requirements met and not ranger upgraded
        }


        //Hide barrack building buttons
        private void humanBarracksButton_Hide()
        {
            footman.Hide();
            archer.Hide();
            ranger.Hide(); //I'm not sure where we actually build the ranger, might be here (or it might replace archer button?)
        }

        //Show buttons when click on footman/archer
        private void footmanButton_Show()
        {
            humanMove.Show();
            humanArmor1.Show();
            humanWeapon1.Show();
            humanPatrol.Show();
        }

        //Hide buttons when not click on footman/archer
        private void footmanButton_Hide()
        {
            humanMove.Hide();
            humanArmor1.Hide();
            humanWeapon1.Hide();
            humanPatrol.Hide();
        }

        private void humanBlackSmithButton_Show()
        {
            int index = SplashScreen.unitUpgradeData.upgradeTranslation("Armor2");
            int index2 = SplashScreen.unitUpgradeData.upgradeTranslation("Armor3");
            int index3 = SplashScreen.unitUpgradeData.upgradeTranslation("Weapon2");
            int index4 = SplashScreen.unitUpgradeData.upgradeTranslation("Weapon3");
            
            if (gameData.allUpgrades[index] != 2 && gameData.allUpgrades[index] != 1)
            { //only show upgrade button if we haven't upgraded yet
                humanArmor2.Show();
            }
            if (gameData.allUpgrades[index2] != 2 && gameData.allUpgrades[index2] != 1 && gameData.allUpgrades[index] == 2)
            { //only show upgrade button if we haven't upgraded yet and armor2 is upgraded
                humanArmor3.Show();
            }
            if (gameData.allUpgrades[index3] != 2 && gameData.allUpgrades[index3] != 1)
            { //only show upgrade button if we haven't upgraded yet
                humanWeapon2.Show();
            }
            if (gameData.allUpgrades[index4] != 2 && gameData.allUpgrades[index4] != 1 && gameData.allUpgrades[index3] == 2)
            { //only show upgrade button if we haven't upgraded yet and completed weapon2 upgrade
                humanWeapon3.Show();
            }

            //if humanWeapon2 is upgraded, then show:
            //humanWeapon3.Show();

            //if humanArmor2 is upgraded, then show:
            //humanArmor3.Show();
        }

        private void humanBlackSmithButton_Hide()
        {
            humanArmor2.Hide();
            humanArmor3.Hide();
            humanWeapon2.Hide();
            humanWeapon3.Hide();
        }

        private void humanScoutTowerButton_Show()
        {
            if (gameData.checkRequirements("GuardTower", allUnits, playerChoice)) humanGuardTower.Show();
            if (gameData.checkRequirements("CannonTower", allUnits, playerChoice)) humanCannonTower.Show();
        }

        private void humanScoutTowerButton_Hide()
        {
            humanGuardTower.Hide();
            humanCannonTower.Hide();
        }

        private void lumbermillupgrade_Show()
        {

            int index0 = SplashScreen.unitUpgradeData.upgradeTranslation("Arrow2");
            int index1 = SplashScreen.unitUpgradeData.upgradeTranslation("Arrow3");
            int index2 = SplashScreen.unitUpgradeData.upgradeTranslation("Ranger");
            int index3 = SplashScreen.unitUpgradeData.upgradeTranslation("Longbow");
            int index4 = SplashScreen.unitUpgradeData.upgradeTranslation("Scouting");
            int index5 = SplashScreen.unitUpgradeData.upgradeTranslation("Marksmanship");
            if (gameData.allUpgrades[index0] != 2 && gameData.allUpgrades[index0] != 1)
            { //only show upgrade button if we haven't upgraded yet
                humanArrow2.Show();
            }
            if (gameData.allUpgrades[index1] != 2 && gameData.allUpgrades[index1] != 1 && gameData.allUpgrades[index0] == 2)
            { //only show upgrade button if we haven't upgraded yet and arrow2 is done
                humanArrow3.Show();
            }
            if (gameData.allUpgrades[index2] != 2 && gameData.allUpgrades[index2] != 1 && gameData.checkRequirements("Ranger", allUnits, playerChoice))
            { //only show upgrade button if we haven't upgraded yet and arrow2 is done and keep is built (as that is required to build ranger)
                rangerUpgrade.Show();
            }

            //Not sure which 'age' of the townhall is needed in order to show these upgrades so defaulted to require keep for now
            if (gameData.allUpgrades[index3] != 2 && gameData.allUpgrades[index3] != 1 && gameData.allUpgrades[index2] == 2)
            { //only show upgrade button if we haven't upgraded yet
                longbow.Show();
            }
            if (gameData.allUpgrades[index4] != 2 && gameData.allUpgrades[index4] != 1 && gameData.allUpgrades[index2] == 2)
            { //only show upgrade button if we haven't upgraded yet (assume that we need ranger for this)
                rangerScouting.Show();
            }
            if (gameData.allUpgrades[index5] != 2 && gameData.allUpgrades[index5] != 1 && gameData.allUpgrades[index2] == 2)
            { //only show upgrade button if we haven't upgraded yet
                marksmanship.Show();
            }

            //if upgrade humanarrow2 first, then show ranger button in Lumber Mill
            //rangerUpgrade.Show();

            //if upgrade ranger first, then show humanarrow2 button in Lumber Mill
            //humanArrow2.Show(); 
        }

        private void lumbermillupgrade_Hide()
        {
            rangerUpgrade.Hide();
            humanArrow2.Hide();
            longbow.Hide();
            rangerScouting.Hide();
            marksmanship.Hide();
        }
    }
}