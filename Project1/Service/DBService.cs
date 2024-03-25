using Microsoft.EntityFrameworkCore;
using Project1.Data;
using Project1.Models.DBModels;
using System.ComponentModel.Design;
using static System.Net.Mime.MediaTypeNames;

namespace Project1.Service
{
    public class DBService
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<DBService> _logger;

        public DBService(AppDBContext dbContext, ILogger<DBService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public List<ProductModel> GetAllProduct()
        {
            try
            {
                return _dbContext.Product.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when retrieving products from the database");
                return null;
            }
        }
        public List<CategoryModel> GetAllCategory()
        {
            try
            {
                return _dbContext.Category.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when retrieving categores from the database");
                return null;
            }
        }
        public List<ProductModel> GetCategoryProduct(int? category)
        {
            try
            {
                if (category == null) return null;
                if (category == 0)
                {
                    return _dbContext.Product
                        .Where(p => p.is_favorite == true)
                        .ToList();
                }
                else
                {
                    return _dbContext.Product
                        .Where(p => p.Category.IdCategory == category)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when retrieving products from the database");
                return null;
            }
        }
        public List<UserModel> GetAllUser()
        {
            try
            {
                return _dbContext.User.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when retrieving users from the database");
                return null;
            }
        }
        public List<ProductModel> GetAllViews()
        {
            try
            {
                var products = _dbContext.Views
                            .GroupBy(v => v.IdProduct)
                            .OrderByDescending(g => g.Count())
                            .Select(g => g.Key)
                            .Join(_dbContext.Product,
                                  id => id,
                                  product => product.id,
                                  (id, product) => product)
                            .ToList();
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when retrieving views from the database");
                return null;
            }
        }
        public ProductModel GetProduct(int? id)
        {
            if (id == null) return null;
            try
            {
                ProductModel product = _dbContext.Product.FirstOrDefault(p => p.id == id);
                product.Category = _dbContext.Category.FirstOrDefault(c => c.IdCategory == product.IdCategory);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when retrieving product from the database");
                return null;
            }
        }
        public bool SetProduct(ProductModel updatedProduct)
        {
            if (updatedProduct == null)
            {
                return false;
            }
            try
            {
                ProductModel existingProduct = _dbContext.Product.FirstOrDefault(p => p.id == updatedProduct.id);

                if (existingProduct != null)
                {
                    existingProduct.name = updatedProduct.name;
                    existingProduct.short_description = updatedProduct.short_description;
                    existingProduct.long_description = updatedProduct.long_description;
                    existingProduct.img = updatedProduct.img;
                    existingProduct.price = updatedProduct.price;
                    existingProduct.is_favorite = updatedProduct.is_favorite;
                    existingProduct.available = updatedProduct.available;
                    existingProduct.IdCategory = updatedProduct.IdCategory;

                    _dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding product to database");
                return false;
            }
        }
        public bool RemoveProduct(int? id)
        {
            try
            {
                if(id == null) return false;
                ProductModel existingProduct = _dbContext.Product.FirstOrDefault(p => p.id == id);

                if (existingProduct != null)
                {
                    _dbContext.Product.Remove(existingProduct);
                    _dbContext.SaveChanges();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when deleting a product from the database");
                return false;
            }
        }
        public bool CheckRepetitionUser(string emailUser)
        {
            if (string.IsNullOrEmpty(emailUser))
            {
                return false;
            }
            try
            {
                var existingUser = _dbContext.User.FirstOrDefault(u => u.email == emailUser);
                if (existingUser != null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return false;
            }
        }
        public bool AddUser(UserModel user)
        {
            if(user == null)
            {
                return false;
            }
            try
            {
                _dbContext.User.Add(user);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding user to database");
                return false;
            }
        }
        public UserModel GetUser(string emailUser)
        {
            try
            {
                UserModel user = _dbContext.User.FirstOrDefault(u => u.email == emailUser);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return null;
            }
        }
        public UserModel GetUser(int? idUser)
        {
            try
            {
                if (idUser == null) return null;
                UserModel user = _dbContext.User.FirstOrDefault(u => u.IdUser == idUser);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return null;
            }
        }
        public List<ProductModel> GetBasketUser(int? idUser)
        {
            try
            {
                if (idUser == null) return null;
                return _dbContext.Basket
                    .Where(b => b.IdUser == idUser)
                    .Select(b => b.Product)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when retrieving basket from the database");
                return null;
            }
        }
        public List<ProductModel> GetViewsUser(int? idUser)
        {
            try
            {
                if (idUser == null) return null;
                return _dbContext.Views
                    .Where(b => b.IdUser == idUser)
                    .Select(b => b.Product)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when retrieving views from the database");
                return null;
            }
        }
        public List<KeyValuePair<ProductModel, int>> GetPurchasesUser(int? idUser)
        {
            try
            {
                if (idUser == null) return null;

                var purchasesInfo = _dbContext.Purchases
                    .Where(b => b.IdUser == idUser)
                    .GroupBy(b => b.Product)
                    .Select(group => new KeyValuePair<ProductModel, int>(group.Key, group.Sum(b => b.quantity)))
                    .ToList();

                return purchasesInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when retrieving purchases from the database");
                return null;
            }
        }
        public bool AddBasket(int? idUser, int idProduct)
        {
            try
            {
                if (idUser != null || idProduct != 0)
                {
                    // Проверка на дублирование записи
                    bool isDuplicate = _dbContext.Basket.Any(b => b.IdUser == idUser && b.IdProduct == idProduct);
                    if (isDuplicate)
                    {
                        return true;
                    }

                    var basket = new Basket
                    {
                        IdUser = idUser.Value,
                        IdProduct = idProduct
                    };

                    _dbContext.Basket.Add(basket);
                    _dbContext.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding to basket");
                return false;
            }
        }
        public bool AddViews(int? idUser, int idProduct)
        {
            try
            {
                if (idUser != null || idProduct != 0)
                {
                    bool isDuplicate = _dbContext.Views.Any(b => b.IdUser == idUser && b.IdProduct == idProduct);
                    if (isDuplicate)
                    {
                        return true;
                    }

                    var views = new Views
                    {
                        IdUser = idUser.Value,
                        IdProduct = idProduct
                    };

                    _dbContext.Views.Add(views);
                    _dbContext.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding to views");
                return false;
            }
        }
        public bool AddPurchases(int? idUser, int idProduct, int quantity)
        {
            try
            {
                if (idUser != null && idProduct != 0 && quantity != 0)
                {
                    var existingPurchase = _dbContext.Purchases
                        .FirstOrDefault(b => b.IdUser == idUser && b.IdProduct == idProduct);

                    if (existingPurchase != null)
                    {
                        existingPurchase.quantity += quantity;
                    }
                    else
                    {
                        var purchases = new Purchases
                        {
                            IdUser = idUser.Value,
                            IdProduct = idProduct,
                            quantity = quantity
                        };

                        _dbContext.Purchases.Add(purchases);
                    }

                    _dbContext.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding to purchases");
                return false;
            }
        }
        public bool DeleteToBasket(int? idUser, int idProduct) 
        {
            try
            {
                var basketItem = _dbContext.Basket.FirstOrDefault(item => item.IdUser == idUser && item.IdProduct == idProduct);

                if (basketItem != null)
                {
                    _dbContext.Basket.Remove(basketItem);
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when delet to basket");
                return false;
            }
        }
        public bool EditProductAdmin(ProductModel obj)
        {
            try
            {
                if (obj != null)
                {
                    ProductModel product = _dbContext.Product.FirstOrDefault(p => p.id == obj.id);
                    product.Category = _dbContext.Category.FirstOrDefault(c => c.IdCategory == obj.IdCategory);
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when edit to product");
                return false;
            }
        }
        public bool DeleteUser(int? idUser)
        {
            if (idUser == null)
            {
                return false;
            }
            try
            {
                var userBasketItems = _dbContext.Basket.Where(b => b.IdUser == idUser).ToList();
                _dbContext.Basket.RemoveRange(userBasketItems);
                var userPurchasesItems = _dbContext.Purchases.Where(b => b.IdUser == idUser).ToList();
                _dbContext.Purchases.RemoveRange(userPurchasesItems);
                var userViewsItems = _dbContext.Views.Where(b => b.IdUser == idUser).ToList();
                _dbContext.Views.RemoveRange(userViewsItems);
                var userCommentsItems = _dbContext.Comments.Where(b => b.IdUser == idUser).ToList();
                _dbContext.Comments.RemoveRange(userCommentsItems);

                var user = _dbContext.User.Find(idUser);
                if (user != null)
                {
                    _dbContext.User.Remove(user);
                    _dbContext.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when delet to user");
                return false;
            }
        }
        public bool DeleteProduct(int? idProduct)
        {
            if (idProduct == null)
            {
                return false;
            }
            try
            {
                var productBasketItems = _dbContext.Basket.Where(b => b.IdProduct == idProduct).ToList();
                _dbContext.Basket.RemoveRange(productBasketItems);
                var productPurchasesItems = _dbContext.Purchases.Where(b => b.IdProduct == idProduct).ToList();
                _dbContext.Purchases.RemoveRange(productPurchasesItems);

                var product = _dbContext.Product.Find(idProduct);
                if (product != null)
                {
                    _dbContext.Product.Remove(product);
                    _dbContext.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when delet to product");
                return false;
            }
        }
        public List<ProductModel> SearchProduct(string text)
        {
            try
            {
                return _dbContext.Product.Where(p => p.name.Contains(text)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when search to product");
                return null;
            }
        }
        public List<ProductModel> FindFilterProduct(int idCategory, int priceRange, bool availableOnly)

        {
            try
            {
                List<ProductModel> filteredProducts = new List<ProductModel>();

                List<ProductModel> allProducts = GetAllProduct();

                if (idCategory > 0)
                {
                    filteredProducts = allProducts.Where(p => p.IdCategory == idCategory).ToList();
                }
                else
                {
                    filteredProducts = allProducts.ToList();
                }
                if (priceRange > 0)
                {
                    filteredProducts = filteredProducts.Where(p => p.price <= priceRange).ToList();
                }
                if (availableOnly)
                {
                    filteredProducts = filteredProducts.Where(p => p.available == true).ToList();
                }

                return filteredProducts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when find to products");
                return null;
            }
        }
        public bool EditUserAdmin(string email,  string name, string role) 
        {
            try
            {
                UserModel user = _dbContext.User.FirstOrDefault(u => u.email == email);
                if(user != null)
                {
                    if(name != "") user.name = name;
                    user.role = role;
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when edit to user");
                return false;
            }
        }
        public bool AddProduct(ProductModel product)
        {
            if (product == null)
            {
                return false;
            }
            try
            {
                _dbContext.Product.Add(product);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when add to product");
                return false;
            }
        }
        public bool AddCategory(CategoryModel category)
        {
            if (category == null)
            {
                return false;
            }
            try
            {
                _dbContext.Category.Add(category);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when add to category");
                return false;
            }
        }
        public bool EditCategoryAdmin(CategoryModel obj)
        {
            try
            {
                if (obj != null)
                {
                    CategoryModel category = _dbContext.Category.FirstOrDefault(p => p.IdCategory == obj.IdCategory);
                    if(category.category != "" && category.category != "Default")
                    category.category = obj.category;
                    if(category.description != "")
                    category.description = obj.description;
                    if(category.img != "")
                    category.img = obj.img;
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when edit to caterory");
                return false;
            }
        }
        public bool DelCategory(int? idCategory)
        {
            if (idCategory == null)
            {
                return false;
            }
            try
            {
                var productsToUpdate = _dbContext.Product.Where(p => p.IdCategory == idCategory).ToList();

                var defaultCategory = _dbContext.Category.SingleOrDefault(c => c.category == "Default");
                if (defaultCategory == null)
                {
                    return false;
                }

                foreach (var product in productsToUpdate)
                {
                    product.IdCategory = defaultCategory.IdCategory;
                }

                var categoryToDelete = _dbContext.Category.Find(idCategory);
                if (categoryToDelete == null)
                {
                    return false;
                }

                _dbContext.Category.Remove(categoryToDelete);

                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when delet to category");
                return false;
            }
        }
        public bool ClearViews()
        {
            try
            {
                var allRecords = _dbContext.Views.ToList();
                _dbContext.Views.RemoveRange(allRecords);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when delet to views");
                return false;
            }
        }
        public bool AddCommentUser(int? idProduct, int? idUser, string? text)
        {
            if(idProduct == null || idUser == null || text == null) return false;
            try
            {
                var newComment = new Comments
                {
                    IdProduct = idProduct.Value,
                    IdUser = idUser.Value,
                    Text = text
                };

                _dbContext.Comments.Add(newComment);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "error when add to comments");
                return false;
            }
        }
        public List<Comments> GetProductComments(int? idProduct)
        {
            if (idProduct == null) return null;
            try
            {
                return _dbContext.Comments
                    .Where(c => c.IdProduct == idProduct.Value)
                    .ToList();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "error when get to comments");
                return null;
            }
        }
        public List<Comments> GetUserComments(int? idUser)
        {
            if (idUser == null) return null;
            try
            {
                return _dbContext.Comments
                    .Where(c => c.IdUser == idUser.Value)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when get to comments");
                return null;
            }
        }
        public bool DeleteCommentsUser(int? idComments) 
        {
            if (idComments == null) return false;
            try
            {
                var commentToDelete = _dbContext.Comments.FirstOrDefault(c => c.IdComments == idComments);

                if (commentToDelete == null) return false;

                _dbContext.Comments.Remove(commentToDelete);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when delet to comments");
                return false;
            }
        }
        public bool DeleteCommentsProduct(int? idProduct)
        {
            if (idProduct == null) return false;
            try
            {
                var commentsToDelete = _dbContext.Comments.Where(c => c.IdProduct == idProduct).ToList();

                if (commentsToDelete.Count == 0) return false;

                _dbContext.Comments.RemoveRange(commentsToDelete);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when delet to comments");
                return false;
            }
        }
    }
}
