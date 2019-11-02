using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectManagement_cum_feedback_systemMVC.Models
{
    public enum issue_stat
    {
        todo,
        inprogress,
        done
    }

    public enum issue_type
    {
        newfeature,
        bug,
        improvement
    }

    public class project_issue
    {
        [Key]
        public int issue_Id { get; set; }

        [Required]
        public int project_Id { get; set; }

        [Required]
        public string issue_title { get; set; }

        [Required]
        public string issue_desc { get; set; }

        public int priority { get; set; } //0-verylow 1-low 2-high 3-veryhigh

        public int assign_status { get; set; } // 0-unassigned 1-assigned

        public issue_stat issue_status { get; set; }

        public issue_type issue_type { get; set; }

        [ForeignKey("project_Id")]
        public virtual project Project { get; set; }
    }
}