using System.ComponentModel.DataAnnotations;

namespace Project1.Models.DBModels
{
    public class Comments
    {
        public int IdComments { get; set; }
        public int IdUser { get; set; }
        public int IdProduct { get; set; }
        public string Text { get; set; }

        public virtual UserModel? User { get; set; }
        public virtual ProductModel? Product { get; set; }
    }
}
