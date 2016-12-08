using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AdventOfCode6
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader("input.txt");
            string line;

            var counters = new Dictionary<char, int>[8];
            for (int i = 0; i < 8; i++)
                counters[i] = new Dictionary<char, int>();

            while ((line = sr.ReadLine()) != null)
            {
                for (int i=0;i<line.Length;i++)
                {
                    if (i >= 8)
                        throw new Exception("Line too long!");

                    if (counters[i].ContainsKey(line[i]))
                        counters[i][line[i]] += 1;
                    else
                        counters[i][line[i]] = 1;
                }
            }

            foreach (var counter in counters)
            {
                Console.Write(counter.Keys.OrderBy(x => counter[x]).First());
            }
            Console.ReadLine();

        }
    }
}
