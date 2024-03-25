using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project1.Models.DBModels;
using Project1.Models;
using Project1.Service;
using System.Security.Claims;

namespace Project1.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly DBService bdService;
        public AdminController(DBService _bdService, IWebHostEnvironment hostingEnvironment)
        {
            bdService = _bdService;
            _hostingEnvironment = hostingEnvironment;
        }

        [Authorize(Roles = "Адмін, Головний Адмін")]
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel model, IFormFile formSignupImgFile)
        {
            try
            {
                if (formSignupImgFile != null && formSignupImgFile.Length > 0 && model.id != 0)
                {
                    var fileName = formSignupImgFile.FileName;

                    var uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "img", "productImg");

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    using (var stream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create))
                    {
                        await formSignupImgFile.CopyToAsync(stream);
                    }
                    model.img = "~/img/productImg/" + fileName;
                }
                if (model.id != 0)
                {
                    ProductModel obj = bdService.GetProduct(model.id);
                    if (obj != null)
                    {
                        obj.name = string.IsNullOrEmpty(model.name) ? obj.name : model.name;
                        obj.img = string.IsNullOrEmpty(model.img) ? obj.img : model.img;
                        obj.price = model.price == 0 ? obj.price : model.price;
                        obj.discount = model.discount == 0 ? obj.discount : model.discount;
                        obj.short_description = string.IsNullOrEmpty(model.short_description) ? obj.short_description : model.short_description;
                        obj.long_description = string.IsNullOrEmpty(model.long_description) ? obj.long_description : model.long_description;
                        obj.is_favorite = model.is_favorite;
                        obj.available = model.available;
                        obj.IdCategory = model.IdCategory;

                        if (bdService.EditProductAdmin(obj))
                        {
                            return Json(new { success = true });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("~/Views/Shared/MessagePurchaseCart.cshtml", false);
            }
            return Json(new { success = false });
        }

        [Authorize(Roles = "Адмін, Головний Адмін")]
        public IActionResult EditUser()
        {
            List<UserModel> users = bdService.GetAllUser();
            if (users != null)
            {
                ViewData["Email"] = User.FindFirstValue(ClaimTypes.Email);
                return View(users);
            }
            return View();
        }

        [Authorize(Roles = "Адмін, Головний Адмін")]
        public IActionResult DelUser(int? idUser)
        {
            if (idUser == null) return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Такого користувача немає");
            int? idAuth = UserVerification.checAuthorization(HttpContext);
            if (idAuth == idUser)
            {
                if (bdService.DeleteUser(idUser))
                {
                    HttpContext.SignOutAsync("Cookies");
                    return RedirectToAction("Index", "Home");
                }
                else return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Помилка при видаленні");
            }
            UserModel userDel = bdService.GetUser(idUser);
            if (userDel != null && userDel.role != "Головний Адмін")
            {
                var roleUser = User.FindFirstValue(ClaimTypes.Role);
                if ((roleUser == "Адмін" && userDel.role != "Адмін") || roleUser == "Головний Адмін")
                {
                    bdService.DeleteUser(idUser);
                }
            }
            return RedirectToAction("EditUser", "Admin");
        }

        [Authorize(Roles = "Адмін, Головний Адмін")]
        [HttpPost]
        public IActionResult UpdateUser(string username, string email, string password, string role)
        {
            UserModel user = bdService.GetUser(email);
            int? idAuth = UserVerification.checAuthorization(HttpContext);
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            if (!((user.role == "Адмін" || user.role == "Головний Адмін") && roleClaim == "Адмін") || user.IdUser == idAuth)
            {
                
                if (bdService.EditUserAdmin(email, username, role))
                {
                    var roleEdit = User.FindFirstValue(ClaimTypes.Role);
                    int? idEdit = UserVerification.checAuthorization(HttpContext);
                    UserModel userAuth = bdService.GetUser(idEdit);
                    return Json(new { success = true });
                }
                    
                else
                    return Json(new { success = false });
            }
            else
                return Json(new { success = false });
        }

        [Authorize(Roles = "Адмін, Головний Адмін")]
        [HttpGet]
        public IActionResult AddProduct()
        {
            ViewBag.Categories = bdService.GetAllCategory();
            return View();
        }

        [Authorize(Roles = "Адмін, Головний Адмін")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductModel obj, IFormFile formSignupImg)
        {
            try
            {
                if (formSignupImg != null && formSignupImg.Length > 0)
                {
                    var fileName = formSignupImg.FileName;

                    var uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "img", "productImg");

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    using (var stream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create))
                    {
                        await formSignupImg.CopyToAsync(stream);
                    }

                    obj.img = "~/img/productImg/" + fileName;
                    if (bdService.AddProduct(obj))
                    {
                        return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Додано");
                    }
                    return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Помилка при додаванні");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Помилка при обробці фото");
            }

            return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Помилка при додаванні");
        }

        [Authorize(Roles = "Адмін, Головний Адмін")]
        public IActionResult DelProduct(int? productId)
        {
            if (productId == null) return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Такого товару немає");
            if (bdService.DeleteProduct(productId))
            {
                return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Видалено");
            }
            return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Помилка при видаленні");
        }

        [Authorize(Roles = "Адмін, Головний Адмін")]
        [HttpGet]
        public IActionResult AddCategory() => View();

        [Authorize(Roles = "Адмін, Головний Адмін")]
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryModel model, IFormFile formSignupImg)
        {
            try
            {
                if (model.category == "Default") return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Категорію не можна назвати таким ім'ям");
                if (formSignupImg != null && formSignupImg.Length > 0)
                {
                    var fileName = formSignupImg.FileName;

                    var uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "img", "categoryImg");

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    using (var stream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create))
                    {
                        await formSignupImg.CopyToAsync(stream);
                    }
                    model.img = "~/img/categoryImg/" + fileName;
                }
                
                if (bdService.AddCategory(model))
                {
                    ListCategory.AddCategores(bdService.GetAllCategory());
                    return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Додано");
                }
                return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Сталася помилка при додаванні");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Сталася помилка при обробці фото");
            }
        }

        [Authorize(Roles = "Адмін, Головний Адмін")]
        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryModel model, IFormFile formSignupImgFile)
        {
            try
            {
                if (formSignupImgFile != null && formSignupImgFile.Length > 0 && model.IdCategory != 0)
                {
                    var fileName = formSignupImgFile.FileName;

                    var uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "img", "categoryImg");

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    using (var stream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create))
                    {
                        await formSignupImgFile.CopyToAsync(stream);
                    }
                    model.img = "~/img/categoryImg/" + fileName;
                }
                if (model.IdCategory != 0 && model.category!="Default")
                {
                     if (bdService.EditCategoryAdmin(model))
                     {
                        ListCategory.AddCategores(bdService.GetAllCategory());
                        return Json(new { success = true });
                     }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false });
            }
            return Json(new { success = false });
        }
        public IActionResult DelCategory(int? idCategory)
        {
            if(idCategory == null) return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Такої категорії немає");
            if(bdService.DelCategory(idCategory))
            {
                ListCategory.AddCategores(bdService.GetAllCategory());
                return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Видалено");
            }
            return View("~/Views/Shared/MessagePurchaseCart.cshtml", "Помилка при видаленні категорії");
        }

        [Authorize(Roles = "Головний Адмін")]
        public IActionResult ClearViews()
        {
            bdService.ClearViews();
            return RedirectToAction("Views","AllProduct");
        }
    }
}
