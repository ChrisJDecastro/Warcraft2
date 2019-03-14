using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Demo
{
    public class ColorsLoader
    {
        //Reference for coloring: https://stackoverflow.com/questions/3255311/color-replacement-in-xna-c-sharp

        //We need a better way to do this. It takes over 300 MB AND takes extremely long time to load.

        //Coloring variables
        public Color[] defaultColor;
        public Texture2D[][][] allUnitTilesT2dColored; //to hold all shaded unit colors

        public ColorsLoader(GraphicsDevice graphicsDevice, int numPlayers) //init
        {
            allUnitTilesT2dColored = new Texture2D[numPlayers][][]; //init for 8 different colors
            for (int i = 0; i < numPlayers; i++) //only init enough for number of players
            { //manually create all
                allUnitTilesT2dColored[i] = new Texture2D[SplashScreen.mapUnits.allUnitTiles.Length][];
                for (int j = 0; j < SplashScreen.mapUnits.allUnitTiles.Length; j++)
                {
                    allUnitTilesT2dColored[i][j] = new Texture2D[SplashScreen.mapUnits.allUnitTiles[j].Length];
                    for (int k = 0; k < SplashScreen.mapUnits.allUnitTiles[j].Length; k++)
                    {
                        allUnitTilesT2dColored[i][j][k] = XNA_InGame.getTextureFromBitmap(SplashScreen.mapUnits.allUnitTiles[j][k],graphicsDevice);
                    }
                }
            }
            loadAllColors(graphicsDevice, numPlayers);
        }

        public void loadAllColors(GraphicsDevice graphicsDevice, int numPlayers)
        { //Function to load and store selected player color for game //doesn't work because we can't deep copy array

            Texture2D color = XNA_InGame.getTextureFromBitmap(SplashScreen.color, graphicsDevice); //convert color bitmap to tex2D

            //Reference for coloring: https://stackoverflow.com/questions/3255311/color-replacement-in-xna-c-sharp
            Color[] tempColors = new Color[color.Width * color.Height];
            color.GetData(tempColors); //get color data
            this.defaultColor = new Color[color.Width];
            for (int c = 0; c < color.Width; c++) //load default color and save
            {
                this.defaultColor[c] = tempColors[c];
            }

            for (int i = 0; i < numPlayers; i++) //iterate through all colors (except 0 which is default red)
            {
                Color[] chosenColor = new Color[color.Width];
                int playerColorChoice = SplashScreen.playerColor[i];
                if (playerColorChoice != 0) //not default red (as if it is, we don't have to edit texture)
                {
                    for (int c = 0; c < color.Width; c++) //load current color
                    {
                        chosenColor[c] = tempColors[playerColorChoice * color.Width + c];
                    }

                    for (int k = 0; k < this.allUnitTilesT2dColored[i].Length; k++) //iterate through texture2D array
                    {
                        //allUnitTilesT2d[k] = new Texture2D[this.allUnitTilesT2dColored[k].Length];
                        for (int l = 0; l < this.allUnitTilesT2dColored[i][k].Length; l++)
                        {
                            //Color the unit texture Reference: https://stackoverflow.com/questions/3255311/color-replacement-in-xna-c-sharp
                            Texture2D tempImg = this.allUnitTilesT2dColored[i][k][l];
                            Color[] data = new Color[tempImg.Width * tempImg.Height];
                            tempImg.GetData(data); //store sprite data for editing
                            for (int c = 0; c < data.Length; c++) //iterate through and swap all pixels of default color to chosen color
                            {
                                if (data[c] == defaultColor[0]) data[c] = chosenColor[0];
                                else if (data[c] == defaultColor[1]) data[c] = chosenColor[1];
                                else if (data[c] == defaultColor[2]) data[c] = chosenColor[2];
                                else if (data[c] == defaultColor[3]) data[c] = chosenColor[3];
                            }
                            //tempImg.SetData(data); //set the image to edited sprite
                            tempImg.SetData(data);
                            //this.allUnitTilesT2dColored[i][k][l].SetData(data);
                            this.allUnitTilesT2dColored[i][k][l] = tempImg;

                        }
                    }
                }
            }


            
        }

    }
}
