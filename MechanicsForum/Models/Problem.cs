using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MechanicsForum.Models
{
    public class Problem
    {
        public int ProblemId { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Image/Video")]
        public string MediaPath { get; set; }

    }
}