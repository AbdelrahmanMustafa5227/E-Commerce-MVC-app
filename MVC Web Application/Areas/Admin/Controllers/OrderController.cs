using _Utilities;
using DataAccess.Data;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Models;
using Models.ViewModels;
using Stripe;
using System.Diagnostics;

namespace MVC_Web_Application.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(string status)
        {
            IEnumerable<OrderHeader> orders = _unitOfWork.OrderHeaderRepo.GetAll(includeProperties:"User").ToList();

            switch (status)
            {
                case "InProcess":
                    orders = orders.Where(u => u.OrderStatus == StaticDetails.StatusInProcess);
                    break;
                case "Pending":
                    orders = orders.Where(u => u.OrderStatus == StaticDetails.PaymentPending);
                    break;
                case "Completed":
                    orders = orders.Where(u => u.OrderStatus == StaticDetails.StatusShipped);
                    break;
                case "Approved":
                    orders = orders.Where(u => u.OrderStatus == StaticDetails.StatusApproved);
                    break;
                default:
                    break;
            }
            return View(orders);
        }
        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new OrderVM
            {
                orderHeader = _unitOfWork.OrderHeaderRepo.Get(u=> u.Id == orderId , includeProperties:"User"),
                orderDetails = _unitOfWork.OrderDetailRepo.GetAll(u => u.OrderId == orderId,includeProperties:"Product")
            };
            return View(orderVM);
        }
        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin+","+ StaticDetails.Role_Company)]
        public IActionResult UpdateOrderDetails(OrderVM orderVM)
        {

            OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeaderRepo.Get(u => u.Id == orderVM.orderHeader.Id);
            orderHeaderFromDb.Name = orderVM.orderHeader.Name;
            orderHeaderFromDb.PhoneNumber = orderVM.orderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderVM.orderHeader.StreetAddress;
            orderHeaderFromDb.City = orderVM.orderHeader.City;
            orderHeaderFromDb.State = orderVM.orderHeader.State;
            orderHeaderFromDb.PostalCode = orderVM.orderHeader.PostalCode;
            orderHeaderFromDb.Carrier = orderVM.orderHeader.Carrier;
            orderHeaderFromDb.TrackingNumber = orderVM.orderHeader.TrackingNumber;
            _unitOfWork.OrderHeaderRepo.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Updated Successfully !";
            return RedirectToAction("Details",new { orderId = orderHeaderFromDb.Id});
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Company)]
        public IActionResult StartProcessing(int id)
        {
            _unitOfWork.OrderHeaderRepo.UpdateStatus(id, StaticDetails.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Is Now In Process !";

            return RedirectToAction("Details", new { orderId = id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Company)]
        public IActionResult Shipping(OrderVM orderVM)
        {
            OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeaderRepo.Get(u => u.Id == orderVM.orderHeader.Id);
            orderHeaderFromDb.Carrier = orderVM.orderHeader.Carrier;
            orderHeaderFromDb.TrackingNumber = orderVM.orderHeader.TrackingNumber;
            orderHeaderFromDb.OrderStatus = StaticDetails.StatusShipped;
            orderHeaderFromDb.ShippingDate = DateTime.Now;

            if(orderHeaderFromDb.PaymentStatus== StaticDetails.PaymentDelayed)
            {
                orderHeaderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _unitOfWork.OrderHeaderRepo.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Order Is Now Shipped !";

            return RedirectToAction("Details", new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Company)]
        public IActionResult CancelOrder(OrderVM orderVM)
        {
            OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeaderRepo.Get(u => u.Id == orderVM.orderHeader.Id);

            if(orderHeaderFromDb.PaymentStatus == StaticDetails.PaymentApproved) 
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeaderFromDb.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(option);
            }

            _unitOfWork.OrderHeaderRepo.UpdateStatus(orderHeaderFromDb.Id, StaticDetails.StatusCancelled , StaticDetails.PaymentRefunded);
            _unitOfWork.Save();
            TempData["Success"] = "Order Is Now Shipped !";

            return RedirectToAction("Details", new { orderId = orderHeaderFromDb.Id });
        }
        
    }
}
