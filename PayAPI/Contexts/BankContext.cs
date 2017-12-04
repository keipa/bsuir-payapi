using System.Data.Entity;

namespace PayAPI.Models
{
    public class BankContext : DbContext
    {
        public BankContext() : base("PayContext")
        {
        }

        public DbSet<Token> Tokens { get; set; }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Activation> Activations { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}