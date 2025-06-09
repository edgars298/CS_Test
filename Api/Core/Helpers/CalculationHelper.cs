using Api.Core.Configs;
using Microsoft.Extensions.Options;

namespace Api.Core.Helpers;

public class CalculationHelper(IOptions<VatConfig> _vatConfig) : ICalculationHelper
{
    private readonly VatConfig _vatConfig = _vatConfig.Value;

    public decimal CalculatePriceWithVat(int quantity, decimal price)
    {
        return quantity * price * (1 + _vatConfig.Value);
    }
}

public interface ICalculationHelper
{
    decimal CalculatePriceWithVat(int quantity, decimal price);
}
