using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RWAT.Models
{
    public class Question
    {
        private List<Answer> _answers;
        private Vote _vote;

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }

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
                if (_answers == null)
                {
                    _vote =new Vote();
                }
                return _vote;
            }
            set { _vote = value; }
        }

        public List<Answer> Answers
        {
            get
            {
                if (_answers == null)
                {
                    _answers = new List<Answer>();
                }
                return _answers;
            }
            set { _answers = value; }
        }
    }
}