using System;
using Microsoft.Extensions.Configuration;

namespace ChatClient
{
    class Program
    {
        static void Main()
        {
            Console.Title = "Chat Client";

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string ip = config["ClientSettings:ServerIP"] ?? "127.0.0.1";
            int port = int.Parse(config["ClientSettings:Port"] ?? "5000");

            var client = new ConnectionManager(ip, port);
            client.Connect();
        }
    }
}
