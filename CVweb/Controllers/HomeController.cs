using CVweb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CVweb.Controllers
{
    
    public class HomeController :Controller
    {
       
        public IActionResult Index()
        {
            
            return View();
        }
        public IActionResult catotogory()
        {
            return View();
        }



    }
}
