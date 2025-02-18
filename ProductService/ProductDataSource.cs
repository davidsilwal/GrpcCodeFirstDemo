namespace ProductService;

public class ProductDataSource
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Product 1", Price = 100 },
        new Product { Id = 2, Name = "Product 2", Price = 200 },
        new Product { Id = 3, Name = "Product 3", Price = 300 }
    };

    public List<Product> Products => _products;
}