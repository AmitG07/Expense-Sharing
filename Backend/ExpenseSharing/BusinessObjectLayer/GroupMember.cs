using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjectLayer
{
    public class GroupMember
    {
        [Key]
        public int GroupMemberId { get; set; }

        [Required]
        [ForeignKey("Group")]
        public int GroupId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User? User { get; set; }

        public Group? Group { get; set; }
        public double UserExpense { get; set; }
        public double GivenAmount { get; set; }
        public double TakenAmount { get; set; }
    }
}