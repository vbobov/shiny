using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace AdventOfCode12
{
    class Program
    {
        public static readonly string copyRegPattern = @"^cpy ([a-d]) ([a-d])$";
        public static readonly string copyIntPattern = @"^cpy (-?\d+) ([a-d])$";
        public static readonly string increasePattern = @"^inc ([a-d])$";
        public static readonly string decreasePattern = @"^dec ([a-d])$";
        public static readonly string jnzRegPattern = @"^jnz ([a-d]) (-?\d+)$";
        public static readonly string jnzIntPattern = @"^jnz (-?\d+) (-?\d+)$";

        static Dictionary<string, int> registers = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            
            registers.Add("a", 0);
            registers.Add("b", 0);
            registers.Add("c", 1);
            registers.Add("d", 0);

            List<string> instrList = new List<string>();

            StreamReader sr = new StreamReader("input.txt");
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                instrList.Add(line);
            }

            int i = 0;
            while (i < instrList.Count)
            {
                var instr = instrList[i];
                int jump = 0;

                if (Regex.IsMatch(instr, copyRegPattern))
                {
                    var reg1 = Regex.Match(instr, copyRegPattern).Groups[1].Value;
                    var reg2 = Regex.Match(instr, copyRegPattern).Groups[2].Value;
                    registers[reg2] = registers[reg1];
                }
                else if (Regex.IsMatch(instr, copyIntPattern))
                {
                    var num = int.Parse(Regex.Match(instr, copyIntPattern).Groups[1].Value);
                    var reg = Regex.Match(instr, copyIntPattern).Groups[2].Value;
                    registers[reg] = num;
                }
                else if (Regex.IsMatch(instr, increasePattern))
                {
                    var reg = Regex.Match(instr, increasePattern).Groups[1].Value;
                    registers[reg] += 1;
                }
                else if (Regex.IsMatch(instr, decreasePattern))
                {
                    var reg = Regex.Match(instr, decreasePattern).Groups[1].Value;
                    registers[reg] -= 1;
                }
                else if (Regex.IsMatch(instr, jnzRegPattern))
                {
                    var num = int.Parse(Regex.Match(instr, jnzRegPattern).Groups[2].Value);
                    var reg = Regex.Match(instr, jnzRegPattern).Groups[1].Value;
                    jump = registers[reg] != 0 ? num : 0;
                }
                else if (Regex.IsMatch(instr, jnzIntPattern))
                {
                    var num1 = int.Parse(Regex.Match(instr, jnzIntPattern).Groups[1].Value);
                    var num2 = int.Parse(Regex.Match(instr, jnzIntPattern).Groups[2].Value);
                    jump = num1 != 0 ? num2 : 0;
                }
                else
                    throw new ArgumentException();

                Debug.Assert(i + jump >= 0);
                if (jump != 0)
                    i += jump;
                else
                    i++;
            }

            Console.WriteLine(registers["a"]);
            Console.ReadLine();
        }

        public static void ExecuteInstruction(string instr)
        {
            
        }
    }
}
