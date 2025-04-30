using SocialMediaMini.Common.Const;
using SocialMediaMini.Common.DTOs.Request;
using SocialMediaMini.Common.DTOs.Respone;
using SocialMediaMini.Common.Helpers;
using SocialMediaMini.DataAccess.Infrastructure;
using SocialMediaMini.DataAccess.Models;
using SocialMediaMini.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Service
{
    public interface IUserService
    {
        Task<ResponeMessage> RegisterAsync(Request_RegisterDTO request);
    }
    public class UserService : IUserService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IAppUserRepository appUserRepository, IUnitOfWork unitOfWork)
        {
            _appUserRepository = appUserRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponeMessage> RegisterAsync(Request_RegisterDTO request)
        {
            var user = new AppUser
            {
                UserName = request.UserName,
                Password = Security.HashPassword(request.Password),
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
    }
}
