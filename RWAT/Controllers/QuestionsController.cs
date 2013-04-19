using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using RWAT.Hubs;
using RWAT.Models;
using RWAT.Utility;
using RWAT.ViewModel;

namespace RWAT.Controllers
{
    public class QuestionsController : Controller
    {
        public ActionResult Index(bool? showNav=true)
        {
             List<QuestionViewModel> questions = new List<QuestionViewModel>();
             
            foreach (var question in MongoHelper.GetCollection<Question>("questions").FindAll().ToList())
            {
                QuestionViewModel questionViewModel = new QuestionViewModel();
                questionViewModel.Question = question;
                User user =
                    MongoHelper.GetCollection<User>("users").FindAll().FirstOrDefault(u => u.UserId == question.UserId);
                questionViewModel.User = user;
                questions.Add(questionViewModel);
            }

            return View(new QuestionsViewModel{QuestionViewModels =  questions,ShowNav = showNav});
        }

        [HttpGet]
        public ActionResult Question(string id)
        {
            var objectId = new ObjectId(id);
            var question =MongoHelper.GetCollection<Question>("questions").AsQueryable().FirstOrDefault(q => q.QuestionId == objectId);

            QuestionViewModel questionViewModel = MongoHelper.GetQuestionViewModel(question,
                                                                                 HttpContext.User.Identity.Name);
            return View(questionViewModel);
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
              
                var user = MongoHelper.GetCollection<User>("users").AsQueryable().FirstOrDefault(u=>u.UserName==HttpContext.User.Identity.Name);

                if (user != null)
                {
                    var questions = MongoHelper.GetCollection<Question>("questions");
                    question.DateAsked = DateTime.Now.ToShortDateString();
                    question.UserId = user.UserId;
                    questions.Save(question);
                    var questionViewModel = MongoHelper.GetQuestionViewModel(question, HttpContext.User.Identity.Name);
                    var jsonString = questionViewModel==null? "new {}" : questionViewModel.ToJson();
                    GlobalHost.ConnectionManager.GetHubContext<QuestionHub>().Clients.All.showQuestion(jsonString);
                    return RedirectToAction("Create");
                }
                ModelState.AddModelError("","Could not create question.");
            }
            
            return View(question);
        }

       
    }
}