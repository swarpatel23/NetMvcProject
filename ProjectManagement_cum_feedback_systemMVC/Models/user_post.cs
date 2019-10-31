using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectManagement_cum_feedback_systemMVC.Models
{
    public class user_post
    {
        [Key]
        public int post_Id { get; set; }

        [Required]
        public string user_Id { get; set; }

        [Required]
        public int project_Id { get; set; }

        public int vote { get; set; }

        [Required]  
        public string post_title { get; set; }

        [Required]
        public string post_desc { get; set; }

        [ForeignKey("project_Id")]
        public virtual project Project { get; set; }

        public ICollection<post_comment> comments { get; set; }

    }
}