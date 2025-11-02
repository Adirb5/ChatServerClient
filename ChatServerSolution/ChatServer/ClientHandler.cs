using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    public class ClientHandler
    {
        private readonly TcpClient _tcpClient;
        private readonly Server _server;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        public string Name { get; private set; }

        public ClientHandler(TcpClient tcpClient, Server server)
        {
            _tcpClient = tcpClient;
            _server = server;
            var stream = _tcpClient.GetStream();
            _reader = new StreamReader(stream, Encoding.UTF8);
            _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        }

        public void HandleClient()
        {
            try
            {
                _writer.WriteLine("Please enter Name will appear:");
                Name = _reader.ReadLine();
                _writer.WriteLine("Client connected successfully.");
                Console.WriteLine($"[Server] {Name} connected.");

                string line;
                while ((line = _reader.ReadLine()) != null)
                    _server.ProcessMessage(this, line);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Server] Client error: {ex.Message}");
            }
            finally
            {
                _server.RemoveClient(this);
            }
        }

        public void Send(string message)
        {
            try
            {
                _writer.WriteLine(message);
            }
            catch { }
        }
    }
}
