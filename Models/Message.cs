using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace userDashboard.Models
{
    public class Message : BaseEntity
    {
        [Required]
        [MinLength(2)]
        public string message {get; set;} 

        public int id {get; set;}

        public int posterId {get; set;}

        public User poster {get; set;}

        public int userId {get; set;}

        public User user {get; set;}

        public DateTime CreatedAt {get; set;}

        public DateTime UpdatedAt {get; set;}

        public List<Comment> comments {get; set;}
    }
}