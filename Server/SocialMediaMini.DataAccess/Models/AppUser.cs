using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Models
{
    [Table("Users")]
    public class AppUser : BaseModel
    {
        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(300)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(300)]
        public string Email { get; set; }

        [Required]
        [Phone]
        [MaxLength(30)]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Images { get; set; }
        public string FriendIds { get; set; }
        public string BlockIds { get; set; }

        [MaxLength(500)]
        public string EncryptionPublicKey { get; set; }

        [MaxLength(500)] // Độ dài đủ cho Base64 của IV (16 byte)
        public string IV { get; set; }

        public AppUser()
        {
            FriendIds = "[]";
            BlockIds = "[]";
        }

        public string GetFirstImage()
        {
            return Images != null ? JsonConvert.DeserializeObject<string[]>(Images)[0] : "no_img_user.png";
        }

        public List<long> GetFriendIds()
        {
            return JsonConvert.DeserializeObject<List<long>>(FriendIds) ?? new List<long>();
        }

        public List<long> GetBlockIds()
        {
            return JsonConvert.DeserializeObject<List<long>>(BlockIds) ?? new List<long>();
        }
    }
}