using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GameOfLifeModel;
using System.Threading;

namespace GameOfLifeVisualizer
{
    class Program
    {
        private const int xsize = 80;
        private const int ysize = 20;
        private const string baseAddress = "http://localhost:8128/api/";

        static void Main(string[] args)
        {
            Task t = new Task(GetCellsFromService);
            t.Start();
            Console.WriteLine("Getting cell states...");
            Console.ReadLine();
        }

        static async void GetCellsFromService()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMinutes(10);

                // init the game
                var result= await client.GetAsync($"cells/1?xsize={xsize}&ysize={ysize}");
                if (result.IsSuccessStatusCode)
                {
                    var timeToInit = JsonConvert.DeserializeObject<TimeSpan>(await result.Content.ReadAsStringAsync());
                    Console.WriteLine($"Time to init: {timeToInit}");
                }

                bool abort = false;

                while (!abort)
                {
                    var responseTask = client.GetAsync("cells");
                    HttpResponseMessage response = await responseTask;

                    if (response.IsSuccessStatusCode)
                    {
                        Console.Clear();
                        Console.WriteLine(DateTime.Now.ToString());
                        var resp = await response.Content.ReadAsStringAsync();
                        var cellList = (List<int>)JsonConvert.DeserializeObject<List<int>>(resp);
                        if (cellList.Count < xsize*ysize)
                        {
                            Console.WriteLine("Cells not yet alive");
                            continue;
                        }
                        for (int j = 0; j < ysize; j++)
                        {
                            for (int i = 0; i < xsize; i++)
                            {
                                Console.Write(cellList[j*xsize+i] == (int)CellState.Alive ? "X" : "_");
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }
        }
    }
}
