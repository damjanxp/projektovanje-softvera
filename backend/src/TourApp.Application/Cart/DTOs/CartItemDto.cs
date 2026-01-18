namespace TourApp.Application.Cart.DTOs;

public class CartItemDto
{
    public Guid TourId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
