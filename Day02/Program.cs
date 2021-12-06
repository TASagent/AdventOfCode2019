using AoCTools.IntCode;

const string inputFile = @"../../../../input02.txt";

Console.WriteLine("Day 02 - 1202 Program Alarm");
Console.WriteLine("Star 1");
Console.WriteLine();

long[] initialRegs = File.ReadAllText(inputFile).Split(',').Select(long.Parse).ToArray();

IntCodeMachine intCode = new IntCodeMachine(
    name: "Star 1 Machine",
    regs: initialRegs)
{
    [1] = 12,
    [2] = 2
};

await intCode.RunToEnd();

Console.WriteLine($"The answer is: {intCode[0]}");

Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();

int matchingInput = await FindMatch(initialRegs);
Console.WriteLine($"The answer is: {matchingInput}");

Console.WriteLine();
Console.ReadKey();


static async Task<int> FindMatch(long[] initialRegs)
{
    for (int noun = 0; noun < 100; noun++)
    {
        for (int verb = 0; verb < 100; verb++)
        {
            IntCodeMachine intCode = new IntCodeMachine(
                name: $"Star 2 Machine ({noun},{verb})",
                regs: initialRegs)
            {
                [1] = noun,
                [2] = verb
            };

            await intCode.RunToEnd();

            if (intCode[0] == 19690720)
            {
                return 100 * noun + verb;
            }
        }
    }

    throw new Exception("Result not found");
}