using _Utilities;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Security.Claims;

namespace MVC_Web_Application.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepo.GetAll(u => u.ApplicationUserId == userId,
                    includeProperties: "Product"),
                Header = new()
            };

            foreach (ShoppingCart cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceByCount(cart);
                shoppingCartVM.Header.OrderTotal += cart.Price * cart.Count;
            }

            return View(shoppingCartVM);
        }
        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepo.GetAll(u => u.ApplicationUserId == userId,
                    includeProperties: "Product"),
                Header = new()
            };

            shoppingCartVM.Header.User = _unitOfWork.ApplicationUserRepo.Get(u => u.Id == userId);

            shoppingCartVM.Header.Name = shoppingCartVM.Header.User.Name;
            shoppingCartVM.Header.StreetAddress = shoppingCartVM.Header.User.StreetAddress;
            shoppingCartVM.Header.City = shoppingCartVM.Header.User.City;
            shoppingCartVM.Header.State = shoppingCartVM.Header.User.State;
            shoppingCartVM.Header.PostalCode = shoppingCartVM.Header.User.PostalCode;
            shoppingCartVM.Header.PhoneNumber = shoppingCartVM.Header.User.PhoneNumber;

            foreach (ShoppingCart cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceByCount(cart);
                shoppingCartVM.Header.OrderTotal += cart.Price * cart.Count;
            }

            return View(shoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            
            shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepo.GetAll(u => u.ApplicationUserId == userId,
                    includeProperties: "Product"),
                Header = new()
            };

            shoppingCartVM.Header.ApplicationUserId = userId;
            shoppingCartVM.Header.OrderDate = DateTime.Now;
            ApplicationUser AppUser = _unitOfWork.ApplicationUserRepo.Get(u => u.Id == userId);
            shoppingCartVM.Header.Name = AppUser.Name;
            shoppingCartVM.Header.StreetAddress = AppUser.StreetAddress;
            shoppingCartVM.Header.City = AppUser.City;
            shoppingCartVM.Header.State = AppUser.State;
            shoppingCartVM.Header.PostalCode = AppUser.PostalCode;
            shoppingCartVM.Header.PhoneNumber = AppUser.PhoneNumber;

            foreach (ShoppingCart cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceByCount(cart);
                shoppingCartVM.Header.OrderTotal += cart.Price * cart.Count;
            }
            if (AppUser.CompanyId.GetValueOrDefault() == 0)
            {
                shoppingCartVM.Header.OrderStatus = StaticDetails.StatusPending;
                shoppingCartVM.Header.PaymentStatus = StaticDetails.PaymentPending;
            }
            else
            {
                shoppingCartVM.Header.OrderStatus = StaticDetails.StatusApproved;
                shoppingCartVM.Header.PaymentStatus = StaticDetails.PaymentDelayed;
            }
            _unitOfWork.OrderHeaderRepo.Add(shoppingCartVM.Header);
            _unitOfWork.Save();

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = shoppingCartVM.Header.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetailRepo.Add(orderDetail);
                _unitOfWork.Save();
            }
            if (AppUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = "https://localhost:7155/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartVM.Header.Id}",
                    CancelUrl = domain + "customer/cart/Index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment"
                    };

                foreach(var item in shoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }  
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Create(options);
                _unitOfWork.OrderHeaderRepo.UpdateStripeHeaderId(shoppingCartVM.Header.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.Header.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepo.Get(u=>u.Id == id);
            if (orderHeader.PaymentStatus != StaticDetails.PaymentDelayed)
            {
                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Get(orderHeader.SessionId); 
                if(session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaderRepo.UpdateStripeHeaderId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeaderRepo.UpdateStatus(id, StaticDetails.StatusApproved, StaticDetails.PaymentApproved);
                    _unitOfWork.Save();
                }
                List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCartRepo.GetAll(u=>
                    u.ApplicationUserId==orderHeader.ApplicationUserId).ToList();
                _unitOfWork.ShoppingCartRepo.RemoveRange(shoppingCarts);
                _unitOfWork.Save();
            }
            return View(id);
        }
        public IActionResult Plus(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCartRepo.Get(u => u.Id == cartId);
            cart.Count++;
            _unitOfWork.ShoppingCartRepo.Update(cart);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult Minus(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCartRepo.Get(u => u.Id == cartId);
            cart.Count--;
            _unitOfWork.ShoppingCartRepo.Update(cart);

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult Remove(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCartRepo.Get(u => u.Id == cartId , tracked:true);
            HttpContext.Session.SetInt32("SCS", _unitOfWork.ShoppingCartRepo.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).Count() - 1);
            _unitOfWork.ShoppingCartRepo.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public double GetPriceByCount(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else
            {
                if (cart.Count <= 100)
                {
                    return cart.Product.Price50;
                }
                else
                {
                    return cart.Product.Price100;
                }
            }
        }
    }
}
