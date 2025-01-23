namespace POS.Services.SalesPanel
{
    public class DiscountService
    {
        public int DiscountValue { get; private set; }

        public void SetDiscount(int discountValue)
        {
            DiscountValue = discountValue;
        }
    }
}