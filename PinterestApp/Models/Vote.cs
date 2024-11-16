using System.ComponentModel.DataAnnotations;

namespace PinterestApp.Models
{
    public class Vote
    {
        [Key]
        public int Id { get; set; }
        public int Rating { get; set; }

        //un vot este lasat de un user

        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        //un vot este lasat unui bookmark

        public int? BookmarkId { get; set; }
        public virtual Bookmark? Bookmark { get; set; }
    }
}
