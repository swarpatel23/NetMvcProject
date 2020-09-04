using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectManagement_cum_feedback_systemMVC.Models
{
    public class project_message
    {
        [Key]
        public int Message_Id { get; set; }

        public string messages { get; set; }

        [Required]
        public int project_Id { get; set; }

        [Required]
        public string userId { get; set;}

        public string username { get; set; }

        public DateTime msgtime { get; set; }

        [ForeignKey("project_Id")]
        public virtual project Project { get; set; }
    }
}