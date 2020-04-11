using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsersTasks.Models
{
    [Table("UserTask", Schema = "dbo")]
    public class UserTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "UserTaskId")]
        public long UserTaskId { get; set; }

        [ForeignKey("UserId")]
        [Required]
        public long UserId { get; set; }

        [ForeignKey("TaskId")]
        [Required]
        public long TaskId { get; set; }

        public UserTask() { }

        public UserTask(long givenUserId, long givenTaskId)
        {
            UserId = givenUserId;
            TaskId = givenTaskId;
        }
    }
}
