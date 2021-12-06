using AoCTools.IntCode;

const string inputFile = @"../../../../input21.txt";


Console.WriteLine("Day 21 - Springdroid Adventure");
Console.WriteLine("Star 1");
Console.WriteLine();

long[] regs = File.ReadAllText(inputFile).Split(',').Select(long.Parse).ToArray();

//First test program:

// IF (!A or !B) && C -> Jump

//if !(A and B) && C -> Jump

string input =
    "OR A T\n" +
    "AND B T\n" +
    "AND C T\n" +
    "NOT T J\n" +
    "AND D J\n" +
    "WALK\n";

IntCodeMachine machine = new IntCodeMachine(
    name: "Star 1",
    regs: regs,
    fixedInputs: input.Select(x => (long)x).ToArray());

long output1 = await machine.GetOutputAsync().LastAsync();


Console.WriteLine($"The answer is: {output1}");

Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();

string input2 =
    "OR A T\n" +
    "AND B T\n" +
    "AND C T\n" +
    "NOT T T\n" +
    "AND D T\n" +
    "OR E J\n" +
    "OR H J\n" +
    "AND T J\n" +
    "RUN\n";

IntCodeMachine machine2 = new IntCodeMachine(
    name: "Star 2",
    regs: regs,
    fixedInputs: input2.Select(x => (long)x).ToArray());

long output2 = await machine2.GetOutputAsync().LastAsync();

Console.WriteLine($"The answer is: {output2}");

Console.WriteLine();
Console.ReadKey();
