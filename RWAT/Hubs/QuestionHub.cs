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
    /// <summary>
    /// Class for bidirectional communication between clients.
    /// </summary>
    [HubName("questionhub")]
    public class QuestionHub : Hub
    {
        private const int QuestionUpvote = 5, QuestionDownVote = -2;

        /// <summary>
        /// Shows a new question in the client
        /// </summary>
        /// <param name="questionViewModel"></param>
        public void ShowQuestion(QuestionViewModel questionViewModel)
        {
            Clients.Caller.showQuestion(questionViewModel);
        }

       /// <summary>
       /// Update downvote action
       /// </summary>
       /// <param name="questionid"></param>
        public void DownVote(string questionid)
        {
            UpdateVote(questionid, false);
        }

        /// <summary>
        /// Update Upvote action
        /// </summary>
        /// <param name="questionid"></param>
        public void UpVote(string questionid)
        {
            UpdateVote(questionid, true);
        }


        /// <summary>
        /// Contains logic to prevent a user is voting him or herself, otherwise apply votes and show proper image.
        /// </summary>
        /// <param name="questionid"></param>
        /// <param name="isUpVote"></param>
       
        private void UpdateVote(string questionid, bool isUpVote)
        {
            var question = MongoHelper.GetCollection<Question>("questions").AsQueryable().FirstOrDefault(q => q.QuestionId == new ObjectId(questionid));
            if (question == null)
            {
                return;
            }

            var asker = MongoHelper.GetCollection<User>("users").AsQueryable().FirstOrDefault(u => u.UserId == question.UserId);
            if (asker == null)
            {
                return;
            }

            VoteViewModel voteviewModel = new VoteViewModel();

            if (asker.UserName.Equals(HttpContext.Current.User.Identity.Name) || !HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (isUpVote)
                {
                    Clients.Caller.updateQuestionUpVoteImage(voteviewModel.NoUpVotePath);
                }
                else
                {
                    Clients.Caller.updateQuestionDownVoteImage(voteviewModel.NoDownVotePath);
                }

                int currVote = question.Vote.UserVotes.Sum(uv => uv.Upvote);
                Clients.Caller.updateQuestionVote(currVote,
                                                  asker.UserName.Equals(HttpContext.Current.User.Identity.Name)
                                                      ? "You cannot vote your own question"
                                                      : "Your must be logged in to vote");
            }
            else
            {
                UserVote userVote = question.Vote.UserVotes.FirstOrDefault(uv => uv.User.UserName == HttpContext.Current.User.Identity.Name);
                
                if (userVote == null)
                {
                    NewQuestionVoteByUser(question, voteviewModel, isUpVote);
                }
                else
                {
                    ReestablishUserVoteChanges(question, voteviewModel, userVote, isUpVote);
                }
                
                MongoHelper.GetCollection<Question>("questions").Save(question);
                int currVote = question.Vote.UserVotes.Sum(uv => uv.Upvote);
                Clients.All.updateQuestionVote(currVote, "");
            }
        }

        
        /// <summary>
        /// User is making a change to an existing vote
        /// </summary>
        /// <param name="question"></param>
        /// <param name="voteviewModel"></param>
        /// <param name="userVote"></param>
        /// <param name="isUpVote"></param>
        private void ReestablishUserVoteChanges(Question question, VoteViewModel voteviewModel, UserVote userVote,
                                               bool isUpVote)
        {
            if (isUpVote)
            {
                userVote.SelectedDownVotePath = "";
                if (userVote.Upvote == QuestionUpvote)
                {
                    Clients.Caller.updateQuestionUpVoteImage(voteviewModel.NoUpVotePath);
                    userVote.SelectedUpVotePath = "";
                    userVote.Upvote = 0;
                }
                else
                {
                    Clients.Caller.updateQuestionUpVoteImage(voteviewModel.UpVotePath);
                     userVote.SelectedUpVotePath = voteviewModel.UpVotePath;
                    userVote.Upvote = QuestionUpvote;
                }

            }
            else
            {
                userVote.SelectedUpVotePath = "";
                if (userVote.Upvote == QuestionDownVote)
                {
                    Clients.Caller.updateQuestionDownVoteImage(voteviewModel.NoDownVotePath);
                    userVote.SelectedDownVotePath = "";
                    userVote.Upvote = 0;
                }
                else
                {
                    Clients.Caller.updateQuestionDownVoteImage(voteviewModel.DownVotePath);
                    userVote.SelectedDownVotePath = voteviewModel.DownVotePath;
                    userVote.Upvote = QuestionDownVote;
                }
            }
        }

        /// <summary>
        /// Check if a user is making a new vote
        /// </summary>
        /// <param name="question"></param>
        /// <param name="voteviewModel"></param>
        /// <param name="isUpVote"></param>
      
        private void NewQuestionVoteByUser(Question question, VoteViewModel voteviewModel, bool isUpVote)
        {
            UserVote userVote = new UserVote();
            userVote.User =
                MongoHelper.GetCollection<User>("users").AsQueryable().FirstOrDefault(
                    u => u.UserName == HttpContext.Current.User.Identity.Name);
            userVote.Upvote += (isUpVote) ? QuestionUpvote : QuestionDownVote;


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