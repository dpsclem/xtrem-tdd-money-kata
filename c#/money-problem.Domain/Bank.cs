using LanguageExt;

namespace money_problem.Domain;

public sealed class Bank
{
    private readonly Dictionary<string, double> _exchangeRates;

    private Bank(Dictionary<string, double> exchangeRates)
        => _exchangeRates = exchangeRates;

    public static Bank WithExchangeRate(Currency from, Currency to, double rate)
    {
        var bank = new Bank(new Dictionary<string, double>());
        return bank.AddExchangeRate(from, to, rate);
    }

    public Bank AddExchangeRate(Currency from, Currency to, double rate)
    {
        Dictionary<string, double> newExchangeRates = new(_exchangeRates);
        newExchangeRates[KeyFor(from, to)] = rate;
        return new Bank(newExchangeRates);
    }

    private static string KeyFor(Currency from, Currency to)
        => $"{from}->{to}";

    private Money ConvertSafely(Money money, Currency to)
        => to == money.Currency
               ? money
               : money with { Amount = money.Amount * _exchangeRates[KeyFor(money.Currency, to)], Currency = to };

    private bool CanConvert(Currency from, Currency to)
        => from == to || _exchangeRates.ContainsKey(KeyFor(from, to));

    internal ConversionResult ConvertWithConversionResult(Money money, Currency to)
        => CanConvert(money.Currency, to)
               ? ToResult(money, to)
               : ToFailure(money, to);

    private static ConversionResult ToFailure(Money money, Currency to)
        => new($"{money.Currency}->{to}");

    private ConversionResult ToResult(Money money, Currency to)
        => new(ConvertSafely(money, to));

    public Either<string,Money> Convert(Money money, Currency currency)
    {
        var result = this.ConvertWithConversionResult(money, currency);
        return result.HasFailure()
                   ? result.Failure!
                   : result.Money!;
    }
}