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
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <returns></returns>
        Task<int> GetCountAsync();

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task SetCountAsync(int count);

        /// <summary>
        /// Communicate cell state to orchestration service.
        /// </summary>
        /// <param name="cell">Cell which state has changed.</param>
        Task SetCellState(Cell cell);

        /// <summary>
        /// Initialize the game. Create first cells which are alive.
        /// </summary>
        /// <returns></returns>
        Task BigBang(int xsize, int ysize);

        /// <summary>
        /// Get the state of all cells.
        /// </summary>
        /// <returns></returns>
        Task<List<Cell>> GetCellStates();
    }
}
