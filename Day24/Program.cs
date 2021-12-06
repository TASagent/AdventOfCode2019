using System.Text;

const string inputFile = @"../../../../input24.txt";


Console.WriteLine("Day 24 - Planet of Discord");
Console.WriteLine("Star 1");
Console.WriteLine();

bool[,] priorMap = new bool[5, 5];
bool[,] currentMap = new bool[5, 5];

HashSet<int> seenLayouts = new HashSet<int>();

string input = File.ReadAllText(inputFile).Replace("\n", "").Replace("\r", "");

for (int i = 0; i < 25; i++)
{
    priorMap[(i % 5), (i / 5)] = (input[i] == '#');
}

while (seenLayouts.Add(EncodeMap(priorMap)))
{
    for (int x = 0; x < 5; x++)
    {
        for (int y = 0; y < 5; y++)
        {
            currentMap[x, y] = IsAlive(priorMap, x, y);
        }
    }

    (priorMap, currentMap) = (currentMap, priorMap);
}

Console.WriteLine($"The answer is: {EncodeMap(priorMap)}");

Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();

//string testInput =
//    "....#" +
//    "#..#." +
//    "#.?##" +
//    "..#.." +
//    "#....";

RecursiveMap startingMap = new RecursiveMap(input);

for (int i = 0; i < 200; i++)
{
    startingMap.Update();
}

startingMap.PrintAll();

Console.WriteLine($"The answer is: {startingMap.GetBugCount()}");



Console.WriteLine();
Console.ReadKey();


int EncodeMap(bool[,] map)
{
    int output = 0;

    for (int i = 0; i < 25; i++)
    {
        if (map[(i % 5), (i / 5)])
        {
            output |= (1 << i);
        }
    }

    return output;
}

bool IsAlive(bool[,] map, int x, int y)
{
    int livingNeighbors = 0;

    if (x > 0 && map[x - 1, y])
    {
        livingNeighbors++;
    }

    if (x < 4 && map[x + 1, y])
    {
        livingNeighbors++;
    }

    if (y > 0 && map[x, y - 1])
    {
        livingNeighbors++;
    }

    if (y < 4 && map[x, y + 1])
    {
        livingNeighbors++;
    }

    return livingNeighbors == 1 || (!map[x, y] && livingNeighbors == 2);
}


class RecursiveMap
{
    private readonly int depth;

    bool[,] priorMap = new bool[5, 5];
    bool[,] currentMap = new bool[5, 5];

    public RecursiveMap Inside { get; private set; } = null;
    public RecursiveMap Outside { get; private set; } = null;

    private RecursiveMap(int depth)
    {
        this.depth = depth;
    }

    public RecursiveMap(string input)
    {
        for (int i = 0; i < 25; i++)
        {
            priorMap[(i % 5), (i / 5)] = (input[i] == '#');
        }

        depth = 0;

        Inside = new RecursiveMap(depth + 1)
        {
            Outside = this
        };

        Outside = new RecursiveMap(depth - 1)
        {
            Inside = this
        };
    }

    public override string ToString() => $"Depth {depth}";

    public void PrintAll()
    {
        RecursiveMap top = Outside;

        while (top.Outside is not null)
        {
            top = top.Outside;
        }

        while (top is not null)
        {
            top.Print();
            top = top.Inside;
        }
    }

    private void Print()
    {
        Console.WriteLine($"Depth {depth}:");

        char[] line = new char[5];
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (x == 2 && y == 2)
                {
                    line[x] = '?';
                }
                else
                {
                    line[x] = priorMap[x, y] ? '#' : '.';
                }
            }

