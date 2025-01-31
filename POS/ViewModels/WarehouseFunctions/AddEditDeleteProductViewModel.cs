using POS.ViewModels.Base;

namespace POS.ViewModels.WarehouseFunctions
{
    public class AddEditDeleteProductViewModel : ViewModelBase
    {
        private string productName;
        private string productCategory;
        private string productPrice;
        private string productDescription;
        private string productRecipe;

        public string ProductCategory
        {
            get => productCategory;
            set => SetField(ref productCategory, value);
        }

        public string ProductPrice
        {
            get => productPrice;
            set => SetField(ref productPrice, value);
        }

        public string ProductName
        {
            get => productName;
            set => SetField(ref productName, value);
        }

        public string ProductDescription
        {
            get => productDescription;
            set => SetField(ref productDescription, value);
        }

        public string ProductRecipe
        {
            get => productRecipe;
            set => SetField(ref productRecipe, value);
        }
    }
}
