﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSolutionWolfpack.Models
{
    public class Pack
    {
        [Key]
        public string Name { get; set; }
    }
}
