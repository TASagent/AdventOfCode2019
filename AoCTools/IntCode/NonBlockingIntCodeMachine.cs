namespace AoCTools.IntCode;

public class NonBlockingIntCodeMachine
{
    public long lastOutput = 0;
    public string Name { get; }
    public long Address { get; }

    private int instr;

    private bool done = false;

    private readonly Dictionary<long, long> regs = new Dictionary<long, long>();

    private long fixedInputIndex = 0;
    private readonly long[] fixedInputs;

    private readonly Action<long, long, long> output;
    private readonly List<long> bufferedOutputs = new List<long>(3);
    private readonly Queue<long> queuedInputs = new Queue<long>();

    private long relativeBase = 0;

    public bool IsIdle { get; private set; } = false;

    public enum State
    {
        Continue = 0,
        Output,
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

    public NonBlockingIntCodeMachine(
        string name,
        IEnumerable<long> regs,
        long networkAddress,
        Action<long, long, long> output)
    {
        instr = 0;
        Name = name;
        Address = networkAddress;

        long index = 0;
        foreach (long value in regs)
        {
            this.regs[index++] = value;
        }

        fixedInputs = new[] { networkAddress };
        this.output = output;
    }

    public void WriteValues(long x, long y)
    {
        IsIdle = false;
        queuedInputs.Enqueue(x);
        queuedInputs.Enqueue(y);
    }

    public bool ExecuteStep()
    {
        if (done)
        {
            return true;
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
                return IsIdle;

            case Instr.Multiply:
                SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) * GetValue(instr + 2, twoMode));
                instr += 4;
                return IsIdle;

            case Instr.Input:
                long inputValue;
                if (fixedInputIndex < fixedInputs.Length)
                {
                    inputValue = fixedInputs[fixedInputIndex++];
                }
                else if (queuedInputs.Count > 0)
                {
                    inputValue = queuedInputs.Dequeue();
                }
                else
                {
                    IsIdle = true;
                    inputValue = -1;
                }

                SetValue(regs[instr + 1], oneMode, inputValue);
                instr += 2;
                return IsIdle;

            case Instr.Output:
                lastOutput = GetValue(instr + 1, oneMode);
                instr += 2;
                bufferedOutputs.Add(lastOutput);
                if (bufferedOutputs.Count == 3)
                {
                    output.Invoke(bufferedOutputs[0], bufferedOutputs[1], bufferedOutputs[2]);
                    bufferedOutputs.Clear();
                }
                return IsIdle;

            case Instr.JIT:
                if (GetValue(instr + 1, oneMode) != 0)
                {
                    instr = (int)GetValue(instr + 2, twoMode);
                }
                else
                {
                    instr += 3;
                }
                return IsIdle;

            case Instr.JIF:
                if (GetValue(instr + 1, oneMode) == 0)
                {
                    instr = (int)GetValue(instr + 2, twoMode);
                }
                else
                {
                    instr += 3;
                }
                return IsIdle;

            case Instr.LT:
                SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) < GetValue(instr + 2, twoMode) ? 1 : 0);
                instr += 4;
                return IsIdle;

            case Instr.EQ:
                SetValue(regs[instr + 3], threeMode, GetValue(instr + 1, oneMode) == GetValue(instr + 2, twoMode) ? 1 : 0);
                instr += 4;
                return IsIdle;

            case Instr.ADJ:
                relativeBase += GetValue(instr + 1, oneMode);
                instr += 2;
                return IsIdle;

            case Instr.Terminate:
                IsIdle = true;
                done = true;
                return IsIdle;

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
