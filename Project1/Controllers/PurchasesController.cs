using Microsoft.AspNetCore.Mvc;
using Project1.Models.DBModels;
using Project1.Models.ViewsModel;
using Project1.Service;
using System.Reflection.Metadata;

namespace Project1.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly DBService bdService;
        public PurchasesController(DBService _bdService)
        {
            bdService = _bdService;
        }
        public IActionResult Buy(int idProduct, int quantity)
        {
            int? userId = UserVerification.checAuthorization(HttpContext);
            if (userId != null)
            {
                if (bdService.AddPurchases(userId, idProduct, quantity))
                {
                    return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Товар успішно куплено");
                }
            }
            return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Виникла помилка при покупці");
        }
        public IActionResult GetAllPurchases(int? idUser) 
        {
            if(idUser == null) {
                int? userId = UserVerification.checAuthorization(HttpContext);
                return View(bdService.GetPurchasesUser(userId));
            }
            return View(bdService.GetPurchasesUser(idUser));
        }
        public IActionResult Purchase(int idProduct, int quantity)
        {
            int? userId = UserVerification.checAuthorization(HttpContext);
            ProductModel Product = bdService.GetProduct(idProduct);
            UserModel user = bdService.GetUser(userId);
             BuyPurchaseViewModel buy = new()
             {
                 Product = Product,
                 Quantity = quantity,
                 User = user
             };
             return View(buy);
        }
        public IActionResult _LayoutAction()
        {
            int? idUser = UserVerification.checAuthorization(HttpContext);
            
            return RedirectToAction("GetAllPurchases", "Purchases", idUser);
        }
    }
}
