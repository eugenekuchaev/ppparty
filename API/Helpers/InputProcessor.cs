using System.Text.RegularExpressions;
using API.Entities;

namespace API.Helpers
{
	public class InputProcessor
	{
		public const string stringPattern = @"^[^a-zA-Z0-9]*$";
		public const string symbolPattern = @"[^a-zA-Z0-9-]";
		
		public bool IsValidInputForInterestsAndTags(string input)
		{
			bool containsOnlySpacesOrSymbols = Regex.IsMatch(input, stringPattern);

			if (input == null || input == "" || containsOnlySpacesOrSymbols)
			{
				return false;
			}

			return true;
		}

		public List<string> SplitString(string input)
		{
			List<string> inputs = input
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(s => TrimDashes(s))
				.Where(i => !string.IsNullOrEmpty(i))
				.ToList();

			return inputs;
		}
		
		public bool HasInvalidDates(ICollection<EventDate> eventDates, out string? invalidDateMessage)
		{
			if (eventDates!.Any(x => x.StartDate > x.EndDate))
			{
				invalidDateMessage = "End date cannot be earlier than start date";
				return true;
			}
			
			if (eventDates!.Any(x => x.StartDate.AddDays(1) < x.EndDate))
			{
				invalidDateMessage = "Start date and end date cannot differ for more than one day";
				return true;
			}
			
			invalidDateMessage = null;
			return false;
		}
		
		public bool HasInvalidTags(Event appEvent, string tag, out string? invalidTagMessage)
		{
			if (tag.Length > 32)
			{
				invalidTagMessage = "One of the tags is too long";
				return true;
			}

			if (appEvent.EventTags.Any(x => x.EventTagName == tag))
			{
				invalidTagMessage = "There's already one of these tags in this event";
				return true;
			}
			
			invalidTagMessage = null;
			return false;
		}
		
		public bool IsCorrectNumberOfElements(int numberOfElementsInEntity, out string? errorMessage, 
			string elementName, int numberOfNewElements = 0)
		{
			if (numberOfElementsInEntity + numberOfNewElements > 50) 
			{
				errorMessage = $"Too many {elementName}s";
				return false;
			}
			
			if (numberOfElementsInEntity + numberOfNewElements < 3 && elementName == "tag") 
			{
				errorMessage = $"There should be at least three {elementName}s";
				return false;
			}
			
			errorMessage = null;
			return true;
		}
		
		private string TrimDashes(string s)
		{
			string trimmed = Regex.Replace(s.Trim(), symbolPattern, "");

			if (trimmed.Contains("-"))
			{
				trimmed = Regex.Replace(trimmed, "-{2,}", "-"); 
				trimmed = trimmed.Trim('-');
			}

			return trimmed;
		}
	}
}