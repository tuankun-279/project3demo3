using PagedList;
using Project_Real__estate.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;


namespace Project_Real__estate.Controllers
{
    public class HomeController : Controller
    {
        private projectEntities db = new projectEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult NewPost()
        {
            ViewBag.AgentId = new SelectList(db.Agents, "AgentId", "AgentName");
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            ViewBag.PaymentId = new SelectList(db.Payments, "PaymentId", "PaymentName");
            ViewBag.SellerId = new SelectList(db.Sellers, "SellerId", "Name");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName");
            ViewBag.Type = new List<SelectListItem>()
            {
                new SelectListItem() { Value="1", Text= "Bán" },
                new SelectListItem() { Value="2", Text= "Thuê" },
                new SelectListItem() { Value="3", Text= "Dự Án" }
             };
            return View();
        }

        // POST: Advertisements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPost([Bind(Include = "adsId,Tiltle,ReleaseDate,ExpirationDate,SellerId,AgentId,PaymentId,CategoryId,Describe,CurrentSymbol,priceOfAds,EstatePrice,Facade,Gateway,floors,Bedrooms,Toilets,furniture,Area,Cityprovince,District,Ward,Street,isActivate,UserId,StatusHouse")] Advertisement advertisement)
        {
            if (ModelState.IsValid)
            {
                List<Image> imglist = new List<Image>();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        Image img = new Image()
                        {
                            FileName = fileName,
                            Extension = Path.GetExtension(fileName)
                        };
                        imglist.Add(img);

                        var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        file.SaveAs(path);

                        WebImage imgSize = new WebImage(file.InputStream);
                        if (imgSize.Width > 200)
                            imgSize.Resize(200, 200);
                        imgSize.Save(path);
                    }
                }

                advertisement.Reports = new List<Report>() {
                    new Report() { ReportDate= DateTime.Now, AdsId = advertisement.adsId, AgentId = advertisement.AgentId, SellerId = advertisement.SellerId, Price = advertisement.priceOfAds } };
                
                advertisement.Images = imglist;
                db.Advertisements.Add(advertisement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AgentId = new SelectList(db.Agents, "AgentId", "AgentName", advertisement.AgentId);
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", advertisement.CategoryId);
            ViewBag.PaymentId = new SelectList(db.Payments, "PaymentId", "PaymentName", advertisement.PaymentId);
            ViewBag.SellerId = new SelectList(db.Sellers, "SellerId", "Name", advertisement.SellerId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", advertisement.UserId);
            ViewBag.Type = new List<SelectListItem>()
            {
                new SelectListItem() { Value="1", Text= "Bán" },
                new SelectListItem() { Value="2", Text= "Thuê" },
                new SelectListItem() { Value="3", Text= "Dự Án" }
             };
            return View(advertisement);
        }


        public ActionResult ListPost()
        {
            var advertisements = db.Advertisements.Include(a => a.Agent).Include(a => a.Category).Include(a => a.Payment).Include(a => a.Seller).Include(a => a.User).Where(a => a.isActivate == true);
            return View(advertisements);
           
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advertisement advertisement = db.Advertisements.Find(id);


            if (advertisement == null)
            {
                return HttpNotFound();
            }
            var images = new List<Image>();
            images = db.Images.Where(x => x.AdsId == id).ToList();
            ViewBag.file = images;
            return View(advertisement);
        }
    }
}