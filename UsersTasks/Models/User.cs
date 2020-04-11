using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsersTasks.Models
{
    [Table("User", Schema = "dbo")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "UserId")]
        public long UserId { get; set; }

        [Column(TypeName = "varchar(50)")]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [Column(TypeName = "varchar(200)")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        [Display(Name = "Phone number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "varchar(1)")]
        [RegularExpression("M|F")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        [Display(Name = "City")]
        public string City { get; set; }

        [NotMapped]
        public List<Task> Tasks { get; set; }
    }
}
