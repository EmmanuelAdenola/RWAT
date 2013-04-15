namespace RWAT.Models
{
    public class Comment
    {
        public User Commenter { get; set; }
        public string Body { get; set; }
    }
}