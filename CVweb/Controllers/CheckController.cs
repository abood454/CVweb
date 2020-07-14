using CVweb.Models;
using CVweb.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayPalCheckoutSdk.Orders;
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
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public CheckController(CVaction cVaction,
                                         IHostingEnvironment hostingEnvironment, UserManager<IdentityUser> userManager
                                                             , SignInManager<IdentityUser> signInManager)
        {
            _cvaction = cVaction;
            _hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            this.signInManager = signInManager;

        }

        public CVz CVz { get; private set; }

        [HttpPost]
        public IActionResult Checkout(string id,string bu)
        {
            if (signInManager.IsSignedIn(User))
            {

                ViewBag.sal = bu;
                return View();
            }
            return RedirectToAction("login", "Account");

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

            

            return RedirectToAction("paypalcshtml", "Check", new {
            id=newcv.id,
            });
        }
        public async Task<IActionResult> paypalcshtml(int id)
        {
            CVz cv = _cvaction.get(id);
            decimal p = 0;

            switch (cv.type)
             {
                  case 1:
                     p += 100;
                     break;
                    case 2:
                    p += 100;
                    break;
                                    case 3:
                    p += 100;
                    break;
                                    case 4:
                    p += 100;
                    break;
                                    case 5:
                    p += 100;
                    break;
                default:
                    p += 100;
                    break;

            }
            if(cv.mid)
                                {
                p += 100;
            }
            if(cv.know)
                                {
                p += 100;
            }
            if(cv.arabic)
                                {
                p += 100;
            }
            if(cv.linkedin)
                                {
                p += 100;
            }
            if(cv.fast)
                                {
                p += 100;
            }


            OrderRequest orderRequest = new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    BrandName = "EXAMPLE INC",
                    LandingPage = "BILLING",
                    UserAction = "CONTINUE",
                    ShippingPreference = "SET_PROVIDED_ADDRESS"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        ReferenceId =  id.ToString(),
                        Description = "Sporting Goods",
                        CustomId = "CUST-HighFashions",
                        SoftDescriptor = "HighFashions",
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "USD",
                            Value = p.ToString(),
                            AmountBreakdown = new AmountBreakdown
                            {
                                ItemTotal = new Money { CurrencyCode = "USD", Value = p.ToString() },
                                
                            }
                        },
                        Items = new List<Item>
                        {
                            new Item
                            {
                                Name = "T-shirt",
                                Description = "Green XL",
                                Sku = "sku01",
                                UnitAmount = new Money { CurrencyCode = "USD", Value = p.ToString() },
                                Tax = new Money { CurrencyCode = "USD", Value = "0.00" },
                                Quantity = "1",
                                Category = "PHYSICAL_GOODS"
                            }
                        },
                        ShippingDetail = new ShippingDetail
                        {
                            Name = new Name{ FullName = "John Doe" },
                            AddressPortable = new AddressPortable
                            {
                                AddressLine1 = "123 Townsend St",
                                AddressLine2 = "Floor 6",
                                AdminArea2 = "San Francisco",
                                AdminArea1 = "CA",
                                PostalCode = "94107",
                                CountryCode = "US"
                            }
                        }
                    }
                }
            };
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");

            request.RequestBody(orderRequest);
            //3. Call PayPal to set up a transaction
            var response = await PayPalClient.GetClient().Execute(request);
            var v= response.Result<Order>().Id;
            var model = _cvaction.get(id);
            model.payid = response.Result<Order>().Id;
            _cvaction.update(model);

                return  RedirectToAction("paypal", "Check", new
                {
                    id = v,
                });


        }

        public IActionResult paypal(string id)
        {
            ViewBag.idcv = id;
            return View();
        }

        public async Task<IActionResult> done(string id)
        {
            var request = new OrdersCaptureRequest(id);
            
            request.Prefer("return=representation");

            request.RequestBody(new OrderActionRequest());
            //3. Call PayPal to set up a transaction
            var response = await PayPalClient.GetClient().Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var model = _cvaction.getpay(id);
                model.paying = true;
                    _cvaction.update(model); 
                return View();
            }
            else
            {
                return base.Forbid();
            }
        }
    }
}
