using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.SalesPanel
{
    public class DiscountWindowViewModel : ViewModelBase
    {
        private bool dialogResult;

        public bool DialogResult
        {
            get => dialogResult;
            set => SetField(ref dialogResult, value);
        }

        public ICommand ApplyDiscountCommand { get; }

        public DiscountWindowViewModel()
        {
            ApplyDiscountCommand = new RelayCommand(ApplyDiscount);
        }

        private void ApplyDiscount()
        {
            DialogResult = true;
            CloseWindowBaseAction!.Invoke();
        }
    }
}
