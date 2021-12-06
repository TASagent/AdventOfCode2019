using AoCTools.IntCode;

const string inputFile = @"../../../../input23.txt";

Console.WriteLine("Day 23 - Category Six");
Console.WriteLine("Star 1");
Console.WriteLine();

bool receivedFirstMessage = false;

long natX = 0;
long natY = 0;
long lastSentY = long.MaxValue;

Dictionary<long, NonBlockingIntCodeMachine> machineLookup = new Dictionary<long, NonBlockingIntCodeMachine>();

long[] regs = File.ReadAllText(inputFile).Split(',').Select(long.Parse).ToArray();

for (long i = 0; i < 50; i++)
{
    machineLookup[i] = new NonBlockingIntCodeMachine(
        name: $"Computer {i}",
        regs: regs,
        networkAddress: i,
        output: HandleMessage);
}

while (true)
{
    bool idle = true;
    for (int i = 0; i < 50; i++)
    {
        idle &= machineLookup[i].ExecuteStep();
    }

    if (idle)
    {
        if (natY == lastSentY)
        {
            break;
        }

        lastSentY = natY;
        machineLookup[0].WriteValues(natX, natY);
    }
}


Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();

Console.WriteLine($"The answer is: {natY}");

Console.WriteLine();
Console.ReadKey();



void HandleMessage(long address, long x, long y)
{
    if (address == 255 && !receivedFirstMessage)
    {
        receivedFirstMessage = true;
        Console.WriteLine($"The answer is: ({x},{y})");
    }

    if (address >= 0 && address < 50)
    {
        machineLookup[address].WriteValues(x, y);
    }
    else if (address == 255)
    {
        natX = x;
        natY = y;
    }
    else
    {
        Console.WriteLine($"Could not send message to address {address}: ({x},{y})");
    }
}
