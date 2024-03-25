using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using Project1.Models.DBModels;
using Project1.Service;

namespace Project1.Controllers
{
    public class AllProductController : Controller
    {
        private readonly DBService bdService;
        public AllProductController(DBService _bdService)
        {
            bdService = _bdService;
        }
        public IActionResult Index(int? category)
        {
            List<ProductModel> products;


            if (category == null)
            {
                products = bdService.GetAllProduct();
            }
            else
            {
                products = bdService.GetCategoryProduct(category);
            }

            return View(products);
        }
        public IActionResult Views()
        {
            return View(bdService.GetAllViews());
        }
        public IActionResult GetViewsUser() 
        {
            if (User.Identity.IsAuthenticated)
            {
                return View("~/Views/User/GetViewsUser.cshtml", bdService.GetViewsUser(UserVerification.checAuthorization(HttpContext)));
            }
            return View("~/Views/User/GetViewsUser.cshtml", null);
        }
        public IActionResult DetailsProduct(int id) 
        {
            ProductModel product = bdService.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                bdService.AddViews(UserVerification.checAuthorization(HttpContext), id);
                ViewBag.IdUserAuthoriz = UserVerification.checAuthorization(HttpContext);
            }
            List<Comments> commentsPr = bdService.GetProductComments(id);
            foreach (Comments comment in commentsPr)
            {
                comment.User = bdService.GetUser(comment.IdUser);
            }
            commentsPr.Reverse();
            ViewBag.Comments = commentsPr;
                ViewBag.Categories = bdService.GetAllCategory();
            return View(product);
        }
        public IActionResult SearchProduct(string text)
        {
            return View("~/Views/AllProduct/Index.cshtml", bdService.SearchProduct(text));
        }

        public IActionResult FindFilterProduct(int idCategory, int priceRange, bool availableOnly)
        {
            return View("~/Views/AllProduct/Index.cshtml", bdService.FindFilterProduct(idCategory,priceRange,availableOnly));
        }
    }
}
