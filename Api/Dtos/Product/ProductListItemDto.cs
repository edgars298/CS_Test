namespace Api.Dtos.Product;

public class ProductListItemDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required int Quantity { get; set; }

    public required decimal Price { get; set; }


    public decimal TotalPriceWithVat { get; set; }
}
