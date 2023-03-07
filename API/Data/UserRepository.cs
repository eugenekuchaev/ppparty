using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class UserRepository : IUserRepository
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public UserRepository(DataContext context, IMapper mapper)
		{
			_mapper = mapper;
			_context = context;
		}

		public async Task<MemberDto?> GetMemberAsync(string username)
		{
			return await _context.Users
				.Where(x => x.UserName == username)
				.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
				.SingleOrDefaultAsync();
		}

		public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
		{
			var query = _context.Users
				.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
				.AsNoTracking();
				
			return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
		}

		public async Task<AppUser?> GetUserByIdAsync(int id)
		{
			return await _context.Users
				.FindAsync(id);
		}

		public async Task<AppUser?> GetUserByUsernameAsync(string username)
		{
			return await _context.Users
				.Include(p => p.UserPhoto)
				.Include(p => p.UserInterests)
				.SingleOrDefaultAsync(x => x.UserName == username);
		}

		public async Task<IEnumerable<AppUser>> GetUsersAsync()
		{
			return await _context.Users
				.Include(p => p.UserPhoto)
				.Include(p => p.UserInterests)
				.ToListAsync();
		}
		
		public async Task<IEnumerable<UserInterest>> GetUserInterestsAsync()
		{
			return await _context.UserInterests.ToListAsync();
		}
		
		public async Task<UserInterest?> GetUserInterestByNameAsync(string interestName)
		{
			return await _context.UserInterests.SingleOrDefaultAsync(x => x.InterestName == interestName);
		}

		public async Task<bool> SaveAllAsync()
		{
			return await _context.SaveChangesAsync() > 0;
		}

		public void UpdateUser(AppUser user)
		{
			_context.Entry(user).State = EntityState.Modified;
		}
		
		public void AddPhoto(UserPhoto photo)
		{
			_context.UserPhotos.Add(photo);
		}
	}
}