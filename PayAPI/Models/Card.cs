﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PayAPI.Models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }

        public virtual User Owner { get; set; }
        public string CardId { get; set; }
        public int CVV { get; set; }

        public decimal Balance { get; set; }

        public bool connected { get; set; }

        public int AuthorizationCode { get; set; }
    }
}