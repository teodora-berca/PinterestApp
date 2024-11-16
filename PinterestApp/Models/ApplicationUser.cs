using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Eventing.Reader;
using System.ComponentModel.DataAnnotations;

namespace PinterestApp.Models
{
    //ApplicationUser mosteneste clasa de baza IdentityUser
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? User_name { get; set; }
        public string? profilePicture { get; set; }

        //un user poate publica mai multe Bookmarks

        public virtual ICollection <Bookmark>? Bookmarks { get; set; }

        //un user poate lasa mai multe cometarii

        public virtual ICollection <Comment>? Comments { get; set; }

        //un user poate avea mai multe colectii

        public virtual ICollection <Board>? Boards { get; set; }

        //un user poate lasa mai multe voturi
        public virtual ICollection<Vote>? Votes { get; set; }

    }
}
