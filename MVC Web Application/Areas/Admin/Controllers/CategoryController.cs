using Microsoft.AspNetCore.Mvc;
using DataAccess.Data;
using Models;
using DataAccess.Repository.IRepository;
using _Utilities;
using Microsoft.AspNetCore.Authorization;

namespace MVC_Web_Application.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfwork)
        {
            _unitOfWork = unitOfwork;
        }
        public IActionResult Index()
        {
            List<Category> categoryList = _unitOfWork.CategoryRepo.GetAll().ToList();
            return View(categoryList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (char.IsDigit(obj.name[0]))
            {
                ModelState.AddModelError("name", "Category Name Cannot Start With a Number");
            }
            if (obj.name == "test")
            {
                // Error Only appears in asp-validation-summary
                ModelState.AddModelError("", "Invalid Name");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepo.Add(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Category Was Created Successfully !";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? cat = _unitOfWork.CategoryRepo.Get(u => u.Id == id);

            if (cat == null)
            {
                return NotFound();
            }
            return View(cat);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (char.IsDigit(obj.name[0]))
            {
                ModelState.AddModelError("name", "Category Name Cannot Start With a Number");
            }
            if (obj.name == "test")
            {
                // This Error Only appears in asp-validation-summary
                ModelState.AddModelError("", "Invalid Name");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepo.Update(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Category Was Updated Successfully !";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? cat = _unitOfWork.CategoryRepo.Get(U => U.Id == id);

            if (cat == null)
            {
                return NotFound();
            }
            return View(cat);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(Category obj)
        {
            _unitOfWork.CategoryRepo.Remove(obj);
            _unitOfWork.Save();
            TempData["Success"] = "Category Was Deleted Successfully !";
            return RedirectToAction("Index");
        }
    }

}

