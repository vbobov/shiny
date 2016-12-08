using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace AdventOfCode5
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "wtnhxymk";
            StringBuilder pw = new StringBuilder();
            char[] finalPw = new char[8];
            int finalPwCharCount = 0;

            using (MD5 md5 = MD5.Create())
            {
                for (int i=0;i<int.MaxValue;i++)
                {
                    var x = string.Concat(input, i);
                    var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(x));
                    StringBuilder sb = new StringBuilder();
                    for (int j=0;j<hash.Length;j++)
                    {
                        sb.Append(hash[j].ToString("x2"));
                    }
                    if (sb.ToString().Substring(0, 5) == "00000")
                    {
                        //var pwLetter = sb.ToString().Substring(5, 1);
                        //pw.Append(pwLetter);
                        //if (pw.Length == 8)
                        //    break;

                        var pos = int.Parse(sb.ToString().Substring(5, 1), System.Globalization.NumberStyles.HexNumber);
                        var pwChar = sb.ToString().Substring(6, 1);

                        if (pos >= 0 && pos < 8 && finalPw[pos] == default(char))
                        {
                            finalPw[pos] = pwChar[0];
                            finalPwCharCount++;

                            Console.WriteLine(pwChar[0]);

                            if (finalPwCharCount == 8)
                                break;
                        }
                    }
                }
            }
            Console.WriteLine(finalPw);
            Console.ReadLine();
        }
    }
}
