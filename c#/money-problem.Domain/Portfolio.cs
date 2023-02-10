using money_problem.Tests;

namespace money_problem.Domain;

public class Portfolio
{
    private readonly List<Money> moneys = new();

    public void Add(Money money)
        => moneys.Add(money);

    public double Evaluate(Currency currency, Bank bank)
    {
        double total = 0;
        var missingExchangeRates = new List<MissingExchangeRateException>();

        foreach (var money in moneys)
        {
            var convertionResult = Convert(currency, bank, money);
            if (convertionResult.HasException)
                missingExchangeRates.Add(convertionResult.MissingExchangeRate);
            else
                total += convertionResult.Money;
        }

        if (missingExchangeRates.Any())
            throw new MissingExchangeRatesException(missingExchangeRates);

        return total;
    }


    private static ConvertionResult Convert(Currency currency, Bank bank, Money money)
    {
        try
        {
            return new ConvertionResult(bank.Convert(money, currency));
        }
        catch (MissingExchangeRateException missingExchangeRate)
        {
            return new ConvertionResult(missingExchangeRate);
        }
    }
}

internal class ConvertionResult
{
    private readonly MissingExchangeRateException? _missingExchangeRate;

    public ConvertionResult(double money)
    {
        Money = money;
    }

    public ConvertionResult(MissingExchangeRateException missingExchangeRate)
    {
        _missingExchangeRate = missingExchangeRate;
    }

    public bool HasException
        => _missingExchangeRate != null;

    public MissingExchangeRateException MissingExchangeRate => _missingExchangeRate!;

    public double Money { get; }
}