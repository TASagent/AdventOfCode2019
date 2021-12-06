using AoCTools;

const string inputFile = @"../../../../input20.txt";


Square[,] grid;
Dictionary<Point2D, int> movementGrid = new Dictionary<Point2D, int>();
Dictionary<Point2D, Point2D> portalDict = new Dictionary<Point2D, Point2D>();
Dictionary<string, Point2D> pendingPortals = new Dictionary<string, Point2D>();

Point2D startPoint = Point2D.Zero;
Point2D endPoint = Point2D.Zero;
Dictionary<Point3D, int> movementGrid3D = new Dictionary<Point3D, int>();


Console.WriteLine("Day 20 - Donut Maze");
Console.WriteLine("Star 1");
Console.WriteLine();

string[] lines = File.ReadAllLines(inputFile);

grid = new Square[lines[0].Length - 4, lines.Length - 4];

//Maze Core
for (int y = 2; y < lines.Length - 2; y++)
{
    for (int x = 2; x < lines[0].Length - 2; x++)
    {
        grid[x - 2, y - 2] = lines[y][x] == '.' ? Square.Open : Square.Wall;
    }
}

int right = lines[0].Length - 2;

for (int y = 3; y < lines.Length - 3; y++)
{
    //Left 
    if (char.IsLetter(lines[y][0]))
    {
        HandlePortal($"{lines[y][0]}{lines[y][1]}", (0, y - 2), true);
    }

    //Right
    if (char.IsLetter(lines[y][right]))
    {
        HandlePortal($"{lines[y][right]}{lines[y][right + 1]}", (lines[0].Length - 5, y - 2), true);
    }

    //32 to 86
    if (y >= 32 && y < 86)
    {
        if (char.IsLetter(lines[y][29]))
        {
            HandlePortal($"{lines[y][29]}{lines[y][30]}", (26, y - 2), false);
        }

        if (char.IsLetter(lines[y][84]))
        {
            HandlePortal($"{lines[y][84]}{lines[y][85]}", (84, y - 2), false);
        }

    }
}

int bottom = lines.Length - 2;

for (int x = 3; x < lines.Length - 3; x++)
{
    //Top 
    if (char.IsLetter(lines[0][x]))
    {
        HandlePortal($"{lines[0][x]}{lines[1][x]}", (x - 2, 0), true);
    }

    //Bottom
    if (char.IsLetter(lines[bottom][x]))
    {
        HandlePortal($"{lines[bottom][x]}{lines[bottom + 1][x]}", (x - 2, bottom - 3), true);
    }

    //32 to 84
    if (x >= 32 && x < 84)
    {
        if (char.IsLetter(lines[29][x]))
        {
            HandlePortal($"{lines[29][x]}{lines[30][x]}", (x - 2, 26), false);
        }

        if (char.IsLetter(lines[86][x]))
        {
            HandlePortal($"{lines[86][x]}{lines[87][x]}", (x - 2, 86), false);
        }

    }
}

if (pendingPortals.Count != 0)
{
    throw new Exception();
}

int output1 = GetDistance(startPoint, endPoint);



Console.WriteLine($"The answer is: {output1}");

Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();


int output2 = GetDeepDistance(startPoint.To3D(), endPoint.To3D());

Console.WriteLine($"The answer is: {output2}");


Console.WriteLine();
Console.ReadKey();


