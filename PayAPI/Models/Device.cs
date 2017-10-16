using System.ComponentModel.DataAnnotations;

namespace PayAPI.Models
{
    public class Device
    {
        [Key]
        public int Id { get; set; }

        public string DeviceHash { get; set; }

        public string Name { get; set; }

        public User Owner { get; set; }
    }
}