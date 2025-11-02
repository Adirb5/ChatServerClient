using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace ChatServer
{
    public class Server
    {
        private TcpListener _listener;
        private readonly List<ClientHandler> _clients = new();
        private readonly LetterCounter _counter = new();
        private readonly object _lock = new();
        private readonly Regex _privateRegex = new(@"^to:\s*(.+?)\s*-\s*(.*)$", RegexOptions.IgnoreCase);

        public Server(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine("[Server] Started and listening for connections...");
            while (true)
            {
                var tcpClient = _listener.AcceptTcpClient();
                var clientHandler = new ClientHandler(tcpClient, this);
                lock (_lock) _clients.Add(clientHandler);
                new Thread(clientHandler.HandleClient) { IsBackground = true }.Start();
            }
        }

        public void Broadcast(string message, ClientHandler sender = null)
        {
            lock (_lock)
            {
                foreach (var client in _clients.ToArray())
                {
                    try
                    {
                        if (client != sender)
                            client.Send(message);
                    }
                    catch { _clients.Remove(client); }
                }
            }
        }

        public void PrivateMessage(string targetName, string message, ClientHandler sender)
        {
            lock (_lock)
            {
                var target = _clients.Find(c => c.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));
                if (target != null)
                {
                    target.Send($"Private from [{sender.Name}]: {message}");
                    if (target != sender)
                        sender.Send($"Private to [{target.Name}]: {message}");
                }
                else
                {
                    sender.Send($"[System] User '{targetName}' not found.");
                }
            }
        }

        public void RemoveClient(ClientHandler client)
        {
            lock (_lock) _clients.Remove(client);
            Console.WriteLine($"[Server] {client.Name} disconnected.");
        }

        public void ProcessMessage(ClientHandler sender, string text)
        {
            var timestamped = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {text}";
            var match = _privateRegex.Match(text);

            if (match.Success)
            {
                string targetName = match.Groups[1].Value.Trim();
                string msg = match.Groups[2].Value.Trim();
                PrivateMessage(targetName, msg, sender);
                _counter.UpdateCounts(msg);
            }
            else
            {
                Broadcast($"{sender.Name}: {text}");
                _counter.UpdateCounts(text);
            }

            string summary = _counter.GetSummary();
            Broadcast(summary);
        }
    }
}
