﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HighLandCoffeeWebsite.Models
{
    public class User
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int Admin { get; set; }
    }
}