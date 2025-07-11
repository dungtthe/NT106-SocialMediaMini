using Client.Const;
using Client.Helpers;
using Client.LocalStorage;
using Newtonsoft.Json;
using SocialMediaMini.Shared.Const;
using SocialMediaMini.Shared.Dto.Request;
using SocialMediaMini.Shared.Dto.Respone;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Client.ViewModels.Chats.ConversationViewModel;

namespace Client.Services
{
    public static class UserService
    {
        public static async Task<bool> LoginAsync(string userName, string password)
        {
            UserStore.Reset();
            var data = new Request_LoginDTO()
            {
                UserName = userName,
                Password = password
            };

            try
            {
                var response = await ApiHelpers.PostAsync(new ApiRequest("/api/user/login", JsonConvert.SerializeObject(data), false));
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var rspData = JsonConvert.DeserializeObject<Respone_LoginDTO>(response.ResponseBody);
                    if (rspData != null)
                    {
                        UserStore.UserIdCur = rspData.UserId;
                        UserStore.Avatar = rspData.Image;
                        UserStore.FullName = rspData.FullName;
                        UserStore.Token = rspData.Token;

                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public static async Task<bool> RegisterAsync(string userName, string password, string email, string phoneNumber)
        {
            UserStore.Reset();
            var data = new Request_RegisterDTO()
            {
                UserName = userName,
                Password = password,
                Email = email,
                PhoneNumber = phoneNumber

            };

            try
            {
                // Tạo cặp khóa ECC P-256 và IV ở client
                using (var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
                {
                    var privateKey = ecdsa.ExportECPrivateKey();
                    var publicKey = ecdsa.ExportSubjectPublicKeyInfo();

                    string privateKeyBase64 = Convert.ToBase64String(privateKey);
                    string publicKeyBase64 = Convert.ToBase64String(publicKey);

                    // Tạo IV ngẫu nhiên (16 byte cho AES-GCM)
                    byte[] iv = new byte[16];
                    using (var rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(iv);
                    }
                    string ivBase64 = Convert.ToBase64String(iv);

                    // Mã hóa và lưu khóa bí mật cùng IV bằng DPAPI
                    string storagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SocialMediaMini");
                    Directory.CreateDirectory(storagePath);
                    string privateKeyFile = Path.Combine(storagePath, "privateKey.enc");
                    string ivFile = Path.Combine(storagePath, "iv.enc");

                    byte[] protectedPrivateKey = ProtectedData.Protect(Encoding.UTF8.GetBytes(privateKeyBase64), null, DataProtectionScope.CurrentUser);
                    byte[] protectedIV = ProtectedData.Protect(Encoding.UTF8.GetBytes(ivBase64), null, DataProtectionScope.CurrentUser);

                    File.WriteAllBytes(privateKeyFile, protectedPrivateKey);
                    File.WriteAllBytes(ivFile, protectedIV);

                    // Gửi khóa công khai và IV lên server
                    var registerData = new
                    {
                        UserName = userName,
                        Password = password,
                        Email = email,
                        PhoneNumber = phoneNumber,
                        EncryptionPublicKey = publicKeyBase64,
                        IV = ivBase64
                    };

                    var response = await ApiHelpers.PostAsync(new ApiRequest("/api/user/register", JsonConvert.SerializeObject(registerData), false));
                    if (response.StatusCode == HttpStatusCode.Ok)
                    {
                        var rspData = JsonConvert.DeserializeObject<Respone_RegisterDTO>(response.ResponseBody);
                        if (rspData != null && rspData.HttpStatusCode == HttpStatusCode.Ok)
                        {
                            // Cập nhật thông tin người dùng
                            UserStore.UserIdCur = rspData.UserId;
                            UserStore.Avatar = rspData.Image ?? "no_img_user.png";
                            UserStore.FullName = rspData.FullName ?? userName;
                            UserStore.Token = rspData.Token ?? "";

                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi đăng ký", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

        // Phương thức để đọc khóa bí mật và IV (nếu cần sau này)
        public static string GetPrivateKey(long userId)
        {
            string storagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SocialMediaMini");
            string privateKeyFile = Path.Combine(storagePath, "privateKey.enc");
            if (File.Exists(privateKeyFile))
            {
                byte[] protectedData = File.ReadAllBytes(privateKeyFile);
                byte[] decryptedData = ProtectedData.Unprotect(protectedData, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedData);
            }
            return null;
        }

        public static string GetIV(long userId)
        {
            string storagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SocialMediaMini");
            string ivFile = Path.Combine(storagePath, "iv.enc");
            if (File.Exists(ivFile))
            {
                byte[] protectedData = File.ReadAllBytes(ivFile);
                byte[] decryptedData = ProtectedData.Unprotect(protectedData, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedData);
            }
            return null;
        }
    }
}