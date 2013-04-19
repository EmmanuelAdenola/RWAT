using System.Collections.Generic;

namespace RWAT.Models
{
    public class Vote
    {
        private List<UserVote> _userVotes;
         
        public List<UserVote> UserVotes
        {
            get
            {
                if (_userVotes == null)
                {
                    _userVotes = new List<UserVote>();
                }
                return _userVotes;
            }
            set { _userVotes = value; }
        }
    }

    public class UserVote
    {
        public User User { get; set; }
        public int Upvote { get; set; }

        public string SelectedUpVotePath { get; set; }

        public string SelectedDownVotePath { get; set; }
    }

}