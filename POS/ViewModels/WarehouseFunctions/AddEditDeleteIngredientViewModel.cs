using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.WarehouseFunctions
{
    public class AddEditDeleteIngredientViewModel : ViewModelBase
    {
        private ObservableCollection<Ingredient> ingredientObservableCollection = new();

        private Ingredient? selectedIngredient;

        private string ingredientName;
        private string ingredientUnit;
        private string ingredientPackage;
        private string ingredientDescription;

        private Visibility isIngredientSelected;
        private bool isNewIngredient;

        private bool isDeleteButtonEnable;
        private bool isAddButtonEnable;

        public ObservableCollection<Ingredient> IngredientObservableCollection
        {
            get => ingredientObservableCollection;
            set => SetField(ref ingredientObservableCollection, value);
        }

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
                if(SetField(ref ingredientName, value))
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            }
        }

        public string IngredientUnit
        {
            get => ingredientUnit;
            set
            {
                if (SetField(ref ingredientUnit, value))
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            }
        }

        public string IngredientPackage
        {
            get => ingredientPackage;
            set
            {
                if(SetField(ref ingredientPackage, value))
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            }
        }

        public string IngredientDescription
        {
            get => ingredientDescription;
            set
            {
                if (SetField(ref ingredientDescription, value))
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            }
        }

        public Visibility IsIngredientSelected
        {
            get => isIngredientSelected;
            set => SetField(ref isIngredientSelected, value);
        }

        public bool IsNewIngredient
        {
            get => isNewIngredient;
            set => SetField(ref isNewIngredient, value);
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

        public AddEditDeleteIngredientViewModel()
        {
            AddNewIngredientCommand = new RelayCommandAsync(AddNewIngredient);
            DeleteIngredientCommand = new RelayCommandAsync(DeleteIngredient);
        }

        private async Task AddNewIngredient()
        {
            //try
            //{
            //    var newIngredient = new Ingredient
            //    {
            //        Name = IngredientName,
            //        Description = IngredientDescription,
            //        Unit = IngredientUnit,
            //        Package = IngredientPackage
            //    };
            //    await _ingredientService.AddIngredient(newIngredient);
            //    LoadItemsToCollection(IngredientObservableCollection, _ingredientService.GetIngredients());
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private async Task DeleteIngredient()
        {
            //try
            //{
            //    await _ingredientService.DeleteIngredient(SelectedIngredient);
            //    LoadItemsToCollection(IngredientObservableCollection, _ingredientService.GetIngredients());
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
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

        private Ingredient CreateProduct()
        {
            return new Ingredient
            {
                Name = ingredientName,
                Description = ingredientDescription,
                Unit = ingredientUnit,
                Package = ingredientPackage,

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
