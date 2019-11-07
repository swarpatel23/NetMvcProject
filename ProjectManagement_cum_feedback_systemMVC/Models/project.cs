using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement_cum_feedback_systemMVC.Models
{
    public class project
    {
        [Key]
        public int Project_Id { get; set; }

        [Required]
        public string user_Id { get; set; }

        [Required]
        [Display(Name = "Project Title")]
        public string project_title { get; set; }

        public string story_title { get; set; }

        public string story_desc { get; set; }

        public int story_status { get; set; } //0-not written 1-written

        public string srs_acceptance_status { get; set; }//0-not accepted 1-accepted

        public string srs_desc { get; set; }

        public int srs_status { get; set; }//0-not written 1-not written


        public ICollection<project_issue> issues { get; set; }

        public ICollection<project_user> members { get; set; }

        public ICollection<user_post> project_posts { get; set; }

        public ICollection<post_comment> project_posts_comments { get; set; }

        public ICollection<srs_comment> project_srs_comments { get; set; }



    }
}