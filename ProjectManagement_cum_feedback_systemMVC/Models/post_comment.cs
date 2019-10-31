using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectManagement_cum_feedback_systemMVC.Models
{
    public class post_comment
    {
        [Key]
        public int comment_Id { get; set; }

        [Required]
        public string user_Id { get; set; }

        [Required]
        public int project_Id { get; set; }

        [Required]
        public int post_Id { get; set; }

        [Required]
        public string comment_desc { get; set; }

        [ForeignKey("project_Id")]
        public virtual project Project { get; set; }

        [ForeignKey("post_Id")]
        public virtual user_post post { get; set; }
    }
}