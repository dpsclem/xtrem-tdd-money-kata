namespace money_problem.Domain;

public record Money(int Amount, Currency Currency);
public static class MoneyCalculator
{
    public static double Times(Money money, int times) => money.Amount * times;
    public static double Divide(double amount, Currency currency, int divisor) => amount / divisor;
    
}