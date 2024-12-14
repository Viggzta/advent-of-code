namespace AdventOfCode._2019.IntComputer;

public class Intcomputer
{
	private readonly Dictionary<
		int,
		Func<
			List<int>,
			Dictionary<int, int>,
			int,
			Task<int>>> _operations;

	private List<int> _memory;
	private readonly Func<Intcomputer, Task<int>> _inputAction;
	private readonly Func<int, Task> _outputAction;
	private int _instructionPointer;
	private readonly List<int> _outputBuffer;

	public Intcomputer(
		List<int> memory,
		Func<Intcomputer, Task<int>>? inputAction = null,
		Func<int, Task>? outputAction = null)
	{
		_memory = memory.ToList();
		_inputAction = inputAction ?? DefaultInputAction;
		_outputAction = outputAction ?? DefaultOutputAction;
		_outputBuffer = [];
		_instructionPointer = 0;
		_operations = new Dictionary<int, Func<List<int>, Dictionary<int, int>, int, Task<int>>>
		{
			{ 1, Add },
			{ 2, Mul },
			{ 3, Input },
			{ 4, Output },
			{ 5, JmpIfTrue },
			{ 6, JmpIfFalse },
			{ 7, LessThan },
			{ 8, IsEq },
		};
	}

	private Task<int> DefaultInputAction(Intcomputer intcomputer)
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

		while (opcode != 99)
		{
			_instructionPointer += await _operations[opcode](_memory, paramModes, _instructionPointer);
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