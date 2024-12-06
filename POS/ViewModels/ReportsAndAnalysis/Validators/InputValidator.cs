using System;
using POS.Models.Validation;

namespace POS.ViewModels.ReportsAndAnalysis.Validators
{
    public class InputValidator
    {
        public ValidationResult ValidateInputs(int selectedReportIndex, DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue && endDate.HasValue)
            {
                return new ValidationResult(false, "Zaznacz przedział czasowy");
            }

            if (startDate >= endDate)
            {
                return new ValidationResult(false, "Niepoprawny przedział czasowy");
            }

            return new ValidationResult(true, null);
        }
    }
}
