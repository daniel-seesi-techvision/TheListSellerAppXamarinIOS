using System.Threading.Tasks;

namespace TheListSellerAppXamariniOS.Services
{
    public interface IAlertService
    {
        void DismissDialog();
        void ShowLoading(string loadingMessage = "Loading");
        void HideLoading();
        Task ShowAlertAsync(string message, string title);
    }
}