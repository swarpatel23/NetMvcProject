using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectManagement_cum_feedback_systemMVC.Models
{
    public class project_issue_assign
    {
        [Required]
        public int project_Id { get; set; }

        [Key, Column(Order = 0)]
        public int issue_Id { get; set; }

        [Key, Column(Order = 1)]
        public string user_Id { get; set; }

        [Required]
        public DateTime startdate { get; set; }

        [Required]
        public DateTime enddate { get; set; }
    }
}