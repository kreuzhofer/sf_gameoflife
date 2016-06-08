using System.Collections.Generic;
using System.Web.Http;
using GameOfLifeModel;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using OrchestrationActor.Interfaces;
using System.Threading.Tasks;

namespace GameOfLifeAPI.Controllers
{
    public class CellsController : ApiController
    {
        // GET api/cells 
        public async Task<List<Cell>> Get()
        {
            //return new string[] { "value1", "value2" };
            //return new List<Cell>
            //{
            //    new Cell() { X = 0, Y = 0, State = CellState.Alive }, new Cell() { X = 0, Y = 0, State = CellState.Dead  }
            //};            

            //return new List<Cell>
            //{
            //    //new Cell() { X = 1337, Y = 0, State = CellState.Alive }, new Cell() { X = 4711, Y = 4711, State = CellState.Dead  }
            //    var orchestrationActor = ActorProxy.Create<IOrchestrationActor>(new ActorId("god"), "fabric:/SFGameOfLife");
            //};

            var orchestrationActor = ActorProxy.Create<IOrchestrationActor>(new ActorId("god"), "fabric:/SFGameOfLife");
            return await orchestrationActor.GetCellStates();
    }

        // GET api/cells/5 
        public async void Get(int id)
        {
            var orchestrationActor = ActorProxy.Create<IOrchestrationActor>(new ActorId("god"), "fabric:/SFGameOfLife");

            orchestrationActor.BigBang();

        }

        // POST api/cells 
        public async void Post([FromBody]string value)
        {
            var orchestrationActor = ActorProxy.Create<IOrchestrationActor>(new ActorId("god"), "fabric:/SFGameOfLife");

            await orchestrationActor.BigBang();
        }

        // PUT api/cells/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/cells/5 
        public void Delete(int id)
        {
        }
    }
}
