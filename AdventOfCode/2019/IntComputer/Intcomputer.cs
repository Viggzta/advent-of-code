namespace AdventOfCode._2019.IntComputer;

public class Intcomputer
{
	private enum Mode
	{
		Position = 0,
		Immidiete = 1,
		Relative = 2,
	}

	private readonly Dictionary<
		long,
		Func<
			Dictionary<long, long>,
			Dictionary<int, Mode>,
			long,
			Task<long>>> _operations;

	private Dictionary<long, long> _memory;
	private readonly Func<Intcomputer, Task<long>> _inputAction;
	private readonly Func<long, Task> _outputAction;
	private long _instructionPointer;
	private long _relativePointer;
	private readonly List<long> _outputBuffer;

	public Intcomputer(
		List<long> memory,
		Func<Intcomputer, Task<long>>? inputAction = null,
		Func<long, Task>? outputAction = null)
	{
		_memory = memory.Index().ToDictionary(m => (long)m.Index, m => m.Item);
		_inputAction = inputAction ?? DefaultInputAction;
		_outputAction = outputAction ?? DefaultOutputAction;
		_outputBuffer = [];
		_instructionPointer = 0;
		_relativePointer = 0;
		_operations = new Dictionary<long, Func<Dictionary<long, long>, Dictionary<int, Mode>, long, Task<long>>>
		{
			{ 1, Add },
			{ 2, Mul },
			{ 3, Input },
			{ 4, Output },
			{ 5, JmpIfTrue },
			{ 6, JmpIfFalse },
			{ 7, LessThan },
			{ 8, IsEq },
			{ 9, AdjustRelBase },
		};
	}

	private Task<long> DefaultInputAction(Intcomputer intcomputer)
	{
		Console.WriteLine("Input:");
		return Task.FromResult(long.Parse(Console.ReadLine() ?? throw new InvalidOperationException()));
	}

	private Task DefaultOutputAction(long output)
	{
		return Task.CompletedTask;
	}

	public long GetAtAddress(long index) => _memory[index];

	public List<long> GetOutputBuffer() => _outputBuffer.ToList();
	public string GetOutputBufferAsString() => string.Join("", _outputBuffer);

	public Dictionary<long, long> DumpMemory() => _memory.ToDictionary();

	public async Task RunAsync()
	{
		var preOpCode = _memory[_instructionPointer];
		long opcode;
		Dictionary<int, Mode> paramModes;
		(paramModes, opcode) = GetInstructions(preOpCode);

		while (opcode != 99)
		{
			_instructionPointer += await _operations[opcode](_memory, paramModes, _instructionPointer);
			(paramModes, opcode) = GetInstructions(_memory[_instructionPointer]);
		}
	}

	private (Dictionary<int, Mode> paramModes, long opcode) GetInstructions(long preOpCode)
	{
		Dictionary<int, Mode> paramModes;
		long opcode;
		if (preOpCode > 99)
		{
			var preOpCodeStr = preOpCode.ToString().Reverse().ToList();
			paramModes = preOpCodeStr
				.Skip(2)
				.Select(c => int.Parse(c.ToString()))
				.Index()
				.ToDictionary(c => c.Index, c => (Mode)c.Item);
			opcode = int.Parse(preOpCodeStr[1] + preOpCodeStr[0].ToString());
		}
		else
		{
			opcode = preOpCode;
			paramModes = new Dictionary<int, Mode>();
		}

		return (paramModes, opcode);
	}

