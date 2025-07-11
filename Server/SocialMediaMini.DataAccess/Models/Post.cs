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
    [Table("Posts")]
    public class Post:BaseModel
    {
        public string Content {  get; set; }
        public string Images {  get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

        public long UserId {  get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }
        public PostType PostType {  get; set; }//0 là ảnh đại diện, 1 là ảnh bìa, 2 là bài đăng thông thường
        public string ReactionType_UserId_Ids { get; set; }
        public bool IsDeleted { get; set; }
        public PostVisibilityType PostVisibilityType {  get; set; }//0 là riêng tư, 1 là bạn bè, 2 là công khai

        public Post()
        {
            ReactionType_UserId_Ids = "[]";
            Images = "[]";
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

        public List<string> GetImages()
        {
            return JsonConvert.DeserializeObject<List<string>>(Images);
        }


        public ReactionType? ReactOrUnReact(long userId, ReactionType reactionType)
        {
            var reactionAndUserIds = GetReactionAndUserIds();
            Tuple<ReactionType, long> temp = null;
            bool hasReact = false;
            foreach (var item in reactionAndUserIds)
            {
                if(item.Item2 == userId)
                {
                    temp = item;
                    break;
                }
            }
            if(temp == null)
            {
                reactionAndUserIds.Add(new Tuple<ReactionType, long>(reactionType,userId));
                hasReact = true;
            }
            else
            {
                reactionAndUserIds.Remove(temp);
                if (temp.Item1 != reactionType)
                {
                    reactionAndUserIds.Add(new Tuple<ReactionType, long>(reactionType, userId));
                    hasReact = true;
                }
            }

            //update lai
            var list = new List<string>();
            foreach (var reactionAndUserId in reactionAndUserIds)
            {
                list.Add((byte)reactionAndUserId.Item1 + "_" + reactionAndUserId.Item2);
            }
            ReactionType_UserId_Ids = JsonConvert.SerializeObject(list);
            if (hasReact)
            {
                return reactionType;
            }
            return null;
        }
    }
}
