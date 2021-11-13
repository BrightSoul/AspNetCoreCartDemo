using System;

namespace AspNetCoreCartDemo.Models.ValueObjects
{
    public record Money(string Currency, decimal Amount)
    {
        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
            {
                throw new InvalidOperationException("Can't sum with different currencies");
            }

            return new Money(a.Currency, a.Amount + b.Amount);
        }

        public static Money operator *(Money a, int quantity)
        {
            return new Money(a.Currency, a.Amount * quantity);
        }

        public override string ToString()
        {
            return $"{Currency} {Amount}";
        }
    }
}