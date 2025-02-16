using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        private bool isNewIngredient;

        private bool isDeleteButtonEnable;
        private bool isAddButtonEnable;

        public ObservableCollection<Ingredient> IngredientObservableCollection => _ingredientService.IngredientCollection;

        public Ingredient? SelectedIngredient
        {
            get => selectedIngredient;
            set
            {
                if (SetField(ref selectedIngredient, value))
                {
                    IsIngredientSelected= Visibility.Collapsed;

                    if (isNewIngredient && selectedIngredient != null)
                        IsNewIngredient = false;

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
                {
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
                    ValidateIngredient(_ingredientValidator.ValidateIngredientName, nameof(IngredientName), value, error => IngredientNameError = error);
                }
            }
        }

        public string IngredientUnit
        {
            get => ingredientUnit;
            set
            {
                if (SetField(ref ingredientUnit, value))
                {
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
                    ValidateIngredient(_ingredientValidator.ValidateIngredientUnit, nameof(IngredientUnit), value, error => IngredientUnitError = error);
                }
            }
        }

        public string IngredientPackage
        {
            get => ingredientPackage;
            set
            {
                if (SetField(ref ingredientPackage, value))
                {
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
                    ValidateIngredient(_ingredientValidator.ValidateIngredientPackage, nameof(IngredientPackage), value, error => IngredientPackageError = error);
                }
            }
        }

        public string IngredientDescription
        {
            get => ingredientDescription;
            set
            {
                if (SetField(ref ingredientDescription, value))
                {
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
                    ValidateIngredient(_ingredientValidator.ValidateIngredientDescription, nameof(IngredientDescription), value, error => IngredientDescriptionError = error);
                }
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

        public ICommand AddNewIngredientCommand { get; }
        public ICommand DeleteIngredientCommand { get; }

        public AddEditDeleteIngredientViewModel(IngredientService ingredientService)
        {
            _ingredientService = ingredientService;
            _ingredientValidator = new IngredientValidator();

            AddNewIngredientCommand = new RelayCommandAsync(AddNewIngredient);
            DeleteIngredientCommand = new RelayCommandAsync(DeleteIngredient);
        }

        private async Task AddNewIngredient()
        {
            try
            {
                var newIngredient = CreateIngredient();
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
        }

        private void ResetForm()
        {
            IngredientName = string.Empty;
            IngredientUnit = string.Empty;
            IngredientPackage = string.Empty;
            IngredientDescription = string.Empty;

            SelectedIngredient = null;
            IsIngredientSelected = Visibility.Visible;
        }

        private Ingredient CreateIngredient()
        {
            return new Ingredient
            {
                Name = ingredientName,
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
                   !ingredientDescription.IsNullOrEmpty();

        }

        private bool CheckIfDeleteButtonCanBeEnabled()
        {
            return selectedIngredient != null;
        }
    }
}
