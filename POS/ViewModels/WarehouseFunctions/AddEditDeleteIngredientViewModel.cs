using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
using POS.Services;
using POS.Utilities.RelayCommands;
using POS.Validators.Models;
using POS.ViewModels.Base.WarehouseFunctions;

namespace POS.ViewModels.WarehouseFunctions
{
    public class AddEditDeleteIngredientViewModel : FormViewModelBase
    {
        private readonly IngredientService _ingredientService;
        private readonly IngredientValidator _ingredientValidator;

        private Ingredient? selectedIngredient;

        private string ingredientName = string.Empty;
        private string ingredientUnit = string.Empty;
        private string ingredientPackage = string.Empty;
        private string ingredientDescription = string.Empty;

        private string ingredientNameError = string.Empty;
        private string ingredientUnitError = string.Empty;
        private string ingredientPackageError = string.Empty;
        private string ingredientDescriptionError = string.Empty;

        private Visibility isIngredientSelected;

        public ObservableCollection<Ingredient> IngredientObservableCollection => _ingredientService.IngredientCollection;

        public Ingredient? SelectedIngredient
        {
            get => selectedIngredient;
            set
            {
                if (SetField(ref selectedIngredient, value))
                {
                    IsIngredientSelected = Visibility.Collapsed;

                    if (IsNewItem && selectedIngredient is not null)
                    {
                        IsNewItem = false;
                    }

                    if(selectedIngredient is not null)
                    {
                        LoadDataIntoFormFields(value!);
                        
                        IsAddButtonVisible = Visibility.Collapsed;
                        IsUpdateButtonVisible = Visibility.Visible;
                    }
                    else
                    {
                        ResetForm();

                        IsUpdateButtonVisible = Visibility.Collapsed;
                        IsAddButtonVisible = Visibility.Visible;
                    } 

                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
                    IsDeleteButtonEnable = CheckIfDeleteButtonCanBeEnabled();
                }
            }
        }

        public required string IngredientName
        {
            get => ingredientName;
            set
            {
                if (SetField(ref ingredientName, value))
                    ValidateProperty(_ingredientValidator.ValidateIngredientName, nameof(IngredientName), value, error => IngredientNameError = error);
            }
        }

        public required string IngredientUnit
        {
            get => ingredientUnit;
            set
            {
                if (SetField(ref ingredientUnit, value))
                    ValidateProperty(_ingredientValidator.ValidateIngredientUnit, nameof(IngredientUnit), value, error => IngredientUnitError = error);
            }
        }

        public required string IngredientPackage
        {
            get => ingredientPackage;
            set
            {
                if (SetField(ref ingredientPackage, value))
                    ValidateProperty(_ingredientValidator.ValidateIngredientPackage, nameof(IngredientPackage), value, error => IngredientPackageError = error);
            }
        }

        public required string IngredientDescription
        {
            get => ingredientDescription;
            set
            {
                if (SetField(ref ingredientDescription, value))
                    ValidateProperty(_ingredientValidator.ValidateIngredientDescription, nameof(IngredientDescription), value, error => IngredientDescriptionError = error);
            }
        }

        public string IngredientNameError
        {
            get => ingredientNameError;
            set => SetField(ref ingredientNameError, value);
        }

        public string IngredientUnitError
        {
            get => ingredientUnitError;
            set => SetField(ref ingredientUnitError, value);
        }

        public string IngredientPackageError
        {
            get => ingredientPackageError;
            set => SetField(ref ingredientPackageError, value);
        }

        public string IngredientDescriptionError
        {
            get => ingredientDescriptionError;
            set => SetField(ref ingredientDescriptionError, value);
        }

        public override bool IsNewItem
        {
            get => isNewItem;
            set
            {
                if (SetField(ref isNewItem, value))
                {
                    if (value)
                    {
                        SelectedIngredient = null;
                        IsIngredientSelected = Visibility.Visible;
                        IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
                    }
                    else
                    {
                        IngredientName = string.Empty;
                        IngredientNameError = string.Empty;
                    }
                }
            }
        }

        public Visibility IsIngredientSelected
        {
            get => isIngredientSelected;
            set => SetField(ref isIngredientSelected, value);
        }

        public ICommand AddNewIngredientCommand { get; }
        public ICommand UpdateIngredientCommand { get; }
        public ICommand DeleteIngredientCommand { get; }

        public AddEditDeleteIngredientViewModel(IngredientService ingredientService)
        {
            _ingredientService = ingredientService;
            _ingredientValidator = new IngredientValidator();

            AddNewIngredientCommand = new RelayCommandAsync(AddNewIngredient);
            UpdateIngredientCommand = new RelayCommandAsync(UpdateIngredient);
            DeleteIngredientCommand = new RelayCommandAsync(DeleteIngredient);
        }

        private async Task AddNewIngredient()
        {
            try
            {
                var newIngredient = await _ingredientService.CreateIngredient(ingredientName, ingredientDescription, ingredientUnit, ingredientPackage);
                await _ingredientService.AddNewIngredientAsync(newIngredient);

                MessageBox.Show("Pomyślnie dodano nowy składnik",
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się utworzyć składnika, przyczyna problemu: {ex.Message}",
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateIngredient()
        {
            try
            {
                var newIngredient = await _ingredientService.CreateIngredient(ingredientName, ingredientDescription, ingredientUnit, ingredientPackage);
                await _ingredientService.UpdateExistingIngredientAsync(selectedIngredient!, newIngredient);

                MessageBox.Show("Pomyślnie edytowano składnik",
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się edytować składnika, przyczyna problemu: {ex.Message}",
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteIngredient()
        {
            try
            {
                await _ingredientService.DeleteIngredientAsync(selectedIngredient!);

                MessageBox.Show("Pomyślnie usunięto produkt",
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się usunąć składnika, przyczyna problemu: {ex.Message}",
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDataIntoFormFields(Ingredient ingredient)
        {
            IngredientName = ingredient.Name;
            IngredientUnit = ingredient.Unit;
            IngredientPackage = ingredient.Package;
            IngredientDescription = ingredient.Description;
        }

        private void ResetForm()
        {
            IngredientName = string.Empty;
            IngredientUnit = string.Empty;
            IngredientPackage = string.Empty;
            IngredientDescription = string.Empty;

            IngredientNameError = string.Empty;
            IngredientUnitError = string.Empty;
            IngredientPackageError = string.Empty;
            IngredientDescriptionError = string.Empty;

            SelectedIngredient = null;
            IsIngredientSelected = Visibility.Visible;

            ClearAllErrors();
        }

        protected override bool CheckIfAddButtonCanBeEnabled()
        {
            return isNewItem &&
                   !ingredientName.IsNullOrEmpty() &&
                   !ingredientUnit.IsNullOrEmpty() &&
                   !ingredientPackage.IsNullOrEmpty() &&
                   !ingredientDescription.IsNullOrEmpty() &&
                   !HasErrors;
        }
        
        protected override bool CheckIfUpdateButtonCanBeEnabled()
        {
            return selectedIngredient != null &&
                   !ingredientName.IsNullOrEmpty() &&
                   !ingredientUnit.IsNullOrEmpty() &&
                   !ingredientPackage.IsNullOrEmpty() &&
                   !ingredientDescription.IsNullOrEmpty() &&
                   !HasErrors;
        }

        protected override bool CheckIfDeleteButtonCanBeEnabled()
        {
            return selectedIngredient != null;
        }
    }
}
