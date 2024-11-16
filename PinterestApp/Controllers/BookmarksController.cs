using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PinterestApp.Data;
using PinterestApp.Models;

namespace PinterestApp.Controllers
{
    public class BookmarksController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public BookmarksController(
                                    ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager
                                   )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        //Si utilizatorii neinregistrati pot vizualiza bookmark-urile
        public IActionResult Index()
        {
            int _perPage = 5;
            var bookmarks = db.Bookmarks.Include("User").OrderByDescending(a => a.Votes.Count).ThenByDescending(a=>a.Date);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
                ViewBag.Tip = TempData["messageIcon"].ToString();
            }
            

            var search = "";
            // motor de cautare
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
               
                //cautam bookmark-urile care contin string-ul
                List<int> bookmarkIds = db.Bookmarks.Where
                     (bm => bm.Title.Contains(search) || bm.Description.Contains(search)).Select(b => b.Id).ToList();
                bookmarks = db.Bookmarks.Where(bookmark => bookmarkIds.Contains(bookmark.Id))
                .Include("User")
                .OrderBy(b=>b.Title);
            }
            ViewBag.SearchString = search;


            int totalItems = bookmarks.Count();
            var currentPage =
            Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedBookmarks =
            bookmarks.Skip(offset).Take(_perPage);
            ViewBag.lastPage = Math.Ceiling((float)totalItems /
            (float)_perPage);
            ViewBag.Bookmarks = paginatedBookmarks;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Bookmarks/Index/?search="
                + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Bookmarks/Index/?page";
            }



            return View();
        }

        //Show - oricine poate vedea bookmark-urile in detaliu
        public IActionResult Show(int id)
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
                ViewBag.Tip = TempData["messageIcon"].ToString();
            }
            Bookmark bookmark = db.Bookmarks.Include("Comments").Include("User").Include("Comments.User").Include("Votes").Include("Votes.User")
                               .Where(bm => bm.Id == id)
                               .First();
            var boards = db.Boards.Where(brd => brd.UserId == _userManager.GetUserId(User)).ToList();
            ViewBag.Boards = boards;
            SetAccessRights();
            UserVote(id);
            NumberOfVotes(bookmark.Id);
            return View(bookmark);
        }

        [HttpPost]
        [Authorize(Roles ="User")]
        public IActionResult Show([FromForm] Comment com)
        {
            com.Date = DateTime.Now;
            com.UserId = _userManager.GetUserId(User);
            if(ModelState.IsValid)
            {
                db.Comments.Add(com);
                db.SaveChanges();
                return Redirect("/Bookmarks/Show/" + com.BookmarkId);
            }
            else
            {
                Bookmark bookmark = db.Bookmarks.Include("User").Include("Comments").Include("Comments.User").Include("Votes").Include("Votes.User")
                                                .Where(bookmark => bookmark.Id == com.BookmarkId)
                                                .First();
                var boards = db.Boards.Where(brd => brd.UserId == _userManager.GetUserId(User)).ToList();
                ViewBag.Boards = boards;
                SetAccessRights();
                UserVote(bookmark.Id);
                NumberOfVotes(bookmark.Id);
                return View(bookmark);
            }
        }


        //New - doar utilizatorii inregistrati pot incarca bookmark-uri
        [Authorize(Roles = "User")]
        public IActionResult New()
        {
            return View();
        }

        // Se adauga bookmark-ul in baza de date
        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult New(Bookmark bookmark)
        {
            //var sanitizer = new HtmlSanitizer();   //pentru editorul de text
            bookmark.UserId = _userManager.GetUserId(User);
            bookmark.Date = DateTime.Now;
            if(ModelState.IsValid)
            {
                //bookmark.MediaContent = sanitizer.Sanitize(bookmark.MediaContent);  //pentru editorul de text
                db.Bookmarks.Add(bookmark);
                db.SaveChanges();
                TempData["message"] = "Ati adaugat cu succes bookmark-ul!";
                TempData["messageType"] = "alert-success";
                TempData["messageIcon"] = "1";
                return RedirectToAction("Index");
            }
            else
            {
                return View(bookmark);
            }
        }



        //Edit - doar utilizatorii inregistrati pot edita bookmark-uri
        [Authorize(Roles = "User")]
        public IActionResult Edit(int id)
        {
            Bookmark bookmark = db.Bookmarks.Where(bm => bm.Id == id)
                                            .First();
            if(bookmark.UserId == _userManager.GetUserId(User))
            {
                return View(bookmark);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui bookmark care nu va apartine";
                TempData["messageType"] = "alert-danger";
                TempData["messageIcon"] = "0";
                return RedirectToAction("Index");
            }
        }


        [Authorize(Roles = "User")]
        // Se adauga bookmark-ul modificat in baza de date
        [HttpPost]
        public IActionResult Edit(int id, Bookmark bm)
        {
            //var sanitizer = new HtmlSanitizer();  //pentru editorul de text
            Bookmark bookmark = db.Bookmarks.Find(id);
            if (ModelState.IsValid)
            {
                if (bookmark.UserId == _userManager.GetUserId(User))
                {

                    bookmark.Title = bm.Title;
                    bookmark.Description = bm.Description;
                    //bm.MediaContent = sanitizer.Sanitize(bm.MediaContent);  //pentru editorul de text
                    bookmark.MediaContent = bm.MediaContent;
                    db.SaveChanges();
                    TempData["message"] = "Ati editat cu succes bookmark-ul";
                    TempData["messageType"] = "alert-success";
                    TempData["messageIcon"] = "1";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui bookmark care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    TempData["messageIcon"] = "0";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(bm);
            }
        }


        // Se sterge un bookmark din baza de date 
        [HttpPost]
        [Authorize(Roles ="User,Admin")]
        public ActionResult Delete(int id)
        {
            Bookmark bookmark = db.Bookmarks.Include("Comments")
                                            .Include("Votes")
                                            .Where(b=>b.Id==id)
                                            .First();
            if((bookmark.UserId==_userManager.GetUserId(User))||User.IsInRole("Admin"))
            {
                db.Bookmarks.Remove(bookmark);
                db.SaveChanges();
                TempData["message"] = "Ati sters cu succes bookmark-ul";
                TempData["messageType"] = "alert-success";
                TempData["messageIcon"] = "1";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti acest bookmark";
                TempData["messageType"] = "alert-danger";
                TempData["messageIcon"] = "0";
                return RedirectToAction("Index");
            }
            
        }
        [HttpPost]
        public IActionResult AddToBoard([FromForm] SavedBookmark savedBookmark)
        {
            if(ModelState.IsValid)
            {
                if(db.SavedBookmarks.Where(sb => sb.BookmarkId == savedBookmark.BookmarkId).Where(sb => sb.BoardId == savedBookmark.BoardId).Count()>0)
                {
                    TempData["message"] = "Ati adaugat deja acest bookmark in board-ul ales";
                    TempData["messageType"] = "alert-danger";
                    TempData["messageIcon"] = "0";
                    return Redirect("/Bookmarks/Show/"+savedBookmark.BookmarkId);
                }
                else
                {
                    savedBookmark.SavingDate = DateTime.Now;
                    db.SavedBookmarks.Add(savedBookmark);
                    db.SaveChanges();
                    TempData["message"] = "Ati salvat bookmark-ul in board-ul ales!";
                    TempData["messageType"] = "alert-success";
                    TempData["messageIcon"] = "1";
                    return Redirect("/Bookmarks/Show/" + savedBookmark.BookmarkId);
                }
            }
            else
            {
                TempData["message"] = "Nu s-a putut adauga bookmark-ul in board-ul ales";
                TempData["messageType"] = "alert-danger";
                TempData["messageIcon"] = "0";
                return Redirect("/Bookmarks/Show/" + savedBookmark.BookmarkId);

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

        private void UserVote(int idBookmark)
        {
            Bookmark bookmark = db.Bookmarks.Find(idBookmark);
            ViewBag.Vote = false;
            if(bookmark.Votes.Count>0)
            {
                foreach (Vote vote in bookmark.Votes)
                {
                    if (vote.UserId == _userManager.GetUserId(User))
                    {
                        ViewBag.Vote = true;
                        ViewBag.VoteId = vote.Id;
                        break;
                    }
                }
            }

        }
        private void NumberOfVotes(int idBookmark)
        {
            Bookmark bookmark = db.Bookmarks.Find(idBookmark);
            ViewBag.NumberOfVotes = 0;
            if(bookmark.Votes.Count>0)
            {
                foreach (Vote vote in bookmark.Votes)
                {
                    ViewBag.NumberOfVotes = ViewBag.NumberOfVotes + 1;
                }
            }
        }
    }
}
