using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinterestApp.Models
{
    public class Bookmark
    {
        //cheia primara a entitatii bookmark
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Titlul unui bookmark este obligatoriu!")]
        [StringLength(50, ErrorMessage = "Titlul unui bookmark nu poate depasi 50 de caractere!")]
        [MinLength(3, ErrorMessage = "Titlul unui bookmark trebuie sa aiba cel putin 3 caractere!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bookmark-ul trebuie sa aiba o descriere!" )]
        [MinLength(10, ErrorMessage ="Descrierea trebuie sa contina cel putin 10 caractere!" )]
        public string Description { get; set; }

        [Required(ErrorMessage ="Bookmark-ul trebuie sa aiba continut media!")]
        public string MediaContent { get; set; }
        public DateTime Date { get; set; }

        //un bookmark este postat de un user

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }


        //un bookmark are o colectie de comentarii
        public virtual ICollection<Comment>? Comments { get; set; }

        //un bookmark are o colectie de voturi
        public virtual ICollection<Vote>? Votes { get; set; }


        //relatia many to many dintre Bookmark si Board

        public virtual ICollection<SavedBookmark>? SavedBookmarks { get; set; }


    }
}
