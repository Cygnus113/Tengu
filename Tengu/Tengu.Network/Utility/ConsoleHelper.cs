using System;
using System.Collections.Generic;
using System.Text;

namespace Tengu.Network
{
    public static class ConsoleHelper
    {
        public static void WriteLine(string line)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(line);
        }
        public static void WriteLine(string line, Boolean success)
        {   // Green for success and Red for failure
            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine(line);
            // Revert to original
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void WriteLine(string line, ConsoleColor color)
        {   // Custom Color
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
