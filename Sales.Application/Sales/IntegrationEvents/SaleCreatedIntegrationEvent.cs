namespace Sales.Application.Sales.IntegrationEvents;

public sealed class SaleCreatedIntegrationEvent
{
    public Guid SaleId { get; init; }
    public string Number { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public decimal Total { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public IReadOnlyCollection<SaleItemDto> Items { get; init; } = Array.Empty<SaleItemDto>();

    public sealed class SaleItemDto
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal DiscountPercent { get; init; }
        public decimal Total { get; init; }
    }
}
