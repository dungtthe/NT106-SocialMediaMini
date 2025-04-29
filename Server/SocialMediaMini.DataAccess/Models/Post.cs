using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Models
{
    [Table("Posts")]
    public class Post:BaseModel
    {
        public string Content {  get; set; }
        public string Images {  get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

        public long UserId {  get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }
        public byte PostType {  get; set; }//0 là ảnh đại diện, 1 là ảnh bìa, 2 là bài đăng thông thường
        public byte PostStatus { get; set; }
        public string ReactionType_UserId_Ids { get; set; }
        public bool IsDeleted { get; set; }
        public byte PostVisibility {  get; set; }//0 là riêng tư, 1 là bạn bè, 2 là công khai
    }
}
