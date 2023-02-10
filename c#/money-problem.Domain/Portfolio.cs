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
            if (convertionResult.HasMoney)
                total += convertionResult.Money.Value;
            else
                missingExchangeRates.Add(convertionResult.MissingExchangeRate);
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
    public ConvertionResult(double money)
    {
        Money = money;
    }

    public ConvertionResult(MissingExchangeRateException missingExchangeRate)
    {
        MissingExchangeRate = missingExchangeRate;
    }

    public bool HasMoney
        => Money is not null;
    public MissingExchangeRateException? MissingExchangeRate { get; }
    public double? Money { get; }
}