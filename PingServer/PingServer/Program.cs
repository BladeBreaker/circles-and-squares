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
        // I can't hit clone-god from my PC using my external IP address so I'm going to hit it internally and just
        // translate from my internal address to my external address.
        public static readonly IPAddress DansInternalAddress = IPAddress.Parse("10.88.111.32");
        public static readonly IPAddress DansExternalAddress = IPAddress.Parse("64.137.136.12");


        public static async Task Main(string[] args)
        {
            Console.WriteLine("Ready, please enter a command");

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


        public static void TranslateDanFromExternalToInternal(EndPoint endpoint)
        {
            IPEndPoint tmp = (IPEndPoint)endpoint;
            if (tmp.Address.Equals(DansExternalAddress))
            {
                tmp.Address = DansInternalAddress;
            }
        }

        public static void TranslateDanFromInternalToExternal(EndPoint endpoint)
        {
            IPEndPoint tmp = (IPEndPoint)endpoint;
            if (tmp.Address.Equals(DansInternalAddress))
            {
                tmp.Address = DansExternalAddress;
            }
        }

        public static async Task ListenAndRespondToPings(CancellationToken token)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Bind(new IPEndPoint(IPAddress.Any, 35357));

            byte[] buffer = new byte[1500];

            EndPoint player1 = null;

            while (!token.IsCancellationRequested)
            {
                if (socket.Available > 0)
                {
                    EndPoint endpoint = new IPEndPoint(IPAddress.Any, 35357);

                    socket.ReceiveFrom(buffer, ref endpoint);

                    TranslateDanFromInternalToExternal((IPEndPoint)endpoint);

                    if (player1 == null)
                    {
                        Console.WriteLine($"Found Player 1: {endpoint}");

                        player1 = endpoint;
                    }
                    else if (!player1.Equals(endpoint))
                    {
                        Console.WriteLine($"Found Player 2: {endpoint}");
                        Console.WriteLine($"Sending info to players");

                        string message1 = $"{player1}";
                        string message2 = $"{endpoint}";

                        TranslateDanFromExternalToInternal(endpoint);
                        TranslateDanFromExternalToInternal(player1);

                        socket.SendTo(Encoding.UTF8.GetBytes(message1), endpoint);
                        socket.SendTo(Encoding.UTF8.GetBytes(message2), player1);
                        player1 = null;
                    }
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }
            }
        }
    }
}
