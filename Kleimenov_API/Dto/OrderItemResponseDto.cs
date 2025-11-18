namespace Kleimenov_API.Dto;

public class OrderItemResponseDto
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int DishId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}
