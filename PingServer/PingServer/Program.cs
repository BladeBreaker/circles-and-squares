using System;

using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace PingServer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            CancellationTokenSource cts = null;
            Task listenTask = null;
            string command = null;
            bool quitNow = false;
            while (!quitNow)
            {
                command = Console.ReadLine();
                switch (command)
                {
                    case "stop":
                        Console.WriteLine("Waiting for server to stop, and shutting down");
                        cts.Cancel();
                        quitNow = true;
                        break;

                    case "start":
                        Console.WriteLine("Starting listen server");
                        if (cts == null)
                        {
                            cts = new CancellationTokenSource();

                            listenTask = ListenAndRespondToPings(cts.Token);
                        }
                        break;

                    default:
                        Console.WriteLine("Unknown Command " + command + ", try help");
                        break;
                }
            }

            if (listenTask != null)
            {
                await listenTask;
            }

            Console.WriteLine("Shutdown Complete");
        }

        public static async Task ListenAndRespondToPings(CancellationToken token)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Bind(new IPEndPoint(IPAddress.Any, 35353));

            byte[] buffer = new byte[1500];

            while (!token.IsCancellationRequested)
            {
                if (socket.Available > 0)
                {
                    EndPoint endpoint = new IPEndPoint(IPAddress.Any, 35353);

                    socket.ReceiveFrom(buffer, ref endpoint);

                    string responseMessage = $"Your public IP is: {endpoint}";

                    Console.WriteLine($"Received message from {endpoint}");

                    socket.SendTo(Encoding.UTF8.GetBytes(responseMessage), new IPEndPoint(((IPEndPoint)endpoint).Address, 35353));
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }
            }
        }
    }
}
