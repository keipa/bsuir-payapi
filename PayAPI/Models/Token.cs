using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayAPI.Models
{
    public class Token
    {
        [Key]
        public int Id { get; set; }
        public Guid Value { get; set; }
        public DateTime ExpiredDate { get; set; }
        public Device Owner { get; set; }
        public bool Used{ get; set; }

    }
}