﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BlogLab.Models.Account
{
    public class ApplicationUserIdentity
    {
        public int ApplicationUserId { get; set; }
        public string Username { get; set; }
        public string NormalizeUsername { get; set; }
        public string Email { get; set; }
        public string NormalizeEmail { get; set; }
        public string Fullname { get; set; }
        public string PasswordHash { get; set; }
    }
}
