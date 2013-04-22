using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RWAT.Models;
using RWAT.ViewModel;

namespace RWAT.Utility
{
    public class MongoHelper
    {
        private const string ConnectionString = "MONGOLAB_URI";

        public static MongoCollection<T> GetCollection<T>(string collection)
        {
            var conn =
                new MongoConnectionStringBuilder(
                    ConfigurationManager.ConnectionStrings[ConnectionString].ConnectionString);
            MongoClient mongoClient = new MongoClient(conn.ConnectionString);
            MongoServer mongoServer = mongoClient.GetServer();
            MongoDatabase mongoDatabase = mongoServer.GetDatabase(conn.DatabaseName);

            return mongoDatabase.GetCollection<T>(collection);
        }


        public static QuestionViewModel GetQuestionViewModel(Question question, string sessionUserName)
        {
            if (question == null) return null;
            MongoCollection<User> userCollection = GetCollection<User>("users");
            User user = userCollection.AsQueryable().FirstOrDefault(u => u.UserId == question.UserId);
            if (user == null)
            {
                return null;
            }
            QuestionViewModel questionViewModel = new QuestionViewModel
                                                      {
                                                          Question = question,
                                                          User = user,
                                                      };
            questionViewModel.Answers = new List<AnswerViewModel>();
            
            foreach (var answer in GetCollection<Answer>("answers").AsQueryable().Where(a => a.QuestionId == question.QuestionId))
            {
                AnswerViewModel answerViewModel = new AnswerViewModel();
                answerViewModel.VoteViewModel = new VoteViewModel();
                answerViewModel.Answer = answer;
                answerViewModel.Answerer = userCollection.AsQueryable().First(u => u.UserId == answer.UserId);
                answerViewModel.VoteViewModel.CurrentVote = answer.Vote.UserVotes.Sum(u => u.Upvote);
                    UserVote _userVote = answer.Vote.UserVotes.FirstOrDefault(uv => uv.User.UserName == sessionUserName);
                    if (_userVote != null)
                    {
                        answerViewModel.VoteViewModel.SelectedUpVotePath = _userVote.SelectedUpVotePath;
                        answerViewModel.VoteViewModel.SelectedDownVotePath = _userVote.SelectedDownVotePath;
                    }
                questionViewModel.Answers.Add(answerViewModel);
            }

            questionViewModel.VoteViewModel = new VoteViewModel
                                                  {CurrentVote = question.Vote.UserVotes.Sum(s => s.Upvote)};

            var userVote = question.Vote.UserVotes.FirstOrDefault(uv => uv.User.UserName == sessionUserName);
            if (userVote != null)
            {
                questionViewModel.VoteViewModel.SelectedUpVotePath = userVote.SelectedUpVotePath;
                questionViewModel.VoteViewModel.SelectedDownVotePath = userVote.SelectedDownVotePath;
            }
            return questionViewModel;
        }
    }
}