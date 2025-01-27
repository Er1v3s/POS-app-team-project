using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataAccess.Models;
using POS.Services;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.WarehouseFunctions
{
    public class WarehouseFunctionsViewModel : ViewModelBase
    {
        private readonly IngredientService _ingredientService;

        private ObservableCollection<Ingredient> runningOutOfIngredientsCollection = new();

        public ObservableCollection<Ingredient> RunningOutOfIngredientsCollection
        {
            get => runningOutOfIngredientsCollection;
            set => SetField(ref runningOutOfIngredientsCollection, value);
        }

        public ICommand LoadRunningOutOfIngredientsCommand { get; }

        public WarehouseFunctionsViewModel(IngredientService ingredientService)
        {
            _ingredientService = ingredientService;

            LoadRunningOutOfIngredientsCommand = new RelayCommandAsync(LoadRunningOutOfIngredientsAsync);
        }

        private async Task LoadRunningOutOfIngredientsAsync()
        {
            runningOutOfIngredientsCollection.Clear();

            var runningOutOfIngredients = await _ingredientService.GetRunningOutOfIngredientsAsync();

            foreach (var runningOutOfIngredient in runningOutOfIngredients)
            {
                await Task.Delay(150);
                RunningOutOfIngredientsCollection.Add(runningOutOfIngredient);
            }
        }
    }
}
