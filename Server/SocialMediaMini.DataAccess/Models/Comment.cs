using Newtonsoft.Json;
using SocialMediaMini.Shared.Const.Type;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Models
{
    [Table("Comments")]
    public class Comment:BaseModel
    {
        [Required]
        public string Content {  get; set; }
        public bool IsLink { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }

        public long? ParrentCommentId { get; set; }
        [ForeignKey(nameof(ParrentCommentId))]
        public virtual Comment ParentComment { set; get; }

        public long PostId { get; set; }
        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }

        public string ReactionType_UserId_Ids { get; set; }

        public bool IsRevoked { get; set; }


        public Comment()
        {
            ReactionType_UserId_Ids = "[]";
            CreateAt = DateTime.Now;
        }

        public List<Tuple<ReactionType, long>> GetReactionAndUserIds()
        {
            var results = new List<Tuple<ReactionType, long>>();
            var items = JsonConvert.DeserializeObject<List<string>>(ReactionType_UserId_Ids);
            foreach (var item in items)
            {
                var ss = item.Split('_');
                results.Add(new Tuple<ReactionType, long>((ReactionType)byte.Parse(ss[0]), long.Parse(ss[1])));
            }
            return results;
        }
    }
}
