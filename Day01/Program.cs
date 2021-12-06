const string inputFile = @"../../../../input01.txt";

Console.WriteLine("Day 01 - The Tyranny of the Rocket Equation");
Console.WriteLine("Star 1");
Console.WriteLine();

string[] lines = File.ReadAllLines(inputFile);

int[] masses = lines.Select(int.Parse).ToArray();

int fuel = masses.Select(GetFuel).Sum();

Console.WriteLine($"The answer is: {fuel}");

Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();

int totalFuel = masses
    .Select(GetFinalFuel)
    .Sum();

Console.WriteLine($"The answer is: {totalFuel}");


Console.WriteLine();
Console.ReadKey();

static int GetFuel(int mass)
{
    return (mass / 3) - 2;
}

static int GetFinalFuel(int mass)
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