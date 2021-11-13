using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreCartDemo.Models.Services.Infrastructure;
using AspNetCoreCartDemo.Models.ViewModels;

namespace AspNetCoreCartDemo.Models.Services.Application
{
    public class FakeProductService : IProductService
    {
        private readonly ICartService cartService;

        public FakeProductService(ICartService cartService)
        {
            this.cartService = cartService;
        }

        public Task<CartViewModel> GetCartAsync()
        {
            return cartService.GetAsync();
        }

        public Task<IList<ProductViewModel>> GetAllAsync()
        {
            // TODO: Actually get these from the database
            // Products are hardcoded for this demo
            IList<ProductViewModel> list = new List<ProductViewModel>
            {
                new ProductViewModel(1, "42\" LCD TV", new("EUR", 399.00m)),
                new ProductViewModel(2, "Microwave oven", new("EUR", 199.00m)),
                new ProductViewModel(3, "Turbo washing machine", new("EUR", 159.00m)),
            };
            return Task.FromResult(list);
        }

        public async Task<ProductViewModel> GetByIdAsync(int id)
        {
            IList<ProductViewModel> products = await GetAllAsync();
            return products.Single(product => product.Id == id);
        }

        public async Task AddToCartAsync(int id)
        {
            ProductViewModel product = await GetByIdAsync(id);
            await cartService.AddAsync(product.Id, product.Title, product.Price);
        }

        public Task RemoveFromCartAsync(int id)
        {
            return cartService.RemoveAsync(id);
        }

        public Task ClearCartAsync()
        {
            return cartService.ClearAsync();
        }
    }
}