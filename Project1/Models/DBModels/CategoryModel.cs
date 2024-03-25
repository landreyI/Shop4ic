using System.ComponentModel.DataAnnotations;

namespace Project1.Models.DBModels
{
    public class CategoryModel
    {
        [Key]
        public int IdCategory { get; set; }
        public string category { get; set; }
        public string img {  get; set; }
        public string description { get; set; }
        public List<ProductModel> products { get; set; }

    }
}
