using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using System.Security.Claims;

namespace MVC_Web_Application.ViewComponent
{
    public class ShoppingCartCountViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartCountViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if(HttpContext.Session.GetInt32("SCS") == null)
                    HttpContext.Session.SetInt32("SCS", _unitOfWork.ShoppingCartRepo.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                return View (HttpContext.Session.GetInt32("SCS"));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
