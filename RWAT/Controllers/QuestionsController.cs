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
        /// <summary>
        ///Method to show all of questions
        /// gets a showNav param to decide whether to show a nav bar
        /// </summary>
        /// <param name="showNav"></param>
        /// <returns></returns>
        public ActionResult Index(bool? showNav=true)
        {
             List<QuestionViewModel> questions = new List<QuestionViewModel>();
             
            foreach (var question in MongoHelper.GetCollection<Question>("questions").FindAll().ToList())
            {
                QuestionViewModel questionViewModel = MongoHelper.GetQuestionViewModel(question,HttpContext.User.Identity.Name);
                questions.Add(questionViewModel);
            }

            return View(new QuestionsViewModel{QuestionViewModels =  questions,ShowNav = showNav});
        }

        /// <summary>
        /// Get a particular question
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Question(string id)
        {
            var objectId = new ObjectId(id);
            var question =MongoHelper.GetCollection<Question>("questions").AsQueryable().FirstOrDefault(q => q.QuestionId == objectId);

            QuestionViewModel questionViewModel = MongoHelper.GetQuestionViewModel(question,HttpContext.User.Identity.Name);
            return View(questionViewModel);
        }

        /// <summary>
        /// return a new to create a question
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create a new question 
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        [HttpPost]
        [System.Web.Mvc.Authorize]
        public ActionResult Create(Question question)
        {
            if (!ModelState.IsValid)
            {
                return View(question);
            }

            var user =MongoHelper.GetCollection<User>("users").AsQueryable().First(u => u.UserName == HttpContext.User.Identity.Name);
            var questions = MongoHelper.GetCollection<Question>("questions");

            question.DateAsked = DateTime.Now.ToShortDateString();
            question.UserId = user.UserId;
            var saveResult  =questions.Save(question);

            if(!saveResult.Ok)
            {
                ModelState.AddModelError("", saveResult.ErrorMessage);
                return View(question);
            }

            var questionViewModel = MongoHelper.GetQuestionViewModel(question, HttpContext.User.Identity.Name);
            var jsonString = questionViewModel == null ? "{}" : questionViewModel.ToJson();
            GlobalHost.ConnectionManager.GetHubContext<QuestionHub>().Clients.All.showQuestion(jsonString);
            return RedirectToAction("Create");
        }
    }
}