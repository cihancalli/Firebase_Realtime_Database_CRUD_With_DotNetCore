using FirebaseRealtimeDatabaseCRUD.Models;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirebaseRealtimeDatabaseCRUD.Controllers
{
    public class NotesController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "Project settings -> Service accounts -> Database Secrets -> AuthSecret",
            BasePath = "Realtime Database -> Data -> BasePath"
        };
        IFirebaseClient client;
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("notes");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Notes>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Notes>(((JProperty)item).Value.ToString()));
            }
            return View(list);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Notes note)
        {
            try
            {
                AddNotesToFirebase(note);
                ModelState.AddModelError(String.Empty, "Added Succesfuly");
            }
            catch (Exception ex)
            {
                throw;
                ModelState.AddModelError(String.Empty, ex.Message);
            }

            return RedirectToAction("Index");
        }

        private void AddNotesToFirebase(Notes note)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = note;
            PushResponse response = client.Push("notes/", data);
            data.note_id = response.Result.name;
            SetResponse setresponse = client.Set("notes/" + data.note_id, data);
        }

        [HttpGet]
        public ActionResult Detail(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("notes/"+id);
            Notes data = JsonConvert.DeserializeObject<Notes>(response.Body);
            return View(data);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("notes/"+id);
            Notes data = JsonConvert.DeserializeObject<Notes>(response.Body);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(Notes note)
        {
            client = new FireSharp.FirebaseClient(config);
            SetResponse response = client.Set("notes/" + note.note_id, note);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("notes/" + id);
            return RedirectToAction("Index");
        }
    }
}
