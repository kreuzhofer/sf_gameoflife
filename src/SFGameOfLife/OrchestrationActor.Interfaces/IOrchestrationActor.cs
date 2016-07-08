using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using GameOfLifeModel;

namespace OrchestrationActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IOrchestrationActor : IActor
    {
        /// <summary>
        /// Initialize the game. Create first cells which are alive.
        /// </summary>
        /// <returns></returns>
        Task BigBang(int xsize, int ysize);

        /// <summary>
        /// Get the state of all cells.
        /// </summary>
        /// <returns></returns>
        Task<List<int>> GetCellStates();
    }
}
