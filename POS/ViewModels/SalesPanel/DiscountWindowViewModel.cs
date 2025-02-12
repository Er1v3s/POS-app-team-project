using System.Windows.Input;
using POS.Services.SalesPanel;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.SalesPanel
{
    public class DiscountWindowViewModel : ViewModelBase
    {
        private readonly DiscountService _discountService;

        private bool dialogResult;

        public bool DialogResult
        {
            get => dialogResult;
            set => SetField(ref dialogResult, value);
        }

        public ICommand SetDiscountValueCommand { get; }
        public ICommand ApplyDiscountCommand { get; }

        public DiscountWindowViewModel(DiscountService discountService)
        {
            _discountService = discountService;

            SetDiscountValueCommand = new RelayCommand<string>(SetDiscountValue);
            ApplyDiscountCommand = new RelayCommand(ApplyDiscount);
        }

        private void SetDiscountValue(string discountValue)
        {
            _discountService.SetDiscount(int.Parse(discountValue));
        }

        private void ApplyDiscount()
        {
            DialogResult = true;
            CloseWindowBaseAction!.Invoke();
        }
    }
}