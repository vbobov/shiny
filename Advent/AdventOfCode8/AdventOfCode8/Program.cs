using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace AdventOfCode8
{
    class Program
    {
        static readonly int Height = 6;
        static readonly int Width = 50;
        static bool[,] Grid = new bool[Height,Width];
        static readonly string PatternRect = @"^rect\s(\d+)x(\d+)";
        static readonly string PatternRow = @"rotate row y=(\d+) by (\d+)$";
        static readonly string PatternCol = @"rotate column x=(\d+) by (\d+)$";

        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader("input.txt");
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (Regex.IsMatch(line, PatternRect))
                {
                    int width = int.Parse(Regex.Match(line, PatternRect).Groups[1].Value);
                    int height = int.Parse(Regex.Match(line, PatternRect).Groups[2].Value);

                    for (int j=0;j< height;j++)
                    {
                        for (int i = 0; i < width; i++)
                        {
                            Grid[j,i] = true;
                        }
                    }
                    //Console.WriteLine($"Width {width} height {height}");
                    //PrintRect();
                    //Console.ReadLine();
                }
                else if (Regex.IsMatch(line, PatternRow))
                {
                    int row = int.Parse(Regex.Match(line, PatternRow).Groups[1].Value);
                    int bump = int.Parse(Regex.Match(line, PatternRow).Groups[2].Value);

                    for (int i = 0; i < bump; i++)
                        ShiftRow(row);

                    //PrintRect();
                    //Console.ReadLine();
                    //Console.WriteLine($"Row {row} bump {bump}");
                }
                else if (Regex.IsMatch(line, PatternCol))
                {
                    int col = int.Parse(Regex.Match(line, PatternCol).Groups[1].Value);
                    int bump = int.Parse(Regex.Match(line, PatternCol).Groups[2].Value);

                    for (int i = 0; i < bump; i++)
                        ShiftCol(col);
                    //Console.WriteLine($"Column {col} bump {bump}");
                }
            }
            int total = 0;
            for (int i=0;i<Height;i++)
            {
                for (int j=0;j<Width;j++)
                {
                    if (Grid[i, j])
                        total++;
                }
            }
            PrintRect();
            Console.WriteLine(total);
            Console.ReadLine();
        }

        static void ShiftRow(int row)
        {
            bool overflow = Grid[row, Width - 1];
            for (int col = Width-1;col > 0;col--)
            {
                Grid[row, col] = Grid[row, col - 1];
            }
            Grid[row, 0] = overflow;
        }

        static void ShiftCol(int col)
        {
            bool overflow = Grid[Height-1, col];
            for (int row = Height - 1; row > 0; row--)
            {
                Grid[row, col] = Grid[row-1, col];
            }
            Grid[0, col] = overflow;
        }
        static void PrintRect()
        {
            for (int i = 0; i < Height; i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int j=0;j<Width;j++)
                {
                    if (Grid[i,j])
                    {
                        sb.Append("#");
                    }
                    else
                    {
                        sb.Append(".");
                    }
                }
                Console.WriteLine(sb);
            }
        }
    }
}
