using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task<TimeSpan> BigBang(int xsize, int ysize)
        {
            Stopwatch watch = Stopwatch.StartNew();
            this._xsize = xsize;
            this._ysize = ysize;
            Random random = new Random(DateTime.Now.Millisecond);

            for (int j = 0; j < _ysize; j++)
            {
                for (int i = 0; i < _xsize; i++)
                {
                    await CreateCellActor(i, j, (CellState)(random.Next(0, 10000)%2));
                }
            }
            return watch.Elapsed;
        }

        private Task CreateCellActor(int x, int y, CellState cellState)
        {
            var cellActor = GetCellActor(x, y);

            // This will invoke a method on the actor. If an actor with the given ID does not exist, it will be activated by this method call.
            return cellActor.GetAlive(x, y, cellState);
        }

        private static ICellActor GetCellActor(int x, int y)
        {
            ActorId actorId = new ActorId($"cell_{x}_{y}");

            // This only creates a proxy object, it does not activate an actor or invoke any methods yet.
            ICellActor cellActor = ActorProxy.Create<ICellActor>(actorId, new Uri("fabric:/SFGameOfLife/CellActorService"));
            return cellActor;
        }

        public async Task<List<int>> GetCellStates()
        {
            var status = new List<int>();

            //for (int j = 0; j < _ysize; j++)
            //{
            //    for (int i = 0; i < _xsize; i++)
            //    {
            //        var cellActor = GetCellActor(i, j);
            //        var state = await cellActor.GetState();
            //        status.Add(state);
            //    }
            //}

            for (int j = 0; j < _ysize; j++)
            {
                for (int i = 0; i < _xsize; i++)
                {
                    var neighbourCoords = GetNeighbourCoords(i, j);
                    var neighbourStates = new List<int>();
                    foreach (var coord in neighbourCoords)
                    {
                        int state;
                        if (coord.Key < 0 || coord.Value < 0 || coord.Key >= _xsize || coord.Value >= _ysize)
                        {
                            state = 0;
                        }
                        else
                        {
                            var cellActor = GetCellActor(coord.Key, coord.Value);
                            state = await cellActor.GetState();
                        }

                        neighbourStates.Add(state);
                    }
                    var myCellActor = GetCellActor(i, j);
                    var newStatus = await myCellActor.ComputeNewState(neighbourStates);
                    status.Add(newStatus);
                }
            }

            return status;
        }
        public KeyValuePair<int, int>[] GetNeighbourCoords(int X, int Y)
        {
            List<KeyValuePair<int, int>> result = new List<KeyValuePair<int, int>>();
            result.Add(new KeyValuePair<int, int>(X + 1, Y));
            result.Add(new KeyValuePair<int, int>(X + 1, Y + 1));
            result.Add(new KeyValuePair<int, int>(X, Y + 1));
            result.Add(new KeyValuePair<int, int>(X - 1, Y + 1));
            result.Add(new KeyValuePair<int, int>(X - 1, Y));
            result.Add(new KeyValuePair<int, int>(X - 1, Y - 1));
            result.Add(new KeyValuePair<int, int>(X, Y - 1));
            result.Add(new KeyValuePair<int, int>(X + 1, Y - 1));

            //TODO Loop over the array and fix the boundary overflow problem
            return result.ToArray();
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
