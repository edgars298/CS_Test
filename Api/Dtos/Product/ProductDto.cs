namespace Api.Dtos.Product;

public class ProductDto
{
    public required int Id { get; set; }

    public required string Title { get; set; }

    public required int Quantity { get; set; }

    public required decimal Price { get; set; }
}
