using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
	public interface IUserRepository
	{
		void AddPhoto(UserPhoto photo);
		Task<UserInterest?> GetUserInterestByNameAsync(string interestName);
		void UpdateUser(AppUser user);
		Task<bool> SaveAllAsync();
		Task<AppUser?> GetUserByUsernameAsync(string username);
		Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
		Task<MemberDto?> GetMemberAsync(string username);
	}
}