using System.Threading.Tasks;
using System.Threading.Channels;

namespace AoCTools.IntCode;

public class IntCodeMachine
{
    private long lastOutput = 0;
    public string Name { get; }

    private int instr;

    private bool done = false;

    private readonly Dictionary<long, long> regs = new Dictionary<long, long>();

    private long fixedInputIndex = 0;
    private readonly long[] fixedInputs;

    private readonly Func<long> input;
    private readonly Action<long> output;
    private readonly Channel<long> inputChannel = Channel.CreateUnbounded<long>();

    private long relativeBase = 0;

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

    public IntCodeMachine(
        string name,
        IEnumerable<long> regs,
        IEnumerable<long> fixedInputs = null,
        Func<long> input = null,
        Action<long> output = null)
    {
        instr = 0;
        Name = name;

        long index = 0;
        foreach (long value in regs)
        {
            this.regs[index++] = value;
        }

        this.fixedInputs = fixedInputs?.ToArray() ?? Array.Empty<long>();
        this.input = input;
        this.output = output;
    }

    public void WriteValue(long value)
    {
        inputChannel.Writer.TryWrite(value);
    }

    public async Task RunToEnd()
    {
        while (await ExecuteStep() != State.Terminate) { }
    }

    public async Task<long> RunToOutput()
    {
        while (true)
        {
            switch (await ExecuteStep())
            {
                case State.Output: return lastOutput;
                case State.Terminate: throw new Exception("Terminated without output.");
                case State.Continue: continue;
            }
        }
    }

    public async IAsyncEnumerable<long> GetOutputAsync()
    {
        while (true)
        {
            switch (await ExecuteStep())
            {
                case State.Output:
                    yield return lastOutput;
                    break;

                case State.Terminate: yield break;
                case State.Continue: continue;
            }
        }
    }

    public async Task<State> ExecuteStep()
    {
        if (done)
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
                long inputValue;
                if (fixedInputIndex < fixedInputs.Length)
                {
                    inputValue = fixedInputs[fixedInputIndex++];
                }
                else if (input != null)
                {
                    inputValue = input.Invoke();
                }
                else
                {
                    inputValue = await inputChannel.Reader.ReadAsync();
                }

                SetValue(regs[instr + 1], oneMode, inputValue);
                instr += 2;
                return State.Continue;

            case Instr.Output:
                lastOutput = GetValue(instr + 1, oneMode);
                instr += 2;
                output?.Invoke(lastOutput);
                return State.Output;

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
                done = true;
                inputChannel.Writer.TryComplete();
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
