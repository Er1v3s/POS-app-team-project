using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
using POS.Services;
using POS.Utilities.RelayCommands;
using POS.Validators;
using POS.Validators.Models;
using POS.ViewModels.Base;
namespace POS.ViewModels.WarehouseFunctions
{
    public class AddEditDeleteIngredientViewModel : ViewModelBase
    {
        private readonly IngredientService _ingredientService;
        private readonly IngredientValidator _ingredientValidator;

        private Ingredient? selectedIngredient;

        private string ingredientName;
        private string ingredientUnit;
        private string ingredientPackage;
        private string ingredientDescription;

        private string ingredientNameError;
        private string ingredientUnitError;
        private string ingredientPackageError;
        private string ingredientDescriptionError;

        private Visibility isIngredientSelected;
        private Visibility isAddButtonVisible;
        private Visibility isUpdateButtonVisible;
        private bool isNewIngredient;

        private bool isDeleteButtonEnable;
        private bool isAddButtonEnable;
        private bool isUpdateButtonEnable;

        public ObservableCollection<Ingredient> IngredientObservableCollection => _ingredientService.IngredientCollection;

        public Ingredient? SelectedIngredient
        {
            get => selectedIngredient;
            set
            {
                if (SetField(ref selectedIngredient, value))
                {
                    IsIngredientSelected = Visibility.Collapsed;

                    if (isNewIngredient && selectedIngredient != null)
                    {
                        IsNewIngredient = false;
                    }

                    if(selectedIngredient != null)
                    {
                        LoadDataIntoFormFields(value);
                        
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

        public string IngredientName
        {
            get => ingredientName;
            set
            {
                if (SetField(ref ingredientName, value))
                    ValidateIngredient(_ingredientValidator.ValidateIngredientName, nameof(IngredientName), value, error => IngredientNameError = error);
            }
        }

        public string IngredientUnit
        {
            get => ingredientUnit;
            set
            {
                if (SetField(ref ingredientUnit, value))
                    ValidateIngredient(_ingredientValidator.ValidateIngredientUnit, nameof(IngredientUnit), value, error => IngredientUnitError = error);
            }
        }

        public string IngredientPackage
        {
            get => ingredientPackage;
            set
            {
                if (SetField(ref ingredientPackage, value))
                    ValidateIngredient(_ingredientValidator.ValidateIngredientPackage, nameof(IngredientPackage), value, error => IngredientPackageError = error);
            }
        }

        public string IngredientDescription
        {
            get => ingredientDescription;
            set
            {
                if (SetField(ref ingredientDescription, value))
                    ValidateIngredient(_ingredientValidator.ValidateIngredientDescription, nameof(IngredientDescription), value, error => IngredientDescriptionError = error);
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

        public Visibility IsIngredientSelected
        {
            get => isIngredientSelected;
            set => SetField(ref isIngredientSelected, value);
        }

        public Visibility IsAddButtonVisible
        {
            get => isAddButtonVisible;
            set => SetField(ref isAddButtonVisible, value);
        }

        public Visibility IsUpdateButtonVisible
        {
            get => isUpdateButtonVisible;
            set => SetField(ref isUpdateButtonVisible, value);
        }

        public bool IsNewIngredient
        {
            get => isNewIngredient;
            set
            {
                if (SetField(ref isNewIngredient, value))
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

        public bool IsDeleteButtonEnable
        {
            get => isDeleteButtonEnable;
            set => SetField(ref isDeleteButtonEnable, value);
        }

        public bool IsAddButtonEnable
        {
            get => isAddButtonEnable;
            set => SetField(ref isAddButtonEnable, value);
        }

        public bool IsUpdateButtonEnable
        {
            get => isUpdateButtonEnable;
            set => SetField(ref isUpdateButtonEnable, value);
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
                var newIngredient = CreateIngredient(ingredientName);
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
                var newIngredient = CreateIngredient(selectedIngredient!.Name);
                await _ingredientService.UpdateExistingIngredientAsync(selectedIngredient, newIngredient);

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

        private void ValidateIngredient(Func<string, ValidationResult> validationFunc, string propertyName, string propertyValue, Action<string> setError)
        {
            ClearErrors(propertyName);

            var isIngredientValidate = validationFunc(propertyValue);

            if (isIngredientValidate.Result == false)
            {
                AddError(propertyName, isIngredientValidate.ErrorMessage!);
                setError(isIngredientValidate.ErrorMessage ?? string.Empty);
            }
            else
                setError(string.Empty);

            if (IsAddButtonVisible == Visibility.Visible)
                IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            if(IsUpdateButtonVisible == Visibility.Visible)
                IsUpdateButtonEnable = CheckIfUpdateButtonCanBeEnabled();
        }

        private void LoadDataIntoFormFields(Ingredient selectedIngredient)
        {
            IngredientName = selectedIngredient.Name;
            IngredientUnit = selectedIngredient.Unit;
            IngredientPackage = selectedIngredient.Package;
            IngredientDescription = selectedIngredient.Description;
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

        private Ingredient CreateIngredient(string ingredientNameValue)
        {
            if (ingredientNameValue == null) throw new ArgumentNullException(ingredientNameValue, "Nieprawidłowa nazwa składnika");

            return new Ingredient
            {
                Name = ingredientNameValue,
                Description = ingredientDescription,
                Unit = ingredientUnit,
                Package = ingredientPackage,
                Stock = 0,
                SafetyStock = 0
            };
        }

        private bool CheckIfAddButtonCanBeEnabled()
        {
            return isNewIngredient &&
                   !ingredientName.IsNullOrEmpty() &&
                   !ingredientUnit.IsNullOrEmpty() &&
                   !ingredientPackage.IsNullOrEmpty() &&
                   !ingredientDescription.IsNullOrEmpty() &&
                   !HasErrors;
        }
        
        private bool CheckIfUpdateButtonCanBeEnabled()
        {
            return selectedIngredient != null &&
                   !ingredientName.IsNullOrEmpty() &&
                   !ingredientUnit.IsNullOrEmpty() &&
                   !ingredientPackage.IsNullOrEmpty() &&
                   !ingredientDescription.IsNullOrEmpty() &&
                   !HasErrors;
        }

        private bool CheckIfDeleteButtonCanBeEnabled()
        {
            return selectedIngredient != null;
        }
    }
}
