using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlogLab.Models.Account
{
    public class ApplicationUserLogin
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(5, ErrorMessage ="Must be at least 5-20 characters")]
        [MaxLength(20, ErrorMessage ="Must be at least 5-20 characters")]
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
