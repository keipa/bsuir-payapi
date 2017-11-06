using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PayAPI.Models
{
    public class Activation
    {
        [Key]
        public int id { get; set; }
        public virtual Card Card { get; set; }
        public virtual Device Device { get; set; }
        public bool isActive{ get; set; }
    }
}