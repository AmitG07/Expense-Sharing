using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjectLayer
{
    public class ExpenseSplit
    {
        [Key]
        public int ExpenseSplitId { get; set; }

        [Required]
        [ForeignKey("Expense")]
        public int ExpenseId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int SplitWithUserId { get; set; }

        [Required]
        public double SplitAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Expense? Expense { get; set; }
        public User? User { get; set; }
    }
}
