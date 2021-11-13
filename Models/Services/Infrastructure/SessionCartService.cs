using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetCoreCartDemo.Models.ValueObjects;
using AspNetCoreCartDemo.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreCartDemo.Models.Services.Infrastructure
{
    public class SessionCartService : ICartService
    {
        private const string cartKey = "Cart";
        private readonly ISession session;

        public SessionCartService(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }

        public Task<CartViewModel> GetAsync()
        {
            CartViewModel cart = GetCartFromSession();
            return Task.FromResult(cart);
        }
        
        public Task AddAsync(int id, string title, Money price)
        {
            CartViewModel cart = GetCartFromSession(); 
            CartLineViewModel productLine = cart.Lines.SingleOrDefault(line => line.Id == id);
            if (productLine == null)
            {
                // It's the first time this product is added to the cart
                productLine = new CartLineViewModel(id, title, 1, price);
                cart.Lines.Add(productLine);
            }
            else
            {
                // It was already existing in the cart, increment the quantity
                int lineIndex = cart.Lines.IndexOf(productLine);
                cart.Lines[lineIndex] = productLine with { Quantity = productLine.Quantity + 1 };
            }

            SaveCartToSession(cart);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(int id)
        {
            CartViewModel cart = GetCartFromSession(); 
            CartLineViewModel productLine = cart.Lines.SingleOrDefault(line => line.Id == id);
            if (productLine == null)
            {
                // Did not exist in the cart, just return
                return Task.CompletedTask;
            }

            cart.Lines.Remove(productLine);
            SaveCartToSession(cart);
            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            session.Remove(cartKey);
            return Task.CompletedTask;
        }

        private CartViewModel CreateNewCart()
        {
            return new CartViewModel(new List<CartLineViewModel>());
        }

        private CartViewModel GetCartFromSession()
        {
            if (session.TryGetValue(cartKey, out byte[] cartData))
            {
                return JsonSerializer.Deserialize<CartViewModel>(cartData);
            }
            
            return CreateNewCart();
        }

        private void SaveCartToSession(CartViewModel cart)
        {
            byte[] cartData = JsonSerializer.SerializeToUtf8Bytes(cart);
            session.Set(cartKey, cartData);
        }
    }
}