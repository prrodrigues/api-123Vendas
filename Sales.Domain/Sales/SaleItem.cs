using Sales.Domain.Abstractions;
using Sales.Domain.Exceptions;

namespace Sales.Domain.Sales;

public sealed class SaleItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public decimal Total { get; private set; }
    public bool IsCanceled { get; private set; }

    private SaleItem() { } // EF

    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new AppException("QUANTITY_ERROR", "Quantidade deve ser maior que zero.");

        if (quantity > 20)
            throw new AppException("QUANTITY_ERROR", "Não é possível vender acima de 20 itens iguais.");

        if (string.IsNullOrWhiteSpace(productName))
            throw new AppException("PRODUCT_NAME_ERROR", "Nome do produto é obrigatório.");

        if (unitPrice <= 0)
            throw new AppException("UNIT_PRICE_ERROR", "Preço unitário deve ser maior que zero.");

        ProductId = productId;
        ProductName = productName.Trim();
        UnitPrice = unitPrice;

        SetQuantity(quantity);
    }

    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new AppException("QUANTITY_ERROR", "Quantidade deve ser maior que zero.");

        if (quantity > 20)
            throw new AppException("QUANTITY_ERROR", "Não é possível vender acima de 20 itens iguais.");

        Quantity = quantity;

        RecalculateDiscountAndTotal();
    }

    private void RecalculateDiscountAndTotal()
    {
        if (Quantity < 4)
            DiscountPercent = 0m;
        else if (Quantity < 10)
            DiscountPercent = 0.10m;
        else // 10-20
            DiscountPercent = 0.20m;

        var subtotal = UnitPrice * Quantity;
        var discountAmount = subtotal * DiscountPercent;
        Total = decimal.Round(subtotal - discountAmount, 2);
    }

    public void Cancel()
    {
        IsCanceled = true;
    }
}
