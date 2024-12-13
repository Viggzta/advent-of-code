namespace AdventOfCode._2019.IntComputer;

public class Intcomputer
{
	private List<int> _memory;
	private int _instructionPointer;
	private static List<int> _outputBuffer = [];

	public Intcomputer(List<int> memory)
	{
		_memory = memory;
		_instructionPointer = 0;
	}

	public int GetAtAddress(int index) => _memory[index];

	public List<int> GetOutputBuffer() => _outputBuffer.ToList();

	private readonly Dictionary<int, Func<List<int>, Dictionary<int, int>, int, int>> _operations = new()
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

	public void Run()
	{
		var preOpCode = _memory[_instructionPointer];
		int opcode;
		Dictionary<int, int> paramModes;
		(paramModes, opcode) = GetInstructions(preOpCode);

		while (opcode != 99)
		{
			_instructionPointer += _operations[opcode](_memory, paramModes, _instructionPointer);
			(paramModes, opcode) = GetInstructions(_memory[_instructionPointer]);
		}
	}

	private static (Dictionary<int, int> paramModes, int opcode) GetInstructions(int preOpCode)
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

	private static int Add(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		var b = paramModes.TryGetValue(1, out var bMode) && bMode == 1
			? mem[instructionPointer + 2]
			: mem[mem[instructionPointer + 2]];

		mem[mem[instructionPointer + 3]] = a + b;
		return 4;
	}

	private static int Mul(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		var b = paramModes.TryGetValue(1, out var bMode) && bMode == 1
			? mem[instructionPointer + 2]
			: mem[mem[instructionPointer + 2]];

		mem[mem[instructionPointer + 3]] = a * b;
		return 4;
	}

	private static int Input(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		Console.WriteLine("Input:");
		var input = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
		mem[mem[instructionPointer + 1]] = input;
		return 2;
	}

	private static int Output(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		_outputBuffer.Add(a);
		return 2;
	}

	private static int JmpIfTrue(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		var b = paramModes.TryGetValue(1, out var bMode) && bMode == 1
			? mem[instructionPointer + 2]
			: mem[mem[instructionPointer + 2]];

		return a != 0
			? -instructionPointer + b
			: 3;
	}

	private static int JmpIfFalse(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		var b = paramModes.TryGetValue(1, out var bMode) && bMode == 1
			? mem[instructionPointer + 2]
			: mem[mem[instructionPointer + 2]];

		return a == 0
			? -instructionPointer + b
			: 3;
	}

	private static int LessThan(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		var b = paramModes.TryGetValue(1, out var bMode) && bMode == 1
			? mem[instructionPointer + 2]
			: mem[mem[instructionPointer + 2]];

		mem[mem[instructionPointer + 3]] = a < b ? 1 : 0;

		return 4;
	}

	private static int IsEq(List<int> mem, Dictionary<int, int> paramModes, int instructionPointer)
	{
		var a = paramModes.TryGetValue(0, out var aMode) && aMode == 1
			? mem[instructionPointer + 1]
			: mem[mem[instructionPointer + 1]];
		var b = paramModes.TryGetValue(1, out var bMode) && bMode == 1
			? mem[instructionPointer + 2]
			: mem[mem[instructionPointer + 2]];

		mem[mem[instructionPointer + 3]] = a == b ? 1 : 0;

		return 4;
	}
}