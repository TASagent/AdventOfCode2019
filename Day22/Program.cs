using System.Numerics;
using AoCTools;

const string inputFile = @"../../../../input22.txt";


Console.WriteLine("Day 22 - Slam Shuffle");
Console.WriteLine("Star 1");
Console.WriteLine();

List<DeckAction> deckActions = File.ReadAllLines(inputFile).Select(DeckAction.Parse).ToList();

{
    const long cardCountStar1 = 10007;

    long currentPosition = 2019;

    foreach (DeckAction deckAction in deckActions)
    {
        currentPosition = deckAction.TransformPosition(currentPosition, cardCountStar1);
    }

    Console.WriteLine($"The answer is: {currentPosition}");
}

Console.WriteLine();
Console.WriteLine("Star 2");
Console.WriteLine();

{
    BigInteger cardCountStar2 = 119315717514047;
    BigInteger repetitions = 101741582076661;
    BigInteger position = 2020;
    BigInteger offsetDifference = 0;
    BigInteger incrementMultiplier = 1;


    foreach (DeckAction deckAction in deckActions)
    {
        (incrementMultiplier, offsetDifference) = deckAction.TransformAction(incrementMultiplier, offsetDifference, cardCountStar2);
    }

    BigInteger increment = BigInteger.ModPow(incrementMultiplier, repetitions, cardCountStar2);
    BigInteger offset = offsetDifference * (1 - increment) * BigInteger.ModPow((1 - incrementMultiplier) % cardCountStar2, cardCountStar2 - 2, cardCountStar2);

    BigInteger card = (offset + 2020 * increment) % cardCountStar2;

    Console.WriteLine($"The answer is: {card}");
}


Console.WriteLine();
Console.ReadKey();

abstract class DeckAction
{
    public static DeckAction Parse(string line)
    {
        if (line.StartsWith("cut"))
        {
            return new CutAction(line);
        }
        else if (line.StartsWith("deal with"))
        {
            return new DealAction(line);
        }
        else if (line.StartsWith("deal into"))
        {
            return new StackAction(line);
        }
        else throw new Exception();
    }

    public abstract long TransformPosition(long currentPosition, long totalCards);

    public abstract (BigInteger incrementMultiplier, BigInteger offsetDifference) TransformAction(
        in BigInteger incrementMultiplier,
        in BigInteger offsetDifference,
        in BigInteger deckSize);
}

class DealAction : DeckAction
{
    private readonly long increment;
    public DealAction(string input)
    {
        increment = long.Parse(input[20..]);
    }

    public override long TransformPosition(long currentPosition, long totalCards) =>
        (increment * currentPosition) % totalCards;

    public override (BigInteger incrementMultiplier, BigInteger offsetDifference) TransformAction(
        in BigInteger incrementMultiplier,
        in BigInteger offsetDifference,
        in BigInteger deckSize)
    {
        return (AoCMath.PositiveMod(incrementMultiplier * BigInteger.ModPow(increment, deckSize - 2, deckSize), deckSize),
            offsetDifference);
    }
}

class CutAction : DeckAction
{
    private readonly long cut;
    public CutAction(string input)
    {
        cut = long.Parse(input[4..]);
    }

    public override long TransformPosition(long currentPosition, long totalCards) =>
        (currentPosition - cut + totalCards) % totalCards;

    public override (BigInteger incrementMultiplier, BigInteger offsetDifference) TransformAction(
        in BigInteger incrementMultiplier,
        in BigInteger offsetDifference,
        in BigInteger deckSize)
    {
        return (incrementMultiplier,
            AoCMath.PositiveMod(offsetDifference + cut * incrementMultiplier, deckSize));
    }
}

class StackAction : DeckAction
{
    public StackAction(string _)
    {
    }

    public override long TransformPosition(long currentPosition, long totalCards) =>
        totalCards - currentPosition - 1;

    public override (BigInteger incrementMultiplier, BigInteger offsetDifference) TransformAction(
        in BigInteger incrementMultiplier,
        in BigInteger offsetDifference,
        in BigInteger deckSize)
    {
        return (AoCMath.PositiveMod(-incrementMultiplier, deckSize),
            AoCMath.PositiveMod(offsetDifference - incrementMultiplier, deckSize));
    }
}
