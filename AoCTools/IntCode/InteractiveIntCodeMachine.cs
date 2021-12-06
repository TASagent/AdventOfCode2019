using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace AoCTools.IntCode;

public class InteractiveIntCodeMachine
{
    public string Name { get; }

    private int instr;

    private readonly Dictionary<long, long> regs = new Dictionary<long, long>();

    private readonly Queue<long> inputQueue = new Queue<long>();
    private readonly StringBuilder outputSinceLastPrompt = new StringBuilder();

    private long relativeBase = 0;
    public bool TrackOutput { get; set; } = false;

    public string RecentOutput => outputSinceLastPrompt.ToString();

    public bool IsDone { get; private set; } = false;

    public enum State
    {
        Continue = 0,
        AwaitingInput,
        Terminate
    }

    public enum Instr
    {
        Add = 1,
        Multiply = 2,
        Input = 3,
        Output = 4,
        JIT = 5,
        JIF = 6,
        LT = 7,
        EQ = 8,
        ADJ = 9,
        Terminate = 99
    }

    public enum Mode
    {
        Position = 0,
        Value,
        Relative
    }

    public long this[long i]
    {
        get => regs[i];
        set => regs[i] = value;
    }

    public InteractiveIntCodeMachine(
        string name,
        IEnumerable<long> regs,
        string initialInputText)
    {
        instr = 0;
        Name = name;

        long index = 0;
        foreach (long value in regs)
        {
            this.regs[index++] = value;
        }

        if (!string.IsNullOrWhiteSpace(initialInputText))
        {
            foreach (char c in initialInputText)
            {
                if (c == '\r')
                {
                    continue;
                }
                inputQueue.Enqueue(c);
            }

            if (!initialInputText.EndsWith('\n'))
            {
                inputQueue.Enqueue('\n');
            }
        }
    }

    public void SubmitText(string value)
    {
        outputSinceLastPrompt.Clear();
        foreach (char c in value)
        {
            if (c == '\r')
            {
                continue;
            }
            inputQueue.Enqueue(c);
        }

        if (!value.EndsWith('\n'))
        {
            inputQueue.Enqueue('\n');
        }

        RunToRequiredInput();
    }

    public void RunToRequiredInput()
    {
        while (true)
        {
            switch (ExecuteStep())
            {
                case State.AwaitingInput:
                case State.Terminate:
                    return;

                case State.Continue:
                    continue;
            }
        }
    }

    public State ExecuteStep()
    {
        if (IsDone)
        {
            return State.Terminate;
        }

        Instr instruction = (Instr)(regs[instr] % 100);
        Mode oneMode = (Mode)((regs[instr] / 100) % 10);
        Mode twoMode = (Mode)((regs[instr] / 1000) % 10);
        Mode threeMode = (Mode)((regs[instr] / 10000) % 10);

        switch (instruction)
        {
            case Instr.Add:
                SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) + GetValue(instr + 2, twoMode));
                instr += 4;
                return State.Continue;

            case Instr.Multiply:
                SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) * GetValue(instr + 2, twoMode));
                instr += 4;
                return State.Continue;

            case Instr.Input:
                if (inputQueue.Count == 0)
                {
                    return State.AwaitingInput;
                }

                SetValue(regs[instr + 1], oneMode, inputQueue.Dequeue());
                instr += 2;
                return State.Continue;

            case Instr.Output:
                char output = (char)GetValue(instr + 1, oneMode);
                if (TrackOutput)
                {
                    outputSinceLastPrompt.Append(output);
                }
                Console.Write(output);
                instr += 2;
                return State.Continue;

            case Instr.JIT:
                if (GetValue(instr + 1, oneMode) != 0)
                {
                    instr = (int)GetValue(instr + 2, twoMode);
                }
                else
                {
                    instr += 3;
                }
                return State.Continue;

            case Instr.JIF:
                if (GetValue(instr + 1, oneMode) == 0)
                {
                    instr = (int)GetValue(instr + 2, twoMode);
                }
                else
                {
                    instr += 3;
                }
                return State.Continue;

            case Instr.LT:
                SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) < GetValue(instr + 2, twoMode) ? 1 : 0);
                instr += 4;
                return State.Continue;

            case Instr.EQ:
                SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) == GetValue(instr + 2, twoMode) ? 1 : 0);
                instr += 4;
                return State.Continue;

            case Instr.ADJ:
                relativeBase += GetValue(instr + 1, oneMode);
                instr += 2;
                return State.Continue;

            case Instr.Terminate:
                IsDone = true;
                return State.Terminate;

            default: throw new Exception($"Unsupported instruction: {instruction}");
        }
    }

    private long GetValue(long reg, Mode mode)
    {
        switch (mode)
        {
            case Mode.Position:
                return GetIndex(GetIndex(reg));

            case Mode.Value:
                return GetIndex(reg);

            case Mode.Relative:
                return GetIndex(GetIndex(reg) + relativeBase);

            default:
                throw new Exception();
        }
    }

    private long GetIndex(long index) => regs.GetValueOrDefault(index, 0L);


    private void SetValue(long reg, Mode setMode, long value)
    {
        switch (setMode)
        {
            case Mode.Position:
                regs[reg] = value;
                break;

            case Mode.Relative:
                regs[relativeBase + reg] = value;
                break;

            case Mode.Value:
            default:
                throw new Exception();
        }
    }
}
