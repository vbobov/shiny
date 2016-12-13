using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode10
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader("input.txt");
            string line;

            string botGetPattern = @"^value (\d+) goes to bot (\d+)$";
            string botGivePattern = @"^bot (\d+) gives low to bot (\d+) and high to bot (\d+)$";

            while ((line = sr.ReadLine()) != null)
            {
                if (Regex.IsMatch(line, botGetPattern))
                {
                    var match = Regex.Match(line, botGetPattern);
                    int value = int.Parse(match.Groups[1].Value);
                    int botNum = int.Parse(match.Groups[2].Value);

                    Console.WriteLine($"{value} to {botNum}");
                }
                else if (Regex.IsMatch(line, botGivePattern))
                {
                    var match = Regex.Match(line, botGivePattern);
                    int sourceBot = int.Parse(match.Groups[1].Value);
                    int lowDestBot = int.Parse(match.Groups[2].Value);
                    int highDestBot = int.Parse(match.Groups[3].Value);
                    Console.WriteLine($"Bot {sourceBot} gives low to {lowDestBot} and high to {highDestBot}");
                }
                else
                    throw new ArgumentException();

                Console.ReadLine();
            }
        }
    }
}
