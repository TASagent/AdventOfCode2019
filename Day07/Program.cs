using AoCTools.IntCode;

const string inputFile = @"../../../../input07.txt";


Console.WriteLine("Day 07 - Amplification Circuit");
Console.WriteLine("Star 1");
Console.WriteLine();

string line = File.ReadAllText(inputFile);

long[] values = line.Split(',').Select(long.Parse).ToArray();
long bestOutput = long.MinValue;
IntCodeMachine[] amplifiers = new IntCodeMachine[5];

foreach (int[] ordering in GetOrderings(new int[5], 0, new bool[5], 0))
{
    for (int i = 0; i < 5; i++)
    {
        long[] input;

        if (i == 0)
        {
            input = new long[] { ordering[i], 0 };
        }
        else
        {
            input = new long[] { ordering[i] };
        }

        int nextMachine = (i + 1) % 5;
        amplifiers[i] = new IntCodeMachine(
            name: $"Machine {i}",
            regs: values,
            fixedInputs: input,
            output: x => amplifiers[nextMachine].WriteValue(x));
    }

    Task[] tasks = amplifiers.Take(4).Select(x => x.RunToEnd()).ToArray();
    long lastOutput = await amplifiers[4].GetOutputAsync().LastAsync();

    bestOutput = Math.Max(bestOutput, lastOutput);

    await Task.WhenAll(tasks);
}

Console.WriteLine($"The answer is: {bestOutput}");


Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();

long bestOutput2 = int.MinValue;

foreach (int[] ordering in GetOrderings(new int[5], 0, new bool[5], 5))
{
    for (int i = 0; i < 5; i++)
    {
        int nextMachine = (i + 1) % 5;
        long[] input;

        if (i == 0)
        {
            input = new long[] { ordering[i], 0 };
        }
        else
        {
            input = new long[] { ordering[i] };
        }

        amplifiers[i] = new IntCodeMachine(
            name: $"Machine {i}",
            regs: values,
            fixedInputs: input,
            output: x => amplifiers[nextMachine].WriteValue(x));

    }

    Task[] tasks = amplifiers.Take(4).Select(x => x.RunToEnd()).ToArray();
    long lastOutput = await amplifiers[4].GetOutputAsync().LastAsync();

    bestOutput2 = Math.Max(bestOutput2, lastOutput);
}

Console.WriteLine($"The answer is: {bestOutput2}");

Console.WriteLine();
Console.ReadKey();



IEnumerable<int[]> GetOrderings(int[] order, int index, bool[] used, int offset)
{
    for (int i = 0; i < 5; i++)
    {
        if (!used[i])
        {
            order[index] = offset + i;
            used[i] = true;

            if (index == 4)
            {
                yield return order;
            }
            else
            {
                foreach (int[] ordering in GetOrderings(order, index + 1, used, offset))
                {
                    yield return ordering;
                }
            }

            used[i] = false;
        }
    }
}