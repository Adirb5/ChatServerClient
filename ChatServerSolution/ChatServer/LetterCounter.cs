using System;
using System.Collections.Generic;

namespace ChatServer
{
    public class LetterCounter
    {
        private readonly Dictionary<char, long> _counts = new();

        public LetterCounter()
        {
        }

        public void UpdateCounts(string text)
        {
            foreach (var c in text.ToLower())
            {
                if (c < 'a' || c > 'z') continue;

                if (_counts.ContainsKey(c))
                    _counts[c]++;
                else
                    _counts[c] = 1;
            }
        }

        public string GetSummary()
        {
            var sortedKeys = new List<char>(_counts.Keys);
            sortedKeys.Sort();

            var lines = new List<string>
            {
                "=== Letter Frequency Summary ==="
            };

            foreach (var c in sortedKeys)
            {
                if (_counts[c] > 0)
                    lines.Add($"{char.ToUpper(c)}: {_counts[c]}");
            }

            lines.Add("===============================");

            return string.Join(Environment.NewLine, lines);
        }
    }
}
