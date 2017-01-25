namespace KendoUIApp.Models
{
    public class ProductModel
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public bool Discontinued { get; set; }
        public ProductCategoryModel ProductCategory { get; set; }
    }
}