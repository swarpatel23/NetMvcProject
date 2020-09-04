using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ProjectManagement_cum_feedback_systemMVC.Models
{
    public class user_post_issue
    {
        [Key, Column(Order = 0)]
        public int post_Id { get; set; }

        [Key, Column(Order = 1)]
        public int issue_Id { get; set; }
    }
}