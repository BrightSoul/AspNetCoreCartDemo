using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreCartDemo.Models.ViewModels;

namespace AspNetCoreCartDemo.Models.Services.Application
{
    public interface IProductService
    {
        Task<IList<ProductViewModel>> GetAllAsync();
        Task<ProductViewModel> GetByIdAsync(int id);
        Task<CartViewModel> GetCartAsync();
        Task AddToCartAsync(int productId);
        Task RemoveFromCartAsync(int productId);
        Task ClearCartAsync();
    }
}