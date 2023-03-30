using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
	public class Group
	{
        public Group()
        {
        }

        public Group(string groupName)
        {
            GroupName = groupName;
        }

        [Key]
		public string GroupName { get; set; } = null!;
		public ICollection<Connection> Connections { get; set; } = new List<Connection>(); 
	}
}