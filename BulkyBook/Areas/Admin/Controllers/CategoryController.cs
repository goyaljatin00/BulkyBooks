using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork __unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            __unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();

            if(id == null)
            {
                return View(category);
            }

            category = __unitOfWork.Category.Get(id.GetValueOrDefault());
            if(category == null)
            {
                return NotFound();
            }

            return View(category);
        }




        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = __unitOfWork.Category.GetAll();
            return Json(new { data = allObj });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if(ModelState.IsValid)
            {
                if(category.Id == 0)
                {
                    __unitOfWork.Category.Add(category);
                   
                }
                else
                {
                    __unitOfWork.Category.Update(category);
                    
                }
                __unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = __unitOfWork.Category.Get(id);
            if(objFromDb != null)
            {
                __unitOfWork.Category.Remove(objFromDb);
                __unitOfWork.Save();
                return Json(new { success = true, message = "Delete Successfull" });
            }
            else
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

        }

        #endregion
    }
}
