using _Utilities;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using System.Diagnostics;

namespace MVC_Web_Application.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfwork)
        {
            _unitOfWork = unitOfwork;
        }
        public IActionResult Index()
        {
            List<Company> CompanyList = _unitOfWork.CompanyRepo.GetAll().ToList();
            return View(CompanyList);
        }
        
        public IActionResult Upsert(int? id)
        {
            
            if(id == null)
            {
                return View(new Company());
            }
            else
            {
                Company company = _unitOfWork.CompanyRepo.Get(u=> u.Id == id);
                return View(company);
            }
            
        }
        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if(obj.Id == 0)
                {
                    _unitOfWork.CompanyRepo.Add(obj);
                }
                else
                {
                    _unitOfWork.CompanyRepo.Update(obj);
                }
                _unitOfWork.Save();
                TempData["Success"] = "Company Was Created Successfully !";
                return RedirectToAction("Index");
            }
            else
            {             
                return View(obj);
            }
        }
       
        public IActionResult Delete(int id)
        {
            Company? obj = _unitOfWork.CompanyRepo.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Delete(Company obj)
        {
            _unitOfWork.CompanyRepo.Remove(obj);
            _unitOfWork.Save();
            TempData["Success"] = "Company Was Deleted Successfully !";
            return RedirectToAction("Index");
        }
        
    }
}
