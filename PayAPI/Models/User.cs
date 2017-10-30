using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PayAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string email { get; set; }
        public string phone { get; set; }

        public string Name { get; set; }

    }
}