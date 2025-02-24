using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using POS.Validators;

namespace POS.ViewModels.Base.WarehouseFunctions
{
    public abstract class FormViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

        private Visibility isAddButtonVisible;
        private Visibility isUpdateButtonVisible;

        private bool isDeleteButtonEnable;
        private bool isAddButtonEnable;
        private bool isUpdateButtonEnable;

        protected bool isNewItem;

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

        public virtual bool IsNewItem
        {
            get => isNewItem;
            set => SetField(ref isNewItem, value);
        }

        public bool HasErrors => _errorsByPropertyName.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void ValidateProperty(Func<string, ValidationResult> validationFunc, string propertyName, string propertyValue, Action<string> setError)
        {
            ClearErrors(propertyName);

            var isPropertyValidate = validationFunc(propertyValue);

            if (isPropertyValidate.Result == false)
            {
                AddError(propertyName, isPropertyValidate.ErrorMessage!);
                setError(isPropertyValidate.ErrorMessage ?? string.Empty);
            }
            else
                setError(string.Empty);

            if (IsAddButtonVisible == Visibility.Visible)
                IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            if (IsUpdateButtonVisible == Visibility.Visible)
                IsUpdateButtonEnable = CheckIfUpdateButtonCanBeEnabled();
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName) ?
                _errorsByPropertyName[propertyName] : null;
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        protected void ClearAllErrors()
        {
            var propertiesWithErrors = _errorsByPropertyName.Keys.ToList();
            _errorsByPropertyName.Clear();

            foreach (var property in propertiesWithErrors)
                OnErrorsChanged(property);
        }

        protected abstract bool CheckIfAddButtonCanBeEnabled();
        protected abstract bool CheckIfDeleteButtonCanBeEnabled();
        protected abstract bool CheckIfUpdateButtonCanBeEnabled();
    }
}
