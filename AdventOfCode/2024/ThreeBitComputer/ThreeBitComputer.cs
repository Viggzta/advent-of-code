namespace AdventOfCode._2024.ThreeBitComputer;

public class ThreeBitComputer
{
	private readonly Dictionary<
		long,
		Func<
			List<long>,
			Dictionary<int, long>,
			int,
			Task<long>>> _operations;

	private List<long> _memory;
	private readonly Func<ThreeBitComputer, Task<long>> _inputAction;
	private readonly Func<long, Task> _outputAction;
	private int _instructionPointer;
	private readonly List<long> _outputBuffer;

	private long _regA;
	private long _regB;
	private long _regC;

	public ThreeBitComputer(
		List<long> memory,
		long regA,
		long regB,
		long regC,
		Func<ThreeBitComputer, Task<long>>? inputAction = null,
		Func<long, Task>? outputAction = null)
	{
		_regA = regA;
		_regB = regB;
		_regC = regC;
		_memory = memory.ToList();
		_inputAction = inputAction ?? DefaultInputAction;
		_outputAction = outputAction ?? DefaultOutputAction;
		_outputBuffer = [];
		_instructionPointer = 0;
		_operations = new Dictionary<
			long,
			Func<
				List<long>,
				Dictionary<int, long>,
				int,
				Task<long>>>
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

	private Task<long> DefaultInputAction(ThreeBitComputer longcomputer)
	{
		Console.WriteLine("Input:");
		return Task.FromResult(long.Parse(Console.ReadLine() ?? throw new InvalidOperationException()));
	}

	private Task DefaultOutputAction(long output)
	{
		return Task.CompletedTask;
	}

	public long GetAtAddress(int index) => _memory[index];

	public List<long> GetOutputBuffer() => _outputBuffer.ToList();
	public string GetOutputBufferAsString() => string.Join("", _outputBuffer);

	public List<long> DumpMemory() => _memory.ToList();

	public async Task RunAsync()
	{
		var preOpCode = _memory[_instructionPointer];
		long opcode;
		Dictionary<int, long> paramModes;
		(paramModes, opcode) = GetInstructions(preOpCode);

		while (_instructionPointer < _memory.Count)
		{
			_instructionPointer += (int)await _operations[opcode](_memory, paramModes, _instructionPointer);
			if (_instructionPointer >= _memory.Count)
			{
				break;
			}
			(paramModes, opcode) = GetInstructions(_memory[_instructionPointer]);
		}
	}

	private (Dictionary<int, long> paramModes, long opcode) GetInstructions(long preOpCode)
	{
		Dictionary<int, long> paramModes;
		long opcode;
		if (preOpCode > 99)
		{
			var preOpCodeStr = preOpCode.ToString().Reverse().ToList();
			paramModes = preOpCodeStr
				.Skip(2)
				.Select(c => long.Parse(c.ToString()))
				.Index()
				.ToDictionary(c => c.Index, c => c.Item);
			opcode = long.Parse(preOpCodeStr[1] + preOpCodeStr[0].ToString());
		}
		else
		{
			opcode = preOpCode;
			paramModes = new Dictionary<int, long>();
		}

		return (paramModes, opcode);
	}

	private long GetComboOperand(long inputOperand)
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

	private Task<long> Adv(List<long> mem, Dictionary<int, long> paramModes, int instructionPointer)
	{
		var operand = mem[instructionPointer + 1];
		var numerator = _regA;
		var denominator = 1 << (int)GetComboOperand(operand);
		_regA = (long)((double)numerator / (double)denominator);
		return Task.FromResult(2L);
	}

	private Task<long> Bxl(List<long> mem, Dictionary<int, long> paramModes, int instructionPointer)
	{
		var operand = mem[instructionPointer + 1];
		_regB = _regB ^ operand;
		return Task.FromResult(2L);
	}

	private Task<long> Bst(List<long> mem, Dictionary<int, long> paramModes, int instructionPointer)
	{
		var operand = mem[instructionPointer + 1];
		var comboOperand = GetComboOperand(operand);
		_regB = comboOperand % 8;
		return Task.FromResult(2L);
	}

	private Task<long> Jnz(List<long> mem, Dictionary<int, long> paramModes, int instructionPointer)
	{
		if (_regA == 0)
		{
			return Task.FromResult(2L);
		}

		var operand = mem[instructionPointer + 1];
		var step = -instructionPointer + operand;
		return Task.FromResult(step);
	}

	private Task<long> Bxc(List<long> mem, Dictionary<int, long> paramModes, int instructionPointer)
	{
		_regB = _regB ^ _regC;
		return Task.FromResult(2L);
	}

	private async Task<long> Out(List<long> mem, Dictionary<int, long> paramModes, int instructionPointer)
	{
		var operand = mem[instructionPointer + 1];
		var comboOperand = GetComboOperand(operand);
		var outValue = comboOperand % 8;
    		_outputBuffer.Add(outValue);
    		await _outputAction(outValue);
    		return 2;
	}

	private Task<long> Bdv(List<long> mem, Dictionary<int, long> paramModes, int instructionPointer)
	{
		var operand = mem[instructionPointer + 1];
		var numerator = _regA;
		var denominator = 1 << (int)GetComboOperand(operand);
		_regB = (long)((double)numerator / (double)denominator);
		return Task.FromResult(2L);
	}

	private Task<long> Cdv(List<long> mem, Dictionary<int, long> paramModes, int instructionPointer)
	{
		var operand = mem[instructionPointer + 1];
		var numerator = _regA;
		var denominator = 1L << (int)GetComboOperand(operand);
		_regC = (long)((double)numerator / (double)denominator);
		return Task.FromResult(2L);
	}
}