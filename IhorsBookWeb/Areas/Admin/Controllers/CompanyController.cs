using IhorsBook.DataAccess;
using System;
using System.IO;
using IhorsBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using IhorsBook.Models;
using IhorsBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IhorsBookWeb.Controllers;
[Area("Admin")]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //IEnumerable<CoverType> objCoverTypeList = _unitOfWork.CoverType.GetAll();
            //return View(objCoverTypeList);
            return View();
        }
        //GET
        public IActionResult Upsert(int? id)
        {
        Company company = new();

        if (id == null || id == 0)
        {
            return View(company);
        }
        else
        {
            company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            return View(company);
        }
    }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if(obj.Id == 0)
                {
                    _unitOfWork.Company.Add(obj);
                    TempData["success"] = "Company created successfully";

                }
            else
                {
                    _unitOfWork.Company.Update(obj);
                TempData["success"] = "Product updated successfully";

                }
            _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var companyList = _unitOfWork.Company.GetAll();
        return Json(new { data = companyList });
    }
    //POST
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new {success = false, massage = "Error while deleting"});
        }

        _unitOfWork.Company.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, massage = "Delete Successful" });
    }
    #endregion
}

