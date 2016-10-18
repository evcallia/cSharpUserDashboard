using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace userDashboard.Models
{
    public class User : BaseEntity
    {
        public User(){
            user_level = "normal";
            description = "";
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        [Required]
        [MinLength(2)]
        public string first_name {get; set;} 

        [Required]
        [MinLength(2)]
        public string last_name {get; set;} 

        [Required]
        [EmailAddress]
        public string email {get; set;}

        [Required]
        [MinLength(8)]
        public string password {get; set;}

        public string user_level {get; set;}

        public string description {get; set;}

        public int id {get; set;}

        public DateTime CreatedAt {get; set;}

        public DateTime UpdatedAt {get; set;}

        public List<Message> messagesTo {get; set;}

        public List<Message> messagesFrom {get; set;}

        public List<Comment> comments {get; set;}
        
        public User Transfer(UserReg old){
            first_name = old.first_name;
            last_name = old.last_name;
            email = old.email;
            password = old.password;
            return this;
        }
    }
}