int GetDistance(in Point2D startPoint, in Point2D endPoint)
{
    movementGrid.Clear();

    Queue<Point2D> pendingPoints = new Queue<Point2D>();
    pendingPoints.Enqueue(startPoint);
    movementGrid.Add(startPoint, 0);

    while (pendingPoints.Count > 0)
    {
        Point2D next = pendingPoints.Dequeue();

        if (next == endPoint)
        {
            break;
        }

        int distance = movementGrid[next] + 1;

        foreach (Point2D adj in next.GetAdjacent())
        {
            if (adj.x < 0 || adj.y < 0 || adj.x >= grid.GetLength(0) || adj.y >= grid.GetLength(1))
            {
                continue;
            }

            Square value = grid[adj.x, adj.y];
            if (value == Square.Open)
            {
                //Can enter
                if (movementGrid.GetValueOrDefault(adj, int.MaxValue) > distance)
                {
                    //Update
                    movementGrid[adj] = distance;
                    if (!pendingPoints.Contains(adj))
                    {
                        pendingPoints.Enqueue(adj);
                    }
                }
            }
            else if (value == Square.PortalOuter || value == Square.PortalInner)
            {
                Point2D exit = portalDict[adj];
                //Can enter
                if (movementGrid.GetValueOrDefault(exit, int.MaxValue) > distance + 1)
                {
                    //Update
                    movementGrid[exit] = distance + 1;
                    if (!pendingPoints.Contains(exit))
                    {
                        pendingPoints.Enqueue(exit);
                    }
                }
            }
        }
    }

    return movementGrid[endPoint];
}

int GetDeepDistance(in Point3D startPoint, in Point3D endPoint)
{
    movementGrid3D.Clear();

    Queue<Point3D> pendingPoints = new Queue<Point3D>();
    pendingPoints.Enqueue(startPoint);
    movementGrid3D.Add(startPoint, 0);

    while (pendingPoints.Count > 0)
    {
        Point3D next = pendingPoints.Dequeue();

        if (next == endPoint)
        {
            break;
        }

        int distance = movementGrid3D[next] + 1;

        foreach (Point3D adj in next.Get2DAdjacent())
        {
            if (adj.x < 0 || adj.y < 0 || adj.x >= grid.GetLength(0) || adj.y >= grid.GetLength(1))
            {
                continue;
            }

            Square value = grid[adj.x, adj.y];
            if (value == Square.Open)
            {
                //Can enter
                if (movementGrid3D.GetValueOrDefault(adj, int.MaxValue) > distance)
                {
                    //Update
                    movementGrid3D[adj] = distance;
                    if (!pendingPoints.Contains(adj))
                    {
                        pendingPoints.Enqueue(adj);
                    }
                }
            }
            else if (value == Square.PortalOuter && adj.z > 0)
            {
                Point2D exit = portalDict[adj.To2D()];
                Point3D trueExit = new Point3D(exit.x, exit.y, adj.z - 1);
                //Can enter
                if (movementGrid3D.GetValueOrDefault(trueExit, int.MaxValue) > distance + 1)
                {
                    //Update
                    movementGrid3D[trueExit] = distance + 1;
                    if (!pendingPoints.Contains(trueExit))
                    {
                        pendingPoints.Enqueue(trueExit);
                    }
                }
            }
            else if (value == Square.PortalInner)
            {
                Point2D exit = portalDict[adj.To2D()];
                Point3D trueExit = new Point3D(exit.x, exit.y, adj.z + 1);
                //Can enter
                if (movementGrid3D.GetValueOrDefault(trueExit, int.MaxValue) > distance + 1)
                {
                    //Update
                    movementGrid3D[trueExit] = distance + 1;
                    if (!pendingPoints.Contains(trueExit))
                    {
                        pendingPoints.Enqueue(trueExit);
                    }
                }
            }
        }
    }

    return movementGrid3D[endPoint];
}

void HandlePortal(string portalLabel, in Point2D location, bool outer)
{
    if (portalLabel == "AA")
    {
        startPoint = location;
    }
    else if (portalLabel == "ZZ")
    {
        endPoint = location;
    }
    else if (pendingPortals.ContainsKey(portalLabel))
    {
        Point2D destination = pendingPortals[portalLabel];
        pendingPortals.Remove(portalLabel);

        portalDict[location] = destination;
        portalDict[destination] = location;

        grid[location.x, location.y] = outer ? Square.PortalOuter : Square.PortalInner;
        grid[destination.x, destination.y] = outer ? Square.PortalInner : Square.PortalOuter;
    }
    else
    {
        pendingPortals[portalLabel] = location;
    }
}

enum Square
{
    Open = 0,
    Wall,
    PortalOuter,
    PortalInner
}
