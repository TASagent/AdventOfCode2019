using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AoCTools;

namespace Day01
{
    class Program
    {
        private const string inputFile = @"../../../../input01.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Day 1 - The Tyranny of the Rocket Equation");
            Console.WriteLine("Star 1");
            Console.WriteLine();

            int[] masses = File.ReadAllLines(inputFile).Select(int.Parse).ToArray();

            int[] fuel = masses.Select(GetFuel).ToArray();

            int output1 = fuel.Sum();



            Console.WriteLine($"The answer is: {output1}");

            Console.WriteLine();
            Console.WriteLine("Star 2");
            Console.WriteLine();


            int output2 = File.ReadAllLines(inputFile).Select(int.Parse).Select(GetTotalFuel).Sum();



            Console.WriteLine($"The answer is: {output2}");


            Console.WriteLine();
            Console.ReadKey();
        }

        static int GetFuel(int mass)
        {
            return (mass / 3) - 2;
        }

        static int GetTotalFuel(int mass)
        {
            int fuel = GetFuel(mass);
            int totalFuel = 0;

            while (fuel > 0)
            {
                totalFuel += fuel;

                fuel = GetFuel(fuel);
            }

            return totalFuel;
        }

    }
}
