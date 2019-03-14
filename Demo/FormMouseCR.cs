using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Demo
{
    public partial class FormMouseCR : Form
    {
        private Point curPos;
        int mouseLeft = 0;
        int mouseRight = 0;
        Bitmap curPointer;
        Bitmap curMagnify;
        Bitmap curUp;
        Bitmap curRight;
        Bitmap curDown;
        Bitmap curLeft;
        Bitmap curGrn;
        Bitmap curRed;

        public FormMouseCR()
        {
            InitializeComponent();
            Bitmap cursors = new Bitmap(@"data\img\Cursors.png");
            Bitmap stretch = new Bitmap(4 * cursors.Size.Width, 4 * cursors.Size.Height / 8);
            curPointer = cursors.Clone(new Rectangle(0, 0, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            curPointer = Overlay(stretch, curPointer, 64, 64);
            curMagnify = cursors.Clone(new Rectangle(0, cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            curMagnify = Overlay(stretch, curMagnify, 64, 64);
            curUp = cursors.Clone(new Rectangle(0, 2 * cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            curUp = Overlay(stretch, curUp, 64, 64);
            curRight = cursors.Clone(new Rectangle(0, 3 * cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            curRight = Overlay(stretch, curRight, 64, 64);
            curDown = cursors.Clone(new Rectangle(0, 4 * cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            curDown = Overlay(stretch, curDown, 64, 64);
            curLeft = cursors.Clone(new Rectangle(0, 5 * cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            curLeft = Overlay(stretch, curLeft, 64, 64);
            curGrn = cursors.Clone(new Rectangle(0, 6 * cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            curGrn = Overlay(stretch, curGrn, 64, 64);
            curRed = cursors.Clone(new Rectangle(0, 7 * cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            curRed = Overlay(stretch, curRed, 64, 64);
            this.Cursor = new Cursor(curPointer.GetHicon());
        }

        private void FormMouseCR_Load(object sender, EventArgs e)
        {
            //Cursor.Hide(); // Hides the cursor because we are going to paint over it
        }

        private void FormMouseCR_MouseMove(object sender, MouseEventArgs e)
        {
            curPos = e.Location;
            //this.Invalidate();
            
            label1.Text = "e.X = " + e.X + " ; e.Y = " + e.Y;
            label2.Text = "HotSpot.X = " + this.Cursor.HotSpot.X + " ; HotSpot.Y = " + this.Cursor.HotSpot.Y;
        }

        private void FormMouseCR_Paint(object sender, PaintEventArgs e)
        {

        }

 

        private void FormMouseCR_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseLeft = 1;
                mouseRight = 0;
                label3.Text = "Left Button : " + mouseLeft;
                label4.Text = "Right Button : " + mouseRight;
            }

            if (e.Button == MouseButtons.Right)
            {
                mouseRight = 1;
                mouseLeft = 0;
                label4.Text = "Right Button : " + mouseRight;
                label3.Text = "Left Button : " + mouseLeft;
            }
        }

        private void FormMouseCR_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseLeft = 0;
                label3.Text = "Left Button : " + mouseLeft;
            }

            if (e.Button == MouseButtons.Right)
            {
                mouseRight = 0;
                label4.Text = "Right Button : " + mouseRight;
            }
        }

        /*
         * Takes a large bitmap and imposes a smaller bitmap at the coordinates provided by left (x) and top (y) of the small bmp
         * */
        public Bitmap Overlay(Bitmap back, Bitmap front, int x, int y)
        {
            Graphics gfx = Graphics.FromImage(back);
            gfx.DrawImage(front, new Point(x, y));
            return back;
        }
    }
}
