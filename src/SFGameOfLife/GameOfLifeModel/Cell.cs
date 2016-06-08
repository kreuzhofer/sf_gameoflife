using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeModel
{
    [DataContract]
    public class Cell
    {
        //State of the cell [PreAlive, Life, Dead]
        [DataMember]
        public CellState State { get; set; }

        //Counts all bordering cell with state == alive
        [DataMember]
        public int AliveNeighbourCounter { get; set; }

        //X-Coordinate of this Cell
        [DataMember]
        public int X { get; set; }

        //Y-Coordinate of this Cell
        [DataMember]
        public int Y { get; set; }

        //Gets all coordinates of all bordering cells
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
