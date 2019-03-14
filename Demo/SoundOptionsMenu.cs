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
    public partial class SoundOptionsMenu : Form
    {
        public static int pubVolBGM;
        public static int pubVolSFX;
        public SoundOptionsMenu()
        {
            InitializeComponent();
            pubVolBGM = 100;
            pubVolSFX = 100;
            this.BackgroundImage = new Bitmap(@"data\img\Texture.png");
        }

        private void volMaster_Scroll(object sender, EventArgs e)
        {
            numMaster.Text = volMaster.Value.ToString();

            pubVolBGM = (int)(0.01f * volBGM.Value * volMaster.Value);
            pubVolSFX = (int)(0.01f * volSFX.Value * volMaster.Value);

            // Music
            //if (XNA_InGame.gameMusic != null) XNA_InGame.gameMusic.settings.volume = pubVolBGM; //we can set when game starts as menu is separate from game
            if (MainMenu.menuMusic != null) MainMenu.menuMusic.settings.volume = pubVolBGM;

            // Effects
            if (MainMenu.buttonClickSnd != null) MainMenu.buttonClickSnd.settings.volume = pubVolSFX;
        }

        private void volBGM_Scroll(object sender, EventArgs e)
        { 
            numBGM.Text = volBGM.Value.ToString();

            pubVolBGM = (int)(0.01f * volBGM.Value * volMaster.Value);

            //if (XNA_InGame.gameMusic != null) XNA_InGame.gameMusic.settings.volume = pubVolBGM;
            if (MainMenu.menuMusic != null) MainMenu.menuMusic.settings.volume = pubVolBGM;
        }

        private void volSFX_Scroll(object sender, EventArgs e)
        {
            numSFX.Text = volSFX.Value.ToString();

            pubVolSFX = (int)(0.01f * volSFX.Value * volMaster.Value);

            if (MainMenu.buttonClickSnd != null) MainMenu.buttonClickSnd.settings.volume = pubVolSFX;
        }

        private void butEsc_Click(object sender, EventArgs e)
        {
            MainMenu.buttonClickSnd.controls.play();
            Application.OpenForms.OfType<Options>().First().Show();
            this.Hide();
        }
    }
}