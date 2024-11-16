using System.ComponentModel.DataAnnotations;

namespace PinterestApp.Models
{
    public class Comment
    {
        //cheia primara a comentariului
        
        [Key]
        public int Id { get; set; }

        //continutul comentariului 

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu!")]

        public string Content { get; set; }

        //data la care este publicat un comentariu de catre un utilizator

        public DateTime Date { get; set; }

        //un comentariu apartine unui bookmark

        public int? BookmarkId { get; set; }

        //un comentariu este postat de un user

        public string? UserId { get; set; }

        //vrem sa avem acces si la proprietatile user-ului si ale bookmark-ului de care apartine

        public virtual ApplicationUser? User { get; set; }

        public virtual Bookmark? Bookmark { get; set; }

    }
}
