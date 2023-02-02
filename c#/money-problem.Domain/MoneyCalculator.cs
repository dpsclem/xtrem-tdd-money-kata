namespace money_problem.Domain;

public record Money(double Amount, Currency Currency);

public static class MoneyCalculator
{
    public static double Times(Money money, int times)
        => money.Amount * times;

    public static double Divide(Money money, int divisor)
        => money.Amount / divisor;
}