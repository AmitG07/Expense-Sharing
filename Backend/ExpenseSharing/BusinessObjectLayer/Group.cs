using BusinessObjectLayer;
using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjectLayer
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [Required]
        [MaxLength(50)]
        public string GroupName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public int GroupAdminId { get; set; }

        [Required]
        public int TotalMembers { get; set; }

        [Required]
        public double TotalExpense { get; set; }
        public List<GroupMember> GroupMember { get; set; } = new();
        public List<Expense> Expense { get; set; } = new();
    }
}