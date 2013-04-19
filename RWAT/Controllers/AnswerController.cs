using System;
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
    public class AnswerController : Controller
    {
        [HttpGet]
        public ActionResult AnswerBox(string questionid)
        {
            return View(new AnswerModel { QuestionId = questionid });
        }

        [HttpPost]
        [System.Web.Mvc.Authorize]
        public ActionResult NewAnswer(AnswerModel answerModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Enter a valid answer.");
                return View("AnswerBox", answerModel);
            }

            var question = MongoHelper.GetCollection<Question>("questions").AsQueryable().FirstOrDefault(q => q.QuestionId == new ObjectId(answerModel.QuestionId));
            if (question != null)
            {
                var user = MongoHelper.GetCollection<User>("users").AsQueryable().FirstOrDefault(u => u.UserName == HttpContext.User.Identity.Name);
                if (user != null)
                {
                    Answer newAnswer = new Answer();
                    newAnswer.Body = answerModel.Answer;
                    newAnswer.UserId = user.UserId;
                    newAnswer.DateAnswered = DateTime.Now.ToShortDateString();
                    newAnswer.QuestionId = question.QuestionId;
                    MongoHelper.GetCollection<Question>("answers").Save(newAnswer);
                    GlobalHost.ConnectionManager.GetHubContext<AnswerHub>().Clients.All.showAnswer(new AnswerViewModel
                                                                                                       {
                                                                                                           Answer =newAnswer,
                                                                                                           Answerer = user
                                                                                                       });
                    return RedirectToAction("AnswerBox", new { questionid = answerModel.QuestionId });
                }
            }
            return View("AnswerBox", answerModel);
        }


    }
}
