using AoCTools.IntCode;

const string inputFile = @"../../../../input09.txt";

Console.WriteLine("Day 09 - Sensor Boost");
Console.WriteLine("Star 1");
Console.WriteLine();

long[] regs = File.ReadAllText(inputFile)
    .Split(",")
    .Select(long.Parse)
    .ToArray();

IntCodeMachine machine = new IntCodeMachine(
    name: "Star 1",
    regs: regs,
    fixedInputs: new long[] { 1 });

Console.WriteLine($"Star 1 output: {await machine.RunToOutput()}");


Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();

IntCodeMachine machine2 = new IntCodeMachine(
    name: "Star 2",
    regs: regs,
    fixedInputs: new long[] { 2 });

Console.WriteLine($"Star 2 output: {await machine2.RunToOutput()}");

Console.WriteLine();
Console.ReadKey();
