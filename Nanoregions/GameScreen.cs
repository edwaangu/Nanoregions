using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nanoregions
{
    public partial class GameScreen : UserControl
    {
        /**
         * 
         *  NANOREGIONS - Ted Angus
         * 
         *  - Regions are automatically created, watch them fight to the death!
         *  - Normally can reach 60 FPS easily, including names are ever so slightly laggier
         * 
        */
        // Random
        Random randGen = new Random();
        int tilePixelSize = 5;

        // Regional Lists
        List<List<Tile>> ts = new List<List<Tile>>();
        List<Region> regions = new List<Region>();

        // Turns
        int turn = -1;
        int mainTurn = 0;

        // Titles
        int averageTitleNum = 0;
        int averageTitleNext = 0;

        // FPS RELATED VARIABLES
        int framesSinceLastSecond = 0;
        int accurateSec = -1;
        int lastSec = -1;
        int fps = 0;

        List<Point> turnPoints = new List<Point>();

        // Color related methods and name
        public struct colorName
        {
            public string name;
            public Color color;
            public SolidBrush sb;

            public colorName(string _name, Color _color)
            {
                name = _name;
                color = _color;
                sb = new SolidBrush(color);
            }
        }

        SolidBrush blankBrush = new SolidBrush(Color.White);
        SolidBrush notlandBrush = new SolidBrush(Color.FromArgb(105, 191, 244));

        public colorName[] cNames = 
        {
            new colorName("Red", Color.FromArgb(255, 0, 0)),
            new colorName("Scarlet", Color.FromArgb(255, 100, 0)),
            new colorName("Orange", Color.FromArgb(255, 200, 0)),
            new colorName("Yellow", Color.FromArgb(255, 255, 0)),
            new colorName("Yellow-Lime", Color.FromArgb(200, 255, 0)),
            new colorName("Lime", Color.FromArgb(0, 255, 0)),
            new colorName("Limeish-Aqua", Color.FromArgb(0, 255, 150)),
            new colorName("Aqua", Color.FromArgb(0, 255, 255)),
            new colorName("Sky Blue", Color.FromArgb(0, 100, 255)),
            new colorName("Blue", Color.FromArgb(0, 0, 255)),
            new colorName("Indigo", Color.FromArgb(100, 0, 255)),
            new colorName("Purple", Color.FromArgb(200, 0, 255)),
            new colorName("Magenta", Color.FromArgb(255, 0, 255)),
            new colorName("Rose Pink", Color.FromArgb(255, 0, 100)),

            new colorName("Maroon", Color.FromArgb(150, 0, 0)),
            new colorName("Dark Scarlet", Color.FromArgb(150, 50, 0)),
            new colorName("Brown", Color.FromArgb(150, 100, 0)),
            new colorName("Dark Yellow", Color.FromArgb(150, 150, 0)),
            new colorName("Lime Green", Color.FromArgb(100, 150, 0)),
            new colorName("Green", Color.FromArgb(0, 150, 0)),
            new colorName("Sea Green", Color.FromArgb(0, 150, 100)),
            new colorName("Teal", Color.FromArgb(0, 150, 150)),
            new colorName("Dark Aqua", Color.FromArgb(0, 100, 150)),
            new colorName("Darker Blue", Color.FromArgb(0, 50, 150)),
            new colorName("Darkest Blue", Color.FromArgb(0, 0, 150)),
            new colorName("Dark Indigo", Color.FromArgb(50, 0, 150)),
            new colorName("Dark Purple", Color.FromArgb(100, 0, 150)),
            new colorName("Dark Magenta", Color.FromArgb(150, 0, 150)),
            new colorName("Dark Pink", Color.FromArgb(150, 0, 100)),
            new colorName("Dark Rose Pink", Color.FromArgb(150, 0, 50)),
        };

        StringFormat textCenter = new StringFormat();

        void SetupBoard()
        {
            // Create the board
            //for (int i = 0; i < 150; i++)
            //{
            //    ts.Add(new List<Tile>());
            //    for (int j = 0; j < 100; j++)
            //    {
            //        ts[i].Add(new Tile(100, new Point(i, j), "Blank", Color.White, -1));
            //    }
            //}

            Bitmap mapImage = new Bitmap("Resources/faireuropeheatmap.png");

            for (int i = 0; i < mapImage.Width; i++)
            {
                ts.Add(new List<Tile>());
                for (int j = 0; j < mapImage.Height; j++)
                {
                    if (mapImage.GetPixel(i, j) != Color.FromArgb(70, 119, 234))
                    {
                        ts[i].Add(new Tile(100, new Point(i, j), "Blank", Color.White, -1, false));
                        ts[i][j].population = Convert.ToInt32(1 + (255 - mapImage.GetPixel(i, j).R));
                    }
                    else
                    {
                        ts[i].Add(new Tile(1, new Point(i, j), "Blank", Color.White, -1, true));
                    }
                }
            }

            // Put sixteen random colors
            int colorsCurrent = 0;

            while (colorsCurrent < cNames.Length)
            {
                int attemptPlaceX = randGen.Next(0, ts.Count);
                int attemptPlaceY = randGen.Next(0, ts[0].Count);

                if (ts[attemptPlaceX][attemptPlaceY].regionName == "Blank" && ts[attemptPlaceX][attemptPlaceY].isNotLand == false)
                {
                    ts[attemptPlaceX][attemptPlaceY].regionName = cNames[colorsCurrent].name;
                    ts[attemptPlaceX][attemptPlaceY].color = cNames[colorsCurrent].color;
                    ts[attemptPlaceX][attemptPlaceY].colorid = colorsCurrent;

                    regions.Add(new Region(cNames[colorsCurrent].name, cNames[colorsCurrent].color, colorsCurrent));

                    regions[colorsCurrent].averageX = attemptPlaceX;
                    regions[colorsCurrent].averageY = attemptPlaceY;

                    colorsCurrent++;
                }
            }

            foreach (Region region in regions) {
                region.CalculateInformation(ts);
            }
        }

        public GameScreen()
        {
            InitializeComponent();

            SetupBoard();

            textCenter.Alignment = StringAlignment.Center;
            textCenter.LineAlignment = StringAlignment.Center;
        }

        private void updateTick_Tick(object sender, EventArgs e)
        {
            // Refresh
            this.Refresh();

            // Framerate
            framesSinceLastSecond++;
            accurateSec = DateTime.Now.Second;
            if (lastSec != accurateSec)
            {
                lastSec = accurateSec;
                fps = framesSinceLastSecond;
                framesSinceLastSecond = 0;
            }

            for (int s = 0; s < 800; s++)
            {

                if (turn == -1)
                {
                    turnPoints.Clear();
                    for (int i = 0; i < ts.Count; i++)
                    {
                        for (int j = 0; j < ts[i].Count; j++)
                        {
                            if (ts[i][j].regionName != "Blank")
                            {
                                ts[i][j].turnId = turnPoints.Count;
                                turnPoints.Add(new Point(i, j));
                            }
                        }
                    }

                    List<Point> tempTurnPoints = new List<Point>();
                    while (turnPoints.Count > 0)
                    {
                        int randnt = randGen.Next(0, turnPoints.Count);
                        tempTurnPoints.Add(turnPoints[randnt]);
                        turnPoints.RemoveAt(randnt);
                    }

                    turnPoints = tempTurnPoints.ToList();

                    turn = 0;
                    mainTurn++;
                }
                else
                {
                    ts = ts[turnPoints[turn].X][turnPoints[turn].Y].attemptTileTakeover(ts, randGen, regions);

                    //foreach (Region region in regions)
                    //{
                    //    region.CalculateInformation(ts);
                    //}

                    turn++;
                    if (turn >= turnPoints.Count)
                    {
                        turn = -1;
                    }
                }
            }

            averageTitleNext -= 1;
            if(averageTitleNext <= 0)
            {
                averageTitleNext = 4;
                regions[averageTitleNum].CalculateAveragePosition(ts);
                averageTitleNum += 1;
                if(averageTitleNum >= regions.Count)
                {
                    averageTitleNum = 0;
                }
            }

        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            // Tiles
            for(int i = 0;i < ts.Count;i++)
            {
                for(int j = 0;j < ts[i].Count;j++)
                {
                    e.Graphics.FillRectangle(ts[i][j].isNotLand == true ? notlandBrush : (ts[i][j].regionName == "Blank" ? blankBrush : cNames[ts[i][j].colorid].sb), i * tilePixelSize, j * tilePixelSize, tilePixelSize, tilePixelSize);
                    if (ts[i][j].isNotLand && ts[i][j].regionName != "Blank")
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, ts[i][j].color.R, ts[i][j].color.G, ts[i][j].color.B)), i * tilePixelSize, j * tilePixelSize, tilePixelSize, tilePixelSize);
                    }
                }
            }

            List<Region> tempRegions = regions.ToList();


            // Land Area Chart
            float _dir = 0;
            tempRegions = tempRegions.OrderByDescending(f => f.totalRegionSize).ToList();

            float totalCount = 0;
            for (int j = 0; j < tempRegions.Count; j++)
            {
                totalCount += tempRegions[j].totalRegionSize;
            }

            _dir = 0;
            for (int j = 0; j < tempRegions.Count; j++)
            {
                float sizePie = ((tempRegions[j].totalRegionSize / totalCount) * 360);
                e.Graphics.FillPie(new SolidBrush(tempRegions[j].regionColor), this.Width - 160, this.Height / 2 - 90 - 75, 150, 150, _dir - 90, sizePie);

                _dir += sizePie;
            }

            _dir = 0;
            for (int j = 0; j < tempRegions.Count; j++)
            {
                float sizePie = ((tempRegions[j].totalRegionSize / totalCount) * 360);

                if (sizePie > 22)
                {
                    e.Graphics.DrawString(tempRegions[j].regionName, new Font(FontFamily.GenericSansSerif, 9, FontStyle.Regular), new SolidBrush(Color.Black), this.Width - 160 + 75 + Convert.ToSingle(Math.Cos((-90 + _dir + sizePie / 2) / (180 / Math.PI)) * 55), this.Height / 2 - 90 + Convert.ToSingle(Math.Sin((-90 + _dir + sizePie / 2) / (180 / Math.PI)) * 55), textCenter);
                    e.Graphics.DrawString(tempRegions[j].regionName, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.White), this.Width - 160 + 75 + Convert.ToSingle(Math.Cos((-90 + _dir + sizePie / 2) / (180 / Math.PI)) * 55), this.Height / 2 - 90+ Convert.ToSingle(Math.Sin((-90 + _dir + sizePie / 2) / (180 / Math.PI)) * 55), textCenter);
                }

                _dir += sizePie;
            }

            // Population Pie Chart
            _dir = 0;
            tempRegions = tempRegions.OrderByDescending(f => f.totalRegionPopulation).ToList();

            totalCount = 0;
            for (int j = 0; j < tempRegions.Count; j++)
            {
                totalCount += tempRegions[j].totalRegionPopulation;
            }

            _dir = 0;
            for (int j = 0; j < tempRegions.Count; j++)
            {
                float sizePie = ((tempRegions[j].totalRegionPopulation / totalCount) * 360);
                e.Graphics.FillPie(new SolidBrush(tempRegions[j].regionColor), this.Width - 160, this.Height / 2 + 90 - 75, 150, 150, _dir - 90, sizePie);

                _dir += sizePie;
            }


            _dir = 0;
            for (int j = 0; j < tempRegions.Count; j++)
            {
                float sizePie = ((tempRegions[j].totalRegionPopulation / totalCount) * 360);

                if (sizePie > 22)
                {
                    e.Graphics.DrawString(tempRegions[j].regionName, new Font(FontFamily.GenericSansSerif, 9, FontStyle.Regular), new SolidBrush(Color.Black), this.Width - 160 + 75 + Convert.ToSingle(Math.Cos((-90 + _dir + sizePie / 2) / (180 / Math.PI)) * 55), this.Height / 2 + 90 + Convert.ToSingle(Math.Sin((-90 + _dir + sizePie / 2) / (180 / Math.PI)) * 55), textCenter);
                    e.Graphics.DrawString(tempRegions[j].regionName, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.White), this.Width - 160 + 75 + Convert.ToSingle(Math.Cos((-90 + _dir + sizePie / 2) / (180 / Math.PI)) * 55), this.Height / 2 + 90 + Convert.ToSingle(Math.Sin((-90 + _dir + sizePie / 2) / (180 / Math.PI)) * 55), textCenter);
                }

                _dir += sizePie;
            }

            // Titles
            for(int i = 0;i < regions.Count;i++)
            {
                if(regions[i].averageX != 0 && regions[i].averageY != 0 && regions[i].totalRegionSize > 0)
                {
                    e.Graphics.DrawString(regions[i].regionName, new Font(FontFamily.GenericSansSerif, Convert.ToSingle(6 + Math.Sqrt(regions[i].totalRegionSize) / 2), FontStyle.Regular), new SolidBrush(Color.Black), regions[i].averageX * tilePixelSize, regions[i].averageY * tilePixelSize, textCenter);
                }
            }

            // FPS
            e.Graphics.DrawString($"FPS: {fps}", DefaultFont, new SolidBrush(Color.Black), new PointF(this.Width - 50, 10));
        }
    }
}
