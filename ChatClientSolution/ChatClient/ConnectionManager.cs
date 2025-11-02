using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    public class ConnectionManager
    {
        private readonly string _ip;
        private readonly int _port;

        public ConnectionManager(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public void Connect()
        {
            try
            {
                using var client = new TcpClient();
                client.Connect(_ip, _port);

                using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                Console.WriteLine(reader.ReadLine());

                string name = Console.ReadLine();
                writer.WriteLine(name);

                Console.WriteLine(reader.ReadLine()); 

     
                var listener = new MessageListener(reader);
                Thread listenerThread = new(listener.Listen) { IsBackground = true };
                listenerThread.Start();

                string line;
                while ((line = Console.ReadLine()) != null)
                {
                    writer.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Client] Connection failed: {ex.Message}");
            }
        }
    }
}
