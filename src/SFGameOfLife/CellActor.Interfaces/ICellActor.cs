﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using GameOfLifeModel;

namespace CellActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface ICellActor : IActor
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
        /// Logs the actor state to some kind of queue. To be implemented sometime in the future.
        /// </summary>
        //Task LogStatus();

        /// <summary>
        /// Returns the state of the cell
        /// </summary>
        /// <returns></returns>
        //Task<CellState> GetCellStatus();

    }
}
