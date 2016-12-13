using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode10
{
    class Receiver
    {
        public int Id { get; private set; }

        public List<int> Chips { get; set; }

        public virtual void AddChip(int chipId)
        {
            Chips.Add(chipId);
        }

        public Receiver(int id)
        {
            this.Id = id;
            Chips = new List<int>();
        }
    }

    class Robot : Receiver
    {
        Receiver lowTarget;
        public Receiver LowTarget
        {
            get
            {
                return lowTarget;
            }
            set
            {
                if (lowTarget != default(Receiver))
                    throw new Exception($"Trying to set LowTarget twice on bot {Id}");

                lowTarget = value;
            }
        }

        Receiver highTarget;
        public Receiver HighTarget
        {
            get
            {
                return highTarget;
            }
            set
            {
                if (highTarget != default(Receiver))
                    throw new Exception($"Trying to set HighTarget twice on bot {Id}");

                highTarget = value;
            }
        }

        public Robot(int id) : base(id) { }

        public override void AddChip(int chip)
        {
            if (Chips.Count >= 2)
                throw new Exception("Trying to add a chip to a full stack");

            Chips.Add(chip);

            if (Chips.Count == 2)
            {
                // got 2, time to redistribute
                int lowChip = Chips.OrderBy(x => x).ElementAt(0);
                int highChip = Chips.OrderBy(x => x).ElementAt(1);

                if (lowChip == 17 && highChip == 61)
                    Console.WriteLine($"Bot {Id} is the one!");

                LowTarget.AddChip(lowChip);
                HighTarget.AddChip(highChip);

                Chips.Clear();
            }
        }
    }

    class Output : Receiver
    {
        public Output(int id) : base(id) { }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string line;

            string botGetPattern = @"^value (\d+) goes to bot (\d+)$";
            string botGivePattern = @"^bot (\d+) gives low to (\S+) (\d+) and high to (\S+) (\d+)$";

            Dictionary<int, Robot> bots = new Dictionary<int, Robot>();
            Dictionary<int, Output> outputs = new Dictionary<int, Output>();

            StreamReader sr = new StreamReader("input.txt");

            while ((line = sr.ReadLine()) != null)
            {
                if (Regex.IsMatch(line, botGivePattern))
                {
                    var match = Regex.Match(line, botGivePattern);
                    int sourceBot = int.Parse(match.Groups[1].Value);
                    string lowDestType = match.Groups[2].Value;
                    int lowDestId = int.Parse(match.Groups[3].Value);
                    string highDestType = match.Groups[4].Value;
                    int highDestId = int.Parse(match.Groups[5].Value);
                    Console.WriteLine($"Bot {sourceBot} gives low to {lowDestType} {lowDestId} and high to {highDestType} {highDestId}");

                    var setBot = bots.ContainsKey(sourceBot) ? bots[sourceBot] : new Robot(sourceBot);

                    if (!bots.ContainsKey(sourceBot))
                        bots[sourceBot] = setBot;

                    if (lowDestType == "bot")
                    {
                        var targetBot = bots.ContainsKey(lowDestId) ? bots[lowDestId] : new Robot(lowDestId);
                        if (!bots.ContainsKey(lowDestId))
                            bots[lowDestId] = targetBot;
                        setBot.LowTarget = targetBot;

                    }
                    else if (lowDestType == "output")
                    {
                        var targetOutput = outputs.ContainsKey(lowDestId) ? outputs[lowDestId] : new Output(lowDestId);
                        if (!outputs.ContainsKey(lowDestId))
                            outputs[lowDestId] = targetOutput;
                        setBot.LowTarget = targetOutput;
                    }
                    else
                    {
                        throw new Exception($"Unknown low dest type {lowDestType}");
                    }

                    if (highDestType == "bot")
                    {
                        var targetBot = bots.ContainsKey(highDestId) ? bots[highDestId] : new Robot(highDestId);
                        if (!bots.ContainsKey(highDestId))
                            bots[highDestId] = targetBot;
                        setBot.HighTarget = targetBot;

                    }
                    else if (highDestType == "output")
                    {
                        var targetOutput = outputs.ContainsKey(highDestId) ? outputs[highDestId] : new Output(highDestId);
                        if (!outputs.ContainsKey(highDestId))
                            outputs[highDestId] = targetOutput;
                        setBot.HighTarget = targetOutput;
                    }
                    else
                    {
                        throw new Exception($"Unknown high dest type {highDestType}");
                    }
                }
            }

            sr = new StreamReader("input.txt");

            while ((line = sr.ReadLine()) != null)
            {
                if (Regex.IsMatch(line, botGetPattern))
                {
                    var match = Regex.Match(line, botGetPattern);
                    int value = int.Parse(match.Groups[1].Value);
                    int botNum = int.Parse(match.Groups[2].Value);

                    if (!bots.ContainsKey(botNum))
                        throw new Exception($"Don't know about bot {botNum}");

                    bots[botNum].AddChip(value);

                    //Console.WriteLine($"{value} to {botNum}");
                }
            }

            int mult = 1;
            for (int i=0;i<3;i++)
            {
                outputs[i].Chips.ForEach(x => mult *= x);
            }
            Console.WriteLine(mult);
            Console.ReadLine();
        }
    }
}
