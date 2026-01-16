using OnlineShop_MobileApp.Models;

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
