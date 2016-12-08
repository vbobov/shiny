using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode7
{
    class Program
    {
        static void Main(string[] args)
        {
            int ipCount = 0;
            int sslCount = 0;
            string pattern = @"(?<normal>\w+)+|(\[(?<bracketed>\w+)\])+";

            StreamReader sr = new StreamReader("input.txt");
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (Regex.IsMatch(line, pattern))
                {
                    var matches = Regex.Matches(line, pattern);
                    var normalStrings = new List<string>();
                    var bracketedStrings = new List<string>();

                    foreach (Match match in matches)
                    {
                        if (match.Groups["normal"].Length != 0)
                        {
                            normalStrings.Add(match.Groups["normal"].Value);
                        }
                        else if (match.Groups["bracketed"].Length != 0)
                        {
                            bracketedStrings.Add(match.Groups["bracketed"].Value);
                        }
                    }

                    //if (normalStrings.Any(x => ContainsAbba(x)) && !bracketedStrings.Any(x => ContainsAbba(x)))
                    //{
                    //    Console.WriteLine("Normal");
                    //    normalStrings.ForEach(x => Console.WriteLine(x));
                    //    Console.WriteLine("Bracketed");
                    //    bracketedStrings.ForEach(x => Console.WriteLine(x));
                    //    ipCount++;
                    //}

                    List<string> abas = new List<string>();
                    normalStrings.ForEach(x => abas.AddRange(GetAbas(x)));
                    foreach (var aba in abas)
                    {
                        
                        if (bracketedStrings.Any(x => x.Contains(InvertAba(aba))))
                        {
                            Console.WriteLine(aba);
                            bracketedStrings.ForEach(x => Console.WriteLine(x));
                            sslCount++;
                            break;
                        }
                    }
                }
                else
                {
                    throw new Exception("Couldn't match line!");
                }
            }
            Console.WriteLine(sslCount);
            Console.ReadLine();
        }

        static bool ContainsAbba(string input)
        {
            int pos = 0;
            while (pos <= input.Length - 4)
            {
                if (IsAbba(input.Substring(pos, 4)))
                    return true;
                pos++;
            }
            return false;
        }

        static List<string> GetAbas(string input)
        {
            var foundList = new List<string>();
            int pos = 0;
            while (pos <= input.Length - 3)
            {
                if (IsAba(input.Substring(pos, 3)))
                {
                    foundList.Add(input.Substring(pos, 3));
                }
                pos++;
            }
            return foundList;
        }

        static bool IsAba(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length != 3)
                throw new ArgumentException();

            return (input[0] == input[2] && input[0] != input[1]);

        }

        static string InvertAba(string input)
        {
            if (!IsAba(input))
                throw new ArgumentException();

            StringBuilder sb = new StringBuilder();
            sb.Append(input[1]);
            sb.Append(input[0]);
            sb.Append(input[1]);
            return sb.ToString();
        }
        static bool IsAbba(string input)
        {
            // test 4-char string
            if (string.IsNullOrWhiteSpace(input) || input.Length != 4)
                throw new ArgumentException();

            if (input[0] == input[1])
                return false;

            return input.Substring(0, 2) == new string(input.Substring(2, 2).Reverse().ToArray());
        }
    }
}
