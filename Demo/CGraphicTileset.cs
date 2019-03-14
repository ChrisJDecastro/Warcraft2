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

    public class EntireMapsIndex
    {
        public int[][] data;

        public EntireMapsIndex clone()
        {
            EntireMapsIndex ans = new EntireMapsIndex();
            ans.data = this.data.Clone() as int[][];
            return ans;
        }
    }

    public class maps
    {
        //Map dimensions for map rendering
        public string[][] mapLines; //for holding parsed file for each map
        public int[][] mapPartials; //hold holding parsed partial bits for changing partial later
        public int mapChoice;
        public int[] mapX; //holds each map's x dimension
        public int[] mapY; //holds each map's y dimension
        public int[][] mapData; //holds index of tile in tileset to render of all maps
        public int[][] mapDataIndices; //hold the index of the specific tile of a tileset
        //public int[][] trueMapData; //holds the bit shifted map data indexes
        public string[] mapNames; //holds the map names
        public int[][] DTileIndices; //for holding organized indexes
        //public int[] tileTypeStart; //holds index of wheere each tiletype starts
        public Bitmap[] entireMaps; //hold entire maps
        public EntireMapsIndex[] entireMapsIndex;//store the index of the entire maps, not the bitmap.
        public string[][] allMapStrings;

    }

   

    public class CGraphicTileset
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

        public Tuple<string[], Bitmap[], int, int, int> LoadTileset(string file, string folder) //loads dat file
        { //Function for loading images
            string imgpath = @"data\" + folder + @"\";
            string[] lines = System.IO.File.ReadAllLines(imgpath + file);

            //lines[1] = lines[1].Replace('/', '\\'); //replace ./ by .\ for windows
            lines[1] = lines[1].Substring(2); //remvoes ./

            int numTiles = 0;
            Int32.TryParse(lines[3], out numTiles); // convert numTile string to int
                                                    //Use public Tuple<string[], int> to return both int and string[]

            Bitmap newImage = new Bitmap(imgpath + lines[1]); //get image using line 1 path
            Bitmap[] tiles = new Bitmap[numTiles]; //an array for holding bitmap images

            int tileHeight = newImage.Size.Height / numTiles;
            int tileWidth = newImage.Size.Width;
            int currentHeight = 0;
            //Color transparentColor = Color.FromName("White");
            for (int i = 0; i < numTiles; i++)
            {
                tiles[i] = newImage.Clone(new Rectangle(0, currentHeight, tileWidth, tileHeight), newImage.PixelFormat);
                
                //tiles[i].MakeTransparent(transparentColor); //makes white bg transparent
                currentHeight += tileHeight;
            }

            return Tuple.Create(lines, tiles, numTiles, tileWidth, tileHeight); //return all parsed/loaded data
                                                                                //Image newImage = Image.FromFile(filepath);

            //return lines;
        }

        public Tuple<int, int> CalculateTileTypeAndIndex(int x, int i, int j, int[] DPartials, string[] names)
        {
            int data = 0;
            int index = 0;
            string UL = names[j * (x + 1) + i];
            string UR = names[j * (x + 1) + i + 1];
            string LL = names[(j + 1) * (x + 1) + i];
            string LR = names[(j + 1) * (x + 1) + i + 1];
            //Requires usage of DPartial not DTerrainMap, but can't figure out yet 
            //light-grass, dark-grass, light-dirt, dark-dirt, rock, forest, shallow-water, deep-water, stump, rubble
            int TypeIndex = ((DPartials[j * (x + 1) + i] & 0x8) >> 3) | ((DPartials[j * (x + 1) + i + 1] & 0x4) >> 1) | ((DPartials[(j + 1) * (x + 1) + i] & 0x2) << 1) | ((DPartials[(j + 1) * (x + 1) + i + 1] & 0x1) << 3);

            //Cases
            if (("dark-grass" == UL) || ("dark-grass" == UR) || ("dark-grass" == LL) || ("dark-grass" == LR))
            {
                TypeIndex &= ("dark-grass" == UL) ? 0xF : 0xE;
                TypeIndex &= ("dark-grass" == UR) ? 0xF : 0xD;
                TypeIndex &= ("dark-grass" == LL) ? 0xF : 0xB;
                TypeIndex &= ("dark-grass" == LR) ? 0xF : 0x7;
                //Add the type to the index (Not sure if this is right)
                //data[j * y + i] = DTerrainMap[j * y + i] + TypeIndex;
                data = 1;
                index = TypeIndex;
            }
            else if (("dark-dirt" == UL) || ("dark-dirt" == UR) || ("dark-dirt" == LL) || ("dark-dirt" == LR))
            {
                TypeIndex &= ("dark-dirt" == UL) ? 0xF : 0xE;
                TypeIndex &= ("dark-dirt" == UR) ? 0xF : 0xD;
                TypeIndex &= ("dark-dirt" == LL) ? 0xF : 0xB;
                TypeIndex &= ("dark-dirt" == LR) ? 0xF : 0x7;
                //Add the type to the index (Not sure if this is right)
                //data[j * y + i] = DTerrainMap[j * y + i] + TypeIndex;
                data = 3;
                index = TypeIndex;
            }
            else if (("deep-water" == UL) || ("deep-water" == UR) || ("deep-water" == LL) || ("deep-water" == LR))
            {
                TypeIndex &= ("deep-water" == UL) ? 0xF : 0xE;
                TypeIndex &= ("deep-water" == UR) ? 0xF : 0xD;
                TypeIndex &= ("deep-water" == LL) ? 0xF : 0xB;
                TypeIndex &= ("deep-water" == LR) ? 0xF : 0x7;
                //Add the type to the index (Not sure if this is right)
                //data[j * y + i] = DTerrainMap[j * y + i] + TypeIndex;
                data = 7;
                index = TypeIndex;
            }
            else if (("shallow-water" == UL) || ("shallow-water" == UR) || ("shallow-water" == LL) || ("shallow-water" == LR))
            {
                TypeIndex &= ("shallow-water" == UL) ? 0xF : 0xE;
                TypeIndex &= ("shallow-water" == UR) ? 0xF : 0xD;
                TypeIndex &= ("shallow-water" == LL) ? 0xF : 0xB;
                TypeIndex &= ("shallow-water" == LR) ? 0xF : 0x7;
                //Add the type to the index (Not sure if this is right)
                //data[j * y + i] = DTerrainMap[j * y + i] + TypeIndex;
                data = 6;
                index = TypeIndex;
            }
            else if (("rock" == UL) || ("rock" == UR) || ("rock" == LL) || ("rock" == LR))
            {
                TypeIndex &= ("rock" == UL) ? 0xF : 0xE;
                TypeIndex &= ("rock" == UR) ? 0xF : 0xD;
                TypeIndex &= ("rock" == LL) ? 0xF : 0xB;
                TypeIndex &= ("rock" == LR) ? 0xF : 0x7;
                //Add the type to the index (Not sure if this is right)
                //Have to have type rubble or rock (no rubble type yet)
                index = TypeIndex;
                if (TypeIndex == 0) //1st index is broken rock
                {
                    data = 9; //for rubble
                }
                else data = 4; //for rock
            }
            else if (("forest" == UL) || ("forest" == UR) || ("forest" == LL) || ("forest" == LR))
            {
                TypeIndex &= ("forest" == UL) ? 0xF : 0xE;
                TypeIndex &= ("forest" == UR) ? 0xF : 0xD;
                TypeIndex &= ("forest" == LL) ? 0xF : 0xB;
                TypeIndex &= ("forest" == LR) ? 0xF : 0x7;
                //Add the type to the index (Not sure if this is right)
                //Need to check type index for stump
                index = TypeIndex;
                if (TypeIndex == 0) //first index is a chopped tree
                {
                    data = 8; //stump
                }
                else data = 5;
            }
            else if (("light-dirt" == UL) || ("light-dirt" == UR) || ("light-dirt" == LL) || ("light-dirt" == LR))
            {
                TypeIndex &= ("light-dirt" == UL) ? 0xF : 0xE;
                TypeIndex &= ("light-dirt" == UR) ? 0xF : 0xD;
                TypeIndex &= ("light-dirt" == LL) ? 0xF : 0xB;
                TypeIndex &= ("light-dirt" == LR) ? 0xF : 0x7;
                //Add the type to the index (Not sure if this is right)
                //data[j * y + i] = DTerrainMap[j * y + i] + TypeIndex;
                data = 2;
                index = TypeIndex;
            }
            else //it says error?
            {
                //TypeIndex = 0xF; //Professor code
                TypeIndex = 0xF;
                //data[j * y + i] = DTerrainMap[j * y + i] + TypeIndex;
                data = 0; //Light grass
                index = TypeIndex;
            }

            return Tuple.Create(data, index);

        }
        public Tuple<int[], int[]> bitShift(int[] DPartials, string[] names, int x, int y) //DTerrainMap is passed in default parsed value of map
        {//Calculate tile and index
            //DTerrainMap is actuall DPartials, names is the DTerrainMaps
            int[] data = new int[x * y]; //for holding tile type
            int[] indexes = new int[x * y]; //for holding specific index of a tile in tile type (0-15)
            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++) //iterate through all points
                {
                    Tuple<int, int> tempRet = CalculateTileTypeAndIndex(x, i, j, DPartials, names);
                    data[j * x + i] = tempRet.Item1;
                    indexes[j * x + i] = tempRet.Item2;
                }
            }

            return Tuple.Create(data, indexes);
        }
        public int StringMap(char indexInner) //helper function of mapping strings to DPartial
        {
            int value = 0;
            if (('0' <= indexInner) && ('9' >= indexInner))
            {
                value = indexInner - '0';
            }
            else if (('A' <= indexInner) && ('F' >= indexInner))
            {
                value = indexInner - 'A' + 0x0A;
            }
            //else goto loadexit aka return
            return value;
        }
        public Tuple<int[][], int[][], string[], int[][], string[][], int[][], string[][]> LoadMap(string folder, string[] tilesetData, int[][] DTileIndices) //folder name is map by default, tilesetData is the array holding names for tiles
        { //Function to load all map in a certain folder
            DirectoryInfo dir = new DirectoryInfo(@"data\" + folder);
            FileInfo[] allFiles = dir.GetFiles("*.map"); //get all .map files only
            int numFiles = allFiles.Length; //get the number of files that we have to create array
            int[][] mapData = new int[numFiles][]; //create array of array of numFiles elements
            int[][] mapDataIndices = new int[numFiles][];
            int[][] DPartials = new int[numFiles][]; //create array of array of numFiles elements
            string[] mapNames = new string[numFiles];
            int[][] mapXY = new int[2][];
            mapXY[0] = new int[numFiles];
            mapXY[1] = new int[numFiles];
            string[][] mapStrings = new string[numFiles][];
            string[][] allLines = new string[numFiles][];

            //int[][] trueMapData = new int[numFiles][]; //create array of array of numFiles elements

            for (int i = 0; i < numFiles; i++) //iterate through all files and load all maps
            {
                //Init map containers
                string filepath = @"data\" + folder + @"\";
                string[] lines = System.IO.File.ReadAllLines(filepath + allFiles[i]); //reads entire file
                allLines[i] = lines;
                
                mapNames[i] = lines[1]; //copy map name over
                string[] dimToken = lines[3].Split(); //split the two dimensions apart
                int[] dimNum = Array.ConvertAll(dimToken, int.Parse); //converet numbers into dimensions
                mapXY[0][i] = dimNum[0]; //set the first value to x
                mapXY[1][i] = dimNum[1];
                //get the total dimension for the map array
                int mapSize = (dimNum[0]) * (dimNum[1]);
                int fileMapSize = (dimNum[0] + 1) * (dimNum[1] + 1); //plus one because there are actually one more data than dimension in map data file
                mapData[i] = new int[mapSize]; //init array for holding map data
                mapDataIndices[i] = new int[mapSize];
                DPartials[i] = new int[fileMapSize]; //init array for holding map partial bit data from file
                string[] DTerrainData = new string[fileMapSize]; //array for holding name for each from file

                //mapStrings[i] = new string[dimNum[1] + 1];

                for (int j = 5; j < dimNum[1] + 5 + 1; j++) //iterate through all lines of map data in file (starts at line 5 always)
                {//the total checks should equal mapSize + 1 (as there is one extra line)
                    int currentChar = 0, currentChar2 = 0;
                    //mapStrings[i][j - 5] = lines[j];
                    foreach (char c in lines[j]) //check each char on every line one by one
                    { //we might want to have a bitmap check for this but not sure how to do it yet 
                        switch (c) //how it was done in Professor's code
                        {
                            case 'G': //DarkGrass
                                DTerrainData[(j - 5) * (dimNum[0] + 1) + currentChar] = "dark-grass";
                                break;
                            case 'g': //LightGrass
                                DTerrainData[(j - 5) * (dimNum[0] + 1) + currentChar] = "light-grass";
                                break;
                            case 'D': //DarkDirt
                                DTerrainData[(j - 5) * (dimNum[0] + 1) + currentChar] = "dark-dirt";
                                break;
                            case 'd': //LightDirt
                                DTerrainData[(j - 5) * (dimNum[0] + 1) + currentChar] = "light-dirt";
                                break;
                            case 'R': //Rock
                                DTerrainData[(j - 5) * (dimNum[0] + 1) + currentChar] = "rock";
                                break;
                            case 'r': //RockPartial (not quite sure which tiles these are) (maybe rock-UK?)
                                DTerrainData[(j - 5) * (dimNum[0] + 1) + currentChar] = "rock-partial";
                                //DTerrainData[(j - 5) * dimNum[1] + currentChar] = "rock";
                                break;
                            case 'F': //Forest
                                DTerrainData[(j - 5) * (dimNum[0] + 1) + currentChar] = "forest";
                                break;
                            case 'f': //ForestPartial (maybe forest-UK?)
                                DTerrainData[(j - 5) * (dimNum[0] + 1) + currentChar] = "forest-partial";
                                break;
                            case 'W': //DeepWater
                                DTerrainData[(j - 5) * (dimNum[0] + 1) + currentChar] = "deep-water";
                                break;
                            case 'w': //ShallowWater
                                DTerrainData[(j - 5) * (dimNum[0] + 1) + currentChar] = "shallow-water";
                                break;
                            default:
                                //MessageBox.Show("Invalid Map Data.");
                                DTerrainData[(j - 5) * (dimNum[0] + 1) + currentChar] = "light-grass";
                                break;
                        }
                        currentChar++; //increment character so we move on to next spot in mapData array
                    }
                    foreach (char c in lines[j + dimNum[1] + 1 + 1]) //parse the Partial bits +1 for extra line, +1 for comment
                    {
                        DPartials[i][(j - 5) * (dimNum[0] + 1) + currentChar2] = StringMap(c);
                        currentChar2++;
                    }
                }

                Tuple<int[], int[]> tempMapStruct = bitShift(DPartials[i], DTerrainData, dimNum[0], dimNum[1]);
                mapStrings[i] = DTerrainData;
                mapData[i] = tempMapStruct.Item1;
                mapDataIndices[i] = tempMapStruct.Item2;
            }

            return Tuple.Create(mapData, mapDataIndices, mapNames, mapXY, mapStrings, DPartials, allLines);
        }

        public int[][] OrganizeTiles(string[] searchArea)
        {
            //Group orders 0-8
            //light-grass, dark-grass, light-dirt, dark-dirt, rock, forest, shallow-water, deep-water, stump, rubble
            int[][] tileset = new int[10][];
            int tileIndex = 0;
            for (int i = 0; i < 10; i++) //initialize lists
            {
                tileset[i] = new int[16];
                /*for (int j = 0; j < 16; j++)
                {
                    tileset[i][j] = new List<int>();
                }*/
            }

            for (int Index = 0; Index < 16; Index++)
            {
                int AltTileIndex;
                string TempStringStream;
                TempStringStream = Index.ToString("X");

                AltTileIndex = 0;
                while (true)
                {
                    int Value = findString("light-grass-" + TempStringStream + "-" + AltTileIndex.ToString(), searchArea); //minus 5 for file metadata at top
                    if (0 > Value)
                    {
                        break;
                    }
                    tileset[0][Index] = Value; //adds index to list at group 0
                    AltTileIndex++;
                    tileIndex++;
                }

                AltTileIndex = 0;
                while (true)
                {
                    int Value = findString("dark-grass-" + TempStringStream + "-" + AltTileIndex.ToString(), searchArea); //minus 5 for file metadata at top
                    if (0 > Value)
                    {
                        break;
                    }
                    //tileset[1][Index].Add(Value); //adds index to list at group 0
                    tileset[1][Index] = Value;
                    AltTileIndex++;
                    tileIndex++;
                }

                AltTileIndex = 0;
                while (true)
                {
                    int Value = findString("light-dirt-" + TempStringStream + "-" + AltTileIndex.ToString(), searchArea); //minus 5 for file metadata at top
                    if (0 > Value)
                    {
                        break;
                    }
                    //tileset[2][Index].Add(Value); //adds index to list at group 0
                    tileset[2][Index] = Value;
                    AltTileIndex++;
                    tileIndex++;
                }

                AltTileIndex = 0;
                while (true)
                {
                    int Value = findString("dark-dirt-" + TempStringStream + "-" + AltTileIndex.ToString(), searchArea); //minus 5 for file metadata at top
                    if (0 > Value)
                    {
                        break;
                    }
                    //tileset[3][Index].Add(Value); //adds index to list at group 0
                    tileset[3][Index] = Value;
                    AltTileIndex++;
                    tileIndex++;
                }

                AltTileIndex = 0;
                while (true)
                {
                    int Value = findString("rock-" + TempStringStream + "-" + AltTileIndex.ToString(), searchArea); //minus 5 for file metadata at top
                    if (0 > Value)
                    {
                        break;
                    }
                    //tileset[4][Index].Add(Value); //adds index to list at group 0
                    tileset[4][Index] = Value;
                    AltTileIndex++;
                    tileIndex++;
                }

                AltTileIndex = 0;
                while (true)
                {
                    int Value = findString("forest-" + TempStringStream + "-" + AltTileIndex.ToString(), searchArea); //minus 5 for file metadata at top
                    if (0 > Value)
                    {
                        break;
                    }
                    //tileset[5][Index].Add(Value); //adds index to list at group 0
                    tileset[5][Index] = Value;
                    AltTileIndex++;
                    tileIndex++;
                }

                AltTileIndex = 0;
                while (true)
                {
                    int Value = findString("shallow-water-" + TempStringStream + "-" + AltTileIndex.ToString(), searchArea); //minus 5 for file metadata at top
                    if (0 > Value)
                    {
                        break;
                    }
                    //tileset[6][Index].Add(Value); //adds index to list at group 0
                    tileset[6][Index] = Value;
                    AltTileIndex++;
                    tileIndex++;
                }

                AltTileIndex = 0;
                while (true)
                {
                    int Value = findString("deep-water-" + TempStringStream + "-" + AltTileIndex.ToString(), searchArea); //minus 5 for file metadata at top
                    if (0 > Value)
                    {
                        break;
                    }
                    //tileset[7][Index].Add(Value); //adds index to list at group 0
                    tileset[7][Index] = Value;
                    AltTileIndex++;
                    tileIndex++;
                }

                AltTileIndex = 0;
                while (true)
                {
                    int Value = findString("stump-" + TempStringStream + "-" + AltTileIndex.ToString(), searchArea); //minus 5 for file metadata at top
                    if (0 > Value)
                    {
                        break;
                    }
                    //tileset[8][Index].Add(Value); //adds index to list at group 0
                    tileset[8][Index] = Value;
                    AltTileIndex++;
                    tileIndex++;
                }

            }

            for (int Index = 0; Index < 16; Index++) //for rubble just like in C++ code
            {
                //tileset[9][Index].Add(tileset[4][Index][0]);
                tileset[9][Index] = tileset[4][0]; //set to first of rock indexes
                tileIndex++;
            }

            //MessageBox.Show(tileIndex.ToString());
            return tileset;
        }

        public EntireMapsIndex generateMapIndex(int mapIndex, loader tileObj, maps mapObj)
        {
            int x = mapObj.mapX[mapIndex];
            int y = mapObj.mapY[mapIndex];

            int tileW = tileObj.tileWidth;
            int tileH = tileObj.tileHeight;
            EntireMapsIndex ans = new EntireMapsIndex();
            ans.data = new int[y][];
            for (int i = 0;i < y;i++)
            {
                ans.data[i] = new int[x];
            }

            //Bitmap mapImage = new Bitmap(x * tileW, y * tileH);
            //Image tempImg = mapImage;

                for (int i = 0; i < x * y; i++) //y-1 to skip last row
                {
                    int currPos = i;
                    int currRow = 0;
                    int typeIndex = mapObj.mapData[mapIndex][i];
                    int tileIndex = mapObj.mapDataIndices[mapIndex][i];
                    //True img is the index of tiles directly from the file.dat (but stored inside an array)
                    int trueImgIndex = mapObj.DTileIndices[typeIndex][tileIndex];
                    while (currPos >= x) //x-1 to skip last column
                    {
                        currPos -= x;
                        currRow += 1;
                    }
                ans.data[currRow][currPos] = trueImgIndex;
                    /*g.DrawImage(tileObj.tiles[trueImgIndex], new Rectangle(currPos * tileW, currRow * tileH, tileW, tileH),  // destination rectangle
                    0,                          // source rectangle x 
                    0,                          // source rectangle y
                    tileW,                        // source rectangle width
                    tileH,                       // source rectangle height
                    GraphicsUnit.Pixel);*/
                }


            return ans;
        }
        public Bitmap generateMiniMap(int mapIndex, loader tileObj, maps mapObj)
        {
            int x = mapObj.mapX[mapIndex];
            int y = mapObj.mapY[mapIndex];

            int tileW = tileObj.tileWidth;
            int tileH = tileObj.tileHeight;
            Bitmap mapImage = new Bitmap(x, y);
            //Image tempImg = mapImage;
           // using (Graphics g = Graphics.FromImage(mapImage))
            {
                for (int i = 0; i < x * y; i++) //y-1 to skip last row
                {
                    int currPos = i;
                    int currRow = 0;
                    int typeIndex = mapObj.mapData[mapIndex][i];
                    int tileIndex = mapObj.mapDataIndices[mapIndex][i];
                    //True img is the index of tiles directly from the file.dat (but stored inside an array)
                    int trueImgIndex = mapObj.DTileIndices[typeIndex][tileIndex];
                    while (currPos >= x) //x-1 to skip last column
                    {
                        currPos -= x;
                        currRow += 1;
                    }

                    mapImage.SetPixel(currPos,currRow, tileObj.tiles[trueImgIndex].GetPixel(15,15));
                   

                }
                
            }
            //mapImage = mapImage.Clone(new Rectangle(0,0,x-1,y-1),mapImage.PixelFormat);

            return mapImage;
        }
        public Bitmap generateMap(int mapIndex, loader tileObj, maps mapObj)
        {
            int x = mapObj.mapX[mapIndex];
            int y = mapObj.mapY[mapIndex];

            int tileW = tileObj.tileWidth;
            int tileH = tileObj.tileHeight;
            Bitmap mapImage = new Bitmap(x * tileW, y * tileH);
            Image tempImg = mapImage;
            using (Graphics g = Graphics.FromImage(mapImage))
            {
                for (int i = 0; i < x * y; i++) //y-1 to skip last row
                {
                    int currPos = i;
                    int currRow = 0;
                    int typeIndex = mapObj.mapData[mapIndex][i];
                    int tileIndex = mapObj.mapDataIndices[mapIndex][i];
                    //True img is the index of tiles directly from the file.dat (but stored inside an array)
                    int trueImgIndex = mapObj.DTileIndices[typeIndex][tileIndex];
                    while (currPos >= x) //x-1 to skip last column
                    {
                        currPos -= x;
                        currRow += 1;
                    }
 
                    g.DrawImage(tileObj.tiles[trueImgIndex], new Rectangle(currPos * tileW, currRow * tileH, tileW, tileH),  // destination rectangle
                    0,                          // source rectangle x 
                    0,                          // source rectangle y
                    tileW,                        // source rectangle width
                    tileH,                       // source rectangle height
                    GraphicsUnit.Pixel);
                }
                g.Save();
            }

            return mapImage;
        }

    }
}

