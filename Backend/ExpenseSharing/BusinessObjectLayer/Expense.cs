using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjectLayer
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int PaidByUserId { get; set; }

        [Required]
        [ForeignKey("Group")]
        public int GroupId { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public double ExpenseAmount { get; set; }

        [Required]
        public bool IsSettled { get; set; } = false;

        public DateTime ExpenseCreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public Group? Group { get; set; }
        public User? User { get; set; }
        public ICollection<ExpenseSplit>? ExpenseSplit { get; set; } = new List<ExpenseSplit>();
    }
}
