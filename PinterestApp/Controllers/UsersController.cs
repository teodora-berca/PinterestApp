using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PinterestApp.Data;
using PinterestApp.Models;

namespace PinterestApp.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class UsersController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(
                                    ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager
                                   )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
                ViewBag.Tip = TempData["messageIcon"].ToString();
            }

            var boards = db.Boards.Include("User").Include("SavedBookmarks.Bookmark").Include("SavedBookmarks.Bookmark.User").Where(b => b.UserId == _userManager.GetUserId(User));
            ViewBag.Boards = boards;
            var id = _userManager.GetUserId(User);
            var user = db.Users.Where(u=>u.Id == id).First();
            if (user.User_name == null)
            {
                user.User_name = _userManager.GetUserName(User);
                db.SaveChanges();
            }
            ViewBag.User = user;
            return View();
        }

        public IActionResult ViewUser(string id)
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
                ViewBag.Tip = TempData["messageIcon"].ToString();
            }
            var user = db.Users.Where(u => u.Id == id).First();
            var username = user?.User_name;
            var boards = db.Boards.Include("User").Include("SavedBookmarks.Bookmark").Include("SavedBookmarks.Bookmark.User").Where(b => b.UserId == id);
            ViewBag.Boards = boards;
            ViewBag.Username = username;
            return View();
        }

        public IActionResult Edit(string id)  //id-ul user-ului care incearca sa intre
        {
            var user = db.Users.Where(u => u.Id == id).First();
            //am preluat userul cu id-ul trimis
            if(user.Id == _userManager.GetUserId(User))  //daca e acelasi ca cel curent
            {
                return View(user);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui profil care nu va apartine";
                TempData["messageType"] = "alert-danger";
                TempData["messageIcon"] = "0";
                return RedirectToAction("Index");
            }
        }

        //se adauga profilul modificat in baza de date
        [HttpPost]
        public IActionResult Edit(string id, ApplicationUser user) //id care e deja 
                //parametru la link + informatiile primite din get
        {
            //preluam user-ul care incearca sa intre
            var user1 = db.Users.Where(u => u.Id == id).First();
            if(ModelState.IsValid)   //daca s-a trecut de verificari
            {
                 if(user1.Id == _userManager.GetUserId(User)) //daca e user-ul bun
                {
                    user1.User_name = user.User_name;
                    db.SaveChanges();
                    TempData["message"] = "Ati editat cu succes board-ul";
                    TempData["messageType"] = "alert-success";
                    TempData["messageIcon"] = "1";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui board care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    TempData["messageIcon"] = "0";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(user);  //ne intoarcem in pagina de edit cu erori
            }
        }

    }
}
