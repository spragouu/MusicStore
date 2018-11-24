using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MusicStore.Models;

namespace MusicStore.Controllers
{
    [Authorize]
    public class AlbumsController : Controller
    {
        //private MusicStoreModel db = new MusicStoreModel();
        private IAlbumsMock db;

        public AlbumsController()
        {
            //if nothing passed to constructor, connect to the db (this is the default)
            this.db = new EFAlbums();
        }

        public AlbumsController(IAlbumsMock albumsMock)
        {
            //if we pass a mock object to the constructor, we are unit testing so no db
            this.db = albumsMock;
        }

        // GET: Albums
        public ActionResult Index()
        {
            //var albums = db.Albums.Include(a => a.Artist).Include(a => a.Genre);
            //return View(albums.OrderBy(a => a.Artist.Name).ThenBy(a => a.Title).ToList());

            var albums = db.Albums.OrderBy(a => a.Artist.Name).ThenBy(a => a.Title).ToList();
            return View("Index", albums);
        }

        [AllowAnonymous]
        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return View("Error");
            }
            // original find code
            // Album album = db.Albums.Find(id);

            // new code to work with both the db and the mock interface
            Album album = db.Albums.SingleOrDefault(a => a.AlbumId == id);

            if (album == null)
            {
                // return HttpNotFound();
                return View("Error");
            }
            return View("Details", album);
        }

        // GET: Albums/Create
        public ActionResult Create()
        {
            ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name");
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name");
            return View("Create");
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AlbumId,GenreId,ArtistId,Title,Price,AlbumArtUrl")] Album album)
        {
            if (ModelState.IsValid)
            {
                //db.Albums.Add(album);
                //db.SaveChanges();
                db.Save(album);
                return RedirectToAction("Index");
            }

            ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
            return View("Create", album);
        }

        // GET: Albums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return View("Error");
            }
            //Album album = db.Albums.Find(id);
            Album album = db.Albums.SingleOrDefault(a => a.AlbumId == id);
            if (album == null)
            {
                //return HttpNotFound();
                return View("Error");
            }
            ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
            return View("Edit", album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AlbumId,GenreId,ArtistId,Title,Price,AlbumArtUrl")] Album album)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(album).State = EntityState.Modified;
                db.Save(album); // Changes();
                return RedirectToAction("Index");
            }
            ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
            return View("Edit", album);
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return View("Error"); // new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Album album = db.Albums.Find(id);
            Album album = db.Albums.SingleOrDefault(a => a.AlbumId == id);
            if (album == null)
            {
                return View("Error"); // HttpNotFound();
            }
            return View("Delete", album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            //Album album = db.Albums.Find(id);
            //db.Albums.Remove(album);
            //db.SaveChanges();
            if (id == null)
            {
                return View("Error");
            }

            Album album = db.Albums.SingleOrDefault(a => a.AlbumId == id);

            if (album == null)
            {
                return View("Error");
            }

            db.Delete(album);

            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
