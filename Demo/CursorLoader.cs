using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    //Created by AZ, used to load cursor images as static bitmaps
    class CursorLoader
    {
        public static Bitmap curPointer;
        public static Bitmap curMagnify;
        public static Bitmap curUp;
        public static Bitmap curRight;
        public static Bitmap curDown;
        public static Bitmap curLeft;
        public static Bitmap curGrn;
        public static Bitmap curRed;

        static public void initialize()
        {
            //InitializeComponent();
            Bitmap cursors = new Bitmap(@"data\img\Cursors.png");
            Bitmap stretch = new Bitmap(4 * cursors.Size.Width, 4 * cursors.Size.Height / 8);
            curPointer = cursors.Clone(new Rectangle(0, 0, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            //curPointer = Overlay(stretch, curPointer, 64, 64);
            curMagnify = cursors.Clone(new Rectangle(0, cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            //curMagnify = Overlay(stretch, curMagnify, 64, 64);
            curUp = cursors.Clone(new Rectangle(0, 2 * cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            //curUp = Overlay(stretch, curUp, 64, 64);
            curRight = cursors.Clone(new Rectangle(0, 3 * cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            // curRight = Overlay(stretch, curRight, 64, 64);
            curDown = cursors.Clone(new Rectangle(0, 4 * cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            // curDown = Overlay(stretch, curDown, 64, 64);
            curLeft = cursors.Clone(new Rectangle(0, 5 * cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            // curLeft = Overlay(stretch, curLeft, 64, 64);
            curGrn = cursors.Clone(new Rectangle(0, 6 * cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            //curGrn = Overlay(stretch, curGrn, 64, 64);
            curRed = cursors.Clone(new Rectangle(0, 7 * cursors.Size.Height / 8, cursors.Size.Width, cursors.Size.Height / 8), cursors.PixelFormat);
            //curRed = Overlay(stretch, curRed, 64, 64);
        }
    }
}