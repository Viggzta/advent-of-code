using System.Text.RegularExpressions;
using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day24 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		List<IGate> gates = new();
		var presetGates = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Replace(" ", "").Split(":"))
			.Select(x => new ValueGate(x[0], x[1] == "1"))
			.ToList();
		gates.AddRange(presetGates);
		var regex = new Regex("(?<A>.+) (?<gateType>AND|OR|XOR) (?<B>.+) -> (?<C>.+)");
		var dependencyGates = input
			.Skip(presetGates.Count + 1)
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Replace("&gt;", ">"))
			.Select(x => regex.Match(x));

		var dependencyGatesGates = dependencyGates.Select(x =>
		{
			return (IGate)(x.Groups["gateType"].Value switch
			{
				"AND" => new AndGate(x.Groups["C"].Value),
				"OR" => new OrGate(x.Groups["C"].Value),
				"XOR" => new XorGate(x.Groups["C"].Value),
				_ => throw new NotImplementedException(),
			});
		});
		gates.AddRange(dependencyGatesGates);

		foreach (var gate in dependencyGates)
		{
			var inGateA = gates.First(x => x.Name == gate.Groups["A"].Value);
			var inGateB = gates.First(x => x.Name == gate.Groups["B"].Value);
			var gateToSetUp = gates.First(x => x.Name == gate.Groups["C"].Value) as IInputGate;
			gateToSetUp.A = inGateA;
			gateToSetUp.B = inGateB;
		}

		var regex2 = new Regex("z(?<number>\\d+)");
		var gatesToFindOut = gates.Where(x => regex2.IsMatch(x.Name)).ToList();

		long number = 0;
		var offsetI = 0;
		foreach (var gate in gatesToFindOut.OrderBy(x => x.Name))
		{
			number += (gate.Value ? 1L : 0L) << offsetI;
			offsetI++;
		}

		return Task.FromResult(number.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		List<IGate> gates = new();
		var presetGates = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Replace(" ", "").Split(":"))
			.Select(x => new ValueGate(x[0], x[1] == "1"))
			.ToList();
		gates.AddRange(presetGates);
		var regex = new Regex("(?<A>.+) (?<gateType>AND|OR|XOR) (?<B>.+) -> (?<C>.+)");
		var dependencyGates = input
			.Skip(presetGates.Count + 1)
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Replace("&gt;", ">"))
			.Select(x => regex.Match(x));

		var dependencyGatesGates = dependencyGates.Select(x =>
		{
			return (IGate)(x.Groups["gateType"].Value switch
			{
				"AND" => new AndGate(x.Groups["C"].Value),
				"OR" => new OrGate(x.Groups["C"].Value),
				"XOR" => new XorGate(x.Groups["C"].Value),
				_ => throw new NotImplementedException(),
			});
		});
		gates.AddRange(dependencyGatesGates);

		foreach (var gate in dependencyGates)
		{
			var inGateA = gates.First(x => x.Name == gate.Groups["A"].Value);
			var inGateB = gates.First(x => x.Name == gate.Groups["B"].Value);
			var gateToSetUp = gates.First(x => x.Name == gate.Groups["C"].Value) as IInputGate;
			gateToSetUp.A = inGateA;
			gateToSetUp.B = inGateB;
		}

		var gates2 = CopyGates(gates);

		var regexX = new Regex("x(?<number>\\d+)");
		var regexY = new Regex("y(?<number>\\d+)");
		var regexZ = new Regex("z(?<number>\\d+)");
		var xGates = gates.Where(x => regexX.IsMatch(x.Name)).ToList();
		var yGates = gates.Where(x => regexY.IsMatch(x.Name)).ToList();
		var zGates = gates.Where(x => regexZ.IsMatch(x.Name)).OrderBy(x => x.Name).ToList();
		var xVal = ToDecimalNumber(xGates);
		var yVal = ToDecimalNumber(yGates);
		var zVal = ToDecimalNumber(zGates);

		/*
		var invalidOrGates = InvalidOrGates(gates);
		var invalidAndGates = InvalidAndGates(gates);
		var invalidXorGates = InvalidXorGates(gates);
		var invalidGates = invalidOrGates
			.Concat(invalidAndGates)
			.Concat(invalidXorGates)
			.Select(x => x as IInputGate)
			.ToList();

		var gatesPossiblePointWrong = invalidGates
			.SelectMany(x => new List<IGate> { x.A, x.B })
			.Distinct()
			.ToList();
			*/

		var wrongGates = new List<string>();
		// z00 and z01 is a bit special but then the pattern repeats
		for (int i = 2; i < zGates.Count - 1; i++) // -1 since there is a carryBit extra
		{
			var zN = zGates[i] as XorGate;
			var xN = xGates[i];
			var yN = yGates[i];


			if (zN == null)
			{
				wrongGates.Add(zGates[i].Name);
				continue;
			}
			var xorGate = gates
				.FirstOrDefault(g => g is XorGate localXor && localXor.Contains(xN) && localXor.Contains(yN));
			if (xorGate == null)
			{
				Console.WriteLine("Unhandled for now");
			}

			if (!zN.Contains(xorGate))
			{
				// Child is wrong
				var theWrongGate = zN.A is OrGate ? zN.B : zN.B is OrGate ? zN.A : null;
				wrongGates.Add(theWrongGate.Name);
				continue;
			}

			var orGate = zN.A is OrGate orA ? orA : zN.B is OrGate orB ? orB : null;
			if (orGate == null)
			{
				var theWrongGate = zN.A == xorGate ? zN.B : zN.B == xorGate ? zN.A : null;
				wrongGates.Add(theWrongGate.Name);
				continue;
			}

			if (orGate.A is not AndGate || orGate.B is not AndGate)
			{
				var wrongGate = orGate.A is AndGate ? orGate.B : orGate.B is AndGate ? orGate.A : null;
				wrongGates.Add(wrongGate.Name);
				continue;
			}

			var prevInputAnd = gates
				.FirstOrDefault(x => x is AndGate prevAnd && prevAnd.Contains(xGates[i - 1]) && prevAnd.Contains(yGates[i - 1]));
			if (prevInputAnd == null)
			{
				Console.WriteLine("What case is this?");
			}
			if (!orGate.Contains(prevInputAnd))
			{
				wrongGates.Add(orGate.Name);
				continue;
			}

			var otherAndGateTemp = orGate.A == prevInputAnd ? orGate.B : orGate.A;
			if (otherAndGateTemp is not AndGate)
			{
				wrongGates.Add(otherAndGateTemp.Name);
				continue;
			}
			var otherAndGate = otherAndGateTemp as AndGate;
			var prevInputXor = gates
				.FirstOrDefault(x => x is XorGate prevXor && prevXor.Contains(xGates[i - 1]) && prevXor.Contains(yGates[i - 1]));
			if (!otherAndGate.Contains(prevInputXor))
			{
				wrongGates.Add(otherAndGate.Name); // Might be wrong, change to prevInputAnd
				continue;
			}
		}

		var wrongGatesPruned = wrongGates.Distinct().Order();
		var result = string.Join(",", wrongGatesPruned);

		return Task.FromResult(result);
	}

	private static List<IGate> InvalidOrGates(List<IGate> gates)
	{
		return gates
			.Where(
				x =>
					x is OrGate or &&
					(or.A is not AndGate || or.B is not AndGate))
			.ToList();
	}

	private static List<IGate> InvalidAndGates(List<IGate> gates)
	{
		return gates
			.Where(
				x =>
					x is AndGate g &&
					(
						g is not { A: ValueGate, B: ValueGate } &&
						g is not { A: XorGate, B: OrGate } &&
						g is not { A: OrGate, B: XorGate } &&
						g is not { A: XorGate, B: AndGate } &&
						g is not { A: AndGate, B: XorGate }
					))
			.ToList();
	}

	private static List<IGate> InvalidXorGates(List<IGate> gates)
	{
		return gates
			.Where(
				x =>
					x is XorGate g &&
					(
						g is not { A: ValueGate, B: ValueGate } &&
						g is not { A: XorGate, B: OrGate } &&
						g is not { A: OrGate, B: XorGate } &&
						g is not { A: XorGate, B: AndGate } &&
						g is not { A: AndGate, B: XorGate }
					))
			.ToList();
	}

	private IList<IGate> CopyGates(List<IGate> gates)
	{
		var gateGates = new List<IGate>();
		foreach (var gate in gates)
		{
			switch (gate)
			{
				case ValueGate vg:
					gateGates.Add(new ValueGate(vg.Name, vg.Value));
					break;
				case AndGate andG:
					gateGates.Add(new AndGate(andG.Name));
					break;
				case OrGate orG:
					gateGates.Add(new OrGate(orG.Name));
					break;
				case XorGate xorG:
					gateGates.Add(new XorGate(xorG.Name));
					break;
			}
		}

		foreach (var gate in gates)
		{
			if (gate is IInputGate inputGate)
			{
					var gg = gateGates.First(x => x.Name == gate.Name);
					var inGG = gg as IInputGate;
					inGG.A = gateGates.First(x => x.Name == inputGate.A.Name);
					inGG.B = gateGates.First(x => x.Name == inputGate.B.Name);
					break;
			}
		}

		return gateGates;
	}

	private long ToDecimalNumber(List<IGate> gates)
	{
		long number = 0;
		var offsetI = 0;
		foreach (var gate in gates.OrderBy(x => x.Name))
		{
			number += (gate.Value ? 1L : 0L) << offsetI;
			offsetI++;
		}

		return number;
	}

	public interface IGate
	{
		public string Name { get; }

		public bool Value { get; }
	}

	public interface IInputGate
	{
		public IGate A { get; set; }
		public IGate B { get; set; }

		public bool Contains(IGate gate);
	}

	public class ValueGate(string name, bool value) : IGate
	{
		public string Name { get; } = name;
		public bool Value { get; } = value;

		public override string ToString()
		{
			return $"{name}: {nameof(ValueGate)}";
		}
	}

	public class AndGate(string name) : IGate, IInputGate
	{
		public IGate A { get; set; }
		public IGate B { get; set; }
		public bool Contains(IGate gate)
		{
			return A == gate || B == gate;
		}

		public string Name { get; } = name;
		public bool Value => A.Value && B.Value;

		public override string ToString()
		{
			return $"{name}: {nameof(AndGate)}";
		}
	}

	public class OrGate(string name) : IGate, IInputGate
	{
		public IGate A { get; set; }
		public IGate B { get; set; }
		public bool Contains(IGate gate)
		{
			return A == gate || B == gate;
		}
		public string Name { get; } = name;
		public bool Value => A.Value || B.Value;
		public override string ToString()
		{
			return $"{name}: {nameof(OrGate)}";
		}
	}

	public class XorGate(string name) : IGate, IInputGate
	{
		public IGate A { get; set; }
		public IGate B { get; set; }
		public bool Contains(IGate gate)
		{
			return A == gate || B == gate;
		}
		public string Name { get; } = name;
		public bool Value => A.Value ^ B.Value;
		public override string ToString()
		{
			return $"{name}: {nameof(XorGate)}";
		}
	}
}