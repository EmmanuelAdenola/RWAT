using System.Web;

namespace RWAT.ViewModel
{
    public class VoteViewModel
    {
        private string _upvotePath = VirtualPathUtility.ToAbsolute("~/Content/images/upvote.png");
        private string _downvotePath = VirtualPathUtility.ToAbsolute("~/Content/images/downvote.png");
        private string _noupvotePath = VirtualPathUtility.ToAbsolute("~/Content/images/noupvote.png");
        private string _nodownvotePath = VirtualPathUtility.ToAbsolute("~/Content/images/nodownvote.png");

        public string UpVotePath { get { return _upvotePath; } set { _upvotePath = value; } }

        public string DownVotePath
        {
            get { return _downvotePath; }
            set { _downvotePath = value; }
        }


        public string NoUpVotePath { get { return _noupvotePath; } set { _noupvotePath = value; } }

        public string NoDownVotePath
        {
            get { return _nodownvotePath; }
            set { _nodownvotePath = value; }
        }


        public string SelectedUpVotePath { get; set; }

        public string SelectedDownVotePath { get; set; }
        public int CurrentVote { get; set; }
    }
}