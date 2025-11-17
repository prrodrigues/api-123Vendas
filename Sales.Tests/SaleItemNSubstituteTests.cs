using NSubstitute;
using Sales.Domain.Sales;
using Sales.Application.Common.Correlation;
using Xunit;
using System;

namespace Sales.Tests;

public class SaleItemNSubstituteTests
{
    [Fact]
    public void Should_Use_Mock_CorrelationContextAccessor()
    {
        var correlationAccessor = Substitute.For<ICorrelationContextAccessor>();
        correlationAccessor.CorrelationId.Returns("fake-correlation-id");

        // Exemplo de uso do mock em um cenário de domínio
        var item = new SaleItem(
            Guid.NewGuid(),
            "Product X",
            5,
            10m
        );

        Assert.NotNull(correlationAccessor.CorrelationId);
        Assert.Equal("fake-correlation-id", correlationAccessor.CorrelationId);
    }
}
