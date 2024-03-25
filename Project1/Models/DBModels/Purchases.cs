namespace Project1.Models.DBModels
{
    public class Purchases
    {
        public int IdUser { get; set; }
        public int IdProduct { get; set; }
        public int quantity { get; set; }
        public virtual UserModel User { get; set; }
        public virtual ProductModel Product { get; set; }
    }
}
