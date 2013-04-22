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
    /// Class used to allow bidirectional communication for answers
    /// </summary>
    [HubName("answerhub")]
    public class AnswerHub :Hub
    {
        private const int AnswerUpvote = 10, AnswerDownVote = -2;

        /// <summary>
        /// Publishes a new answer to the client
        /// </summary>
        /// <param name="answer"></param>
        public void ShowAnswer(Answer answer)
        {
            Clients.Caller.showAnswer(answer.ToJson());
        }

        /// <summary>
        /// Down vote action from client
        /// </summary>
        /// <param name="answerid"></param>
        public void DownVote(string answerid)
        {
            UpdateVote(answerid, false);
        }

        /// <summary>
        /// Upvote action from client
        /// </summary>
        /// <param name="answerid"></param>
        public void UpVote(string answerid)
        {
            UpdateVote(answerid, true);
        }

        /// <summary>
        /// Contains logic to prevent a user is voting him or herself, otherwise apply votes and show proper image.
        /// </summary>
        /// <param name="answerid"></param>
        /// <param name="isUpVote"></param>
        private void UpdateVote(string answerid, bool isUpVote)
        {
            var answer = MongoHelper.GetCollection<Answer>("answers").AsQueryable().FirstOrDefault(a => a.AnswerId == new ObjectId(answerid));
            if (answer == null)
            {
                return;
            }

            var answerer = MongoHelper.GetCollection<User>("users").AsQueryable().FirstOrDefault(u => u.UserId == answer.UserId);
            if (answerer == null)
            {
                return;
            }

            VoteViewModel voteviewModel = new VoteViewModel();

            if (answerer.UserName.Equals(HttpContext.Current.User.Identity.Name) || !HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (isUpVote)
                {
                    Clients.Caller.updateAnswerUpVoteImage(answerid,voteviewModel.NoUpVotePath);
                }
                else
                {
                    Clients.Caller.updateAnswerDownVoteImage(answerid,voteviewModel.NoDownVotePath);
                }

                int currVote = answer.Vote.UserVotes.Sum(uv => uv.Upvote);
                Clients.Caller.updateAnswerVote(answerid,currVote,
                                                  answerer.UserName.Equals(HttpContext.Current.User.Identity.Name)
                                                      ? "You cannot vote your own answer"
                                                      : "You must be logged in to vote");
            }
            else
            {
                UserVote userVote = answer.Vote.UserVotes.FirstOrDefault(uv => uv.User.UserName == HttpContext.Current.User.Identity.Name);

                if (userVote == null)
                {
                    NewAnswerVoteByUser(answer, voteviewModel, isUpVote);
                }
                else
                {
                    ReestablishUserVoteChanges(answer, voteviewModel, userVote, isUpVote);
                }

                MongoHelper.GetCollection<Answer>("answers").Save(answer);
                int currVote = answer.Vote.UserVotes.Sum(uv => uv.Upvote);
                Clients.All.updateAnswerVote(answer.AnswerId.ToString(),currVote, "");
            }
        }

        /// <summary>
        /// User is making a change to an existing vote
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="voteviewModel"></param>
        /// <param name="userVote"></param>
        /// <param name="isUpVote"></param>
        private void ReestablishUserVoteChanges(Answer answer, VoteViewModel voteviewModel, UserVote userVote,
                                               bool isUpVote)
        {
            if (isUpVote)
            {
                userVote.SelectedDownVotePath = "";
                if (userVote.Upvote == AnswerUpvote)
                {
                    Clients.Caller.updateAnswerUpVoteImage(answer.AnswerId.ToString(),voteviewModel.NoUpVotePath);
                    userVote.SelectedUpVotePath = "";
                    userVote.Upvote = 0;
                    answer.Vote.UserVotes.Remove(userVote);
                }
                else
                {
                    Clients.Caller.updateAnswerUpVoteImage(answer.AnswerId.ToString(),voteviewModel.UpVotePath);
                    userVote.SelectedUpVotePath = voteviewModel.UpVotePath;
                    userVote.Upvote = AnswerUpvote;
                }

            }
            else
            {
                userVote.SelectedUpVotePath = "";
                if (userVote.Upvote == AnswerDownVote)
                {
                    Clients.Caller.updateAnswerDownVoteImage(answer.AnswerId.ToString(),voteviewModel.NoDownVotePath);
                    userVote.SelectedDownVotePath = "";
                    userVote.Upvote = 0;
                    answer.Vote.UserVotes.Remove(userVote);
                }
                else
                {
                    Clients.Caller.updateAnswerDownVoteImage(answer.AnswerId.ToString(),voteviewModel.DownVotePath);
                    userVote.SelectedDownVotePath = voteviewModel.DownVotePath;
                    userVote.Upvote = AnswerDownVote;
                }
            }
        }

        /// <summary>
        /// Check if a user is making a new vote
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="voteviewModel"></param>
        /// <param name="isUpVote"></param>
        private void NewAnswerVoteByUser(Answer answer, VoteViewModel voteviewModel, bool isUpVote)
        {
            UserVote userVote = new UserVote();
            userVote.User =
                MongoHelper.GetCollection<User>("users").AsQueryable().FirstOrDefault(
                    u => u.UserName == HttpContext.Current.User.Identity.Name);
            userVote.Upvote += (isUpVote) ? AnswerUpvote : AnswerDownVote;

            answer.Vote.UserVotes.Add(userVote);

            if (isUpVote)
            {
                userVote.SelectedUpVotePath = voteviewModel.SelectedUpVotePath = voteviewModel.UpVotePath;
                Clients.Caller.updateAnswerUpVoteImage(answer.AnswerId.ToString(),voteviewModel.SelectedUpVotePath);
            }
            else
            {
                userVote.SelectedDownVotePath = voteviewModel.SelectedDownVotePath = voteviewModel.DownVotePath;
                Clients.Caller.updateAnswerDownVoteImage(answer.AnswerId.ToString(),voteviewModel.SelectedDownVotePath);
            }
        }

    }
}