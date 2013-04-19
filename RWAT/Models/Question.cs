using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RWAT.Models
{
    public class Question
    {
        private Vote _vote;

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId QuestionId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public ObjectId UserId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }


        public string DateAsked { get; set; }

        public Vote Vote
        {
            get
            {
                if (_vote == null)
                {
                    _vote = new Vote();
                }
                return _vote;
            }
            set { _vote = value; }
        }

    }
}