﻿const string inputFile = @"../../../../input06.txt";

Dictionary<string, Planet> planets = new Dictionary<string, Planet>();

Console.WriteLine("Day 06 - Universal Orbit Map");
Console.WriteLine("Star 1");
Console.WriteLine();

string[] lines = File.ReadAllLines(inputFile);

foreach (string line in lines)
{
    Planet parent = GetPlanet(line.Substring(0, 3));
    Planet child = GetPlanet(line.Substring(4, 3));

    parent.AddChild(child);
}

int output1 = planets.Values.Select(x => x.Tier).Sum();

Console.WriteLine($"The cumulative depth is: {output1}");

Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();


Planet start = planets["YOU"].parent;
Planet end = planets["SAN"].parent;

int bfsDistance = DistanceBFS(start, end);
int treeSearchDistance = SimpleTreeDistance(start, end);

if (bfsDistance != treeSearchDistance)
{
    throw new Exception("Oh no!  The distances disagreed.  Some sort of problem exists.");
}

Console.WriteLine($"The distance from you to santa is: {bfsDistance}");

Console.WriteLine();
Console.ReadKey();



// Breadth-first search
static int DistanceBFS(Planet start, Planet end)
{
    Dictionary<Planet, int> distanceMap = new Dictionary<Planet, int>() { { start, 0 } };

    Queue<Planet> pendingEvals = new Queue<Planet>();

    pendingEvals.Enqueue(start);

    while (pendingEvals.Count > 0)
    {
        Planet current = pendingEvals.Dequeue();

        if (current == end)
        {
            return distanceMap[current];
        }

        int newDistance = distanceMap[current] + 1;

        foreach (Planet next in current.Connections())
        {
            if (!distanceMap.ContainsKey(next) || distanceMap[next] > newDistance)
            {
                distanceMap[next] = newDistance;
                pendingEvals.Enqueue(next);
            }
        }
    }

    throw new Exception("Path not found");
}

//Faster search for simple tree
static int SimpleTreeDistance(Planet start, Planet end)
{
    Dictionary<Planet, int> startParentDistances = new Dictionary<Planet, int>();

    int distance = 0;
    Planet current = start;

    while (current != null)
    {
        startParentDistances.Add(current, distance);
        current = current.parent;
        distance++;
    }

    distance = 0;
    current = end;

    while (current != null)
    {
        if (startParentDistances.ContainsKey(current))
        {
            return distance + startParentDistances[current];
        }

        distance++;
        current = current.parent;
    }

    throw new Exception("Path not found");
}

Planet GetPlanet(string name)
{
    if (!planets.ContainsKey(name))
    {
        planets.Add(name, new Planet(name));
    }

    return planets[name];
}


class Planet
{
    public string Name { get; }

    public Planet parent = null;
    public List<Planet> children = new List<Planet>();

    public int Tier
    {
        get
        {
            if (parent == null)
            {
                return 0;
            }

            return parent.Tier + 1;
        }
    }

    public Planet(string name)
    {
        Name = name;
    }

    public void AddChild(Planet child)
    {
        children.Add(child);

        if (child.parent != null)
        {
            throw new Exception("Already coupled");
        }

        child.parent = this;
    }

    public IEnumerable<Planet> Connections()
    {
        foreach (Planet child in children)
        {
            yield return child;
        }

        if (parent != null)
        {
            yield return parent;
        }
    }
}