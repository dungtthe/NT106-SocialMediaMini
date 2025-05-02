using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Models
{
    [Table("PostHistories")]
    public class PostHistory:BaseModel
    {
        public string Content { get; set; }
        public string Images { get; set; }
        public DateTime CreateAt { get; set; }

        public long PostId { get; set; }
        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }

    }
}
