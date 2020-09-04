using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ProjectManagement_cum_feedback_systemMVC.Models
{
    public class post_user_vote
    {
        [Key, Column(Order = 0)]
        public int post_id { get; set; }

        [Key, Column(Order = 1)]
        public string user_id { get; set; }
    }
}