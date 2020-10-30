using System;
using System.Web.Mvc;

namespace AnvilGroup.Applications.Fit2Work.Controllers {
    public class LogController : BaseController {

        // GET: Log
        public ActionResult Index(DateTime? date = null, int pageNumber = 1, int maxPerPage = 25) {

            if (!date.HasValue) {
                date = DateTime.UtcNow;
            }

            ViewBag.Date = date;
            ViewBag.pageNumber = pageNumber;
            ViewBag.maxPerPage = maxPerPage;
            ViewBag.LogCount = LoggingService.GetLogCount(date.Value);

            return View(LoggingService.GetLogs(date.Value, pageNumber, maxPerPage));
        }

        // GET: Details
        public ActionResult Details(int id) {
            return View(LoggingService.GetLogById(id));
        }
    }
}