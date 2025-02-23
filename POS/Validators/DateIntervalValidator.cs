using System;

namespace POS.Validators
{
    public static class DateIntervalValidator
    {
        public static ValidationResult ValidateDateInterval(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue && endDate.HasValue)
            {
                return new ValidationResult(false, "Zaznacz przedział czasowy");
            }

            if (startDate >= endDate)
            {
                return new ValidationResult(false, "Niepoprawny przedział czasowy");
            }

            return new ValidationResult(true);
        }
    }
}