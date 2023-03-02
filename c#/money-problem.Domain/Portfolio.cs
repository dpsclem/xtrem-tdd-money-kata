using money_problem.Tests;

namespace money_problem.Domain;

public class Portfolio
{
    private readonly List<Money> moneys;

    public Portfolio()
    {
        moneys = new();
    }

    private Portfolio(List<Money> newMoneys)
    {
        moneys = newMoneys;
    }

    public Portfolio Add(Money money)
    {
        List<Money> newMoneys = moneys.ToList();
        newMoneys.Add(money);
        return new Portfolio(newMoneys);
    }

    public double Evaluate(Currency currency, Bank bank)
    {
        var conversionResults = GetConvertedMoneys(currency, bank);
        return ContainsFailure(conversionResults)
                   ? ToException(conversionResults)
                   : ToTotal(conversionResults);
    }

    private static double ToTotal(List<ConversionResult> conversionResults)
        => conversionResults
            .Sum(result => result.Money);

    private static double ToException(List<ConversionResult> conversionResults)
        => throw new MissingExchangeRatesException(GetFailures(conversionResults));

    private List<ConversionResult> GetConvertedMoneys(Currency currency, Bank bank)
        => moneys.Select(money => Convert(currency, bank, money)).ToList();

    private static List<MissingExchangeRateException> GetFailures(List<ConversionResult> conversionResults)
        => conversionResults
            .Where(result => result.HasException)
            .Select(result => result.Exception)
            .ToList();

    private static bool ContainsFailure(List<ConversionResult> conversionResults)
        => conversionResults
            .Any(result => result.HasException);


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
    public ConversionResult(double money)
        => Money = money;

    public ConversionResult(MissingExchangeRateException exception)
        => Exception = exception;

    public bool HasException
        => Exception != null;

    public MissingExchangeRateException Exception { get; }

    public double Money { get; }
}