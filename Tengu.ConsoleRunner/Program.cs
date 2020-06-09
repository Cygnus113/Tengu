using Microsoft.Extensions.Logging;
using System;
using Tengu.Network;
using Tengu.Network.Events;

namespace Tengu.ConsoleRunner
{
    class Program
    {
        private static ILogger _logger;
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter a command:");
            string command = Console.ReadLine();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
            });

            _logger = loggerFactory.CreateLogger("Tengu Logger");

            switch (command)
            {
                case "host":
                    RunAsHost();
                    break;
                default:
                    break;
            }
        }

        private static void RunAsHost()
        {
            var socket = new TenguSocket(_logger);
            socket.OnClientConnect += OnClientConnect;
            socket.OnMessage += OnMessage;
            socket.BeginAccept(4440, "127.0.0.1", System.Net.Sockets.ProtocolType.Tcp);
        }

        private static void OnClientConnect(object sender, PacketEventArgs args)
        {
            Console.WriteLine("Client Connected!");
        }

        private static void OnMessage(object sender, PacketEventArgs args)
        {
            args.Packet.Read();
            Console.WriteLine("Message Received");
        }
    }
}
