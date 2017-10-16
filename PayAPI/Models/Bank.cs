using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PayAPI.Models
{
    public class Bank
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public List<Card> CardList { get; set; }
    }
}