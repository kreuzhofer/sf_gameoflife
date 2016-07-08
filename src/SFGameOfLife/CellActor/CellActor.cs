using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using CellActor.Interfaces;
using GameOfLifeModel;
using System.Diagnostics;
using OrchestrationActor.Interfaces;

namespace CellActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class CellActor : Actor, ICellActor
    {
        private Cell ActorCell { get; set; }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see http://aka.ms/servicefabricactorsstateserialization

            //return this.StateManager.TryAddStateAsync("count", 0);
        }

        /// <summary>
        /// Logs the actor coordinates and it's state to the orchestration actor.
        /// </summary>
        public void LogStatus()
        {
            Debug.WriteLine($"cell_{ActorCell.X}_{ActorCell.Y}: {ActorCell.State}"); 
        }

        public CellState GetCellStatus()
        {
            return ActorCell.State;
        }

        public async Task GetAlive(int x, int y, CellState cellState)
        {
            ActorCell = new Cell
            {
                State = cellState,
                X = x,
                Y = y
            };
            LogStatus();
        }

        public async Task<int> ComputeNewState(List<int> neighbourStates)
        {
            int sum = neighbourStates.Sum(x => Convert.ToInt32(x));

            if (sum >= Rules.UpperAliveNeighboursForDeath || sum <= Rules.LowerAliveNeighboursForDeath)
            {
                ActorCell.State = CellState.Dead;
            }
            LogStatus();

            return (int)ActorCell.State;
        }

        public async Task<int> GetState()
        {
            return (int)ActorCell.State;
        }
    }
}
