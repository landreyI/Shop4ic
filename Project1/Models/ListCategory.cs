using Project1.Models.DBModels;

namespace Project1.Models
{
    public static class ListCategory
    {
        private static List<CategoryModel> listCategory = new List<CategoryModel>();
        public static void AddCategores(List<CategoryModel> list)
        {
            listCategory = list;
        }
        public static List<CategoryModel> GetListCategores()
        {
            return listCategory;
        }
    }
}
