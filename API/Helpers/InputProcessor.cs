using System.Text.RegularExpressions;

namespace API.Helpers
{
	public class InputProcessor
	{
		public const string pattern = @"^[ ,]*$";

		public bool ValidateInput(string input)
		{
			bool containsOnlySpacesOrCommas = Regex.IsMatch(input, pattern);

			if (input == null || input == "" || containsOnlySpacesOrCommas)
			{
				return false;
			}

			return true;
		}

		public List<string> SplitString(string input)
		{
			List<string> inputs = input
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.Where(i => !string.IsNullOrEmpty(i))
				.ToList();

			return inputs;
		}
	}
}