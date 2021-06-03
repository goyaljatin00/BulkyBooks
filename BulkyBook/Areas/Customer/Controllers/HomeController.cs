using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork __unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            __unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = __unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                var count = __unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value).ToList().Count;

                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
            }

            return View(productList);


        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var productFromDb = __unitOfWork.Product.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType");
            ShoppingCart shoppingCart = new ShoppingCart
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };

            return View(shoppingCart);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(ShoppingCart cartObject)
        {
            cartObject.Id = 0;
            if(ModelState.IsValid)
            {
                //Add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = __unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == cartObject.ApplicationUserId &&
                    u.ProductId == cartObject.ProductId,
                    includeProperties: "Product"
                    );

                if(cartFromDb == null)
                {
                    __unitOfWork.ShoppingCart.Add(cartObject);
                }
                else
                {
                    cartFromDb.Count += cartObject.Count; ;
                    __unitOfWork.ShoppingCart.Update(cartFromDb);
                }

                __unitOfWork.Save();
                var count = __unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == cartObject.ApplicationUserId).ToList().Count;

                //              HttpContext.Session.SetObject(SD.ssShoppingCart, cartObject);
                HttpContext.Session.SetObject(SD.ssShoppingCart, count);
                //               var obj = HttpContext.Session.GetObject<ShoppingCart>(SD.ssShoppingCart);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productFromDb = __unitOfWork.Product.GetFirstOrDefault(u => u.Id == cartObject.ProductId, includeProperties: "Category,CoverType");
                ShoppingCart shoppingCart = new ShoppingCart
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };

                return View(shoppingCart);
            }

        }
    }
}
