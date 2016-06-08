using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeModel
{
    public static class Rules
    {
        public const int MinAliveNeighboursForNewLife = 3;
        public const int MaxAliveNeighboursForDeath = 5;
    }
}
