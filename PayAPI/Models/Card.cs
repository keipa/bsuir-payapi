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

        public Dictionary<Device, bool> DevicesConnected { get; set; } // authorized devices(device/isactivated)

        public decimal Balance { get; set; }

        public bool connected { get; set; }

        public int AuthorizationCode { get; set; }
    }
}