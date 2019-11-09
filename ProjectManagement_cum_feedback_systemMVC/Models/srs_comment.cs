using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectManagement_cum_feedback_systemMVC.Models
{
    public class srs_comment
    {
        [Key]
        public int comment_id { get; set; }

        [Required]
        public int project_id { get; set; }

        [Required]
        public string user_id { get; set; }

        public DateTime cdate { get; set; }

        public string comment_desc { get; set; }

        [ForeignKey("project_id")]
        public virtual project Project { get; set; }




    }
}