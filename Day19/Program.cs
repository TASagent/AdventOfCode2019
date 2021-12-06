using AoCTools;
using AoCTools.IntCode;

const string inputFile = @"../../../../input19.txt";

Console.WriteLine("Day 19 - Tractor Beam");
Console.WriteLine("Star 1");
Console.WriteLine();

long[] regs = File.ReadAllText(inputFile).Split(',').Select(long.Parse).ToArray();

long totalAffected = 0;


for (int x = 0; x < 50; x++)
{
    for (int y = 0; y < 50; y++)
    {
        IntCodeMachine machine = new IntCodeMachine(
            name: "Star 1",
            regs: regs,
            fixedInputs: new long[] { x, y });

        await foreach(long value in machine.GetOutputAsync())
        {
            totalAffected += value;
        }
    }
}

Console.WriteLine($"The answer is: {totalAffected}");

Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();


int x0 = 0;
int y1 = 150;
while (true)
{
    //Find first x coordinate of beam.

    while (!await TestCoordinate(x0, y1))
    {
        x0++;
    }

    if (await TestCoordinate(x0 + 99, y1 - 99))
    {
        break;
    }

    y1++;
}

Console.WriteLine($"The answer is: {10000 * x0 + (y1 - 99)}");


Console.WriteLine();
Console.ReadKey();


async Task<bool> TestCoordinate(int x, int y)
{
    IntCodeMachine machine = new IntCodeMachine(
        name: "Star 2",
        regs: regs,
        fixedInputs: new long[] { x, y });

    return 1 == await machine.GetOutputAsync().LastAsync();
}