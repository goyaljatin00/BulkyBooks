using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork __unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            __unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company = new Company();

            if (id == null)
            {
                return View(company);
            }

            company = __unitOfWork.Company.Get(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }




        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = __unitOfWork.Company.GetAll();
            return Json(new { data = allObj });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    __unitOfWork.Company.Add(company);

                }
                else
                {
                    __unitOfWork.Company.Update(company);

                }
                __unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = __unitOfWork.Company.Get(id);
            if (objFromDb != null)
            {
                __unitOfWork.Company.Remove(objFromDb);
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
