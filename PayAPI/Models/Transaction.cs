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
        public virtual Device UseDevice { get; set; }
        public virtual Card from { get; set; }

        public virtual Card to { get; set; }

        public decimal amount { get; set; }
    }
}