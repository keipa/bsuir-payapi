using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PayAPI.Models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }

        public User Owner { get; set; }
        public string CardId { get; set; }
        public int CVV { get; set; }

        public List<Device> DevicesConnected { get; set; }

        public decimal Balance { get; set; }
    }
}