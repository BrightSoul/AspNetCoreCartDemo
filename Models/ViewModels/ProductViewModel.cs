using AspNetCoreCartDemo.Models.ValueObjects;

namespace AspNetCoreCartDemo.Models.ViewModels
{
    public record ProductViewModel(int Id, string Title, Money Price);
}