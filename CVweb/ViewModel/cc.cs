using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CVweb.ViewModel
{
    public class cc
    {
        public int id { get; set; }
        [Required]
        public IFormFile cv { get; set; }
        [Required]
        public IFormFile photo{ get; set; }
        public string notes { get; set; }

        public bool mid { get; set; }
        public bool know { get; set; }
        public bool arabic { get; set; }
        public bool linkedin { get; set; }
        public bool fast { get; set; }
        public int cato { get; set; }
        public string userID { get; set; }
        public bool paying { get; set; }
        public bool inwork { get; set; }
        public int type { get; set; }

    }
}
