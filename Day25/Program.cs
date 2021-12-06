using System.Text.RegularExpressions;
using AoCTools.IntCode;

const string inputFile = @"../../../../input25.txt";
const string textInputFile = @"../../../../input25a.txt";


Console.WriteLine("Day 25 - Cryostasis");
Console.WriteLine("Star 1");
Console.WriteLine();

long[] regs = File.ReadAllText(inputFile).Split(',').Select(long.Parse).ToArray();

string initialInputs = File.ReadAllText(textInputFile).Trim().Replace("\r", "");

List<string> manualInputs = new List<string>();

InteractiveIntCodeMachine machine = new InteractiveIntCodeMachine(
    name: $"Day25 Machine",
    regs: regs,
    initialInputText: initialInputs);

machine.RunToRequiredInput();
machine.TrackOutput = true;

machine.SubmitText("inv");

string invOutput = machine.RecentOutput;

int correctItemMask = 0;

Regex itemFinder = new Regex(@"^- (.*?)$", RegexOptions.Multiline);
List<string> itemNames = new List<string>();

foreach (Match match in itemFinder.Matches(invOutput))
{
    string itemValue = match.Groups[1].Value;
    if (itemValue == "north" || itemValue == "east")
    {
        continue;
    }

    itemNames.Add(itemValue);
}

bool[] itemHeld = new bool[itemNames.Count];
for (int i = 0; i < itemNames.Count; i++)
{
    itemHeld[i] = true;
}

//Iterate through options
for (int itemMask = (1 << itemNames.Count) - 1; itemMask >= 0; itemMask--)
{
    //Hold the right items
    for (int item = 0; item < itemHeld.Length; item++)
    {
        if (itemHeld[item] != ShouldHoldItem(itemMask, item))
        {
            machine.TrackOutput = false;
            machine.SubmitText($"{(itemHeld[item] ? "drop" : "take")} {itemNames[item]}");
            itemHeld[item] = !itemHeld[item];
        }
    }

    machine.TrackOutput = true;
    machine.SubmitText("east");

    if (!machine.RecentOutput.Contains("you are ejected"))
    {
        correctItemMask = itemMask;
        break;
    }
}

while (!machine.IsDone)
{
    string inputText = Console.ReadLine();
    if (inputText == "quit")
    {
        break;
    }

    manualInputs.Add(inputText);
    machine.SubmitText(inputText);
}


Console.WriteLine();
Console.WriteLine();

Console.WriteLine("---------------------");

Console.WriteLine("Manual Input:");
Console.WriteLine();
foreach (string input in manualInputs)
{
    Console.WriteLine(input);
}

Console.WriteLine();
Console.WriteLine();
Console.ReadKey();

bool ShouldHoldItem(int itemMask, int index) => (itemMask & (1 << index)) > 0;