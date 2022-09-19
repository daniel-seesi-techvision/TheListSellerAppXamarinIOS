using System.Threading.Tasks;
using Acr.UserDialogs;
using static TheListSellerAppXamariniOS.Constants.StringConstants;

namespace TheListSellerAppXamariniOS.Services
{
    public class AlertService : IAlertService
    {

        private readonly IUserDialogs _userDialog;

        public AlertService()
        {
            _userDialog = UserDialogs.Instance;
        }

        public void DismissDialog()
        {
            _userDialog.HideLoading();
        }

        public void HideLoading()
        {
            _userDialog.HideLoading();
        }


        public void ShowLoading(string loadingMessage = "Loading")
        {
            _userDialog.ShowLoading(loadingMessage);
        }

        public async Task ShowAlertAsync(string message, string title)
        {
            _userDialog.HideLoading();
            await _userDialog.AlertAsync(message, title, OK);
        }
    }
}

