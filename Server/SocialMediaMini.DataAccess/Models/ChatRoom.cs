using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Models
{
    [Table("ChatRooms")]
    public class ChatRoom:BaseModel
    {
        public long LeaderId {  get; set; }
        [Required]
        public string UserIds {  get; set; }
        public string Name { get; set; }
        public bool IsGroupChat {  get; set; }
        public bool IsDelete {  get; set; }
        public bool CanAddMember { get; set; }
        public bool CanSendMessage { get; set;}

    }
}
