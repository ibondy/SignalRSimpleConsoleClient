using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace SignalRClient
{
    using System.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            if (ConfigurationManager.AppSettings["UseHubs"] == "False")
            {
                try
                {
                    var client = new Connection(ConfigurationManager.AppSettings["SignalRUrl"])
                    {
                        TraceLevel = TraceLevels.None,
                        TraceWriter = Console.Out
                    };
                    client.Received += Console.WriteLine;
                    client.Start();
                    Thread.Sleep(3000);
                    var x = 0;
                    while (client.State != ConnectionState.Connected)
                    {
                        Console.WriteLine("Connecting...." + x);
                        Thread.Sleep(1000);
                        x++;
                        if (x > 20)
                        {
                            break;
                        }

                    }

                    var guid = Guid.NewGuid();
                    client.Send($"Hello:{guid}");
                    Console.WriteLine($"Sending Hello:{guid}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            else
            {
                try
                {
                    var hc = new HubConnection(ConfigurationManager.AppSettings["SignalRUrl"])
                    {
                        TraceLevel = TraceLevels.All,
                        TraceWriter = Console.Out
                    };

                    var proxy = hc.CreateHubProxy("HelloHub");

                    proxy.On<string>("notifyNewMessage", Console.WriteLine);
                    proxy.On("Connected", () => Console.WriteLine("Connected"));
                    hc.Start();
                    Thread.Sleep(1000);

                    proxy.Invoke("Send", "Hello");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }


            Console.ReadLine();
 

        }
    }
}
