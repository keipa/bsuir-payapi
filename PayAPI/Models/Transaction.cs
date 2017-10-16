using System;
using System.ComponentModel.DataAnnotations;

namespace PayAPI.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public DateTime Requseted { get; set; }
        public DateTime Executed { get; set; }

        public Device from { get; set; }

        public User to { get; set; }

        public decimal amount { get; set; }
    }
}