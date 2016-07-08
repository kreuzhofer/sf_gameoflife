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
        private const int xsize = 100;
        private const int ysize = 100;
        private const string baseAddress = "http://localhost:8128/api/";

        static void Main(string[] args)
        {
            // init game matrix
            using (var client = new HttpClient())
            {
                
            }


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

                bool abort = false;

                while (!abort)
                {
                    var responseTask = client.GetAsync("cells");
                    HttpResponseMessage response = await responseTask;

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(DateTime.Now.ToString());
                        var resp = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(resp);
                        var cellList = (List<Cell>)JsonConvert.DeserializeObject<List<Cell>>(resp);
                        foreach (var item in cellList)
                        {
                            Console.WriteLine($"X:{item.X}, Y:{item.Y}, {item.State}");
                        }
                    }

                    Thread.Sleep(2000);

                    Console.Clear();
                }

            }
        }
    }
}
