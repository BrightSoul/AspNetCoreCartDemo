using System.Threading.Tasks;
using AspNetCoreCartDemo.Models.ValueObjects;
using AspNetCoreCartDemo.Models.ViewModels;

namespace AspNetCoreCartDemo.Models.Services.Infrastructure
{
    public interface ICartService
    {
        Task<CartViewModel> GetAsync();
        Task AddAsync(int id, string title, Money price);
        Task RemoveAsync(int id);
        Task ClearAsync();
    }
}