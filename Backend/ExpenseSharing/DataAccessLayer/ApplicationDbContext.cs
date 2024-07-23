using BusinessObjectLayer;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseSplit> ExpenseSplits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Define relationships

            // Expense entity configuration


            modelBuilder.Entity<User>()
                .Property(u => u.AvailableBalance)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Expense>()
                .HasKey(e => e.ExpenseId);

            modelBuilder.Entity<Expense>()
                .Property(e => e.Description)
                .IsRequired();

            modelBuilder.Entity<Expense>()
                .Property(e => e.ExpenseAmount)
                .IsRequired();

            modelBuilder.Entity<Expense>()
                .Property(e => e.IsSettled)
                .IsRequired();

            modelBuilder.Entity<Expense>()
                .Property(e => e.ExpenseCreatedAt)
                .IsRequired();

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.PaidByUserId)
                .OnDelete(DeleteBehavior.NoAction); // Adjust delete behavior as needed

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Group)
                .WithMany(g => g.Expense)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust delete behavior as needed

            // ExpenseSplit entity configuration
            modelBuilder.Entity<ExpenseSplit>()
                .HasKey(es => es.ExpenseSplitId);

            modelBuilder.Entity<ExpenseSplit>()
                .Property(es => es.SplitAmount)
                .IsRequired();

            modelBuilder.Entity<ExpenseSplit>()
                .Property(es => es.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<ExpenseSplit>()
                .HasOne(es => es.Expense)
                .WithMany(es => es.ExpenseSplit) // Use ExpenseSplit instead of ExpenseSplits
                .HasForeignKey(es => es.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust delete behavior as needed

            modelBuilder.Entity<ExpenseSplit>()
                .HasOne(es => es.User)
                .WithMany()
                .HasForeignKey(es => es.SplitWithUserId)
                .OnDelete(DeleteBehavior.NoAction); // Adjust delete behavior as needed

            // Group entity configuration
            modelBuilder.Entity<Group>()
                .HasKey(g => g.GroupId);

            modelBuilder.Entity<Group>()
                .Property(g => g.GroupName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Group>()
                .Property(g => g.Description)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Group>()
                .Property(g => g.CreatedDate)
                .IsRequired();

            modelBuilder.Entity<Group>()
                .Property(g => g.GroupAdminId)
                .IsRequired();

            modelBuilder.Entity<Group>()
                .Property(g => g.TotalMembers)
                .IsRequired();

            modelBuilder.Entity<Group>()
                .Property(g => g.TotalExpense)
                .IsRequired();

            modelBuilder.Entity<Group>()
                .HasMany(g => g.GroupMember)
                .WithOne(gm => gm.Group)
                .HasForeignKey(gm => gm.GroupId);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Expense)
                .WithOne(e => e.Group)
                .HasForeignKey(e => e.GroupId);

            // GroupMember entity configuration
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => gm.GroupMemberId);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.GroupMember)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust delete behavior as needed

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany()
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Adjust delete behavior as needed

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.GroupMember)
                .HasForeignKey(gm => gm.GroupId);


            base.OnModelCreating(modelBuilder);
        }
    }
}
