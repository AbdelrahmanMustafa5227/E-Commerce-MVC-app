using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MVC_Web_Application.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger , IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            
            IEnumerable<Product> productList = _unitOfWork.ProductRepo.GetAll(includeProperties:"Category");
            return View(productList);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new ShoppingCart()
            {
                Product = _unitOfWork.ProductRepo.Get(u => u.Id == productId, includeProperties: "Category"),
                ProductId = productId,
                Count =1

            };
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            var cartFromDb = _unitOfWork.ShoppingCartRepo.Get(u=>u.ApplicationUserId == userId &&
                u.ProductId == cart.ProductId);
            if(cartFromDb != null)
            {
                cartFromDb.Count += cart.Count;
                _unitOfWork.Save();
                _unitOfWork.ShoppingCartRepo.Update(cartFromDb);
                
            }
            else
            {
                _unitOfWork.ShoppingCartRepo.Add(cart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32("SCS", _unitOfWork.ShoppingCartRepo.GetAll(u => u.ApplicationUserId == userId).Count());
                
            }
            
            TempData["success"] = "Cart has been updated";
            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
