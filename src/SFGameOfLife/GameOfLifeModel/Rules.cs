using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeModel
{
    public static class Rules
    {
        public const int AliveNeighboursForNewLife = 3;
        public const int UpperAliveNeighboursForDeath = 4;
        public const int LowerAliveNeighboursForDeath = 1;
    }
}
