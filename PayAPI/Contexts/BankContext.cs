using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PayAPI.Models
{
    public class BankContext: DbContext
    {
        public BankContext(): base("PayContext")
        {

        }
        public DbSet<Token> Tokens { get; set; }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Log> Logs { get; set; }

    }
}