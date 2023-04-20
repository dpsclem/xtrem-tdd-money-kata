namespace money_problem.Domain;

public class Portfolio
{
    private readonly ICollection<Money> moneys;

    public Portfolio()
    {
        moneys = new List<Money>();
    }

    private Portfolio(List<Money> newMoneys)
    {
        moneys = newMoneys;
    }

    public Portfolio Add(Money money)
    {
        var newMoneys = moneys.ToList();
        newMoneys.Add(money);
        return new Portfolio(newMoneys);
    }

    private static Money ToMoney(IEnumerable<ConversionResult<MissingExchangeRateException>> results, Currency currency)
    {
        return new(results.Sum(result => result.GetMoneyUnsafe().Amount), currency);
    }

    private static bool ContainsFailure(IEnumerable<ConversionResult<MissingExchangeRateException>> results)
    {
        return results.Any(result => result.HasException());
    }

    private List<ConversionResult<MissingExchangeRateException>> GetConvertedMoneys(Bank bank, Currency currency)
    {
        return moneys
            .Select(money => ConvertMoney(bank, currency, money))
            .ToList();
    }

    private static ConversionResult<MissingExchangeRateException> ConvertMoney(Bank bank, Currency currency,
        Money money)
    {
        try
        {
            return new ConversionResult<MissingExchangeRateException>(bank.Convert(money, currency));
        }
        catch (MissingExchangeRateException exception)
        {
            return new ConversionResult<MissingExchangeRateException>(exception);
        }
    }

    public ConversionResult<string> Evaluate(Bank bank, Currency currency)
    {
        var results = GetConvertedMoneys(bank, currency);
        return ContainsFailure(results)
            ? ToFailure(results)
            : new ConversionResult<string>(ToMoney(results, currency));
    }

    private static ConversionResult<string> ToFailure(List<ConversionResult<MissingExchangeRateException>> results)
    {
        var missingExchangeRates = GetMissingExchangeRates(results);
        return new ConversionResult<string>(
            $"Missing exchange rate(s): {string.Join(",", missingExchangeRates.Select(x => $"[{x.Message}]"))}");
    }

    private static List<MissingExchangeRateException> GetMissingExchangeRates(
        List<ConversionResult<MissingExchangeRateException>> results)
    {
        return results
            .Where(result => result.HasException())
            .Select(result => result.GetFailureUnsafe())
            .ToList();
    }

    public static string GetExchangeRates(List<MissingExchangeRateException> missingExchangeRates)
    {
        return string.Join(",", missingExchangeRates.Select(x => $"[{x.Message}]"));
    }

    public class ConversionResult<T>
    {
        private readonly T? exception;

        private readonly Money? money;

        public ConversionResult(Money money)
        {
            this.money = money;
        }

        public ConversionResult(T exception)
        {
            this.exception = exception;
        }

        public bool HasException()
        {
            return exception != null;
        }

        public T GetFailureUnsafe()
        {
            return exception!;
        }

        public bool HasMoney()
        {
            return money != null;
        }

        public Money GetMoneyUnsafe()
        {
            return money!;
        }
    }
}