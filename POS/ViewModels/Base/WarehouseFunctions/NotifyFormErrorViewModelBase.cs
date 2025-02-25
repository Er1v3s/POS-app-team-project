using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using POS.Validators;

namespace POS.ViewModels.Base.WarehouseFunctions
{
    public abstract class NotifyFormErrorViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new ();

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

            CheckWhichButtonShouldBeEnable();
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

        protected abstract void CheckWhichButtonShouldBeEnable();
    }
}
