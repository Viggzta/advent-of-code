using System.Text.RegularExpressions;

namespace AdventOfCode.Utility;

public partial class TestFetchRegex
{
	[GeneratedRegex(
		"<pre><code>(?<Code>(?:.|\\n)*?)<\\/code><\\/pre>",
		RegexOptions.None,
		"en-US")]
	private static partial Regex TestRegex();

	public static MatchCollection Matches(string input)
	{
		return TestRegex().Matches(input);
	}
}