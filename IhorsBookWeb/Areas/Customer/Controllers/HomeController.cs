using IhorsBook.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using IhorsBook.DataAccess.Repository.IRepository;
using IhorsBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace IhorsBookWeb.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
	[HttpGet]
	public IActionResult Index(string searchString, int? category, int? cover)
    {
        ViewData["currentFilter"] = searchString;

		var categories = _unitOfWork.Category.GetAll();
		ViewData["Categories"] = categories;

		var covers = _unitOfWork.CoverType.GetAll();
		ViewData["Covers"] = covers;

		var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

		if (!String.IsNullOrEmpty(searchString))
        {
            productList = productList.Where(s => s.Title.Contains(searchString)
                            || s.Author.Contains(searchString)
                            || s.Category.Name.Contains(searchString)
						 );
        }
        else if( category != null)
        {
            productList = productList.Where(s => s.CategoryId == category);

		}
        else if(cover != null)
        {
            productList = productList.Where(s => s.CoverTypeId == cover);
        }

		return View(productList);
    }
    public IActionResult Details(int productId)
    {
        ShoppingCart cartObj = new()
        {
            Count = 1,
            ProductId = productId,
            Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType"),
        };

        return View(cartObj);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        shoppingCart.ApplicationUserId = claim.Value;

        ShoppingCart cartFromBd = _unitOfWork.ShoppingCart.GetFirstOrDefault(
            u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);
        if(cartFromBd == null)
        {
            _unitOfWork.ShoppingCart.Add(shoppingCart);
        }
        else
        {
            _unitOfWork.ShoppingCart.IncrementCount(cartFromBd,shoppingCart.Count);
        }
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

	[HttpPost]
	public IActionResult Search(string searchQuery)
	{
		var results = _unitOfWork.Product
			.GetAll(e => e.Title.Contains(searchQuery))
			.ToList();

		// Pass the search results to the view
		return View(results);
	}
}
