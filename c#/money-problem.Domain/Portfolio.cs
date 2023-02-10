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
            var result = Todo2(currency, bank, money);
            if (result.HasResult)
                total += result.Result.Value;
            else
                missingExchangeRates.Add(result.MissingExchangeRate);
        }

        if (missingExchangeRates.Any())
            throw new MissingExchangeRatesException(missingExchangeRates);

        return total;
    }


    private static Todo2Result Todo2(Currency currency, Bank bank, Money money)
    {
        try
        {
            return new Todo2Result(bank.Convert(money, currency));
        }
        catch (MissingExchangeRateException missingExchangeRate)
        {
            return new Todo2Result(missingExchangeRate);
        }
    }
}

internal class Todo2Result
{
    public Todo2Result(double convert)
    {
        Result = convert;
        HasResult = true;
    }

    public Todo2Result(MissingExchangeRateException missingExchangeRate)
    {
        MissingExchangeRate = missingExchangeRate;
        HasResult = false;
    }

    public bool HasResult { get; }
    public MissingExchangeRateException? MissingExchangeRate { get; }
    public double? Result { get; }
}