namespace Api.Dtos.Product;

public class ProductFormDto
{
    public required string Title { get; set; }

    public required int Quantity { get; set; }

    public required decimal Price { get; set; }
}