	private Task<long> Add(Dictionary<long, long> mem, Dictionary<int, Mode> paramModes, long instructionPointer)
	{
		var aMode = paramModes.GetValueOrDefault(0, Mode.Position);
		var a = aMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 1)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 1),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 1)),
			_ => throw new ArgumentOutOfRangeException()
		};
		var bMode = paramModes.GetValueOrDefault(1, Mode.Position);
		var b = bMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 2)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 2),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 2)),
			_ => throw new ArgumentOutOfRangeException()
		};
		var cMode = paramModes.GetValueOrDefault(2, Mode.Position);
		var cPos = cMode switch
		{
			Mode.Position => mem.GetValueOrDefault(instructionPointer + 3),
			Mode.Immidiete => throw new InvalidOperationException(),
			Mode.Relative => _relativePointer + mem.GetValueOrDefault(instructionPointer + 3),
			_ => throw new ArgumentOutOfRangeException()
		};

		mem[cPos] = a + b;
		return Task.FromResult(4L);
	}

	private Task<long> Mul(Dictionary<long, long> mem, Dictionary<int, Mode> paramModes, long instructionPointer)
	{
		var aMode = paramModes.GetValueOrDefault(0, Mode.Position);
		var a = aMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 1)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 1),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 1)),
			_ => throw new ArgumentOutOfRangeException()
		};
		var bMode = paramModes.GetValueOrDefault(1, Mode.Position);
		var b = bMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 2)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 2),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 2)),
			_ => throw new ArgumentOutOfRangeException()
		};
		var cMode = paramModes.GetValueOrDefault(2, Mode.Position);
		var cPos = cMode switch
		{
			Mode.Position => mem.GetValueOrDefault(instructionPointer + 3),
			Mode.Immidiete => throw new InvalidOperationException(),
			Mode.Relative => _relativePointer + mem.GetValueOrDefault(instructionPointer + 3),
			_ => throw new ArgumentOutOfRangeException()
		};

		mem[cPos] = a * b;
		return Task.FromResult(4L);
	}

	private async Task<long> Input(Dictionary<long, long> mem, Dictionary<int, Mode> paramModes, long instructionPointer)
	{
		var input = await _inputAction(this);

		var cMode = paramModes.GetValueOrDefault(2, Mode.Position);
		var cPos = cMode switch
		{
			Mode.Position => mem.GetValueOrDefault(instructionPointer + 3),
			Mode.Immidiete => throw new InvalidOperationException(),
			Mode.Relative => _relativePointer + mem.GetValueOrDefault(instructionPointer + 3),
			_ => throw new ArgumentOutOfRangeException()
		};

		mem[cPos] = input;
		return 2L;
	}

	private async Task<long> Output(Dictionary<long, long> mem, Dictionary<int, Mode> paramModes, long instructionPointer)
	{
		var aMode = paramModes.GetValueOrDefault(0, Mode.Position);
		var a = aMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 1)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 1),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 1)),
			_ => throw new ArgumentOutOfRangeException()
		};
		_outputBuffer.Add(a);
		await _outputAction(a);
		return 2;
	}

	private Task<long> JmpIfTrue(Dictionary<long, long> mem, Dictionary<int, Mode> paramModes, long instructionPointer)
	{
		var aMode = paramModes.GetValueOrDefault(0, Mode.Position);
		var a = aMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 1)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 1),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 1)),
			_ => throw new ArgumentOutOfRangeException()
		};
		var bMode = paramModes.GetValueOrDefault(1, Mode.Position);
		var b = bMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 2)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 2),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 2)),
			_ => throw new ArgumentOutOfRangeException()
		};

		var step = a != 0
			? -instructionPointer + b
			: 3;
		return Task.FromResult(step);
	}

	private Task<long> JmpIfFalse(Dictionary<long, long> mem, Dictionary<int, Mode> paramModes, long instructionPointer)
	{
		var aMode = paramModes.GetValueOrDefault(0, Mode.Position);
		var a = aMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 1)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 1),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 1)),
			_ => throw new ArgumentOutOfRangeException()
		};
		var bMode = paramModes.GetValueOrDefault(1, Mode.Position);
		var b = bMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 2)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 2),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 2)),
			_ => throw new ArgumentOutOfRangeException()
		};

		var step = a == 0
			? -instructionPointer + b
			: 3;
		return Task.FromResult(step);
	}

	private Task<long> LessThan(Dictionary<long, long> mem, Dictionary<int, Mode> paramModes, long instructionPointer)
	{
		var aMode = paramModes.GetValueOrDefault(0, Mode.Position);
		var a = aMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 1)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 1),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 1)),
			_ => throw new ArgumentOutOfRangeException()
		};
		var bMode = paramModes.GetValueOrDefault(1, Mode.Position);
		var b = bMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 2)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 2),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 2)),
			_ => throw new ArgumentOutOfRangeException()
		};
		var cMode = paramModes.GetValueOrDefault(2, Mode.Position);
		var cPos = cMode switch
		{
			Mode.Position => mem.GetValueOrDefault(instructionPointer + 3),
			Mode.Immidiete => throw new InvalidOperationException(),
			Mode.Relative => _relativePointer + mem.GetValueOrDefault(instructionPointer + 3),
			_ => throw new ArgumentOutOfRangeException()
		};

		mem[cPos] = a < b ? 1 : 0;

		return Task.FromResult(4L);
	}

	private Task<long> IsEq(Dictionary<long, long> mem, Dictionary<int, Mode> paramModes, long instructionPointer)
	{
		var aMode = paramModes.GetValueOrDefault(0, Mode.Position);
		var a = aMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 1)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 1),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 1)),
			_ => throw new ArgumentOutOfRangeException()
		};
		var bMode = paramModes.GetValueOrDefault(1, Mode.Position);
		var b = bMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 2)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 2),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 2)),
			_ => throw new ArgumentOutOfRangeException()
		};
		var cMode = paramModes.GetValueOrDefault(2, Mode.Position);
		var cPos = cMode switch
		{
			Mode.Position => mem.GetValueOrDefault(instructionPointer + 3),
			Mode.Immidiete => throw new InvalidOperationException(),
			Mode.Relative => _relativePointer + mem.GetValueOrDefault(instructionPointer + 3),
			_ => throw new ArgumentOutOfRangeException()
		};

		mem[cPos] = a == b ? 1 : 0;

		return Task.FromResult(4L);
	}

	private Task<long> AdjustRelBase(Dictionary<long, long> mem, Dictionary<int, Mode> paramModes, long instructionPointer)
	{
		var aMode = paramModes.GetValueOrDefault(0, Mode.Position);
		var a = aMode switch
		{
			Mode.Position => mem.GetValueOrDefault(mem.GetValueOrDefault(instructionPointer + 1)),
			Mode.Immidiete => mem.GetValueOrDefault(instructionPointer + 1),
			Mode.Relative => mem.GetValueOrDefault(_relativePointer + mem.GetValueOrDefault(instructionPointer + 1)),
			_ => throw new ArgumentOutOfRangeException()
		};

		_relativePointer += a;

		return Task.FromResult(2L);
	}
}