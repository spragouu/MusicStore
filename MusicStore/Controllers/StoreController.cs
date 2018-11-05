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

        // GET: Store/AddToCart/5
        public ActionResult AddToCart(int AlbumId)
        {
            // create new cart item
            GetCartId();
            string CurrentCartId = Session["CartId"].ToString();

            // check if album is already in the user's cart
            Cart cartItem = db.Carts.SingleOrDefault(c => c.CartId == CurrentCartId
                && c.AlbumId == AlbumId);

            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    AlbumId = AlbumId,
                    Count = 1,
                    DateCreated = DateTime.Now,
                    CartId = CurrentCartId
                };

                // save to db
                db.Carts.Add(cartItem);
            }
            else
            {
                // increment the cout
                cartItem.Count++;
            }

            db.SaveChanges();

            // show cart page
            return RedirectToAction("ShoppingCart");
        }

        private void GetCartId()
        {
            // is there already a CartId?
            if (Session["CartId"] == null)
            {
                // is user logged in?
                if (User.Identity.Name == "")
                {
                    // generate unique id that is session-specific
                    Session["CartId"] = Guid.NewGuid().ToString();
                }
                else
                {
                    Session["CartId"] = User.Identity.Name;
                }
            }
        }

        // GET: Store/ShoppingCart
        public ActionResult ShoppingCart()
        {
            // get current cart for display
            GetCartId();
            string CurrentCartId = Session["CartId"].ToString();

            var cartItems = db.Carts.Where(c => c.CartId == CurrentCartId);

            // load view and pass it the list of items in this user's cart
            return View(cartItems);
        }

        // GET: Store/RemoveFromCart/5
        public ActionResult RemoveFromCart(int RecordId)
        {
            // remove item from this user's cart
            Cart cartItem = db.Carts.SingleOrDefault(c => c.RecordId == RecordId);
            db.Carts.Remove(cartItem);
            db.SaveChanges();

            // reload cart page
            return RedirectToAction("ShoppingCart");
        }

        [Authorize]
        // GET: Store/Checkout
        public ActionResult Checkout()
        {
            MigrateCart();
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        // POST: Store/Checkout
        public ActionResult Checkout(FormCollection values)
        {
            // create a new order and populate from the form values
            Order order = new Order();
            TryUpdateModel(order);

            // populate the 4 automatic order fields
            order.Username = User.Identity.Name;
            order.Email = User.Identity.Name;
            order.OrderDate = DateTime.Now;

            var cartItems = db.Carts.Where(c => c.CartId == order.Username);
            decimal CartTotal = (from c in cartItems
                                 select (int)c.Count * c.Album.Price).Sum();

            order.Total = CartTotal;

            // save the order
            db.Orders.Add(order);
            db.SaveChanges();

            // save the items
            foreach (Cart item in cartItems)
            {
                OrderDetail od = new OrderDetail
                {
                    OrderId = order.OrderId,
                    AlbumId = item.AlbumId,
                    Quantity = item.Count,
                    UnitPrice = item.Album.Price
                };

                db.OrderDetails.Add(od);
            }

            db.SaveChanges();
            return RedirectToAction("Details", "Orders", new { id = order.OrderId });
        }

        public void MigrateCart()
        {
            if (!String.IsNullOrEmpty(Session["CartId"].ToString()) && User.Identity.IsAuthenticated)
            {
                // if the user has been shopping anonymously, now attach cart to their username
                if (Session["CartId"].ToString() != User.Identity.Name)
                {
                    string CurrentCartId = Session["CartId"].ToString();
                    // get items with the random id
                    var cartItems = db.Carts.Where(c => c.CartId == CurrentCartId);

                    foreach (Cart item in cartItems)
                    {
                        item.CartId = User.Identity.Name;
                    }

                    // attach all the items to this username
                    db.SaveChanges();
                    Session["CartId"] = User.Identity.Name;
                }
            }
        }
    }
}