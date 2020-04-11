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
    public class CheckController :Controller
    {
        private readonly CVaction _cvaction;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CheckController(CVaction cVaction,
                                         IHostingEnvironment hostingEnvironment)
        {
            _cvaction = cVaction;
            _hostingEnvironment = hostingEnvironment;

        }
        [HttpPost]
        public IActionResult Checkout(string id,string bu)
        {
            ViewBag.sal = bu;
            return View();
        }
        public IActionResult pay(cc model)
        {
            string uniph = null;
            string unicv = null;

            if (model.photo != null)
            {
                string upload = Path.Combine(_hostingEnvironment.WebRootPath, "img");
                uniph = Guid.NewGuid().ToString() + "_" + model.photo.FileName;
                string pathphoto = Path.Combine(upload, uniph);
                model.photo.CopyTo(new FileStream(pathphoto, FileMode.Create));
            }
            if (model.cv != null)
            {
                string upload = Path.Combine(_hostingEnvironment.WebRootPath, "pdf");
                unicv = Guid.NewGuid().ToString() + "_" + model.cv.FileName;
                string pathphoto = Path.Combine(upload, unicv);
                model.cv.CopyTo(new FileStream(pathphoto, FileMode.Create));

            }

            CVz newcv= new CVz {
                arabic = model.arabic,
                cato = model.cato,
                know = model.know,
                linkedin = model.linkedin,
                notes = model.notes,
                fast = model.fast,
                userID = User.Identity.Name,
                photopath = uniph,
                cvpath = unicv,
                mid = model.mid,
                paying=false,
                type=model.type,
                inwork=false

            };
            _cvaction.add(newcv);



            return View();
        }
    }
}
