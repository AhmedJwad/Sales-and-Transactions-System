using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UsersRepository(DataContext context, UserManager<User> userManager
            , RoleManager<IdentityRole> roleManager,SignInManager<User> signInManager )
        {
           _context = context;
           _userManager = userManager;
          _roleManager = roleManager;
          _signInManager = signInManager;
        }
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
           return await _userManager.CreateAsync(user, password);
        }
        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if(!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }          
        }
        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
           return await _userManager.ConfirmEmailAsync(user, token);    
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<ActionResponse<IEnumerable<User>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Users.Include(x=>x.City).ThenInclude(x=>x.State).ThenInclude(x=>x.Country).AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower(),
                    StringComparison.CurrentCultureIgnoreCase) || x.LastName.ToLower().Contains(pagination.Filter.ToLower(), 
                    StringComparison.CurrentCultureIgnoreCase));
            }
            return new ActionResponse<IEnumerable<User>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).Paginate(pagination).ToListAsync()
            };
        }
        public async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Users.Include(x => x.City).ThenInclude(x => x.State).ThenInclude(x => x.Country).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower(),
                    StringComparison.CurrentCultureIgnoreCase) || x.LastName.ToLower().Contains(pagination.Filter.ToLower(),
                    StringComparison.CurrentCultureIgnoreCase));
            }
            int recordsNumber = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Users.Include(x => x.City).ThenInclude(x => x.State).ThenInclude(x => x.Country).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower(),
                    StringComparison.CurrentCultureIgnoreCase) || x.LastName.ToLower().Contains(pagination.Filter.ToLower(),
                    StringComparison.CurrentCultureIgnoreCase));
            }
            double Count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(Count / pagination.RecordsNumber);

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<User> GetUserAsync(string email)
        {
            var user = await _context.Users.Include(x => x.City).ThenInclude(x => x!.State)
                 .ThenInclude(x => x.Country).FirstOrDefaultAsync(x => x.Email == email);
            return user!;
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            var user = await _context.Users.Include(x => x.City).ThenInclude(x => x!.State)
                 .ThenInclude(x => x!.Country).FirstOrDefaultAsync(x => x.Id == userId.ToString());
            return user!;
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email) ??
                await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.Email);
            if(user==null)
            {
                return SignInResult.Failed;
            }
            return await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, false, true);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        
        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }
        
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }
        public async Task RevokeAllRefreshTokensAsync(User user)
        {

            var userTokens = await _context.refreshTokens.Where(rt => rt.UserId == user.Id && !rt.IsRevoked).ToListAsync();
            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<ActionResponse<bool>> RevokeRefreshTokenAsync(TokenDTO model)
        {
            var tokenEntity = await _context.refreshTokens.FirstOrDefaultAsync(rt => rt.Token == model.RefreshToken);
            if (tokenEntity == null)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "there is no token in entity"
                };
            }
            tokenEntity.IsRevoked = true;
            await _context.SaveChangesAsync();
            return new ActionResponse<bool>
            {
                WasSuccess = true,

            };
        }

        public async Task SaveRefreshTokenAsync(User user, TokenDTO model)
        {
            var expiredTokens = await _context.refreshTokens
                .Where(rt => rt.UserId == user.Id && (rt.ExpirationDate < DateTime.Now || !rt.IsRevoked)).ToListAsync();
            _context.refreshTokens.RemoveRange(expiredTokens);
            var refreshtoken = new RefreshToken
            {
                Token = model.RefreshToken!,
                UserId = user.Id,
                ExpirationDate = model.Expiration,
                CreatedDate = DateTime.Now,
                IsRevoked = false
            };
            _context.refreshTokens.Add(refreshtoken);
            await _context.SaveChangesAsync();
        }

        public async Task<ActionResponse<string?>> ValidateRefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _context.refreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken
                                    && !rt.IsRevoked
                                    && rt.ExpirationDate > DateTime.Now);
            return new ActionResponse<string?>
            {
                WasSuccess = true,
                Result = tokenEntity!.UserId
            };
        }
    }
}