using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSolutionWolfpack.Models
{
    public class WolfPack
    {
        [Key]
        public long Id { get; set; }
        public string PackName { get; set; }
        public long WolfId { get; set; }
    }
}
