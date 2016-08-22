using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace moxiCommunity.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "UserName length should be between 4 and 50")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "UserName length should be between 4 and 50")]
        [Display(Name = "密码")]
        public string Password { get; set; }

       
    }



    public class LoginCookieViewModel
    {
        
        public int id { get; set; }

        public string username { get; set; }

        public string name { get; set; }
        public DateTime overtime { get; set; }
    }

}