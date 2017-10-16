using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayAPI.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; }

        public string Exception { get; set; }
        public DateTime BrokenAt { get; set; }
    }
}