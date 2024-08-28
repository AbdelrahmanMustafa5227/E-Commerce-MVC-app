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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfwork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfwork;
            _webHostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> ProductList = _unitOfWork.ProductRepo.GetAll(includeProperties:"Category").ToList();
            return View(ProductList);
        }
        
        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> Category = _unitOfWork.CategoryRepo.GetAll().Select(u => new SelectListItem
            {
                Text = u.name,
                Value = u.Id.ToString()
            });
            ProductVM productVM = new()
            {
                CatList = Category,
                Product = new Product()
            };
            if(id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.ProductRepo.Get(u=> u.Id == id);
                return View(productVM);
            }
            
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj , IFormFile? asd)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if(asd != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(asd.FileName);
                    string productPath = Path.Combine(rootPath, @"images\product");
                    if (!string.IsNullOrEmpty(obj.Product.ImageURL))
                    {
                        string oldPath = Path.Combine(rootPath,obj.Product.ImageURL.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        asd.CopyTo(fileStream);
                    };
                    obj.Product.ImageURL = @"\images\product\" + fileName;
                }
                
                if(obj.Product.Id == 0)
                {
                    _unitOfWork.ProductRepo.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.ProductRepo.Update(obj.Product);
                }
                _unitOfWork.Save();
                TempData["Success"] = "Product Was Created Successfully !";
                return RedirectToAction("Index");
            }
            else
            {
                obj.CatList = _unitOfWork.CategoryRepo.GetAll().Select(u => new SelectListItem
                {
                    Text = u.name,
                    Value = u.Id.ToString()
                });

                return View(obj);

            }
        }
       
        public IActionResult Delete(int id)
        {
            Product? obj = _unitOfWork.ProductRepo.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Delete(Product obj)
        {
            string rootPath = _webHostEnvironment.WebRootPath;
            Product a = _unitOfWork.ProductRepo.Get(u=> u.Id == obj.Id);

            if (!string.IsNullOrEmpty(a.ImageURL))
            {
                string oldPath = Path.Combine(rootPath, a.ImageURL.TrimStart('\\'));
                if (System.IO.File.Exists(oldPath)) { System.IO.File.Delete(oldPath); }
            }
            _unitOfWork.ProductRepo.Remove(a);
            _unitOfWork.Save();
            TempData["Success"] = "Product Was Deleted Successfully !";
            return RedirectToAction("Index");
        }
        
    }
}
