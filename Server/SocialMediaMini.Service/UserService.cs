using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SocialMediaMini.Common.Helpers;
using SocialMediaMini.DataAccess.Infrastructure;
using SocialMediaMini.DataAccess.Models;
using SocialMediaMini.DataAccess.Repositories;
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
        Task<Respone_LoginDTO> LoginAsync(Request_LoginDTO request);
    }
    public class UserService : IUserService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public UserService(IAppUserRepository appUserRepository, IUnitOfWork unitOfWork,IConfiguration configuration)
        {
            _appUserRepository = appUserRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<Respone_LoginDTO> LoginAsync(Request_LoginDTO request)
        {
            var fUser = (await _appUserRepository.FindAsync(x => x.UserName == request.UserName && x.Password == SecurityHelper.HashPassword(request.Password))).FirstOrDefault();
            if (fUser == null)
            {
                return new Respone_LoginDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Tên tài khoản hoặc mật khẩu không chính xác!"
                };
            }
            var token = GenerateJwtToken(fUser);
            string[] imgs = fUser.Images != null
            ? JsonConvert.DeserializeObject<string[]>(fUser.Images)
            : new string[] { "no_img_user.png" };
            var rsp = new Respone_LoginDTO
            {
                UserId = fUser.Id,
                HttpStatusCode = HttpStatusCode.Ok,
                FullName = fUser.FullName,
                Image = imgs[0],
                Token = token,
            };
            return rsp;
        }

        public async Task<ResponeMessage> RegisterAsync(Request_RegisterDTO request)
        {
            var user = new AppUser
            {
                UserName = request.UserName,
                Password = SecurityHelper.HashPassword(request.Password),
                FullName= request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            };
            var checkUserName = await _appUserRepository.FindAsync(x => x.UserName == request.UserName);
            if(checkUserName.Any())
            {
                return new ResponeMessage
                {
                    HttpStatusCode = HttpStatusCode.Conflict,
                    Message = "Tên tài khoản đã tồn tại!"
                };
            }

            var checkEmail = await _appUserRepository.FindAsync(x => x.Email == request.Email);
            if (checkEmail.Any())
            {
                return new ResponeMessage
                {
                    HttpStatusCode = HttpStatusCode.Conflict,
                    Message = "Email đã tồn tại!"
                };
            }

            var checkPhoneNumber = await _appUserRepository.FindAsync(x => x.PhoneNumber == request.PhoneNumber);
            if (checkPhoneNumber.Any())
            {
                return new ResponeMessage
                {
                    HttpStatusCode = HttpStatusCode.Conflict,
                    Message = "Số điện thoại đã tồn tại!"
                };
            }

            await _appUserRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();
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
