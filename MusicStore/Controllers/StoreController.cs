using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//refernece models namespace
using MusicStore.Models;

namespace MusicStore.Controllers
{
    public class StoreController : Controller
    {
        //connect to database
        MusicStoreModel db = new MusicStoreModel();
        // GET: Store
        public ActionResult Index()
        {
            var genres = db.Genres.OrderBy(g => g.Name).ToList();
            return View(genres);
        }

        //GET: Store/Product
        public ActionResult Product(string ProductName)
        {
            ViewBag.ProductName = ProductName;
            return View();
        }

        //GET: Store/Albums
        public ActionResult Albums(string genre)
        {
            //mock up some album data
            //var albums = new List<Album>();

            //for (int i = 1; i<=10; i++)
            //{
            //    albums.Add(new Album { Title = "Album" + i.ToString() });
            //}
            var albums = db.Albums.Where(a => a.Genre.Name == genre).OrderBy(a => a.Title).ToList();
            ViewBag.genre = genre;
            return View(albums);
        }
    }
}