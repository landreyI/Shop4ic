using Microsoft.AspNetCore.Mvc;
using Project1.Service;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Project1.Controllers
{
    public class UserController : Controller
    {
        private readonly DBService bdService;
        public UserController(DBService _bdService)
        {
            bdService = _bdService;
        }
        public IActionResult DeleteComment(int? idComment)
        {
            if (idComment == null)
            {
                return BadRequest("ID коментаря не був передан");
            }

            if (!bdService.DeleteCommentsUser(idComment))
            {
                return NotFound("Коментар не знайден або не вдала спроба видалення!");
            }
            return Ok();
        }
        public IActionResult AddComment(string? text, int? productId)
        {
            if (text == null || productId == null)
            {
                return BadRequest("ID - 0 або текст відгуку ");
            }

            if (User.Identity.IsAuthenticated)
            {
                if (!bdService.AddCommentUser(productId, UserVerification.checAuthorization(HttpContext), text))
                {
                    return NotFound("Помилка при додаванні у БД відгуку");
                }
            }
            else return NotFound("Ви не авторизовані");

            return Ok();
        }
        public IActionResult GetCommentUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var listComments = bdService.GetUserComments(UserVerification.checAuthorization(HttpContext));
                foreach (var comment in listComments)
                {
                    comment.Product = bdService.GetProduct(comment.IdProduct);
                }
                if (listComments != null) return View(listComments);
            }
            return View(null);
        }
    }
}
