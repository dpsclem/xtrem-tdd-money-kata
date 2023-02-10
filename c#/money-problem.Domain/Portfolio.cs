using money_problem.Tests;

namespace money_problem.Domain;

public class Portfolio
{
    private readonly List<Money> moneys = new();

    public void Add(Money money)
        => moneys.Add(money);

    public double Evaluate(Currency currency, Bank bank)
    {
        var conversionResults = moneys.Select(money => Convert(currency, bank, money)).ToList();
        
        var total = conversionResults
            .Where(result => !result.HasException)
            .Sum(result => result.Money);
        
        var missingExchangeRates = conversionResults
            .Where(result => result.HasException)
            .Select(result => result.MissingExchangeRate)
            .ToList();

        if (missingExchangeRates.Any())
            throw new MissingExchangeRatesException(missingExchangeRates);

        return total;
    }


    private static ConversionResult Convert(Currency currency, Bank bank, Money money)
    {
        try
        {
            return new ConversionResult(bank.Convert(money, currency));
        }
        catch (MissingExchangeRateException missingExchangeRate)
        {
            return new ConversionResult(missingExchangeRate);
        }
    }
}

internal class ConversionResult
{
    private readonly MissingExchangeRateException? _missingExchangeRate;

    public ConversionResult(double money)
    {
        Money = money;
    }

    public ConversionResult(MissingExchangeRateException missingExchangeRate)
    {
        _missingExchangeRate = missingExchangeRate;
    }

    public bool HasException
        => _missingExchangeRate != null;

    public MissingExchangeRateException MissingExchangeRate
        => _missingExchangeRate!;

    public double Money { get; }
}