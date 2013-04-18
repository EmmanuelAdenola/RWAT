using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using RWAT.Hubs;
using RWAT.Models;

namespace RWAT.Controllers
{
    public class QuestionsViewModel
    {
        public bool? ShowNav { get; set; }
        public List<QuestionViewModel> QuestionViewModels { get; set; }
    }

    public class QuestionsController : Controller
    {
        private string connectionString = "MongoDb";

        public ActionResult Index(bool? showNav=true)
        {
            var conn = new MongoConnectionStringBuilder(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString);
            MongoClient mongoClient = new MongoClient(conn.ConnectionString);
            MongoServer mongoServer = mongoClient.GetServer();
            MongoDatabase mongoDatabase = mongoServer.GetDatabase(conn.DatabaseName);
            List<QuestionViewModel> questions = new List<QuestionViewModel>();

            foreach (var question in mongoDatabase.GetCollection<Question>("questions").FindAll())
            {
                QuestionViewModel questionModel = new QuestionViewModel();
                questionModel.Question = question;
                User user =
                    mongoDatabase.GetCollection<User>("users").FindOne(Query<User>.EQ(u => u.UserId, question.UserId));
                questionModel.User = user;
                questions.Add(questionModel);
            }

            return View(new QuestionsViewModel{QuestionViewModels =  questions,ShowNav = showNav});
        }

        [HttpGet]
        public ActionResult Question(string id)
        {
            var conn =
                  new MongoConnectionStringBuilder(
                      ConfigurationManager.ConnectionStrings[connectionString].ConnectionString);

            MongoClient mongoClient = new MongoClient(conn.ConnectionString);
            MongoServer mongoServer = mongoClient.GetServer();
            MongoDatabase mongoDatabase = mongoServer.GetDatabase(conn.DatabaseName);
            ObjectId objectId = new ObjectId(id);
            var question = mongoDatabase.GetCollection<Question>("questions").AsQueryable().FirstOrDefault(q=>q.Id == objectId);
            if(question != null)
            {
                User user =
                    mongoDatabase.GetCollection<User>("users").AsQueryable().FirstOrDefault(u=>u.UserId == question.UserId);
                QuestionViewModel questionViewModel = new QuestionViewModel { Question = question, User = user };
                return View(questionViewModel);
            }
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [System.Web.Mvc.Authorize]
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


                var user = mongoDatabase.GetCollection<User>("users").AsQueryable().FirstOrDefault(u=>u.UserName==HttpContext.User.Identity.Name);

                if (user != null)
                {
                    var questions = mongoDatabase.GetCollection<Question>("questions");
                    user.Questions.Add(question);
                    question.DateAsked = DateTime.Now.ToShortDateString();
                    question.UserId = user.UserId;
                    questions.Save(question);
                    QuestionViewModel questionModel = new QuestionViewModel();
                    questionModel.Question = question;
                    questionModel.User = user;
                    GlobalHost.ConnectionManager.GetHubContext<QuestionHub>().Clients.All.showQuestion(questionModel);
                    return RedirectToAction("Create");
                }
                else
                {
                    ModelState.AddModelError("","Could not create question.");
                }
            }
            
            return View(question);
        }

        [HttpGet]
        public ActionResult AnswerBox(string questionid)
        {
            return View(new AnswerModel{QuestionId =questionid});
        }

        [HttpPost]
        [System.Web.Mvc.Authorize]
        public ActionResult Answer(AnswerModel answerModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Enter a valid answer.");
                return View("AnswerBox",answerModel);
            }
            
            var conn =
                  new MongoConnectionStringBuilder(
                      ConfigurationManager.ConnectionStrings[connectionString].ConnectionString);
            MongoClient mongoClient = new MongoClient(conn.ConnectionString);
            MongoServer mongoServer = mongoClient.GetServer();
            MongoDatabase mongoDatabase = mongoServer.GetDatabase(conn.DatabaseName);
            var question = mongoDatabase.GetCollection<Question>("questions").AsQueryable().FirstOrDefault(q=>q.Id == new ObjectId(answerModel.QuestionId));
            if (question != null)
            {
                var user = mongoDatabase.GetCollection<User>("users").AsQueryable().FirstOrDefault(u => u.UserId == question.UserId);
                if (user != null)
                {
                    Answer newAnswer = new Answer();
                    newAnswer.Body = answerModel.Answer;
                    newAnswer.User = user;
                    newAnswer.DateAnswered = DateTime.Now.ToShortDateString();
                    question.Answers.Add(newAnswer);
                    mongoDatabase.GetCollection<Question>("questions").Save(question);
                    GlobalHost.ConnectionManager.GetHubContext<QuestionHub>().Clients.All.showAnswer(newAnswer);
                    return RedirectToAction("AnswerBox", new {questionid = answerModel.QuestionId});
                }
            }
            return View("AnswerBox",answerModel);
        }
    }
}