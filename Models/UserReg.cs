using System;
using System.ComponentModel.DataAnnotations;

namespace userDashboard.Models
{
    public abstract class BaseEntity {}

    public class UserReg : BaseEntity
    {
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

        [Compare("password")]
        public string password_confirmation {get; set;}

        public int id {get; set;}

        public string description {get; set;}

        public string user_level {get; set;}

        public UserReg Transfer(User old){
            first_name = old.first_name;
            last_name = old.last_name;
            email = old.email;
            id = old.id;
            description = old.description;
            password = old.password;
            password_confirmation = old.password;
            user_level = old.user_level;
            return this;
        }
    }
}