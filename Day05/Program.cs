using AoCTools.IntCode;

const string inputFile = @"../../../../input05.txt";


Console.WriteLine("Day 05 - Sunny with a Chance of Asteroids");
Console.WriteLine("Star 1");
Console.WriteLine();

string line = File.ReadAllText(inputFile);

long[] values = line.Split(',').Select(long.Parse).ToArray();

IntCodeMachine machine = new IntCodeMachine(
    name: $"Star 1 Machine",
    regs: values,
    fixedInputs: new[] { 1L });

long output1 = await machine.GetOutputAsync().LastAsync();

Console.WriteLine($"The answer is: {output1}");


Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();

IntCodeMachine machine2 = new IntCodeMachine(
    name: $"Star 2 Machine",
    regs: values,
    fixedInputs: new[] { 5L });

long output2 = await machine2.GetOutputAsync().LastAsync();

Console.WriteLine($"The answer is: {output2}");

Console.WriteLine();
Console.ReadKey();