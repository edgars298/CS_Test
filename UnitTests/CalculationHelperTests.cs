using Api.Core.Configs;
using Api.Core.Helpers;
using Microsoft.Extensions.Options;

namespace UnitTests;

public class CalculationHelperTests
{
    [Fact]
    public void CalculatePriceWithVatTest()
    {
        // Arrange
        var vatConfig = new VatConfig { Value = 0.2m };
        var options = Options.Create(vatConfig);

        var helper = new CalculationHelper(options);

        var quantity = 5;
        var price = 10m;

        // Act
        var result = helper.CalculatePriceWithVat(quantity, price);

        // Assert
        decimal expected = quantity * price * (1 + vatConfig.Value);
        Assert.Equal(expected, result);
    }
}