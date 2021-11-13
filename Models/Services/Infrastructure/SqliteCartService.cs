using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetCoreCartDemo.Models.ValueObjects;
using AspNetCoreCartDemo.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;

namespace AspNetCoreCartDemo.Models.Services.Infrastructure
{
    public class SqliteCartService : ICartService
    {
        private const string cookieName = "CartId";
        private readonly IHttpContextAccessor httpContextAccessor;

        public SqliteCartService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<CartViewModel> GetAsync()
        {
            CartViewModel cart = await GetCartFromDatabaseAsync();
            return cart;
        }

        public async Task AddAsync(int id, string title, Money price)
        {
            CartViewModel cart = await GetCartFromDatabaseAsync();
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

            await SaveCartToDatabaseAsync(cart);
        }

        public async Task RemoveAsync(int id)
        {
            CartViewModel cart = await GetCartFromDatabaseAsync();
            CartLineViewModel productLine = cart.Lines.SingleOrDefault(line => line.Id == id);
            if (productLine == null)
            {
                // Did not exist in the cart, just return
                return;
            }

            cart.Lines.Remove(productLine);
            await SaveCartToDatabaseAsync(cart);
        }

        public async Task ClearAsync()
        {
            string cartId = GetOrCreateCartId();
            using DbConnection conn = await GetOpenConnectionAsync();
            using DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Carts WHERE CartId=@cartId";

            DbParameter cartIdParameter = cmd.CreateParameter();
            cartIdParameter.ParameterName = "cartId";
            cartIdParameter.Value = cartId;
            cmd.Parameters.Add(cartIdParameter);

            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<CartViewModel> GetCartFromDatabaseAsync()
        {
            string cartId = GetOrCreateCartId();
            using DbConnection conn = await GetOpenConnectionAsync();
            using DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Contents FROM Carts WHERE CartId=@cartId LIMIT 1";

            DbParameter cartIdParameter = cmd.CreateParameter();
            cartIdParameter.ParameterName = "cartId";
            cartIdParameter.Value = cartId;
            cmd.Parameters.Add(cartIdParameter);

            string content = (await cmd.ExecuteScalarAsync()) as string;

            if (!string.IsNullOrEmpty(content))
            {
                return JsonSerializer.Deserialize<CartViewModel>(content);
            }

            return CreateNewCart();
        }

        private async Task SaveCartToDatabaseAsync(CartViewModel cart)
        {
            string cartId = GetOrCreateCartId();
            using DbConnection conn = await GetOpenConnectionAsync();
            using DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT OR REPLACE INTO Carts (CartId, Contents, LastActivity) VALUES (@cartId, @contents, CURRENT_TIMESTAMP)";
            
            DbParameter cartIdParam = cmd.CreateParameter();
            cartIdParam.ParameterName = "cartId";
            cartIdParam.Value = cartId;
            cmd.Parameters.Add(cartIdParam);

            DbParameter contentsParam = cmd.CreateParameter();
            contentsParam.ParameterName = "contents";
            contentsParam.Value = JsonSerializer.Serialize(cart);
            cmd.Parameters.Add(contentsParam);

            await cmd.ExecuteNonQueryAsync();
        }

        private CartViewModel CreateNewCart()
        {
            return new CartViewModel(new List<CartLineViewModel>());
        }

        private string GetOrCreateCartId()
        {
            // Try to get the cartId from the request
            string cartId = httpContextAccessor.HttpContext.Request.Cookies[cookieName];
            if (!string.IsNullOrEmpty(cartId))
            {
                return cartId;
            }

            // No cartId was found, let's give it one
            cartId = Guid.NewGuid().ToString();

            // Add it as a persistent cookie to the request
            // So that the browser will return it on each subsequent request
            CookieOptions options = new()
            {
                Expires = DateTimeOffset.UtcNow.AddMonths(1), // cart will be accessible to the user for one month
                HttpOnly = true,
                IsEssential = true,
                Secure = true
            };

            httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, cartId, options);
            return cartId;
        }

        private async Task<DbConnection> GetOpenConnectionAsync()
        {
            SqliteConnection conn = new("DataSource=Data/Cart.db");
            await conn.OpenAsync();
            return conn;
        }
    }
}