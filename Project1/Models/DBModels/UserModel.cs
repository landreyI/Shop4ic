using System.ComponentModel.DataAnnotations;

namespace Project1.Models.DBModels
{
    public class UserModel
    {
        [Key]
        public int IdUser { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public virtual ICollection<Basket> Basket { get; set; }
        public virtual ICollection<Purchases> Purchases { get; set; }
        public virtual ICollection<Views> Views { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }
    }
}
