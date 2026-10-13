namespace MyShop.Business.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Img { get; set; } = null!;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = null!;
}
