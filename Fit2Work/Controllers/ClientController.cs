using System;
using System.Web.Mvc;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Applications.Fit2Work.Controllers {
    public class ClientController : BaseController {
        public ClientController() : base() { }
        // GET: Client
        public ActionResult Index() {
            return View();
        }

        // GET: Client/Details/5
        public ActionResult Details(int id) {
            var client = ClientService.GetClientById(id);
            SetClientViewBag(id);
            return View(client);
        }

        // GET: Client/Create
        public ActionResult Create() {
            return View(new ClientModel());
        }

        // POST: Client/Create
        [HttpPost]
        public ActionResult Create(ClientModel client) {
            if (!ModelState.IsValid)
            {
                return View(client);
            }
            if (!ClientService.IsMemberCodeUnique(client))
            {
                ModelState.AddModelError("MemberCode", $"Member code {client.MemberCode} is not unique.");
                return View(client);
            }
            try
            {
                ClientService.CreateClient(client);
                return RedirectToAction("Details", new { id = client.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error:  {ex.Message} is not unique.");
                return View(client);
            }

            return View();
        }

        // GET: Client/Edit/5
        public ActionResult Edit(int id) {
            return View(ClientService.GetClientById(id));
        }

        // POST: Client/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ClientModel client) {
            if (!ModelState.IsValid) {
                return View(client);
            }
            if (!ClientService.IsMemberCodeUnique(client)) {
                ModelState.AddModelError("MemberCode", $"Member code {client.MemberCode} is not unique.");
                return View(client);
            }
            try {
                ClientService.UpdateClient(id, client);
                client.Users = UserService.GetUsersByClientId(client.Id);
                SetClientViewBag(id);
                return View("Details", client);
            } catch (Exception ex) {
                ModelState.AddModelError(string.Empty, $"Error:  {ex.Message}.");
                return View(client);
            }
        }

        private void SetClientViewBag(int clientId) {
            ViewBag.RegiseredUserCount = UserService.RegisteredUserCountByClientId(clientId);
            ViewBag.QuestionnareDailyCount = UserService.DailyQuestionnaireCountByClientId(clientId);
            ViewBag.QuestionnareTotalCount = UserService.TotalQuestionnaireCountByClientId(clientId);
            ViewBag.AllowSelectUsers = true;
        }
    }
}
