using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render3DLib
{
    public static class Serial
    {
        public static void println(string message)
        {
            Console.WriteLine(message);
        }
        public static void print(string message)
        {
            Console.Write(message);
        }

        public static void print(int i)
        {
            Console.Write(i);
        }
        public static void print(double d)
        {
            Console.Write(d);
        }
        public static void println(double d)
        {
            Console.Write(d);
        }
    }
}
