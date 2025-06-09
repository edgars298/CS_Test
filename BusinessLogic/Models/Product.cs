namespace BusinessLogic.Models;

public class Product
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required int Quantity { get; set; }

    public required decimal Price { get; set; }
}
