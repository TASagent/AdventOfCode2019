using AoCTools;
using AoCTools.IntCode;

const string inputFile = @"../../../../input11.txt";

Console.WriteLine("Day 11 - Space Police");
Console.WriteLine("Star 1");
Console.WriteLine();

string line = File.ReadAllText(inputFile);

long[] regs = line.Split(",").Select(long.Parse).ToArray();

HullPaintingRobot firstRobot = new HullPaintingRobot(1);

IntCodeMachine firstMachine = new IntCodeMachine(
    name: "Star 1",
    regs: regs,
    input: firstRobot.GetLocationColor,
    output: firstRobot.HandleInput);

await firstMachine.RunToEnd();

Console.WriteLine($"The answer is: {firstRobot.PaintedLocations.Count}");

Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();

HullPaintingRobot secondRobot = new HullPaintingRobot(2);

IntCodeMachine secondMachine = new IntCodeMachine(
    name: "Star 2",
    regs: regs,
    input: secondRobot.GetLocationColor,
    output: secondRobot.HandleInput);

await secondMachine.RunToEnd();

HashSet<Point2D> finalLocations = new HashSet<Point2D>(
    secondRobot.PaintedLocations.Where(x => x.Value == 1).Select(x => x.Key));

Point2D min = finalLocations.MinCoordinate();
Point2D max = finalLocations.MaxCoordinate();

Console.BackgroundColor = ConsoleColor.Black;
for (int y = min.y; y <= max.y; y++)
{
    for (int x = min.x; x <= max.x; x++)
    {
        if (finalLocations.Contains((x, y)))
        {
            Console.BackgroundColor = ConsoleColor.White;
        }
        else
        {
            Console.BackgroundColor = ConsoleColor.Black;
        }

        Console.Write(' ');
    }

    Console.BackgroundColor = ConsoleColor.Black;
    Console.WriteLine();
}

Console.WriteLine();
Console.ReadKey();


class HullPaintingRobot
{
    bool paintMode = true;

    public Dictionary<Point2D, long> PaintedLocations { get; } = new Dictionary<Point2D, long>();

    private Point2D location = new Point2D(0, 0);
    private Point2D heading = new Point2D(0, -1);

    public HullPaintingRobot(int puzzle)
    {
        if (puzzle == 2)
        {
            PaintedLocations[location] = 1L;
        }
    }

    public void HandleInput(long value)
    {
        if (paintMode)
        {
            //Paint
            PaintedLocations[location] = value;
        }
        else
        {
            //Rotate
            //Left/Right reversed because of the coordinate system
            heading = heading.Rotate(value == 0);
            location += heading;
        }

        paintMode = !paintMode;
    }

    public long GetLocationColor() => PaintedLocations.GetValueOrDefault(location, 0L);
}
