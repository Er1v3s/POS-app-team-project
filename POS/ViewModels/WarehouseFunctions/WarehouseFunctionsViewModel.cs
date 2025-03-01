﻿using System.Threading.Tasks;
using System.Windows.Input;
using DataAccess.Models;
using POS.Services;
using POS.Utilities;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;
using POS.Views.Windows.WarehouseFunctions;

namespace POS.ViewModels.WarehouseFunctions
{
    public class WarehouseFunctionsViewModel : ViewModelBase
    {
        private readonly IngredientService _ingredientService;
        private readonly NavigationService _navigationService;

        private MyObservableCollection<Ingredient> runningOutOfIngredientsCollection = new();

        public MyObservableCollection<Ingredient> RunningOutOfIngredientsCollection
        {
            get => runningOutOfIngredientsCollection;
            set => SetField(ref runningOutOfIngredientsCollection, value);
        }

        public ICommand LoadRunningOutOfIngredientsCommand { get; }
        public ICommand OpenStockManagementWindowCommand { get; }
        public ICommand OpenCreateDeliveryWindowCommand { get; }

        public WarehouseFunctionsViewModel(IngredientService ingredientService, NavigationService navigationService)
        {
            _ingredientService = ingredientService;
            _navigationService = navigationService;

            LoadRunningOutOfIngredientsCommand = new RelayCommandAsync(LoadRunningOutOfIngredientsAsync);
            OpenStockManagementWindowCommand = new RelayCommand<StockManagementWindow>(OpenWindow);
            OpenCreateDeliveryWindowCommand = new RelayCommand<CreateDeliveryWindow>(OpenWindow);
        }

        private async Task LoadRunningOutOfIngredientsAsync()
        {
            runningOutOfIngredientsCollection.Clear();

            var runningOutOfIngredients = await _ingredientService.GetRunningOutOfIngredientsAsync();
            await RunningOutOfIngredientsCollection.AddRangeWithDelay(runningOutOfIngredients, 100);
        }

        private void OpenWindow<T>(T windowType)
        {
            _navigationService.OpenNewWindow(windowType);
            _navigationService.CloseCurrentWindow<Views.Windows.MainWindow>();
        }
    }
}
