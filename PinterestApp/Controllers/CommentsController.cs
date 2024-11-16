using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PinterestApp.Data;
using PinterestApp.Models;

namespace PinterestApp.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(
                                    ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager
                                    )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpPost]
        [Authorize(Roles ="User,Admin")]
        public IActionResult Delete(int id)
        {
            Comment com = db.Comments.Find(id);
            if((_userManager.GetUserId(User) == com.UserId)||User.IsInRole("Admin"))
            {
                db.Comments.Remove(com);
                db.SaveChanges();
                TempData["message"] = "Comentariul a fost sters cu succes";
                TempData["messageType"] = "alert-success";
                TempData["messageIcon"] = "1";
                return Redirect("/Bookmarks/Show/" + com.BookmarkId);
            }
            else
            {
                TempData["message"] = "Nu puteti sterge acest comentariu";
                TempData["messageType"] = "alert-danger";
                TempData["messageIcon"] = "0";
                return Redirect("/Bookmarks/Show/" + com.BookmarkId);
            }
            
        }
        [Authorize(Roles ="User")]
        public IActionResult Edit(int id)
        {
            Comment com = db.Comments.Find(id);
            if(_userManager.GetUserId(User) == com.UserId)
            {
                return View(com);
            }
            else
            {
                TempData["message"] = "Nu puteti edita acest comentariu";
                TempData["messageType"] = "alert-danger";
                TempData["messageIcon"] = "0";

                return Redirect("/Bookmarks/Show/" + com.BookmarkId);
            }
            
        }
        [HttpPost]
        [Authorize(Roles ="User")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment com = db.Comments.Find(id);
            if (ModelState.IsValid)
            {
                
                if(_userManager.GetUserId(User)==com.UserId)
                {
                    com.Content = requestComment.Content;

                    db.SaveChanges();

                    TempData["message"] = "Comentariul a fost editat cu succes";
                    TempData["messageType"] = "alert-success";
                    TempData["messageIcon"] = "1";

                    return Redirect("/Bookmarks/Show/" + com.BookmarkId);
                }
                else
                {
                    TempData["message"] = "Nu puteti edita acest comentaru";
                    TempData["messageType"] = "alert-danger";
                    TempData["messageIcon"] = "0";

                    return Redirect("/Bookmarks/Show/" + com.BookmarkId);
                }
                
            }
            else
            {
                return View(requestComment);
            }
        }
    }
}
