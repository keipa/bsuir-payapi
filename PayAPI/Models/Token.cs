using System;
using System.ComponentModel.DataAnnotations;

namespace PayAPI.Models
{
    public class Token
    {
        [Key]
        public int Id { get; set; }

        public Guid Value { get; set; }
        public DateTime ExpiredDate { get; set; }
        public Device RelatedDevice { get; set; }
        public Card RelatedCard { get; set; }

        public bool Used { get; set; }
    }
}