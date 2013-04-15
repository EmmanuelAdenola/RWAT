using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RWAT.Hubs;
using RWAT.Models;

namespace RWAT.Controllers
{
    public class QuestionsController : Controller
    {
        private string connectionString = "MongoDb";

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Question question)
        {
            if (ModelState.IsValid)
            {
                var conn =
                    new MongoConnectionStringBuilder(
                        ConfigurationManager.ConnectionStrings[connectionString].ConnectionString);
                MongoClient mongoClient = new MongoClient(conn.ConnectionString);
                MongoServer mongoServer = mongoClient.GetServer();
                MongoDatabase mongoDatabase = mongoServer.GetDatabase(conn.DatabaseName);
                var query = Query<User>.EQ(u => u.Email, "emmanuel.adenola@gmail.com");
                var users = mongoDatabase.GetCollection<User>("users");
                User user = users.FindOne(query);
                user.Questions.Add(question);
                users.Save(user);
                GlobalHost.ConnectionManager.GetHubContext<QuestionHub>().Clients.All.addQuestion(question);
                return RedirectToAction("Create");
            }
            return View(question);
        }
    }
}