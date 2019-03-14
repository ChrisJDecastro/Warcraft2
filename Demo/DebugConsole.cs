using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{

    //By AZ
    //When In game, press C to activate the console.
    //You can add any code here to help you debug the game!
    class ConsoleForm : Form
    {
        public TextBox textBox1;
        public Button confirm;
        public XNA_InGame g;
        public ConsoleForm()
        {
            textBox1 = new TextBox();
            confirm = new Button();
            confirm.Click += new EventHandler(confirm_Click);
            confirm.Location = new System.Drawing.Point(10,30);
            confirm.Text = "Confirm";

            this.Height = 100;
            this.Width = 400;
           
            this.Text = "Debug Console";




            this.Controls.Add(textBox1);
            this.Controls.Add(confirm);
        }


 
        private void confirm_Click(object sender, EventArgs e)
        {

                //this.txtResult.Text = testDialog.TextBox1.Text;
                string result = textBox1.Text;
            //MessageBox.Show(result);

            Console.WriteLine("Result:" + result);
            if (result == "fog")
                {
                    if (g.thisMap.renderFogFlag == 0) { g.thisMap.renderFogFlag = 1; }
                    else { g.thisMap.renderFogFlag = 0; }

                Console.WriteLine("Fog of war triggered");
                   // MessageBox.Show("Fog of war triggered.");
                }
            if (result == "fullscreen")
            {
                if (g.inGame_form.WindowState == FormWindowState.Normal)
                {
                    g.inGame_form.WindowState = FormWindowState.Normal;
                    g.inGame_form.FormBorderStyle = FormBorderStyle.None;
                    g.inGame_form.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    g.inGame_form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    g.inGame_form.WindowState = FormWindowState.Normal;
                }

                Console.WriteLine("Fullscreen switched");
                // MessageBox.Show("Fog of war triggered.");
            }

            this.Dispose();
        }

    }
  

    class DebugConsole
    {
        public static void createDebugConsole(XNA_InGame g, Form f)
        {
            //MessageBox.Show("Fog of war triggered.");
            ConsoleForm testDialog = new ConsoleForm();
            testDialog.g = g;
            testDialog.Parent = null;
            testDialog.Show();

             

            //MessageBox.Show("Fog of war triggered.");
            //System.Windows.Forms.MessageBox.Show("Selected unit type:" + u.unitType + "TileX:" + u.unitTileWidth + "TileY:" + u.unitTileHeight);
            // Show testDialog as a modal dialog and determine if DialogResult = OK.



        }

   
    }
}
