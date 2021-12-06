using AoCTools;
using AoCTools.IntCode;

const string inputFile = @"../../../../input13.txt";

Dictionary<Point2D, int> tileMap = new Dictionary<Point2D, int>();

Point2D ballPos = Point2D.Zero;
Point2D paddlePos = Point2D.Zero;
int inputStage = 0;
int cachedX = 0;
Point2D cachedPoint = Point2D.Zero;
int score = 0;

Console.WriteLine("Day 13 - Care Package");
Console.WriteLine("Star 1");
Console.WriteLine();

long[] regs = File.ReadAllText(inputFile).Split(',').Select(long.Parse).ToArray();

IntCodeMachine machine = new IntCodeMachine(
    name: "Star 1",
    regs: regs,
    output: RenderOutput1);

await machine.RunToEnd();

Console.WriteLine($"The answer is: {tileMap.Values.Where(x => x == 2).Count()}");

Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();

//Reset
tileMap.Clear();
inputStage = 0;

IntCodeMachine machine2 = new IntCodeMachine(
    name: "Star 2",
    regs: regs,
    input: Input2,
    output: RenderOutput2)
{
    [0] = 2
};

await machine2.RunToEnd();

Console.WriteLine($"The answer is: {score}");

Console.WriteLine();
Console.ReadKey();


void RenderOutput1(long value)
{
    switch (inputStage)
    {
        case 0:
            cachedX = (int)value;
            inputStage = 1;
            break;

        case 1:
            if (cachedX == -1 && value == 0)
            {
                inputStage = 3;
            }
            else
            {
                cachedPoint = new Point2D(cachedX, (int)value);
                inputStage = 2;
            }
            break;

        case 2:
            tileMap[cachedPoint] = (int)value;
            inputStage = 0;
            break;

        case 3:
            //Score
            score = (int)value;
            inputStage = 0;
            break;


        default:
            throw new Exception();
    }
}

void RenderOutput2(long value)
{
    switch (inputStage)
    {
        case 0:
            cachedX = (int)value;
            inputStage = 1;
            break;

        case 1:
            if (cachedX == -1 && value == 0)
            {
                inputStage = 3;
            }
            else
            {
                cachedPoint = new Point2D(cachedX, (int)value);
                inputStage = 2;
            }
            break;

        case 2:
            if (value == 3)
            {
                paddlePos = cachedPoint;
            }
            else if (value == 4)
            {
                ballPos = cachedPoint;
            }

            tileMap[cachedPoint] = (int)value;
            inputStage = 0;
            break;

        case 3:
            //Score
            score = (int)value;
            inputStage = 0;
            break;

        default:
            throw new Exception();
    }
}

long Input2()
{
    if (ballPos.x == paddlePos.x)
    {
        return 0;
    }

    return ballPos.x < paddlePos.x ? -1 : 1;
}
