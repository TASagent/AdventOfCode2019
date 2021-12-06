using AoCTools;
using AoCTools.IntCode;

const string inputFile = @"../../../../input15.txt";

Dictionary<Point2D, bool> grid = new Dictionary<Point2D, bool>();

Point2D currentPosition = new Point2D(0, 0);
Point2D targetPosition = new Point2D(0, 0);

int lastOutput = 0;

IEnumerator<int> movement = ExploreMap().GetEnumerator();

Console.WriteLine("Day 15 - Oxygen System");
Console.WriteLine("Star 1");
Console.WriteLine();

long[] regs = File.ReadAllText(inputFile).Split(',').Select(long.Parse).ToArray();

grid[(0, 0)] = true;

IntCodeMachine machine = new IntCodeMachine(
    name: "Star 1",
    regs: regs,
    input: GetInput,
    output: x => lastOutput = (int)x);

await machine.RunToEnd();

currentPosition = (0, 0);

Console.WriteLine($"The answer is: {TravelTo(targetPosition).Count()}");


Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();

Console.WriteLine($"The answer is: {FillGridFrom(targetPosition)}");


Console.WriteLine();
Console.ReadKey();



long GetInput()
{
    if (movement.MoveNext())
    {
        return movement.Current;
    }

    return 99;
}

IEnumerable<int> ExploreMap()
{
    Queue<Point2D> pointsToExplore = new Queue<Point2D>();

    pointsToExplore.Enqueue(currentPosition);

    while (pointsToExplore.Count > 0)
    {
        Point2D nextPoint = pointsToExplore.Dequeue();

        IEnumerable<int> travel = TravelTo(nextPoint);

        foreach (int dir in travel)
        {
            yield return dir;
        }

        //Now at target position

        int direction = 0;
        foreach (Point2D next in GetAdjacents(currentPosition))
        {
            direction++;

            if (!grid.ContainsKey(next))
            {
                yield return direction;

                if (lastOutput == 0)
                {
                    grid.Add(next, false);
                    continue;
                }

                grid.Add(next, true);

                if (lastOutput == 2)
                {
                    targetPosition = next;
                }

                if (!pointsToExplore.Contains(next))
                {
                    pointsToExplore.Enqueue(next);
                }

                yield return InvertDirection(direction);
            }
        }
    }

}

IEnumerable<int> TravelTo(Point2D destination)
{
    if (currentPosition == destination)
    {
        yield break;
    }

    Dictionary<Point2D, int> travelMap = new Dictionary<Point2D, int>();
    Queue<Point2D> travelPoints = new Queue<Point2D>();
    travelPoints.Enqueue(destination);
    travelMap.Add(destination, 0);

    while (travelPoints.Count > 0)
    {
        Point2D current = travelPoints.Dequeue();

        if (current == currentPosition)
        {
            break;
        }

        int distance = travelMap[current] + 1;

        foreach (Point2D next in GetAdjacents(current))
        {
            if (!grid.GetValueOrDefault(next))
            {
                continue;
            }

            if (travelMap.GetValueOrDefault(next, int.MaxValue) > distance)
            {
                travelMap[next] = distance;
                if (!travelPoints.Contains(next))
                {
                    travelPoints.Enqueue(next);
                }
            }
        }
    }

    while (currentPosition != destination)
    {
        int distance = travelMap[currentPosition];

        int direction = 0;

        foreach (Point2D next in GetAdjacents(currentPosition))
        {
            direction++;
            if (travelMap.GetValueOrDefault(next, int.MaxValue) == distance - 1)
            {
                currentPosition = next;
                yield return direction;
                break;
            }
        }
    }
}

int FillGridFrom(Point2D startPoint)
{
    Dictionary<Point2D, int> distanceMap = new Dictionary<Point2D, int>();
    Queue<Point2D> distancePoints = new Queue<Point2D>();
    distancePoints.Enqueue(startPoint);
    distanceMap.Add(startPoint, 0);

    while (distancePoints.Count > 0)
    {
        Point2D current = distancePoints.Dequeue();

        int distance = distanceMap[current] + 1;

        foreach (Point2D next in GetAdjacents(current))
        {
            if (!grid.GetValueOrDefault(next))
            {
                continue;
            }

            if (distanceMap.GetValueOrDefault(next, int.MaxValue) > distance)
            {
                distanceMap[next] = distance;
                if (!distancePoints.Contains(next))
                {
                    distancePoints.Enqueue(next);
                }
            }
        }
    }

    return distanceMap.Values.Max();
}

static IEnumerable<Point2D> GetAdjacents(Point2D center)
{
    yield return center + Point2D.XAxis;
    yield return center - Point2D.XAxis;
    yield return center - Point2D.YAxis;
    yield return center + Point2D.YAxis;
}

static int InvertDirection(int direction)
{
    switch (direction)
    {
        case 1: return 2;
        case 2: return 1;
        case 3: return 4;
        case 4: return 3;
        default: throw new Exception();
    }
}
