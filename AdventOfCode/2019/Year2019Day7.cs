using System.Collections.Concurrent;
using AdventOfCode._2019.IntComputer;
using AdventOfCode.Utility;

namespace AdventOfCode._2019;

public class Year2019Day7 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var code = input.First().Split(',').Select(int.Parse).ToList();
		var permutations = Enumerable.Range(0, 5)
			.Permute()
			.Select(x => x.ToList())
			.ToList();
		List<int> thrusterCodes = new();
		foreach (var perm in permutations)
		{
			var prevResult = 0;
			foreach (var phaseSetting in perm)
			{
				Queue<int> inputs = new Queue<int>(2);
				inputs.Enqueue(phaseSetting);
				inputs.Enqueue(prevResult);
				Intcomputer intcomputer = new Intcomputer(
					code,
					i => Task.FromResult(inputs.Dequeue()));
				intcomputer.RunAsync().Wait();
				prevResult = intcomputer.GetOutputBuffer().First();
			}
			thrusterCodes.Add(prevResult);
		}

		var result = thrusterCodes.Max();
		return Task.FromResult(result.ToString());
	}

	public async Task<string> RunSolution2Async(IList<string> input)
	{
		var code = input.First().Split(',').Select(int.Parse).ToList();
		var permutations = Enumerable.Range(5, 5)
			.Permute()
			.Select(x => x.ToList())
			.ToList();
		List<int> thrusterCodes = new();

		foreach (var perm in permutations)
		{
			var computers = new List<Intcomputer>();
			var compInputs = new List<ConcurrentQueue<int>>();
			var compInSemaphores = new List<SemaphoreSlim>();
			perm.ForEach(_ =>
			{
				compInputs.Add(new ConcurrentQueue<int>());
				compInSemaphores.Add(new SemaphoreSlim(0));
			});
			var computerTasks = new List<Task>();
			foreach (var phaseSetting in perm.Index())
			{
				compInputs[phaseSetting.Index].Enqueue(phaseSetting.Item);
				compInSemaphores[phaseSetting.Index].Release();
				Intcomputer intcomputer = new Intcomputer(
					code,
					async i =>
					{
						await compInSemaphores[phaseSetting.Index].WaitAsync();
						compInputs[phaseSetting.Index].TryDequeue(out var number);
						return number;
					},
					i =>
					{
						var nextIndex = (phaseSetting.Index + 1) % perm.Count;
						compInputs[nextIndex].Enqueue(i);
						compInSemaphores[nextIndex].Release();
						return Task.CompletedTask;
					});
				computers.Add(intcomputer);
				var t = intcomputer.RunAsync();
				computerTasks.Add(t);
			}
			// Kickstart the loop
			compInputs[0].Enqueue(0);
			compInSemaphores[0].Release();

			await computerTasks.Last();
			var thrusterCode = computers.Last().GetOutputBuffer().Last();
			thrusterCodes.Add(thrusterCode);
		}

		var result = thrusterCodes.Max();
		return result.ToString();
	}
}