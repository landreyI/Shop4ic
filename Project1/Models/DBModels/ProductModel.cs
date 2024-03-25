using System.ComponentModel.DataAnnotations;

namespace Project1.Models.DBModels
{
    public class ProductModel
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string short_description { get; set; }
        public string long_description { get; set; }
        public string img { get; set; }
        public ushort price { get; set; }
        public int discount { get; set; }
        public bool is_favorite { get; set; }
        public int like { get; set; }
        public bool available { get; set; }
        public int IdCategory { get; set; }
        public virtual CategoryModel Category { get; set; }
        public virtual ICollection<Basket> Basket { get; set; }
        public virtual ICollection<Purchases> Purchases { get; set; }
        public virtual ICollection<Views> Views { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }
    }
}
