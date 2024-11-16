using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PinterestApp.Data;
using PinterestApp.Models;

namespace PinterestApp.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class BoardsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public BoardsController(
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
            var boards = db.Boards.Include("User").Include("SavedBookmarks.Bookmark").Where(b => b.UserId == _userManager.GetUserId(User));
            ViewBag.Boards = boards;
            return View();
        }
        [HttpPost]
        public IActionResult DeleteBookmark (int id)
        {
            SavedBookmark savedBookmark = db.SavedBookmarks.Include("Board").Where(sb => sb.Id == id).First();
            if(savedBookmark.Board.UserId == _userManager.GetUserId(User))
            {
                db.SavedBookmarks.Remove(savedBookmark);
                db.SaveChanges();
                return Redirect("/Boards/Show/" + savedBookmark.BoardId);
            }
            else
            {
                TempData["message"] = "Nu puteti sterge acest SavedBookmark";
                TempData["messageType"] = "alert-danger";
                TempData["messageIcon"] = "0";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Show(int id)
        {
            Board board = db.Boards.Include("SavedBookmarks.Bookmark")
                                   .Include("SavedBookmarks.Bookmark.User")
                                   .Where(b => b.Id == id)
                                   .Where(b => b.UserId == _userManager.GetUserId(User))
                                   .First();
            return View(board);
        }
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }
        [HttpPost]
        public IActionResult New(Board board)
        {
            board.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                db.Boards.Add(board);
                db.SaveChanges();
                TempData["message"] = "Ati creat un nou Board!";
                TempData["messageType"] = "alert-success";
                TempData["messageIcon"] = "1";
                return RedirectToAction("Index");
            }
            else
            {
                return View(board);
            }
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Board board = db.Boards.Where(b => b.Id == id).First();
            //daca board-ul cu id-ul transmis este al userului curent putem face edit
            if (board.UserId == _userManager.GetUserId(User))
            {
                return View(board);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui board care nu va apartine";
                TempData["messageType"] = "alert-danger";
                TempData["messageIcon"] = "0";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Board brd)
        {
            Board board = db.Boards.Find(id);
            if (ModelState.IsValid)
            {
                if (board.UserId == _userManager.GetUserId(User))
                {
                    board.Title = brd.Title;
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
                return View(brd);
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Board board = db.Boards.Include("SavedBookmarks").Where(b => b.Id == id).First();
            if(board.UserId == _userManager.GetUserId(User))
            {
                db.Remove(board);
                db.SaveChanges();
                TempData["message"] = "Ati sters cu succes board-ul";
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

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}
