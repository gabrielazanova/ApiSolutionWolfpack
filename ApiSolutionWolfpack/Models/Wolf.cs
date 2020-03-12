using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSolutionWolfpack.Models
{
    public class Wolf
    {
        [Key]
        public long WolfId { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public PointF Location { get; set; }
    }
}
