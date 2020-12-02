using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day02
{
    class Program
    {
        private const string inputFile = @"../../../../input02.txt";

        private const int TARGET_VALUE = 19690720;

        static void Main(string[] args)
        {
            Console.WriteLine("Day 2 - 1202 Program Alarm");
            Console.WriteLine("Star 1");
            Console.WriteLine();

            int[] instructions = File.ReadAllText(inputFile).Split(',').Select(int.Parse).ToArray();

            instructions[1] = 12;
            instructions[2] = 2;

            int output1 = Execute(instructions);



            Console.WriteLine($"The answer is: {output1}");

            Console.WriteLine();
            Console.WriteLine("Star 2");
            Console.WriteLine();

            int noun = -1;
            int verb = -1;

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    instructions[1] = i;
                    instructions[2] = j;

                    if (Execute(instructions) == TARGET_VALUE)
                    {
                        noun = i;
                        verb = j;

                        break;
                    }
                }

                if (noun != -1)
                {
                    break;
                }
            }

            Console.WriteLine($"Found it at ({noun}, {verb})");

            int output2 = 100 * noun + verb;

            Console.WriteLine($"The answer is: {output2}");


            Console.WriteLine();
            Console.ReadKey();
        }

        static int GetValue(Dictionary<int, int> memory, int addr)
        {
            if (memory.ContainsKey(addr))
            {
                return memory[addr];
            }

            return 0;
        }


        static int Execute(int[] instructions)
        {
            int addr = 0;

            Dictionary<int, int> memory = new Dictionary<int, int>();

            for (int i = 0; i < instructions.Length; i++)
            {
                memory[i] = instructions[i];
            }

            while (true)
            {
                switch (GetValue(memory, addr))
                {
                    case 1:
                        //ADD X, Y -> Z
                        {
                            int position1 = GetValue(memory, addr + 1);
                            int position2 = GetValue(memory, addr + 2);
                            int outputPosition = GetValue(memory, addr + 3);

                            memory[outputPosition] = GetValue(memory, position1) + GetValue(memory, position2);
                            addr += 4;
                        }
                        break;

                    case 2:
                        //MULT X, Y -> Z
                        {
                            int position1 = GetValue(memory, addr + 1);
                            int position2 = GetValue(memory, addr + 2);
                            int outputPosition = GetValue(memory, addr + 3);

                            memory[outputPosition] = GetValue(memory, position1) * GetValue(memory, position2);
                            addr += 4;
                        }
                        break;

                    case 99:
                        //HALT operation
                        return memory[0];

                    default:
                        throw new Exception($"Unexpected OpCode: memory[{addr}] = {GetValue(memory, addr)}");
                }
            }

        }


    }
}
