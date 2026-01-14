using OnlineShop_MobileApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Navigators
{
    public interface IMainPageNavigator
    {
        void ShowCatalog();
        void ShowProductDetails(Product product);
        void ShowAllert(string title, string message);
        void ShowMessage(string Message);
        void RedirectToLoginPage();
    }
}
