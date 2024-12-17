namespace AdventOfCode._2024.ThreeBitComputer;

public class ThreeBitComputer
{
	private readonly Dictionary<
		int,
		Func<
			List<int>,
			Dictionary<int, int>,
			int,
			Task<int>>> _operations;

	private List<int> _memory;
	private readonly Func<ThreeBitComputer, Task<int>> _inputAction;
	private readonly Func<int, Task> _outputAction;
	private int _instructionPointer;
	private readonly List<int> _outputBuffer;

	private int _regA;
	private int _regB;
	private int _regC;

	public ThreeBitComputer(
		List<int> memory,
		int regA,
		int regB,
		int regC,
		Func<ThreeBitComputer, Task<int>>? inputAction = null,
		Func<int, Task>? outputAction = null)
	{
		_regA = regA;
		_regB = regB;
		_regC = regC;
		_memory = memory.ToList();
		_inputAction = inputAction ?? DefaultInputAction;
		_outputAction = outputAction ?? DefaultOutputAction;
		_outputBuffer = [];
		_instructionPointer = 0;
		_operations = new Dictionary<int, Func<List<int>, Dictionary<int, int>, int, Task<int>>>
		{
			{ 0, Adv },
			{ 1, Bxl },
			{ 2, Bst },
			{ 3, Jnz },
			{ 4, Bxc },
			{ 5, Out },
			{ 6, Bdv },
			{ 7, Cdv },
		};
	}

	private Task<int> DefaultInputAction(ThreeBitComputer intcomputer)
	{
		Console.WriteLine("Input:");
		return Task.FromResult(int.Parse(Console.ReadLine() ?? throw new InvalidOperationException()));
	}

	private Task DefaultOutputAction(int output)
	{
		return Task.CompletedTask;
	}

	public int GetAtAddress(int index) => _memory[index];

	public List<int> GetOutputBuffer() => _outputBuffer.ToList();
	public string GetOutputBufferAsString() => string.Join("", _outputBuffer);

	public List<int> DumpMemory() => _memory.ToList();

	public async Task RunAsync()
	{
		var preOpCode = _memory[_instructionPointer];
		int opcode;
		Dictionary<int, int> paramModes;
		(paramModes, opcode) = GetInstructions(preOpCode);

		while (_instructionPointer < _memory.Count)
		{
			_instructionPointer += await _operations[opcode](_memory, paramModes, _instructionPointer);
			if (_instructionPointer >= _memory.Count)
			{
				break;
			}
			(paramModes, opcode) = GetInstructions(_memory[_instructionPointer]);
		}
	}

	private (Dictionary<int, int> paramModes, int opcode) GetInstructions(int preOpCode)
	{
		Dictionary<int, int> paramModes;
		int opcode;
		if (preOpCode > 99)
		{
			var preOpCodeStr = preOpCode.ToString().Reverse().ToList();
			paramModes = preOpCodeStr
				.Skip(2)
				.Select(c => int.Parse(c.ToString()))
				.Index()
				.ToDictionary(c => c.Index, c => c.Item);
			opcode = int.Parse(preOpCodeStr[1] + preOpCodeStr[0].ToString());
		}
		else
		{
			opcode = preOpCode;
			paramModes = new Dictionary<int, int>();
		}

		return (paramModes, opcode);
	}

	private int GetComboOperand(int inputOperand)
	{
		return inputOperand switch
		{
			1 => 1,
			2 => 2,
			3 => 3,
			4 => _regA,
			5 => _regB,
			6 => _regC,
			_ => throw new InvalidOperationException()
		};
	}

	private Task<int> Adv(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var operand = mem[instructionPointer + 1];
		var numerator = _regA;
		var denominator = 1 << GetComboOperand(operand);
		_regA = (int)((double)numerator / (double)denominator);
		return Task.FromResult(2);
	}

	private Task<int> Bxl(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var operand = mem[instructionPointer + 1];
		_regB = _regB ^ operand;
		return Task.FromResult(2);
	}

	private Task<int> Bst(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var operand = mem[instructionPointer + 1];
		var comboOperand = GetComboOperand(operand);
		_regB = comboOperand % 8;
		return Task.FromResult(2);
	}

	private Task<int> Jnz(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		if (_regA == 0)
		{
			return Task.FromResult(2);
		}

		var operand = mem[instructionPointer + 1];
		var step = -instructionPointer + operand;
		return Task.FromResult(step);
	}

	private Task<int> Bxc(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		_regB = _regB ^ _regC;
		return Task.FromResult(2);
	}

	private async Task<int> Out(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var operand = mem[instructionPointer + 1];
		var comboOperand = GetComboOperand(operand);
		var outValue = comboOperand % 8;
    		_outputBuffer.Add(outValue);
    		await _outputAction(outValue);
    		return 2;
	}

	private Task<int> Bdv(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var operand = mem[instructionPointer + 1];
		var numerator = _regA;
		var denominator = 1 << GetComboOperand(operand);
		_regB = (int)((double)numerator / (double)denominator);
		return Task.FromResult(2);
	}

	private Task<int> Cdv(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var operand = mem[instructionPointer + 1];
		var numerator = _regA;
		var denominator = 1 << GetComboOperand(operand);
		_regC = (int)((double)numerator / (double)denominator);
		return Task.FromResult(2);
	}

	private Task<int> Add(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		var b = paramModes.TryGetValue(1, out var bMode) && bMode == 1
			? mem[instructionPointer + 2]
			: mem[mem[instructionPointer + 2]];

		mem[mem[instructionPointer + 3]] = a + b;
		return Task.FromResult(4);
	}

	private Task<int> Mul(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		var b = paramModes.TryGetValue(1, out var bMode) && bMode == 1
			? mem[instructionPointer + 2]
			: mem[mem[instructionPointer + 2]];

		mem[mem[instructionPointer + 3]] = a * b;
		return Task.FromResult(4);
	}

	private async Task<int> Input(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var input = await _inputAction(this);
		mem[mem[instructionPointer + 1]] = input;
		return 2;
	}

	private async Task<int> Output(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		_outputBuffer.Add(a);
		await _outputAction(a);
		return 2;
	}

	private Task<int> JmpIfTrue(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		var b = paramModes.TryGetValue(1, out var bMode) && bMode == 1
			? mem[instructionPointer + 2]
			: mem[mem[instructionPointer + 2]];

		var step = a != 0
			? -instructionPointer + b
			: 3;
		return Task.FromResult(step);
	}

	private Task<int> JmpIfFalse(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		var b = paramModes.TryGetValue(1, out var bMode) && bMode == 1
			? mem[instructionPointer + 2]
			: mem[mem[instructionPointer + 2]];

		var step = a == 0
			? -instructionPointer + b
			: 3;
		return Task.FromResult(step);
	}

	private Task<int> LessThan(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		var b = paramModes.TryGetValue(1, out var bMode) && bMode == 1
			? mem[instructionPointer + 2]
			: mem[mem[instructionPointer + 2]];

		mem[mem[instructionPointer + 3]] = a < b ? 1 : 0;

		return Task.FromResult(4);
	}

	private Task<int> IsEq(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		var b = paramModes.TryGetValue(1, out var bMode) && bMode == 1
			? mem[instructionPointer + 2]
			: mem[mem[instructionPointer + 2]];

		mem[mem[instructionPointer + 3]] = a == b ? 1 : 0;

		return Task.FromResult(4);
	}
}