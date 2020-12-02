using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day05
{
    class Program
    {
        private const string inputFile = @"../../../../input05.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Day 5 - Sunny with a Chance of Asteroids");
            Console.WriteLine("Star 1");
            Console.WriteLine();

            int[] instructions = File.ReadAllText(inputFile).Split(',').Select(int.Parse).ToArray();

            List<int> inputs = new List<int>() { 1 };
            List<int> outputs = new List<int>();

            Execute(instructions, inputs, outputs);


            Console.WriteLine(
                $"Program Output: {string.Join(',', outputs.Select(x => x.ToString()))}");


            Console.WriteLine();
            Console.WriteLine("Star 2");
            Console.WriteLine();


            inputs.Clear();
            inputs.Add(5);
            outputs.Clear();

            Execute(instructions, inputs, outputs);

            Console.WriteLine(
                $"Program Output: {string.Join(',', outputs.Select(x => x.ToString()))}");


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

        static int InterpretValue(Dictionary<int, int> memory, int addr, bool positionMode)
        {
            if (positionMode)
            {
                if (memory.ContainsKey(addr))
                {
                    return memory[addr];
                }

                return 0;
            }
            else
            {
                return addr;
            }
        }


        static int Execute(int[] instructions, List<int> input, List<int> output)
        {
            int addr = 0;
            int inputIndex = 0;

            Dictionary<int, int> memory = new Dictionary<int, int>();

            for (int i = 0; i < instructions.Length; i++)
            {
                memory[i] = instructions[i];
            }


            while (true)
            {
                int instr = GetValue(memory, addr);
                int opCode = instr % 100;

                bool argAPositionMode = ((instr / 100) % 10) == 0;
                bool argBPositionMode = ((instr / 1000) % 10) == 0;
                bool argCPositionMode = ((instr / 10000) % 10) == 0;

                switch (opCode)
                {
                    case 1:
                        //ADD X, Y -> Z
                        {
                            int position1 = GetValue(memory, addr + 1);
                            int position2 = GetValue(memory, addr + 2);
                            int outputPosition = GetValue(memory, addr + 3);

                            memory[outputPosition] = 
                                InterpretValue(memory, position1, argAPositionMode) +
                                InterpretValue(memory, position2, argBPositionMode);

                            addr += 4;
                        }
                        break;

                    case 2:
                        //MULT X, Y -> Z
                        {
                            int position1 = GetValue(memory, addr + 1);
                            int position2 = GetValue(memory, addr + 2);
                            int outputPosition = GetValue(memory, addr + 3);

                            memory[outputPosition] = 
                                InterpretValue(memory, position1, argAPositionMode) *
                                InterpretValue(memory, position2, argBPositionMode);

                            addr += 4;
                        }
                        break;

                    case 3:
                        //INPUT -> X
                        {
                            int outputPosition = GetValue(memory, addr + 1);

                            memory[outputPosition] = input[inputIndex++];

                            addr += 2;
                        }
                        break;

                    case 4:
                        //OUTPUT -> X
                        {
                            int position1 = GetValue(memory, addr + 1);
                            output.Add(InterpretValue(memory, position1, argAPositionMode));

                            addr += 2;
                        }
                        break;

                    case 5:
                        //Jump If True
                        {
                            int position1 = GetValue(memory, addr + 1);
                            int value1 = InterpretValue(memory, position1, argAPositionMode);

                            if (value1 != 0)
                            {
                                int position2 = GetValue(memory, addr + 2);
                                int value2 = InterpretValue(memory, position2, argBPositionMode);

                                addr = value2;
                            }
                            else
                            {
                                addr += 3;
                            }
                        }
                        break;

                    case 6:
                        //Jump If False
                        {
                            int position1 = GetValue(memory, addr + 1);
                            int value1 = InterpretValue(memory, position1, argAPositionMode);

                            if (value1 == 0)
                            {
                                int position2 = GetValue(memory, addr + 2);
                                int value2 = InterpretValue(memory, position2, argBPositionMode);

                                addr = value2;
                            }
                            else
                            {
                                addr += 3;
                            }
                        }
                        break;

                    case 7:
                        //Less Than
                        {
                            int position1 = GetValue(memory, addr + 1);
                            int position2 = GetValue(memory, addr + 2);
                            int position3 = GetValue(memory, addr + 3);

                            int value1 = InterpretValue(memory, position1, argAPositionMode);
                            int value2 = InterpretValue(memory, position2, argBPositionMode);

                            memory[position3] = value1 < value2 ? 1 : 0;

                            addr += 4;
                        }
                        break;

                    case 8:
                        //Equals
                        {
                            int position1 = GetValue(memory, addr + 1);
                            int position2 = GetValue(memory, addr + 2);
                            int position3 = GetValue(memory, addr + 3);

                            int value1 = InterpretValue(memory, position1, argAPositionMode);
                            int value2 = InterpretValue(memory, position2, argBPositionMode);

                            memory[position3] = value1 == value2 ? 1 : 0;

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
