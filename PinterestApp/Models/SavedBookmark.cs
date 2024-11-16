using System.ComponentModel.DataAnnotations.Schema;

namespace PinterestApp.Models
{
    public class SavedBookmark
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? BookmarkId { get; set; }

        public virtual Bookmark? Bookmark { get; set; }

        public int? BoardId { get; set; } 

        public virtual Board? Board { get; set;}

        public DateTime SavingDate { get; set; }


    }
}
