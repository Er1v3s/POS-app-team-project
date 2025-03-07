using System.Windows;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;
using System.Windows.Input;
using DataAccess.Models;
using System.Threading.Tasks;
using POS.Models.Warehouse;
using POS.Services.WarehouseFunctions;
using POS.Utilities;

namespace POS.ViewModels.WarehouseFunctions
{
    public class CreateDeliveryViewModel : ViewModelBase
    {
        private readonly DeliveryService _deliveryService;

        public MyObservableCollection<DeliveryDto> DeliveryObservableCollection => _deliveryService.DeliveryCollection;

        public ICommand IncreaseIngredientQuantityCommand { get; }
        public ICommand DeleteIngredientFromDeliveryCommand { get; }
        public ICommand CancelDeliveryCommand { get; }
        public ICommand GenerateDeliveryCommand { get; }

        public CreateDeliveryViewModel(
            DeliveryService deliveryService)
        {
            _deliveryService = deliveryService;

            _deliveryService.DeliveryCollectionUpdated += OnDeliveryCollectionUpdated;

            IncreaseIngredientQuantityCommand = new RelayCommand<DeliveryDto>(IncreaseIngredientQuantityInDelivery);
            DeleteIngredientFromDeliveryCommand = new RelayCommand<Ingredient>(DeleteIngredientFromDelivery);
            CancelDeliveryCommand = new RelayCommand(CancelDelivery);
            GenerateDeliveryCommand = new RelayCommandAsync(GenerateDelivery);
        }

        private void IncreaseIngredientQuantityInDelivery(DeliveryDto deliveryDto)
        {
            _deliveryService.IncreaseIngredientQuantityInDelivery(deliveryDto);
        }

        private void DeleteIngredientFromDelivery(Ingredient ingredient)
        {
            _deliveryService.DeleteIngredientFromDeliveryCollection(ingredient);
        }

        private void CancelDelivery()
        {
            var result = MessageBox.Show("Czy na pewno chcesz anulować zamówienie?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                _deliveryService.CancelDelivery();
        }

        private async Task GenerateDelivery()
        {
            await _deliveryService.GenerateDeliveryDocument();
        }

        private void OnDeliveryCollectionUpdated()
        {
            OnPropertyChanged(nameof(DeliveryObservableCollection));
        }
    }
}
