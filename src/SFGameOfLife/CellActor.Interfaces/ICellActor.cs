using System;
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
        /// This is for giving life: the state of the cell with the given coordinates (x,y) is set to ALIVE,
        /// the state manager is told about the changes for this cell, and all neighbours of this cell are 
        /// informed about the new life. 
        /// </summary>
        /// <param name="x">The X-Coordinate of the cell that gets alive. </param>
        /// <param name="y">The Y-Coordinate of the cell that gets alive.</param>
        Task GetAlive(int x, int y, CellState cellState);

        /// <summary>
        /// This method get's the states of all neighbour cells and computes it's own state, set's it and returns it.
        /// </summary>
        /// <param name="neighbourStates">All states of the neighbours of one cell (8). If the cell is at the edge of the game matrix, it gets dead states of the non-existing cells.</param>
        /// <returns></returns>
        Task<int> ComputeNewState(List<int> neighbourStates);

        Task<int> GetState();


        /// <summary>
        /// If the state of a cell changes (e.g. from PreAlive->Life, Life->Dead) then all bordering
        /// cells are informed. 
        /// 
        /// - If a neighbour cell is dead, it will be created and it's state is set to PreAlive. 
        /// - If a neighbour cell is already existant, it's AliveNeighbourCount is 
        ///   updated (-1 if state changed to DEAD and +1 if state changed to LIFE)
        /// - If the state of a neighbour changed through the AliveNeighbourCount Update, then all the 
        ///   neighbours of this this cell are informed about it's state change. 
        /// 
        /// </summary>
        /// <param name="x">The X-Coordinate of the cell that state got changed. </param>
        /// <param name="y">The Y-Coordinate of the cell that state got changed. </param>
        /// <param name="newstate">The new state of the cell. </param>
        /// <param name="getAliveCall">True: this method is called from a cell that was spawened via getAlive().
        ///                            False: this method is called from a cell that changed it's state. </param>
        /// <returns></returns>
        //Task NeighbourStateChanged(int x, int y, CellState newstate, bool getAliveCall);
    }
}
