namespace API.Helpers
{
	public class LinkTransformer
	{
		public string? AddHttpsToLink(string? link)
		{
			if (link == "" || link == null) 
			{
				return null;
			}
			else if (link.StartsWith("https://"))
			{
				return link;
			}
			else if (link.StartsWith("www."))
			{
				link = "https://" + link;
				return link;
			}
			else 
			{
				link = "https://" + link;
				return link;
			}
		}
	}
}