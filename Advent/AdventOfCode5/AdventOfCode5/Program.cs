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

                        var pol = sb.ToString().Substring(5, 1);
                        var pwChar = sb.ToString().Substring(6, 1);
                    }
                }
            }
            Console.WriteLine(pw);
            Console.ReadLine();
        }
    }
}
