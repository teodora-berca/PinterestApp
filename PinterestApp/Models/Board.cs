using System.ComponentModel.DataAnnotations;

namespace PinterestApp.Models
{
    public class Board
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Un board trebuie sa aiba un titlu!")]
        public string Title { get; set; }

        //Un board apartine de un user
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        //Un board va contine mai multe bookmark-uri
        public virtual ICollection<SavedBookmark>? SavedBookmarks { get; set; }

    }
}
