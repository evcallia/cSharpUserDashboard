using System;
using System.ComponentModel.DataAnnotations;

namespace userDashboard.Models
{
    public class Comment : BaseEntity
    {
        [Required]
        [MinLength(2)]
        public string comment {get; set;} 

        public int id {get; set;}

        public int messageId {get; set;}

        public Message message {get; set;}

        public int commenterId {get; set;}

        public User commenter {get; set;}

        public DateTime CreatedAt {get; set;}

        public DateTime UpdatedAt {get; set;}
    }
}