using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CVweb.Models
{
    public class CVz
    {
        public int id { get; set; }
        public string cvpath { get; set; }
        public string photopath { get; set; }
        public string notes { get; set; }

        public bool mid { get; set; }
        public bool know { get; set; }
        public bool arabic { get; set; }
        public bool linkedin { get; set; }
        public bool fast { get; set; }
        public int cato { get; set; }
        public string userID { get; set; }
        public bool paying { get; set; }
    }
}
