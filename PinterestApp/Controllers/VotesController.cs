using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PinterestApp.Data;
using PinterestApp.Models;

namespace PinterestApp.Controllers
{
    public class VotesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public VotesController(
                                    ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager
                                    )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult New(Vote vote)
        {
            vote.UserId = _userManager.GetUserId(User);
            db.Votes.Add(vote);
            db.SaveChanges();
            return Redirect("/Bookmarks/Show/" + vote.BookmarkId);
        }
        [HttpPost]
        [Authorize(Roles="User")]
        public IActionResult Delete(int id)
        {
            Vote vote = db.Votes.Find(id);
            if(vote.UserId == _userManager.GetUserId(User))
            {
                db.Votes.Remove(vote);
                db.SaveChanges();
                return Redirect("/Bookmarks/Show/" + vote.BookmarkId);
            }
            else
            {
                return Redirect("/Bookmarks/Show/" + vote.BookmarkId);
            }
        }
    }
}