            Console.WriteLine(line);
        }

        Console.WriteLine();
    }


    public void Update()
    {
        Update(true, true);
        Advance(true, true);
    }

    private void Update(bool outward, bool inward)
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (x == 2 && y == 2)
                {
                    continue;
                }

                currentMap[x, y] = IsAlive(x, y);
            }
        }

        if (outward && Outside is not null)
        {
            Outside.Update(true, false);
        }

        if (inward && Inside is not null)
        {
            Inside.Update(false, true);
        }
    }

    private void Advance(bool outward, bool inward)
    {
        if (outward && Outside is not null)
        {
            Outside.Advance(true, false);
        }

        if (inward && Inside is not null)
        {
            Inside.Advance(false, true);
        }

        if (Inside is null &&
            (currentMap[2, 1] || currentMap[2, 3] || currentMap[1, 2] || currentMap[3, 2]))
        {
            Inside = new RecursiveMap(depth + 1)
            {
                Outside = this
            };
        }

        if (Outside is null)
        {
            bool spawnOutside = false;

            for (int i = 0; i < 5; i++)
            {
                spawnOutside |= currentMap[0, i];
                spawnOutside |= currentMap[i, 0];
                spawnOutside |= currentMap[4, i];
                spawnOutside |= currentMap[i, 4];

                if (spawnOutside) break;
            }

            if (spawnOutside)
            {
                Outside = new RecursiveMap(depth - 1)
                {
                    Inside = this
                };
            }
        }

        (priorMap, currentMap) = (currentMap, priorMap);
    }

    bool IsAlive(int x, int y)
    {
        int livingNeighbors = 0;

        //Check Left
        if (x == 0)
        {
            //Check Outside
            if (Outside is not null && Outside.priorMap[1, 2])
            {
                livingNeighbors++;
            }
        }
        else if (x == 3 && y == 2)
        {
            //Check Inside
            if (Inside is not null)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (Inside.priorMap[4, i])
                    {
                        livingNeighbors++;
                    }
                }
            }
        }
        else if (priorMap[x - 1, y])
        {
            //Check Self
            livingNeighbors++;
        }

        //Check Right
        if (x == 4)
        {
            //Check Outside
            if (Outside is not null && Outside.priorMap[3, 2])
            {
                livingNeighbors++;
            }
        }
        else if (x == 1 && y == 2)
        {
            //Check Inside
            if (Inside is not null)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (Inside.priorMap[0, i])
                    {
                        livingNeighbors++;
                    }
                }
            }

        }
        else if (priorMap[x + 1, y])
        {
            //Check Self
            livingNeighbors++;
        }

        //Check Above
        if (y == 0)
        {
            //Check Outside
            if (Outside is not null && Outside.priorMap[2, 1])
            {
                livingNeighbors++;
            }

        }
        else if (x == 2 && y == 3)
        {
            //Check Inside
            if (Inside is not null)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (Inside.priorMap[i, 4])
                    {
                        livingNeighbors++;
                    }
                }
            }

        }
        else if (priorMap[x, y - 1])
        {
            //Check Self
            livingNeighbors++;
        }

        //Check Below
        if (y == 4)
        {
            //Check Outside
            if (Outside is not null && Outside.priorMap[2, 3])
            {
                livingNeighbors++;
            }
        }
        else if (x == 2 && y == 1)
        {
            //Check Inside
            if (Inside is not null)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (Inside.priorMap[i, 0])
                    {
                        livingNeighbors++;
                    }
                }
            }

        }
        else if (priorMap[x, y + 1])
        {
            //Check Self
            livingNeighbors++;
        }

        return livingNeighbors == 1 || (!priorMap[x, y] && livingNeighbors == 2);
    }

    public int GetBugCount() => GetBugCount(true, true);

    private int GetBugCount(bool outward, bool inward)
    {
        int count = 0;
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (x == 2 && y == 2)
                {
                    continue;
                }

                if (priorMap[x, y])
                {
                    count++;
                }
            }
        }

        if (outward && Outside is not null)
        {
            count += Outside.GetBugCount(true, false);
        }

        if (inward && Inside is not null)
        {
            count += Inside.GetBugCount(false, true);
        }

        return count;
    }
}