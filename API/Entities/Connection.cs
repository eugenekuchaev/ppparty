namespace API.Entities
{
	public class Connection
	{
		public Connection()
		{
		}

		public Connection(string connectionId, string username)
		{
			ConnectionId = connectionId;
			Username = username;
		}

		public string ConnectionId { get; set; } = null!;
		public string Username { get; set; } = null!;
	}
}