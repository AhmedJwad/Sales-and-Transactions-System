using MailKit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Sale.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUsersUnitOfWork _usersUnitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;
        private readonly IFileStorage _fileStorage;
        private readonly DataContext _context;

        public AccountsController(IUsersUnitOfWork usersUnitOfWork , IConfiguration configuration ,
            IMailHelper mailHelper, IFileStorage fileStorage, DataContext context )
        {
           _usersUnitOfWork = usersUnitOfWork;
           _configuration = configuration;
           _mailHelper = mailHelper;
           _fileStorage = fileStorage;
           _context = context;
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAsync([FromQuery]PaginationDTO paginationDTO )
        {
            var response = await _usersUnitOfWork.GetAsync(paginationDTO);
            if(response.WasSuccess)
            {
                return Ok(response.Result);    
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO paginationDTO)
        {
            var response = await _usersUnitOfWork.GetTotalPagesAsync(paginationDTO);
            if(response.WasSuccess)
            {
                return Ok(response.Result); 
            }
            return BadRequest();
        }
        [HttpPost("RecoverPassword")]
        public async Task<IActionResult> RecoverPasswordAsync([FromBody] EmailDTO model)
        {
            var user = await _usersUnitOfWork.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }
            var myToken = await _usersUnitOfWork.GeneratePasswordResetTokenAsync(user);
            var tokenLink = Url.Action("ResetPassword", "Accounts", new
            {
                userid = user.Id,
                token = myToken
            }, HttpContext.Request.Scheme, _configuration["UrlFrontend"]);
            var response = _mailHelper.SendEmail(user.FullName, user.Email!,
                $"Orders - Password Recovery",
                $"<h1>>Orders - Password Recovery</h1>" +
                $"<p>To recover your password, please click 'Recover Password':</p>" +
                $"<b><a href ={tokenLink}>Recover Password</a></b>");
            if(response.WasSuccess)
            {
                return NoContent();
            }
            return BadRequest(response.Message);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDTO model)
        {
            var user = await _usersUnitOfWork.GetUserAsync(model.Email);
            if(user== null)
            {
                return NotFound();
            }           
            var result = await _usersUnitOfWork.ResetPasswordAsync(user, model.token, model.Password);
            if(result.Succeeded)
            {
                return NoContent();
            }           
            return BadRequest(result.Errors!.FirstOrDefault()!.Description);
        }
        [HttpPost("ResedToken")]
        public async Task<IActionResult> ResedTokenAsync([FromBody] EmailDTO model)
        {
            var user = await _usersUnitOfWork.GetUserAsync(model.Email);
            if(user==null)
            {
                return NotFound();
            }
            var response=await SendConfirmationEmailAsync(user);
            if (response.WasSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            token = token.Replace(" ", "+");            
            var user = await _usersUnitOfWork.GetUserAsync(new Guid(userId));
            if(user ==null)
            {
                return NotFound();
            }
            var result =await _usersUnitOfWork.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault());
            }

            return NoContent();
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutAsync(User user)
        {
            try
            {
                var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
                if(currentUser==null)
                {
                    return NotFound();
                }
                if(!string.IsNullOrEmpty(user.Photo))
                {
                    var photoUser =Convert.FromBase64String(user.Photo);
                    user.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", "users");
                }
                currentUser.FirstName = user.FirstName;
                currentUser.LastName= user.LastName;
                currentUser.Address = user.Address;
                currentUser.CountryCode = user.CountryCode;
                currentUser.Photo = !string.IsNullOrEmpty(user.Photo) && user.Photo != currentUser.Photo ? user.Photo : currentUser.Photo;
                currentUser.CityId = user.CityId;
                currentUser.Latitude = user.Latitude;
                currentUser.Longitude = user.Longitude; 
                var result = await _usersUnitOfWork.UpdateUserAsync(currentUser);
                if (result.Succeeded)
                {
                    //return Ok(BuildToken(currentUser));
                    var token = BuildToken(currentUser);
                    return Ok(new { token });
                }

                return BadRequest(result.Errors.FirstOrDefault());

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!));
        }
        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO model)
        {
            User user = model;            
            if(!string.IsNullOrEmpty(model.Photo))
            {
                var photoUser = Convert.FromBase64String(model.Photo);
                model.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", "users");
            }
            var result = await _usersUnitOfWork.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _usersUnitOfWork.AddUserToRoleAsync(user, user.UserType.ToString());
                var response = await SendConfirmationEmailAsync(user);
                if(response.WasSuccess)
                {
                    return NoContent();
                }
                return BadRequest(response.Message);
            }
            return BadRequest(result.Errors.FirstOrDefault());
        }
        [HttpPost("changePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _usersUnitOfWork.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()!.Description);
            }

            return NoContent();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO model)
        {
            var result = await _usersUnitOfWork.LoginAsync(model);
            if(result.Succeeded)
            {
                var user = await _usersUnitOfWork.GetUserAsync(model.Email);
                var tokenResponse = BuildToken(user);
                var tokenTDO = new TokenDTO
                {
                    Token=tokenResponse.Token,
                    RefreshTokenExpiration=tokenResponse.RefreshTokenExpiration,
                    RefreshToken=tokenResponse.RefreshToken,
                    Expiration=tokenResponse.Expiration,                    
                };
                await _usersUnitOfWork.SaveRefreshTokenAsync(user, tokenTDO);
                return Ok(tokenResponse);

            }
            if (result.IsLockedOut)
            {
                return BadRequest("You have exceeded the maximum number of attempts, your account is blocked, please try again in 5 minutes.");
            }

            if (result.IsNotAllowed)
            {
                return BadRequest("The user has not been enabled. You must follow the instructions in the email sent to enable the user.");
            }

            return BadRequest("Incorrect email or password.");
        }
        private TokenDTO BuildToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email!),
                new(ClaimTypes.Role, user.UserType.ToString()),
                new ("PhoneNumber", user.PhoneNumber!),
                new("CountryCode", user.CountryCode),
                new("FirstName", user.FirstName),
                new("LastName", user.LastName),
                new("Address", user.Address),
                new("Photo", user.Photo ?? string.Empty),
                new("CityId", user.CityId.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
            var credentials=new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration =DateTime.Now.AddHours(8);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.Now.AddDays(90);
            var token = new JwtSecurityToken
            (
                issuer:null,
                audience:null,
                claims:claims,                
                expires: expiration,
                signingCredentials: credentials
            );
            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiration
            };

        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<ActionResponse<string>> SendConfirmationEmailAsync(User user)
        {
            var myToken=await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = Url.Action("ConfirmEmail", "accounts", new
            {
                userid = user.Id,
                token = myToken
            }, HttpContext.Request.Scheme, _configuration["UrlFrontend"]);

            return _mailHelper.SendEmail(user.FullName, user.Email!,
                $"Orders - Account Confirmation",
                $"<h1>Orders - Account Confirmation</h1>" +
                $"<p>To activate the user, please click 'Confirm Email':</p>" +
                $"<b><a href ={tokenLink}>Confirm Email</a></b>");
        }

        [HttpPost("RefreshToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody]TokenDTO model)
        {
            if (string.IsNullOrEmpty(model.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }
            
            var userId = await _usersUnitOfWork.ValidateRefreshTokenAsync(model.RefreshToken);
            if (string.IsNullOrEmpty(userId.Result))
            {
                return BadRequest("Invalid or expired refresh token.");
            }          

            var user = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound("User not found.");
            }
           
            var tokenResponse = BuildToken(user);
            var refreshTokenDto = new TokenDTO
            {
                RefreshToken = tokenResponse.RefreshToken!,
                Expiration = tokenResponse.RefreshTokenExpiration,
                Token=tokenResponse.Token,
                RefreshTokenExpiration=tokenResponse.RefreshTokenExpiration,               
               
            };            
           
            await _usersUnitOfWork.SaveRefreshTokenAsync(user, refreshTokenDto);
            await _usersUnitOfWork.RevokeRefreshTokenAsync(refreshTokenDto);
            return Ok(tokenResponse);
        }
        [HttpPost("RevokeToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RevokeTokenAsync([FromBody] TokenDTO model)
        {
            if (string.IsNullOrEmpty(model.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }
            var refreshtoken = new TokenDTO
            {
                RefreshToken = model.RefreshToken,
            };
            var result = await _usersUnitOfWork.RevokeRefreshTokenAsync(refreshtoken);
            if (!result.WasSuccess)
            {
                return BadRequest("Invalid refresh token.");
            }

            return NoContent();
        }
        [HttpPost("Logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> LogoutAsync([FromBody] TokenDTO model)
        {
            var user = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }
            var userId = new User
            {
                Id=user.Id,
                FirstName=user.FirstName,
                LastName=user.LastName,
                Address=user.Address,
                CityId=user.CityId,
                CountryCode=user.CountryCode,
                Email=user.Email,
                UserName=user.Email,
                Latitude=user.Latitude,
                Longitude=user.Longitude,
                PhoneNumber=user.PhoneNumber,
                Photo=user.Photo
            };
            await _usersUnitOfWork.RevokeAllRefreshTokensAsync(userId);
            
            if (!string.IsNullOrEmpty(model.RefreshToken))
            {
                var refreshtoken = new TokenDTO
                {
                    RefreshToken = model.RefreshToken
                };
                await _usersUnitOfWork.RevokeRefreshTokenAsync(refreshtoken);
            }

            return NoContent();
        }

    }
}
