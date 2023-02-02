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
            try
            {
                total += bank.Convert(money, currency);
            }
            catch (MissingExchangeRateException missingExchangeRate)
            {
                missingExchangeRates.Add(missingExchangeRate);
            }

        if (missingExchangeRates.Any())
            throw new MissingExchangeRatesException(missingExchangeRates);

        return total;
    }
}