using Microsoft.AspNetCore.Mvc;
using Project1.Models.DBModels;
using Project1.Service;

namespace Project1.Controllers
{
    public class BasketController : Controller
    {
        private readonly DBService bdService;
        public BasketController(DBService _bdService)
        {
            bdService = _bdService;
        }
        public IActionResult AddToBasket(int idProduct)
        {
            int? userId = UserVerification.checAuthorization(HttpContext);
            if (userId != null)
            {
                if(bdService.AddBasket(userId, idProduct))
                {
                    return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Додано до корзини");
                }
            }
            return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Виникла помилка при додаванні у корзину");
        }
        public IActionResult GetBasket()
        {
            int? userId = UserVerification.checAuthorization(HttpContext);
            if (userId != null)
            {
                List<ProductModel> products;
                products = bdService.GetBasketUser(userId);
                return View(products);
            }
            return View();
        }
        public IActionResult DeleteToBasket(int idProduct)
        {
            int? userId = UserVerification.checAuthorization(HttpContext);
            if (userId != null)
            {
                bdService.DeleteToBasket(userId,idProduct);
            }
            return RedirectToAction("GetBasket", "Basket");
        }
    }
}
