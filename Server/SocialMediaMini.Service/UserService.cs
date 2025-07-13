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
        Task<Result<string>> RegisterAsync(Request_RegisterDTO request);
        Task<Result<Respone_LoginDTO>> LoginAsync(Request_LoginDTO request);
        Task<Result<List<Respone_FriendSumaryDto>>> GetFriendsSummaryAsync(long userId);
        Task<Result<string>> RequestForgotPasswordAsync(Request_ForgotPasswordDto request);
        Task<Result<string>> ValidateResetPasswordTokenAsync(string token);
    }

    public class UserService : IUserService
    {
        private readonly SocialMediaMiniContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UserService(SocialMediaMiniContext dbContext, IConfiguration configuration, IEmailService emailService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = emailService;
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

        public async Task<Result<string>> RegisterAsync(Request_RegisterDTO request)
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
                return Result<string>.Failure(HttpStatusCode.Conflict, "Tên tài khoản đã tồn tại!");
            }

            var checkEmail = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (checkEmail != null)
            {
                return Result<string>.Failure(HttpStatusCode.Conflict, "Email đã tồn tại!");
            }

            var checkPhoneNumber = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
            if (checkPhoneNumber != null)
            {
                return Result<string>.Failure(HttpStatusCode.Conflict, "Số điện thoại đã tồn tại!");
            }

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return Result<string>.Success("Đăng ký thành công!");
        }

        private string GenerateJwtToken(AppUser user)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var key = _configuration["Jwt:Key"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddYears(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ResponeMessage> SendFriendRequestAsync(long senderId, long receiverId)
        {
            try
            {
                if (senderId == receiverId)
                {
                    return new ResponeMessage
                    {
                        HttpStatusCode = HttpStatusCode.BadRequest,
                        Message = "Không thể gửi yêu cầu cho chính bạn!"
                    };
                }

                var existingRequest = await _dbContext.FriendRequests
                    .FirstOrDefaultAsync(fr => (fr.SenderId == senderId && fr.ReceiverId == receiverId) ||
                                              (fr.SenderId == receiverId && fr.ReceiverId == senderId));
                if (existingRequest != null)
                {
                    return new ResponeMessage
                    {
                        HttpStatusCode = HttpStatusCode.Conflict,
                        Message = "Yêu cầu kết bạn đã tồn tại!"
                    };
                }

                var friendRequest = new FriendRequest
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _dbContext.FriendRequests.AddAsync(friendRequest);
                await _dbContext.SaveChangesAsync();

                return new ResponeMessage
                {
                    HttpStatusCode = HttpStatusCode.Ok,
                    Message = "Yêu cầu kết bạn đã được gửi!"
                };
            }
            catch (Exception ex)
            {
                return new ResponeMessage
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<ResponeMessage> AcceptFriendRequestAsync(long requestId)
        {
            try
            {
                var friendRequest = await _dbContext.FriendRequests.FindAsync(requestId);
                if (friendRequest == null || friendRequest.Status != "Pending")
                {
                    return new ResponeMessage
                    {
                        HttpStatusCode = HttpStatusCode.NotFound,
                        Message = "Yêu cầu không hợp lệ hoặc đã được xử lý!"
                    };
                }

                friendRequest.Status = "Accepted";
                friendRequest.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return new ResponeMessage
                {
                    HttpStatusCode = HttpStatusCode.Ok,
                    Message = "Yêu cầu kết bạn đã được chấp nhận!"
                };
            }
            catch (Exception ex)
            {
                return new ResponeMessage
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<ResponeMessage> RejectFriendRequestAsync(int requestId)
        {
            try
            {
                var friendRequest = await _dbContext.FriendRequests.FindAsync(requestId);
                if (friendRequest == null || friendRequest.Status != "Pending")
                {
                    return new ResponeMessage
                    {
                        HttpStatusCode = HttpStatusCode.NotFound,
                        Message = "Yêu cầu không hợp lệ hoặc đã được xử lý!"
                    };
                }

                friendRequest.Status = "Rejected";
                friendRequest.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return new ResponeMessage
                {
                    HttpStatusCode = HttpStatusCode.Ok,
                    Message = "Yêu cầu kết bạn đã bị từ chối!"
                };
            }
            catch (Exception ex)
            {
                return new ResponeMessage
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<List<Response_FriendResponseDto>> GetFriendRequestsAsync(long userId)
        {
            var requests = await _dbContext.FriendRequests
                .Where(fr => fr.ReceiverId == userId && fr.Status == "Pending")
                .Select(fr => new Response_FriendResponseDto
                {
                    Id = fr.Id,
                    Sender = new UserDto
                    {
                        Id = fr.Sender.Id,
                        UserName = fr.Sender.UserName,
                        FullName = fr.Sender.FullName,
                        Avatar = fr.Sender.GetFirstImage(),
                        Status = fr.Sender.Status
                    },
                    Receiver = new UserDto
                    {
                        Id = fr.Receiver.Id,
                        UserName = fr.Receiver.UserName,
                        FullName = fr.Receiver.FullName,
                        Avatar = fr.Receiver.GetFirstImage(),
                        Status = fr.Receiver.Status
                    },
                    Status = fr.Status,
                    CreatedAt = fr.CreatedAt
                })
                .ToListAsync();

            return requests;
        }

        public async Task<Result<List<Respone_FriendSumaryDto>>> GetFriendsSummaryAsync(long userId)
        {
            var fUser = await _dbContext.Users.FindAsync(userId);
            if (fUser == null)
            {
                return Result<List<Respone_FriendSumaryDto>>.Failure(HttpStatusCode.NotFound, "Có lỗi xảy ra. Vui lòng thử lại sau");
            }

            var result = new List<Respone_FriendSumaryDto>();

            var friendIds = fUser.GetFriendIds();
            foreach (var friendId in friendIds)
            {
                var fFriend = await _dbContext.Users.FindAsync(friendId);
                if (fFriend != null)
                {
                    result.Add(new Respone_FriendSumaryDto()
                    {
                        UserId = friendId,
                        FullName = fFriend.FullName,
                        Avatar = fFriend.GetFirstImage(),
                    });
                }
            }
            return Result<List<Respone_FriendSumaryDto>>.Success(result);

        }

        public async Task<Result<string>> RequestForgotPasswordAsync(Request_ForgotPasswordDto request)
        {
            var alreadyPending = UserServiceHelper.PasswordResetTokenToEmailMap.Any(kvp => kvp.Value.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
            if (alreadyPending)
            {
                return Result<string>.Failure(HttpStatusCode.Conflict, "Vui lòng kiểm tra Email để kích hoạt mật khẩu mới");
            }

            var fUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (fUser == null || fUser.UserName.ToLower() != request.UserName.ToLower())
            {
                return Result<string>.Failure(HttpStatusCode.NotFound, "HMM");
            }


            var linkGe = UserServiceHelper.GenerateLinkResetPassword();
            if (!UserServiceHelper.PasswordResetTokenToEmailMap.TryAdd(linkGe, fUser.Email))
            {
                return Result<string>.Failure(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi. Vui lòng thử lại!");
            }
            var issuer = _configuration["Jwt:Issuer"];
            var link = $"{issuer}/validate-reset-password/{linkGe}";
            string body = $@"
                        <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu từ bạn.</p>
                        <p>Vui lòng nhấn vào đường dẫn bên dưới để đặt lại mật khẩu:</p>
                        <p><a href=""{link}"" target=""_blank"">Đặt lại mật khẩu</a></p>
                        <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>";

            // Gửi email
            await _emailService.SendEmailAsync(request.Email, "Đặt lại mật khẩu", body);
            return Result<string>.Success("Vui lòng kiểm tra Email để kích hoạt mật khẩu mới");
        }

        public async Task<Result<string>> ValidateResetPasswordTokenAsync(string token)
        {
            if (UserServiceHelper.PasswordResetTokenToEmailMap.TryGetValue(token, out var email))
            {
                UserServiceHelper.PasswordResetTokenToEmailMap.TryRemove(token, out _);

                var fUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (fUser == null)
                {
                    return Result<string>.Failure(HttpStatusCode.NotFound, "Hệ thống đã xảy ra lỗi. Vui lòng thử lại");
                }

                var passNew = UserServiceHelper.GeneratePassword();
                fUser.Password = SecurityHelper.HashPassword(passNew);
                _dbContext.Users.Update(fUser);
                await _dbContext.SaveChangesAsync();
                return Result<string>.Success($"Mật khẩu mới của bạn là: {passNew}");
            }
            else
            {
                return Result<string>.Failure(HttpStatusCode.NotFound, "HMM");
            }
        }
    }
}