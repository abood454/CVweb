using CVweb.Models;
using CVweb.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CVweb.Controllers
{
    
    public class HomeController :Controller
    {

        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly CVaction _cvaction;

        public HomeController(CVaction cVaction,  IHostingEnvironment hostingEnvironment)
        {

            _hostingEnvironment = hostingEnvironment;
            _cvaction = cVaction;
        }
        public IActionResult aboutus()
        {
            return View();
        }

        public IActionResult indexadmin()
        {
            return View();
        }
        public IActionResult Index()
        {
            if (User.Identity.Name == "admin@admin.com")
            {
                return RedirectToAction("indexadmin", "home");
            }
            else

            return View();
        }
        public IActionResult catotogory()
        {
            return View();
        }
        public IActionResult upload( video v)
        {
            string unicv = null;

            if (v.cv != null)
            {
                string upload = Path.Combine(_hostingEnvironment.WebRootPath, "video");
                unicv = "ff.mp4";
                string pathphoto = Path.Combine(upload, unicv);
                v.cv.CopyTo(new FileStream(pathphoto, FileMode.Create));
               
            }

            return RedirectToAction("indexadmin", "home");

        }


    }
}
