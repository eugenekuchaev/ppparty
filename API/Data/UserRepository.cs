using System.Text.RegularExpressions;
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
			var query = _context.Users.AsQueryable();

			if (!string.IsNullOrEmpty(userParams.FullName))
			{
				query = query.Where(u => u.FullName.ToLower() == userParams.FullName.ToLower());
			}

			if (!string.IsNullOrEmpty(userParams.Username))
			{
				query = query.Where(u => u.UserName.ToLower() == userParams.Username.ToLower());
			}

			if (!string.IsNullOrEmpty(userParams.Country))
			{
				query = query.Where(u => u.Country != null && u.Country.ToLower() == userParams.Country.ToLower());
			}

			if (!string.IsNullOrEmpty(userParams.Region))
			{
				query = query.Where(u => u.Region != null && u.Region.ToLower() == userParams.Region.ToLower());
			}

			if (!string.IsNullOrEmpty(userParams.City))
			{
				query = query.Where(u => u.City != null && u.City.ToLower() == userParams.City.ToLower());
			}

			if (!string.IsNullOrEmpty(userParams.UserInterest))
			{
				query = query.Where(u => u.UserInterests!.Any(i => 
					i.InterestName.ToLower() == userParams.UserInterest.ToLower()));
			}
			
			query = query.Where(u => u.UserName != userParams.CurrentUsername);

			return await PagedList<MemberDto>.CreateAsync(
				query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(),
				userParams.PageNumber,
				userParams.PageSize);
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