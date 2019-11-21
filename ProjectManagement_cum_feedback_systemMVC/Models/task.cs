using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ProjectManagement_cum_feedback_systemMVC.Models
{
    public class task
    {
        [Key]
        public int task_id { get; set; }

        [Required]
        public int Project_Id { get; set; }

        [Required]
        public string task_Id_toshow { get; set; }

        [Required]
        public string task_name { get; set; }
        
        public System.Nullable<DateTime> start_date { get; set; }

        public System.Nullable<DateTime> end_date { get; set; }

        public System.Nullable<int> duration { get; set; }

        [Required]
        public int percentage { get; set; }

        public string dependencies { get; set; }
    }
}