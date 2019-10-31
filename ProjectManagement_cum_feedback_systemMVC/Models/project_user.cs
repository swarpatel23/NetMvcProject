using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectManagement_cum_feedback_systemMVC.Models
{
    public enum Roles
    {
        admin,
        teammember,
        customer
    }
    public class project_user
    {
        [Key, Column(Order = 0)]
        public int project_Id { get; set; }

        [Key, Column(Order = 1)]
        public string  user_Id { get; set; }  

        [Required]
        public Roles role { get; set; }


    }
}