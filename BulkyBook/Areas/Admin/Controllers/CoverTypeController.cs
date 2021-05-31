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
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork __unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            __unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();

            if(id == null)
            {
                return View(coverType);
            }

            coverType = __unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if(coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }




        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = __unitOfWork.CoverType.GetAll();
            return Json(new { data = allObj });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if(ModelState.IsValid)
            {
                if(coverType.Id == 0)
                {
                    __unitOfWork.CoverType.Add(coverType);
                   
                }
                else
                {
                    __unitOfWork.CoverType.Update(coverType);
                    
                }
                __unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = __unitOfWork.CoverType.Get(id);
            if(objFromDb != null)
            {
                __unitOfWork.CoverType.Remove(objFromDb);
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
