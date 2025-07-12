using Newtonsoft.Json;
using SocialMediaMini.Shared.Const.Type;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Models
{
    [Table("Messages")]
    public class Message:BaseModel
    {
        public string Content { get; set; }
        public DateTime CreateAt { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }

        public long? ParrentMessageId { get; set; }
        [ForeignKey(nameof(ParrentMessageId))]
        public virtual Message ParentMessage { set; get; }

        public long ChatRoomId { get; set; }
        [ForeignKey(nameof(ChatRoomId))]
        public virtual ChatRoom ChatRoom { get; set; }
        public string ReactionType_UserId_Ids { get; set; }
        public string ReadByUserIds { get; set; }
        public MessageType MessageType { get; set; }

        public Message()
        {
            ReactionType_UserId_Ids = "[]";
            ReadByUserIds = "[]";
        }

        public List<Tuple<ReactionType,long>> GetReactionAndUserIds()
        {
            var results = new List<Tuple<ReactionType,long>>();
            var items =  JsonConvert.DeserializeObject<List<string>>(ReactionType_UserId_Ids);
            foreach(var item in items)
            {
                var ss = item.Split('_');
                results.Add(new Tuple<ReactionType, long>((ReactionType)byte.Parse(ss[0]), long.Parse(ss[1])));
            }
            return results;
        }

        public List<long> GetUserIdsRead()
        {
            return JsonConvert.DeserializeObject<List<long>>(ReadByUserIds);
        }

        public bool AddUserIdRead(long userId)
        {
            var userIds = JsonConvert.DeserializeObject<List<long>>(ReadByUserIds);
            if (!userIds.Contains(userId))
            {
                userIds.Add(userId);
                ReadByUserIds = JsonConvert.SerializeObject(userIds);
                return true;
            }
            return false;
        }
    }
}
