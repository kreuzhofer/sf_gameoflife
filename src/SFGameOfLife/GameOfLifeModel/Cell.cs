using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeModel
{
    public class Cell
    {
        public CellState State { get; set; }
        public int AliveNeighbourCounter { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public KeyValuePair<int, int>[] GetNeighbourCoords()
        {
            List<KeyValuePair<int, int>> result = new List<KeyValuePair<int, int>>();
            result.Add(new KeyValuePair<int, int>(X + 1, Y));
            result.Add(new KeyValuePair<int, int>(X + 1, Y+1));
            result.Add(new KeyValuePair<int, int>(X, Y+1));
            result.Add(new KeyValuePair<int, int>(X - 1, Y+1));
            result.Add(new KeyValuePair<int, int>(X - 1, Y));
            result.Add(new KeyValuePair<int, int>(X - 1, Y-1));
            result.Add(new KeyValuePair<int, int>(X, Y-1));
            result.Add(new KeyValuePair<int, int>(X + 1, Y-1));

            //TODO Loop over the array and fix the boundary overflow problem
            return result.ToArray();
        }
    }
}
