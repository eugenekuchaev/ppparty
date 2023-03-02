using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
	public interface IUserRepository
	{
		Task<UserInterest?> GetUserInterestByNameAsync(string interestName);
		Task<IEnumerable<UserInterest>> GetUserInterestsAsync();
		void UpdateUser(AppUser user);
		Task<bool> SaveAllAsync();
		Task<IEnumerable<AppUser>> GetUsersAsync();
		Task<AppUser?> GetUserByIdAsync(int id);
		Task<AppUser?> GetUserByUsernameAsync(string username);
		Task<IEnumerable<MemberDto>> GetMembersAsync();
		Task<MemberDto?> GetMemberAsync(string username);
	}
}