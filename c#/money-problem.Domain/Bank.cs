namespace money_problem.Domain
{
    public sealed class Bank
    {
        private readonly Dictionary<string, double> _exchangeRates;

        private Bank(Dictionary<string, double> exchangeRates) => _exchangeRates = exchangeRates;

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

        private static string KeyFor(Currency from, Currency to) => $"{from}->{to}";

        public Money ConvertOld(Money money, Currency to) =>
            CanConvert(money.Currency, to)
                ? ConvertSafely(money, to)
                : throw new MissingExchangeRateException(money.Currency, to);
        
     

        private Money ConvertSafely(Money money, Currency to)
            => to == money.Currency
                   ? money
                   : money with { Amount = money.Amount * _exchangeRates[KeyFor(money.Currency, to)], Currency = to };

        private bool CanConvert(Currency from, Currency to) =>
            from == to || _exchangeRates.ContainsKey(KeyFor(from, to));


        public Portfolio.ConversionResult<string> Convert(Money money, Currency to)
        {
            try
            {
                return new Portfolio.ConversionResult<string>(ConvertOld(money, to));
            }
            catch (MissingExchangeRateException missingExchangeRateException)
            {
                return new Portfolio.ConversionResult<string>(missingExchangeRateException.Message);
            }
        }
    }
}