using System.Linq;
using System.Windows;

namespace POS.ViewModels.Base.WarehouseFunctions
{
    public abstract class FormViewModelBase : NotifyFormErrorViewModelBase
    {
        private object? selectedItem;
        private Visibility isItemSelected;

        private bool isNewItem;

        private Visibility isAddButtonVisible;
        private Visibility isUpdateButtonVisible;

        private bool isDeleteButtonEnable;
        private bool isAddButtonEnable;
        private bool isUpdateButtonEnable;

        public object? SelectedItem
        {
            get => selectedItem;
            set
            {
                if (SetField(ref selectedItem, value))
                {
                    IsItemSelected = Visibility.Collapsed;

                    if (IsNewItem && selectedItem is not null)
                    {
                        IsNewItem = false;
                    }

                    if (selectedItem is not null)
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

        public Visibility IsItemSelected
        {
            get => isItemSelected;
            set => SetField(ref isItemSelected, value);
        }

        public virtual bool IsNewItem
        {
            get => isNewItem;
            set
            {
                if (SetField(ref isNewItem, value))
                {
                    if (value)
                    {
                        SelectedItem = null;
                        IsItemSelected = Visibility.Visible;
                    }

                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
                }
            }
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

        protected override void CheckWhichButtonShouldBeEnable()
        {
            if (IsAddButtonVisible == Visibility.Visible)
                IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            if (IsUpdateButtonVisible == Visibility.Visible)
                IsUpdateButtonEnable = CheckIfUpdateButtonCanBeEnabled();
        }

        protected virtual bool CheckIfAddButtonCanBeEnabled()
        {
            return 
                IsNewItem &&
                ArePropertiesValid() &&
                !HasErrors;
        }

        protected virtual bool CheckIfUpdateButtonCanBeEnabled()
        {
            return
                SelectedItem is not null &&
                ArePropertiesValid() &&
                !HasErrors;
        }

        protected virtual bool CheckIfDeleteButtonCanBeEnabled()
        {
            return
                SelectedItem is not null;
        }

        protected virtual void ResetForm()
        {
            var properties = GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanWrite);

            foreach (var property in properties)
                property.SetValue(this, string.Empty);

            SelectedItem = null;
            IsItemSelected = Visibility.Visible;

            ClearAllErrors();
        }

        private bool ArePropertiesValid()
        {
            var properties = GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanWrite)
                .Where(p => !p.Name.EndsWith("Error"));

            return properties.All(p => !string.IsNullOrEmpty(p.GetValue(this) as string));
        }

        protected abstract void LoadDataIntoFormFields(object obj);
    }
}
