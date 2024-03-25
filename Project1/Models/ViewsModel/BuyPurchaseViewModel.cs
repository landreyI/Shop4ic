using Project1.Models.DBModels;

namespace Project1.Models.ViewsModel
{
    public class BuyPurchaseViewModel
    {
        public ProductModel Product { get; set; }
        public int Quantity { get; set; }
        public UserModel User { get; set; }
    }
}
