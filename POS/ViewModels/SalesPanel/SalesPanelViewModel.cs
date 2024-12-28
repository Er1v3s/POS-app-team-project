using POS.Services.Login;
using POS.ViewModels.Base;

namespace POS.ViewModels.SalesPanel
{
    public class SalesPanelViewModel : ViewModelBase
    {
        private string loggedInUserName;

        public string LoggedInUserName
        {
            get => loggedInUserName;
            set => SetField(ref loggedInUserName, value);
        }

        public SalesPanelViewModel()
        {
            loggedInUserName = LoginManager.Instance.GetLoggedInUserFullName();
        }
    }
}
