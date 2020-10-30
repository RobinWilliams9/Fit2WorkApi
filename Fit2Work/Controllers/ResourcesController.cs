using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Applications.Fit2Work.Controllers {
    public class ResourcesController : BaseController {

        public ResourcesController() : base() { }

        // GET: Resources
        public ActionResult Index() {
            return View(Fit2WorkDb.Resources.ToList());
        }

        // GET: Resources/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResourceModel resourceModel = Fit2WorkDb.Resources.Find(id);
            if (resourceModel == null) {
                return HttpNotFound();
            }
            return View(resourceModel);
        }

        // GET: Resources/Create
        public ActionResult Create() {
            return View();
        }

        // POST: Resources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CultureCode,EmailHeader,EmailFooter," +
            "PrimaryEmailSubject,PrimaryEmailBody," +
            "SecondaryEmailSubject,SecondaryEmailBody," +
            "UserMessageText,UserPrimaryMessageText,UserSecondaryMessageText")]
            ResourceModel resourceModel) {
            if (ModelState.IsValid) {
                Fit2WorkDb.Resources.Add(resourceModel);
                Fit2WorkDb.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(resourceModel);
        }

        // GET: Resources/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResourceModel resourceModel = Fit2WorkDb.Resources.Find(id);
            if (resourceModel == null) {
                return HttpNotFound();
            }
            return View(resourceModel);
        }

        // POST: Resources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CultureCode,EmailHeader,EmailFooter," +
            "PrimaryEmailSubject,PrimaryEmailBody," +
            "SecondaryEmailSubject,SecondaryEmailBody," +
            "UserMessageText,UserPrimaryMessageText,UserSecondaryMessageText,DownloadMessageText")]
        ResourceModel resourceModel) {
            if (ModelState.IsValid) {
                var resources = Fit2WorkDb.Resources
                    .SingleOrDefault(r => r.Id == resourceModel.Id);
                resources.EmailHeader = resourceModel.EmailHeader;
                resources.EmailFooter = resourceModel.EmailFooter;
                resources.PrimaryEmailSubject = resourceModel.PrimaryEmailSubject;
                resources.PrimaryEmailBody = resourceModel.PrimaryEmailBody;
                resources.SecondaryEmailSubject = resourceModel.SecondaryEmailSubject;
                resources.SecondaryEmailBody = resourceModel.SecondaryEmailBody;
                resources.UserMessageText = resourceModel.UserMessageText;
                resources.UserPrimaryMessageText = resourceModel.UserPrimaryMessageText;
                resources.UserSecondaryMessageText = resourceModel.UserSecondaryMessageText;
                resources.DownloadMessageText = resourceModel.DownloadMessageText;
                Fit2WorkDb.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(resourceModel);
        }

        // GET: Resources/Delete/5
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResourceModel resourceModel = Fit2WorkDb.Resources.Find(id);
            if (resourceModel == null) {
                return HttpNotFound();
            }
            return View(resourceModel);
        }

        // POST: Resources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            ResourceModel resourceModel = Fit2WorkDb.Resources.Find(id);
            Fit2WorkDb.Resources.Remove(resourceModel);
            Fit2WorkDb.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {            
            base.Dispose(disposing);
        }
    }
}
