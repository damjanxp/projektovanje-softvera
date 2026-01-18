namespace TourApp.Application.Cart.DTOs;

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public int AvailableBonusPoints { get; set; }
    public decimal MaxBonusDiscount { get; set; }
}
