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
            return View(new AnswerModel {QuestionId = questionid});
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

            var question =
                MongoHelper.GetCollection<Question>("questions").AsQueryable().FirstOrDefault(
                    q => q.QuestionId == new ObjectId(answerModel.QuestionId));

            //if the question id was tampered with
            if (question == null)
            {
                ModelState.AddModelError("", "Invalid question id");
                return View("AnswerBox", answerModel);
            }

            var user =
                MongoHelper.GetCollection<User>("users").AsQueryable().First(
                    u => u.UserName == HttpContext.User.Identity.Name);

            Answer newAnswer = new Answer();
            newAnswer.Body = answerModel.Answer;
            newAnswer.UserId = user.UserId;
            newAnswer.DateAnswered = DateTime.Now.ToShortDateString();
            newAnswer.QuestionId = question.QuestionId;
            var saveResult = MongoHelper.GetCollection<Question>("answers").Save(newAnswer);

            if(!saveResult.Ok)
            {
                ModelState.AddModelError("",saveResult.ErrorMessage);
                return View("AnswerBox", answerModel.QuestionId);
            }

            GlobalHost.ConnectionManager.GetHubContext<AnswerHub>().Clients.All.showAnswer(new AnswerViewModel
                                                                                                   {
                                                                                                       Answer = newAnswer,
                                                                                                       Answerer = user,
                                                                                                       VoteViewModel =new VoteViewModel()
                                                                                                   });
                return RedirectToAction("AnswerBox", new {questionid = answerModel.QuestionId});

        }
    }

}
