using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using RWAT.Models;
using RWAT.Utility;
using RWAT.ViewModel;

namespace RWAT.Hubs
{
    [HubName("questionhub")]
    public class QuestionHub : Hub
    {
        private const int questionUpvote = 5, questionDownVote = -2;

        public void ShowQuestion(QuestionViewModel questionModel)
        {
            Clients.Caller.showQuestion(questionModel);
        }

        public void ShowAnswer(Answer answer)
        {
            Clients.Caller.showAnswer(answer);
        }

        public void DownVote(string questionid)
        {
           UpdateVote(questionid,false);
        }

        public void UpVote(string questionid)
        {
            UpdateVote(questionid, true);
        }

        private void UpdateVote(string questionid, bool isUpVote)
        {
            var question =
               MongoHelper.GetCollection<Question>("questions").AsQueryable().FirstOrDefault(
                   q => q.Id == new ObjectId(questionid));
            if (question != null)
            {
                var asker =
                    MongoHelper.GetCollection<User>("users").AsQueryable().FirstOrDefault(
                        u => u.UserId == question.UserId);
                if (asker != null)
                {
                    VoteViewModel voteviewModel = new VoteViewModel();

                    if (asker.UserName.Equals(HttpContext.Current.User.Identity.Name))
                    {
                        voteviewModel.SelectedUpVotePath = voteviewModel.NoUpVotePath;
                        voteviewModel.IsVoteUpdated = false;
                        voteviewModel.ErrorMessage = "You cannot vote your own question";
                        voteviewModel.CurrentVote = question.Vote.UserVotes.Sum(uv => uv.Upvote);

                            Clients.Caller.updateQuestionVote(voteviewModel);

                    }
                    else if (HttpContext.Current.User.Identity.IsAuthenticated == false)
                    {
                        voteviewModel.SelectedUpVotePath = voteviewModel.NoUpVotePath;
                        voteviewModel.IsVoteUpdated = false;
                        voteviewModel.ErrorMessage = "You must log in to vote";
                        voteviewModel.CurrentVote = question.Vote.UserVotes.Sum(uv => uv.Upvote);
                        Clients.Caller.updateQuestionVote(voteviewModel);
                    }
                    else
                    {
                        UserVote userVote =
                            question.Vote.UserVotes.FirstOrDefault(
                                uv => uv.User.UserName == HttpContext.Current.User.Identity.Name);
                        if (userVote == null)
                        {
                            NewQuestionVoteByUser(question, voteviewModel, isUpVote);
                        }
                        else
                        {
                            ReestablishUserVoteChanges(question, voteviewModel, userVote, isUpVote);
                        }
                        MongoHelper.GetCollection<Question>("questions").Save(question);
                        question =
                            MongoHelper.GetCollection<Question>("questions").AsQueryable().FirstOrDefault(
                                q => q.Id == new ObjectId(questionid));
                        if (question != null)
                        {
                            voteviewModel.CurrentVote = question.Vote.UserVotes.Sum(uv => uv.Upvote);
                        }
                        Clients.All.updateQuestionVote(voteviewModel);
                    }
                }
            }
        }

        public void ReestablishUserVoteChanges(Question question, VoteViewModel voteviewModel, UserVote userVote,bool isUpVote)
        {
            if (isUpVote)
            {
                userVote.SelectedDownVotePath = "";
                if (userVote.Upvote == questionUpvote)
                {
                    voteviewModel.SelectedUpVotePath = voteviewModel.NoUpVotePath;
                    userVote.SelectedUpVotePath = "";
                    userVote.Upvote = 0;
                }
                else
                {
                    voteviewModel.SelectedUpVotePath=userVote.SelectedUpVotePath = voteviewModel.UpVotePath;
                    userVote.Upvote = questionUpvote;
                }
                
                Clients.Caller.updateQuestionUpVoteImage(voteviewModel.SelectedUpVotePath);
            }
            else
            {
                userVote.SelectedUpVotePath = "";
                if (userVote.Upvote == questionDownVote)
                {
                    voteviewModel.SelectedDownVotePath = voteviewModel.NoDownVotePath;
                    userVote.SelectedDownVotePath = "";
                    userVote.Upvote = 0;
                }
                else
                {
                    voteviewModel.SelectedDownVotePath = userVote.SelectedDownVotePath = voteviewModel.DownVotePath;
                    userVote.Upvote = questionDownVote;
                }
                Clients.Caller.updateQuestionDownVoteImage(voteviewModel.SelectedDownVotePath);
            }
        }

        public void NewQuestionVoteByUser(Question question,VoteViewModel voteviewModel,bool isUpVote)
        {
            UserVote userVote = new UserVote();
            userVote.User =
                MongoHelper.GetCollection<User>("users").AsQueryable().FirstOrDefault(
                    u => u.UserName == HttpContext.Current.User.Identity.Name);
            userVote.Upvote += (isUpVote) ? questionUpvote : questionDownVote;


            question.Vote.UserVotes.Add(userVote);

            if (isUpVote)
            {
                userVote.SelectedUpVotePath = voteviewModel.SelectedUpVotePath = voteviewModel.UpVotePath;
                Clients.Caller.updateQuestionUpVoteImage(voteviewModel.SelectedUpVotePath);
            }
            else
            {
                userVote.SelectedDownVotePath = voteviewModel.SelectedDownVotePath = voteviewModel.DownVotePath;
                Clients.Caller.updateQuestionDownVoteImage(voteviewModel.SelectedDownVotePath);
            }
        }
}
}