using OnlineShop_MobileApp.Models;

namespace OnlineShop_MobileApp.Navigators
{
    //To be honest it have became controller instead of navigator
    public interface IMainPageNavigator
    {
        void ShowCatalog();
        void ShowProductDetails(Product product);
        void ShowAllert(string title, string message);
        void ShowMessage(string Message);
        void RedirectToLoginPage();
    }
}
