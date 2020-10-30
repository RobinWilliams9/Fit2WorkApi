using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Applications.Fit2Work.Controllers {
    public class ResourceUrlsController : BaseController {
        // GET: ResourceUrls
        public ActionResult Index() {
            return View(Fit2WorkDb.ResourceUrls.ToList());
        }

        // GET: ResourceUrls/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResourceUrlModel resourceUrlModel = Fit2WorkDb.ResourceUrls.Find(id);
            if (resourceUrlModel == null) {
                return HttpNotFound();
            }
            return View(resourceUrlModel);
        }

        // GET: ResourceUrls/Create
        public ActionResult Create() {
            return View();
        }

        // POST: ResourceUrls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CultureCode,Name,Url")] ResourceUrlModel resourceUrlModel) {
            if (ModelState.IsValid) {
                Fit2WorkDb.ResourceUrls.Add(resourceUrlModel);
                Fit2WorkDb.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(resourceUrlModel);
        }

        // GET: ResourceUrls/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResourceUrlModel resourceUrlModel = Fit2WorkDb.ResourceUrls.Find(id);
            if (resourceUrlModel == null) {
                return HttpNotFound();
            }
            return View(resourceUrlModel);
        }

        // POST: ResourceUrls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CultureCode,Name,Url")] ResourceUrlModel resourceUrlModel) {
            if (ModelState.IsValid) {
                var resourceUrl = Fit2WorkDb.ResourceUrls
                    .SingleOrDefault(r => r.Id == resourceUrlModel.Id);
                resourceUrl.Name = resourceUrlModel.Name;
                resourceUrl.Url = resourceUrlModel.Url;
                Fit2WorkDb.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(resourceUrlModel);
        }

        // GET: ResourceUrls/Delete/5
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResourceUrlModel resourceUrlModel = Fit2WorkDb.ResourceUrls.Find(id);
            if (resourceUrlModel == null) {
                return HttpNotFound();
            }
            return View(resourceUrlModel);
        }

        // POST: ResourceUrls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            ResourceUrlModel resourceUrlModel = Fit2WorkDb.ResourceUrls.Find(id);
            Fit2WorkDb.ResourceUrls.Remove(resourceUrlModel);
            Fit2WorkDb.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }
    }
}
