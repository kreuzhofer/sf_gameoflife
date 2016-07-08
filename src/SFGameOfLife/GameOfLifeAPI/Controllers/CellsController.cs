using System;
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
        public async Task<List<int>> Get()
        {
            var orchestrationActor = ActorProxy.Create<IOrchestrationActor>(new ActorId("god"), "fabric:/SFGameOfLife");
            var result = await orchestrationActor.GetCellStates();
            return result;
        }

        // GET api/cells/5 
        public Task<TimeSpan> Get(int id, [FromUri] int xsize, [FromUri] int ysize)
        {
            var orchestrationActor = ActorProxy.Create<IOrchestrationActor>(new ActorId("god"), "fabric:/SFGameOfLife");

            return orchestrationActor.BigBang(xsize, ysize);
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