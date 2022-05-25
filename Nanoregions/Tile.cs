using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Nanoregions
{
    internal class Tile
    {
        public float population;
        public Point pos;

        public string regionName;
        public Color color;
        public int colorid;

        public bool isNotLand;

        public int turnId = -1;

        public Tile(int _population, Point _pos, string _regionName, Color _color, int _colorid, bool _isNotLand)
        {
            population = _population;
            pos = _pos;
            regionName = _regionName;
            color = _color;
            colorid = _colorid;
            isNotLand = _isNotLand;
        }

        public int getRegionID(List<Region> _regions)
        {
            for(int i = 0;i < _regions.Count;i++)
            {
                if (_regions[i].regionName == regionName)
                {
                    return i;
                }
            }
            return -1;
        }

        public Point chooseTile(List<List<Tile>> _ts, Random _randGen)
        {
            List<string> possibleDirections = new List<string> { "left", "right", "up", "down" };

            // Left
            if(pos.X <= 0)
            {
                possibleDirections[0] = "";
            }
            else
            {
                if (_ts[pos.X - 1][pos.Y].regionName == "Notland" || _ts[pos.X - 1][pos.Y].regionName == regionName)
                {
                    possibleDirections[0] = "";
                }
            }

            // Right
            if (pos.X >= _ts.Count - 1)
            {
                possibleDirections[1] = "";
            }
            else
            {
                if (_ts[pos.X + 1][pos.Y].regionName == "Notland" || _ts[pos.X + 1][pos.Y].regionName == regionName)
                {
                    possibleDirections[1] = "";
                }
            }
            
            // Up
            if (pos.Y <= 0)
            {
                possibleDirections[2] = "";
            }
            else
            {
                if (_ts[pos.X][pos.Y - 1].regionName == "Notland" || _ts[pos.X][pos.Y - 1].regionName == regionName)
                {
                    possibleDirections[2] = "";
                }
            }

            // Down
            if (pos.Y >= _ts[0].Count - 1)
            {
                possibleDirections[3] = "";
            }
            else
            {
                if (_ts[pos.X][pos.Y + 1].regionName == "Notland" || _ts[pos.X][pos.Y + 1].regionName == regionName)
                {
                    possibleDirections[3] = "";
                }
            }

            for(int i = possibleDirections.Count - 1; i >= 0; i--)
            {
                if (possibleDirections[i] == "")
                {
                    possibleDirections.RemoveAt(i);
                }
            }

            if(possibleDirections.Count == 0)
            { // If all else fails, don't move
                return new Point(-1, -1);
            }

            int randomDir = _randGen.Next(0, possibleDirections.Count);

            if (possibleDirections[randomDir] == "left")
            {
                return new Point(pos.X - 1, pos.Y);
            }

            else if (possibleDirections[randomDir] == "right")
            {
                return new Point(pos.X + 1, pos.Y);
            }

            else if (possibleDirections[randomDir] == "up")
            {
                return new Point(pos.X, pos.Y - 1);
            }

            else // "down"
            {
                return new Point(pos.X, pos.Y + 1);
            }
        }


        public Point chooseNotLandTile(List<List<Tile>> _ts, Random _randGen)
        {
            List<string> possibleDirections = new List<string> { "left", "right", "up", "down" };

            // Left
            if (pos.X <= 0)
            {
                possibleDirections[0] = "";
            }
            else
            {
                if (_ts[pos.X - 1][pos.Y].regionName == regionName)
                {
                    possibleDirections[0] = "";
                }
            }

            // Right
            if (pos.X >= _ts.Count - 1)
            {
                possibleDirections[1] = "";
            }
            else
            {
                if (_ts[pos.X + 1][pos.Y].regionName == regionName)
                {
                    possibleDirections[1] = "";
                }
            }

            // Up
            if (pos.Y <= 0)
            {
                possibleDirections[2] = "";
            }
            else
            {
                if (_ts[pos.X][pos.Y - 1].regionName == regionName)
                {
                    possibleDirections[2] = "";
                }
            }

            // Down
            if (pos.Y >= _ts[0].Count - 1)
            {
                possibleDirections[3] = "";
            }
            else
            {
                if (_ts[pos.X][pos.Y + 1].regionName == regionName)
                {
                    possibleDirections[3] = "";
                }
            }

            for (int i = possibleDirections.Count - 1; i >= 0; i--)
            {
                if (possibleDirections[i] == "")
                {
                    possibleDirections.RemoveAt(i);
                }
            }

            if (possibleDirections.Count == 0)
            { // If all else fails, don't move
                return new Point(-1, -1);
            }

            int randomDir = _randGen.Next(0, possibleDirections.Count);

            if (possibleDirections[randomDir] == "left")
            {
                return new Point(pos.X - 1, pos.Y);
            }

            else if (possibleDirections[randomDir] == "right")
            {
                return new Point(pos.X + 1, pos.Y);
            }

            else if (possibleDirections[randomDir] == "up")
            {
                return new Point(pos.X, pos.Y - 1);
            }

            else // "down"
            {
                return new Point(pos.X, pos.Y + 1);
            }
        }

        public List<List<Tile>> attemptTileTakeover(List<List<Tile>> _ts, Random _randGen, List<Region> _regions)
        {
            // Choose a tile
            Point selectedTilePos = chooseTile(_ts, _randGen);

            // If no tile was selected, simply just return
            if(selectedTilePos.X == -1)
            {
                selectedTilePos = chooseNotLandTile(_ts, _randGen);
                if(selectedTilePos.X == -1)
                {
                    return _ts;
                }
            }

            float regionBonus1 = _regions[getRegionID(_regions)].totalRegionPopulation;
            float regionBonus2 = 0;
            if (_ts[selectedTilePos.X][selectedTilePos.Y].getRegionID(_regions) != -1)
            {
                if (_regions[_ts[selectedTilePos.X][selectedTilePos.Y].getRegionID(_regions)].totalRegionPopulation != 0)
                {
                    regionBonus2 = (_regions[_ts[selectedTilePos.X][selectedTilePos.Y].getRegionID(_regions)].totalRegionPopulation / _regions[getRegionID(_regions)].totalRegionPopulation);
                    regionBonus1 = (_regions[getRegionID(_regions)].totalRegionPopulation / _regions[_ts[selectedTilePos.X][selectedTilePos.Y].getRegionID(_regions)].totalRegionPopulation);
                }
                else
                {
                    regionBonus1 = 1;
                    regionBonus2 = 0;
                }
            }

            float ratio = Convert.ToSingle((population * (1 + regionBonus1 / 3)) / ((population * (1 + regionBonus1 / 3)) + (_ts[selectedTilePos.X][selectedTilePos.Y].population* (1 + regionBonus2 / 3))));

            double rand1 = _randGen.NextDouble();
            if(_ts[selectedTilePos.X][selectedTilePos.Y].regionName == "Blank" && _ts[selectedTilePos.X][selectedTilePos.Y].isNotLand == true)
            {
                ratio = ratio / 2f;
            }
            if (rand1 < ratio || (_ts[selectedTilePos.X][selectedTilePos.Y].regionName == "Blank" && _ts[selectedTilePos.X][selectedTilePos.Y].isNotLand == false))
            {
                if (_ts[selectedTilePos.X][selectedTilePos.Y].isNotLand == false)
                {
                    if (_ts[selectedTilePos.X][selectedTilePos.Y].getRegionID(_regions) != -1)
                    {
                        _regions[_ts[selectedTilePos.X][selectedTilePos.Y].getRegionID(_regions)].totalRegionPopulation -= _ts[selectedTilePos.X][selectedTilePos.Y].population;
                    }
                    _regions[getRegionID(_regions)].totalRegionPopulation += _ts[selectedTilePos.X][selectedTilePos.Y].population;

                    if (_ts[selectedTilePos.X][selectedTilePos.Y].getRegionID(_regions) != -1)
                    {
                        _regions[_ts[selectedTilePos.X][selectedTilePos.Y].getRegionID(_regions)].totalRegionSize -= 1;
                    }
                    _regions[getRegionID(_regions)].totalRegionSize += 1;
                }

                _ts[selectedTilePos.X][selectedTilePos.Y].regionName = regionName;
                _ts[selectedTilePos.X][selectedTilePos.Y].color = color;
                _ts[selectedTilePos.X][selectedTilePos.Y].colorid = colorid;
            }
            return _ts;
        }
    }
}
