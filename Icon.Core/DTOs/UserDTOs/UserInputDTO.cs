﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icon.Core.DTOs.UserDTOs
{
    public class UserInputDTO
    {
        [Required]
        [EmailAddress]
        //[RegularExpression(@"^.*@gmail.*$", ErrorMessage = "The text must contain the '@gmail' symbol.")]
        public string? Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        public string? UserName { get; set; }

        public int AccessFailedCount { get; set; } 
        public bool LockOutEnabled { get; set; } 
        public bool RememberMe { get; set; } 
    }
}
