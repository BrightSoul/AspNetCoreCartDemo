using AspNetCoreCartDemo.Models.ValueObjects;

namespace AspNetCoreCartDemo.Models.ViewModels
{
    public record CartLineViewModel(int Id, string Title, int Quantity, Money Total);
}