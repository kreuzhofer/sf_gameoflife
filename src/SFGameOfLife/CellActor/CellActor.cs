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
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see http://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <returns></returns>
        Task<int> ICellActor.GetCountAsync()
        {
            return this.StateManager.GetStateAsync<int>("count");
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task ICellActor.SetCountAsync(int count)
        {
            // Requests are not guaranteed to be processed in order nor at most once.
            // The update function here verifies that the incoming count is greater than the current count to preserve order.
            return this.StateManager.AddOrUpdateStateAsync("count", count, (key, value) => count > value ? count : value);
        }

        /// <summary>
        /// Logs the actor state to the orchestration actor.
        /// </summary>
        public void LogStatus()
        {
            // Log the status for this actor
            Debug.WriteLine("cell_{0}_{1}: {2} nc:{3}", ActorCell.X, ActorCell.Y, ActorCell.State, ActorCell.AliveNeighbourCounter);
        }

        public CellState GetCellStatus()
        {
            return ActorCell.State;
        }

        public async Task GetAlive(int x, int y)
        {
            if (ActorCell == null)
            {
                ActorCell = new Cell
                {
                    State = CellState.Alive,
                    X = x,
                    Y = y
                };
            }
            else
            {
                ActorCell.State = CellState.Alive;
            }
            await this.StateManager.TryAddStateAsync("cellstate", ActorCell);
            //await NotifyNeighboursAsync(CellState.Alive, true);
            LogStatus();
        }

        private async Task NotifyNeighboursAsync(CellState state, bool getAliveCall)
        {
            var neighbourcoords = ActorCell.GetNeighbourCoords();
            foreach (var coord in neighbourcoords)
            {
                var id = new ActorId(String.Format("cell_{0}_{1}", coord.Key, coord.Value));
                var neighbourcell = ActorProxy.Create<ICellActor>(id, new Uri("fabric:/SFGameOfLife/CellActorService"));
                try
                {
                    await neighbourcell.NeighbourStateChanged(coord.Key, coord.Value, state, getAliveCall);

                }
                catch (Exception)
                {
                    // cyclic calls are not allowed
                }
            }
        }

        public async Task NeighbourStateChanged(int x, int y, CellState newstate, bool getAliveCall)
        {
            //Cell was dead: create a new one in Prelive State
            if (ActorCell == null)
            {
                ActorCell = new Cell
                {
                    State = CellState.PreAlive,
                    X = x,
                    Y = y
                };
            }

            //Update AliveNeighbourCounter
            if (newstate == CellState.Alive)
            {
                ActorCell.AliveNeighbourCounter++;
            }
            else if (newstate == CellState.Dead)
            {
                if (ActorCell.AliveNeighbourCounter > 0)
                    ActorCell.AliveNeighbourCounter--;
            }

            //Check rules and update the state if necessary => notify neighbours if state changed
            if (ActorCell.State == CellState.PreAlive &&
                ActorCell.AliveNeighbourCounter >= Rules.AliveNeighboursForNewLife)
            {
                ActorCell.State = CellState.Alive;
                await NotifyNeighboursAsync(ActorCell.State, false);
            }
            //Cell can only die if this method is not called from a cell that was created via getAlive()
            if (ActorCell.State == CellState.Alive && !getAliveCall)
            {
                if (ActorCell.AliveNeighbourCounter >= Rules.UpperAliveNeighboursForDeath ||
                    ActorCell.AliveNeighbourCounter <= Rules.LowerAliveNeighboursForDeath)
                {
                    ActorCell.State = CellState.Dead;
                    await NotifyNeighboursAsync(ActorCell.State, false);
                }
            }
            LogStatus();
            if (ActorCell.State == CellState.Dead)
            {
                var id = new ActorId(String.Format("cell_{0}_{1}", ActorCell.X, ActorCell.Y));
                var cellActorService = ActorServiceProxy.Create(new Uri("fabric:/SFGameOfLife/CellActorService"), id);
                var task = cellActorService.DeleteActorAsync(id, CancellationToken.None);
            }
            else
            {
                await this.StateManager.TryAddStateAsync("cellstate", ActorCell);
            }
        }
    }
}
