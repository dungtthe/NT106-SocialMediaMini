using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Models
{
    public class FriendRequest
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long SenderId { get; set; }

        [Required]
        public long ReceiverId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [ForeignKey("SenderId")]
        public virtual AppUser Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual AppUser Receiver { get; set; }
    }
}
