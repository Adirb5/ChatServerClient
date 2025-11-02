using System;
using Microsoft.Extensions.Configuration;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Chat Server";

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            int port = int.Parse(config["ServerSettings:Port"] ?? "5000");

            var server = new Server(port);
            server.Start();
        }
    }
}