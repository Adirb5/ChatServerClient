using System;
using System.IO;

namespace ChatClient
{
    public class MessageListener
    {
        private readonly StreamReader _reader;

        public MessageListener(StreamReader reader)
        {
            _reader = reader;
        }

        public void Listen()
        {
            try
            {
                string line;
                while ((line = _reader.ReadLine()) != null)
                {
                    if (line.StartsWith("Private"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(line);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("[Client] Disconnected from server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Client] Error: {ex.Message}");
            }
        }
    }
}
