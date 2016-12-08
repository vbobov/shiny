using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AdventOfCode4
{
    class Program
    {
        static void Main(string[] args)
        {
            string pattern = @"^(\S+)-(\d+)\[(\S+)\]$";

            StreamReader sr = new StreamReader("input.txt");
            string line;
            int sectorSum = 0;

            while ((line = sr.ReadLine()) != null)
            {
                var match = System.Text.RegularExpressions.Regex.Match(line, pattern);
                if (match.Success)
                {
                    //Console.WriteLine($"Data:{match.Groups[1].Value} number:{match.Groups[2].Value} checksum:{match.Groups[3].Value}");
                    var charCount = new Dictionary<char, int>();

                    foreach (char c in match.Groups[1].Value)
                    {
                        if (c != '-')
                        {
                            if (charCount.ContainsKey(c))
                                charCount[c]++;
                            else
                                charCount[c] = 1;
                        }
                    }

                    var rank = charCount.Keys.OrderByDescending(x => charCount[x]).GroupBy(x => charCount[x]);

                    List<char> check = new List<char>();
                    int checkChars = 0;

                    foreach (var c in rank)
                    {
                        var alpha = c.OrderBy(x => x);

                        foreach (var i in alpha)
                        {
                            check.Add(i);
                            checkChars++;

                            if (checkChars == 5)
                                break;
                        }
                        if (checkChars == 5)
                            break;
                    }

                    var checkStr = new String(check.ToArray());

                    if (checkStr == match.Groups[3].Value)
                    {
                        var sectorId = int.Parse(match.Groups[2].Value);
                        sectorSum += sectorId;

                        var roomParts = match.Groups[1].Value.Split('-');
                        StringBuilder sb = new StringBuilder();

                        // A == 65, Z == 90
                        foreach (var word in roomParts)
                        {
                            var realChars = new List<char>();
                            var bytes = Encoding.ASCII.GetBytes(word.ToUpper());
                            foreach (var oneByte in bytes)
                            {
                                var shift = sectorId % 26;
                                var newByte = 65 + (((oneByte - 65) + shift) % 26);
                                realChars.Add(ASCIIEncoding.ASCII.GetChars(new byte[] { (byte) newByte })[0]);
                            }
                            string realWord = new string(realChars.ToArray());
                            sb.Append(realWord + " ");
                        }
                        Console.WriteLine($"{sb} ({sectorId})");


                    }
                }
            }
            //Console.WriteLine(sectorSum);
            Console.ReadLine();
        }
    }
}
