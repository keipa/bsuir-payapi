using System;
using System.ComponentModel.DataAnnotations;

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