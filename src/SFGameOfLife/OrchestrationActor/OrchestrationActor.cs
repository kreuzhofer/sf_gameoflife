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
        private int _ysize;
        private int _xsize;

        public async Task BigBang(int xsize, int ysize)
        {
            this._xsize = xsize;
            this._ysize = ysize;
            Random random = new Random(DateTime.Now.Millisecond);

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        await CreateCellActor(i, j, (CellState)random.Next(0, 2));
                    }
                }
      
        }

        private Task CreateCellActor(int x, int y, CellState cellState)
        {
            // Create a randomly distributed actor ID
            ActorId actorId = new ActorId($"cell_{x}_{y}");

            // This only creates a proxy object, it does not activate an actor or invoke any methods yet.
            ICellActor cellActor = ActorProxy.Create<ICellActor>(actorId, new Uri("fabric:/SFGameOfLife/CellActorService"));

            // This will invoke a method on the actor. If an actor with the given ID does not exist, it will be activated by this method call.
            return cellActor.GetAlive(x, y);
        }

        public async Task<List<Cell>> GetCellStates()
        {
            return cells;
        }

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
    }
}
