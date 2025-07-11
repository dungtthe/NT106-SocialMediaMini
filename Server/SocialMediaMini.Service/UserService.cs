using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SocialMediaMini.Common.Helpers;
using SocialMediaMini.Common.ResultPattern;
using SocialMediaMini.DataAccess;
using SocialMediaMini.DataAccess.Models;
using SocialMediaMini.Shared.Const;
using SocialMediaMini.Shared.Dto.Request;
using SocialMediaMini.Shared.Dto.Respone;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Service
{
    public interface IUserService
    {
        Task<ResponeMessage> RegisterAsync(Request_RegisterDTO request);
        Task<Result<Respone_LoginDTO>> LoginAsync(Request_LoginDTO request);
    }
    public class UserService : IUserService
    {
        private readonly SocialMediaMiniContext _dbContext;

        private readonly IConfiguration _configuration;
        public UserService(SocialMediaMiniContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Result<Respone_LoginDTO>> LoginAsync(Request_LoginDTO request)
        {
            var fUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);
            if (fUser == null || fUser.Password != SecurityHelper.HashPassword(request.Password))
            {
                return Result<Respone_LoginDTO>.Failure(HttpStatusCode.NotFound, "Tên tài khoản hoặc mật khẩu không chính xác!");
              
            }
            var token = GenerateJwtToken(fUser);
            var rs = Result<Respone_LoginDTO>.Success(new Respone_LoginDTO() 
            {
                UserId = fUser.Id,
                FullName = fUser.FullName,
                Image = fUser.GetFirstImage(),
                Token = token,
            });
            return rs;
        }

        public async Task<ResponeMessage> RegisterAsync(Request_RegisterDTO request)
        {
            var user = new AppUser
            {
                UserName = request.UserName,
                Password = SecurityHelper.HashPassword(request.Password),
                FullName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            };
            var checkUserName = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);
            if (checkUserName != null)
            {
                return new ResponeMessage
                {
                    HttpStatusCode = HttpStatusCode.Conflict,
                    Message = "Tên tài khoản đã tồn tại!"
                };
            }

            var checkEmail = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (checkEmail != null)
            {
                return new ResponeMessage
                {
                    HttpStatusCode = HttpStatusCode.Conflict,
                    Message = "Email đã tồn tại!"
                };
            }

            var checkPhoneNumber = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
            if (checkPhoneNumber != null)
            {
                return new ResponeMessage
                {
                    HttpStatusCode = HttpStatusCode.Conflict,
                    Message = "Số điện thoại đã tồn tại!"
                };
            }

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return new ResponeMessage
            {
                HttpStatusCode = HttpStatusCode.Ok,
                Message = "Đăng ký thành công!"
            };
        }



        private string GenerateJwtToken(AppUser user)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var key = _configuration["Jwt:Key"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Tạo danh sách claims cơ bản
            var claims = new List<Claim>
            {
            new Claim("UserId", user.Id.ToString()),
            };
            // Tạo JWT token
            var token = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddYears(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
