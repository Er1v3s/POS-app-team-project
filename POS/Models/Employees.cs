﻿using System;

namespace POS.Models
{
    public class Employees
    {
        public int Employee_id { get; set; }
        public required string First_name { get; set; }
        public required string Last_name { get; set; }
        public string? Job_title { get; set; }
        public string? Email { get; set; }
        public int? Phone_number { get; set; }
        public string? Address { get; set; }
        public DateTime? Hire_date { get; set; }
        public required string Login { get; set; }
        public required string Password { get; set; }
        public required bool Is_User_LoggedIn { get; set; }
    }
}
