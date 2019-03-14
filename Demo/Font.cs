using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Demo
{
    public class Font
    {
        public int findString(string value, string[] searchArea)
        {//searchs a list of parsed strings by line and returns index of its tile img
            int index = 0;
            foreach (string line in searchArea)
            {
                if (line.Contains(value))
                { //found string
                    return index - 5; //minus 5 as offset for line file to tilesetarray
                }
                else //was not current line so move on
                {
                    index++;
                }
            }
            return -1;
        }

        public Bitmap[] addText(string value, Bitmap[] outputChar)
        {
            Bitmap[] word = new Bitmap[value.Length];
            string[] stringArray = { " ","!","##","$","%","&","'","(",")","*","+",",","-",".","/",
                                "0","1","2","3","4","5","6","7","8","9",
                                ":",";","<","=",">","?","@",
                                "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
                                "[","\\","]","^","_","`",
                                "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
                                "{","|","}","~"};


            for (int i = 0; i < value.Length; i++)
            {
                for (int j = 0; j < stringArray.Length; j++)
                {
                    if (value[i].ToString() == stringArray[j])
                    {
                        word[i] = outputChar[j];
                        //Console.WriteLine(test[i]);
                        //images[i] = stringArray2[j];
                    }
                }


            }
            return word;//dont know how to output a bitmap array

        }



        public Tuple<string[], Bitmap[], int, int, int> LoadFont(string file, string folder) //loads dat file
        { 
            CGraphicTileset fontLoader = new CGraphicTileset();
            Tuple<string[], Bitmap[], int, int, int> fontData = fontLoader.LoadTileset(file, folder);
            return fontData;

        }
    }
}