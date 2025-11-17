using Bogus;
using Sales.Domain.Sales;
using Shouldly;
using Xunit;

namespace Sales.Tests;

public class SaleItemFakerTests
{
    [Fact]
    public void Should_Create_SaleItem_With_Fake_Data()
    {
        var faker = new Faker();
        var item = new SaleItem(
            faker.Random.Guid(),
            faker.Commerce.ProductName(),
            faker.Random.Int(1, 20),
            faker.Random.Decimal(1, 100)
        );
        item.ProductName.ShouldNotBeNullOrWhiteSpace();
        item.Quantity.ShouldBeGreaterThan(0);
        item.UnitPrice.ShouldBeGreaterThan(0);
    }
}
