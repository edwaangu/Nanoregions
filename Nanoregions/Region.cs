using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Nanoregions
{
    internal class Region
    {
        public string regionName;
        public Color regionColor;
        public int regionColorId;

        public float totalRegionPopulation = 0;
        public float totalRegionSize = 0;

        public float averageX, averageY;

        public Region(string _regionName, Color _regionColor, int _regionColorId)
        {
            regionName = _regionName;
            regionColor = _regionColor;
            regionColorId = _regionColorId;
        }

        public void CalculateInformation(List<List<Tile>> _ts)
        {
            totalRegionPopulation = 0;
            totalRegionSize = 0;
            for(int i = 0;i < _ts.Count;i++)
            {
                for(int j = 0;j < _ts[i].Count;j++)
                {
                    if (_ts[i][j].regionName == regionName)
                    {
                        totalRegionPopulation += _ts[i][j].population;
                        totalRegionSize += 1;
                    }
                }
            }
        }

        public void CalculateAveragePosition(List<List<Tile>> _ts)
        {
            averageX = 0;
            averageY = 0;

            if(totalRegionSize == 0)
            {
                return;
            }

            for (int i = 0; i < _ts.Count; i++)
            {
                for (int j = 0; j < _ts[i].Count; j++)
                {
                    if (_ts[i][j].regionName == regionName && _ts[i][j].isNotLand == false)
                    {
                        averageX += i;
                        averageY += j;
                    }
                }
            }

            averageX = averageX / totalRegionSize;
            averageY = averageY / totalRegionSize;
        }
    }
}
