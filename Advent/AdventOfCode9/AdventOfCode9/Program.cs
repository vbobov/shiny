using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode9
{
    class Program
    {
        static readonly string pattern = @"\((\d+)x(\d+)\)";
        static ulong length = 0;

        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader("input.txt");
            string line;
            line = sr.ReadLine();

            // recursively count up the number of expanded characters (leaf case increments length)
            // then skip ahead
            //while (Regex.IsMatch(line, pattern))
            //{
            //    line = Decode(line);
            //    length += line.Length;
            //    Console.WriteLine(length);
            //}
            length = Decode(line);

            Console.WriteLine(length);
            Console.ReadLine();
        }

        static ulong Decode(string input)
        {
            int pos = 0;
            string line = input;
            ulong localLength = 0;

            while (Regex.IsMatch(line, pattern))
            {
                var match = Regex.Match(line, pattern);
                int markerIndex = match.Index;
                int markerLength = match.Length;
                int seqLength = int.Parse(match.Groups[1].Value);
                int seqRepeat = int.Parse(match.Groups[2].Value);

                // count up to the marker
                localLength += (ulong) (markerIndex - pos);

                string subProblem = line.Substring(markerIndex + markerLength, seqLength);

                localLength += ((ulong) seqRepeat * Decode(subProblem));

                line = line.Substring(pos + markerIndex + markerLength + seqLength);

                //Console.WriteLine($"{match.Index},{match.Length}:{match.Groups[1].Value} x {match.Groups[2].Value}");
                //Console.WriteLine(decoded.ToString());
                //Console.ReadLine();
            }
            localLength += (ulong) line.Length;
            return localLength;
        }
    }
}
