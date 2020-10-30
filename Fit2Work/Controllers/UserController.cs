using System.Web.Mvc;

namespace AnvilGroup.Applications.Fit2Work.Controllers {
    public class UserController : BaseController {
        // GET: User
        public ActionResult Index(int pageNumber = 1, int maxPerPage = 25) {
            
            ViewBag.pageNumber = pageNumber;
            ViewBag.maxPerPage = maxPerPage;
            ViewBag.UserCount = UserService.GetUserCount();

            return View(UserService.GetUsers(pageNumber, maxPerPage));
        }
        // GET: Details
        public ActionResult Details(int id) {
            ViewBag.UserQuestionnaires = UserService.GetUserQuestionnaires(id);
            return View(UserService.GetUserById(id));
        }
    }
}