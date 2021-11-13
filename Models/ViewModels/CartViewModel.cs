using System.Collections.Generic;
using System.Linq;
using AspNetCoreCartDemo.Models.ValueObjects;

namespace AspNetCoreCartDemo.Models.ViewModels
{
    public record CartViewModel(IList<CartLineViewModel> Lines)
    {
        public Money Total
        {
            get
            {
                if (Lines.Count == 0)
                {
                    return new Money("EUR", 0.00m);
                }

                return Lines.Select(line => line.Total * line.Quantity).Aggregate((grandTotal, lineTotal) => grandTotal + lineTotal);
            }
        }
    }
}