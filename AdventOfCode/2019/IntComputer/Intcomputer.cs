namespace AdventOfCode._2019.IntComputer;

public class Intcomputer
{
	private List<int> _memory;
	private int _instructionPointer;

	public Intcomputer(List<int> memory)
	{
		_memory = memory;
		_instructionPointer = 0;
	}

	public int GetAtAddress(int index) => _memory[index];

	private readonly Dictionary<int, Func<List<int>, int, int>> _operations = new()
	{
		{ 1, Add },
		{ 2, Mul },
	};

	public void Run()
	{
		var opcode = _memory[_instructionPointer];
		while (opcode != 99)
		{
			_instructionPointer += _operations[opcode](_memory, _instructionPointer);
			opcode = _memory[_instructionPointer];
		}
	}

	private static int Add(List<int> mem, int instructionPointer)
	{
		mem[mem[instructionPointer + 3]] =
			mem[mem[instructionPointer + 1]] +
			mem[mem[instructionPointer + 2]];
		return 4;
	}

	private static int Mul(List<int> mem, int instructionPointer)
	{
		mem[mem[instructionPointer + 3]] =
			mem[mem[instructionPointer + 1]] *
			mem[mem[instructionPointer + 2]];
		return 4;
	}
}