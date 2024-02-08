using Microsoft.AspNetCore.Mvc;
using WebShop.Models;

namespace WebShop.Utilites;

public class SessionServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionServices(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    //  метод чинит сессию
    public void RepairSession(int productId)
    {
        try
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            var session = _httpContextAccessor.HttpContext.Session.Get<List<ShoppingCart>>(WebConst.SessionCart);
            if (session != null && session.Any())
            {
                shoppingCartList = session;
            }

            var currentProduct = shoppingCartList.SingleOrDefault(x => x.ProductId == productId);

            //  Добавляем первый раз в корзину или ошибочно добавлено 2 предмета
            if (currentProduct == null)
            {
                //  проверОчка вдруг 2 объекта с одинаковым id
                var list = shoppingCartList.Where(x => x.ProductId == productId);
                if (!list.Any()) shoppingCartList.Add(new ShoppingCart() { ProductId = productId, Amount = 1 });
                else
                {
                    int amount = 0;
                    // объединить количество и удалить
                    foreach (var item in list)
                    {
                        amount = item.Amount;
                        shoppingCartList.Remove(item);
                    }

                    shoppingCartList.Add(new ShoppingCart() { ProductId = productId, Amount = amount });
                }
            }

            _httpContextAccessor.HttpContext.Session.Set(WebConst.SessionCart, shoppingCartList);
        }
        catch (Exception e)
        {
        }
    }

    public List<ShoppingCart> GetSession(string key)
    {
        List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
        var session = _httpContextAccessor.HttpContext.Session.Get<List<ShoppingCart>>(key);
        if (session != null && session.Count() > 0)
        {
            shoppingCartList = session;
        }

        return shoppingCartList;
    }

    [HttpPost]
    public bool PlusToCart(int id)
    {
        try
        {
            var shoppingCartList = GetSession(WebConst.SessionCart);

            //  починили сессию
            RepairSession(id);

            var currentProduct = shoppingCartList.FirstOrDefault(x => x.ProductId == id);

            //  Добавляем первый раз в корзину
            if (currentProduct == null)
            {
                shoppingCartList.Add(new ShoppingCart() { ProductId = id, Amount = 1 });
            }
            //  есть в корзине
            else
            {
                shoppingCartList.Remove(currentProduct);
                currentProduct.Amount = ++currentProduct.Amount;
                shoppingCartList.Add(currentProduct);
            }

            //  храним серелизованный list в сессии
            _httpContextAccessor.HttpContext.Session.Set(WebConst.SessionCart, shoppingCartList);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    // -1 в корзину
    public bool MinusToCart(int id)
    {
        try
        {
            var shoppingCartList = GetSession(WebConst.SessionCart);

            var currentProduct = shoppingCartList.FirstOrDefault(x => x.ProductId == id);
            var Productamount = currentProduct.Amount;
            //  уменьшаем количество
            if (currentProduct != null && Productamount >= 1)
            {
                // если элементов стало 0, полное удаление
                shoppingCartList.Remove(currentProduct);
                --Productamount;

                if (Productamount > 0)
                {
                    currentProduct.Amount = Productamount;
                    shoppingCartList.Add(currentProduct);
                }
            }

            _httpContextAccessor.HttpContext.Session.Set(WebConst.SessionCart, shoppingCartList);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}