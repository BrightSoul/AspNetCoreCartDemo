using Microsoft.AspNetCore.Mvc;
using AspNetCoreCartDemo.Models.Services.Application;
using System.Threading.Tasks;

namespace AspNetCoreCartDemo.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await productService.GetAllAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            await productService.AddToCartAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            await productService.RemoveFromCartAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ClearCart()
        {
            await productService.ClearCartAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
