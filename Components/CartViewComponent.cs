using Microsoft.AspNetCore.Mvc;
using AspNetCoreCartDemo.Models.Services.Application;
using System.Threading.Tasks;
using AspNetCoreCartDemo.Models.ViewModels;

namespace AspNetCoreCartDemo.Components
{
    public class CartViewComponent : ViewComponent
    {
        private readonly IProductService productService;

        public CartViewComponent(IProductService productService)
        {
            this.productService = productService;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            CartViewModel viewModel = await productService.GetCartAsync();
            return View(viewModel);
        }
    }
}
