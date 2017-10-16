using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PayAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public List<Card> UserCards { get; set; }
        public List<Device> OwnedDevices { get; set; }
    }
}