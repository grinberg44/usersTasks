using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsersTasks.Models
{
    [Table("Task", Schema = "dbo")]
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "TaskId")]
        public long TaskId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        [Display(Name = "Name")]
        public string TaskName { get; set; }

        [Required]
        [Column(TypeName = "tinyint")]
        [Display(Name = "Status")]
        [Range(0, 1)]
        public int TaskStatus { get; set; } = 0;

        [Required]
        [Column(TypeName = "datetime")]
        [Display(Name = "Target finish date")]
        public DateTime TargetFinishDate { get; set; }

        [Required]
        [Column(TypeName = "tinyint")]
        [Display(Name = "Priority")]
        [Range(1, 3)]
        public int Priority { get; set; }

        [Required]
        [Column(TypeName = "tinyint")]
        [Display(Name = "IsActivate")]
        [Range(0, 1)]
        public int TaskIsActivate { get; set; } = 1;
    }
}
