using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Models
{
    [Table("CommentHistories")]
    public class CommentHistory:BaseModel
    {
        [Required]
        public string Content { get; set; }
        public bool IsLink { get; set; }
        public DateTime CreateAt { get; set; }
        public long CommentId { get; set; }
        [ForeignKey(nameof(CommentId))]
        public Comment Comment { get; set; }
    }
}
