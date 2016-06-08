using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using OrchestrationActor.Interfaces;
using GameOfLifeModel;
using CellActor.Interfaces;
using CellActor;

namespace OrchestrationActor
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
    internal class OrchestrationActor : Actor, IOrchestrationActor
    {

        private List<Cell> cells;

        public Task BigBang()
        {
            return new Task(() =>
            {
                Random random = new Random(DateTime.Now.Millisecond);

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        CreateCellActor(i, j, (CellState)random.Next(0, 2));
                    }
                }
            });
        }

        private Task CreateCellActor(int x, int y, CellState cellState)
        {
            // Create a randomly distributed actor ID
            ActorId actorId = new ActorId($"cell_{x}_{y}");

            // This only creates a proxy object, it does not activate an actor or invoke any methods yet.
            ICellActor cellActor = ActorProxy.Create<ICellActor>(actorId, new Uri("fabric:/SFGameOfLife/CellActorService"));

            // This will invoke a method on the actor. If an actor with the given ID does not exist, it will be activated by this method call.
            // TODO: Use Alive function using cellState
            return cellActor.GetAlive(x, y);
        }

        public Task SetCellState(Cell cell)
        {
            return new Task(() =>
            {
                // Search list for given cell.
                var cellToUpdate = cells.FirstOrDefault<Cell>(i => i.X == cell.X && i.Y == cell.Y);

                if (cellToUpdate != null)
                {
                    cellToUpdate.State = cell.State;
                }
                else
                {
                    // If not found add cell update to list.
                    cells.Add(cell);
                }
            });
        }

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
        Task<int> IOrchestrationActor.GetCountAsync()
        {
            return this.StateManager.GetStateAsync<int>("count");
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task IOrchestrationActor.SetCountAsync(int count)
        {
            // Requests are not guaranteed to be processed in order nor at most once.
            // The update function here verifies that the incoming count is greater than the current count to preserve order.
            return this.StateManager.AddOrUpdateStateAsync("count", count, (key, value) => count > value ? count : value);
        }


    }
}
