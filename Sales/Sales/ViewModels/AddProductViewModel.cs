

namespace Sales.ViewModels
{
    using System.Windows.Input;
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Services;
    using Xamarin.Forms;

    public class AddProductViewModel:BaseViewModel // IMPORTANTE: las ViewModel deben heredar de la BaseViewmodel
    {
        #region Attributes
        private ApiService apiService;
        private bool isRunning;
        private bool isEnabled;
        #endregion

        #region Properties
        public string Description { get; set; }

        public string Price { get; set; } // IMPORTANTE: El proce es Decimal, pero es conveniente Capturarlo en string y luego convertir.!!!

        public string Remarks { get; set; }

        public bool IsRunning
        {
            get { return this.isRunning; }
            set { SetValue(ref this.isRunning, value); }
        }
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { SetValue(ref this.isEnabled, value); }
        }

        #endregion

        #region Constructors
        public AddProductViewModel()
        {
            this.isEnabled = true;
            this.apiService = new ApiService();
        }
        #endregion

        #region Commands
        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Description)) {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error, 
                    Languages.DescriptionError, 
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.Price))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PriceError,
                    Languages.Accept);
                return;
            }

            var price = decimal.Parse(this.Price);
            if (price < 0)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PriceError,
                    Languages.Accept);
                return;
            }

            this.IsRunning = true; // Esto muestra el activity indicator
            this.IsEnabled = false; // Desabilito el botón de SAVE para que el usuario no le pegue varias veces.


            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error, 
                    connection.Message, 
                    Languages.Accept);
                return;
            }

            var product = new Product
            {
                Description = this.Description,
                Price = price,
                Remarks = this.Remarks,
            };

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.Post(url, prefix, controller, product);

            if (!response.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error, 
                    response.Message, 
                    Languages.Accept);
                return;
            }

            /*
             *   Agregamos el producto en la ListView. 
             *   NOTA: El producto que enviamos no tiene fecha, ni IsAvailable, 
             *   ni el Id del producto, en cambio, cuando va a la api y vuelve, ya trae todos esos datos.
             */

            var newProduct = (Product) response.Result;
            var viewModel = ProductsViewModel.GetInstance();
            viewModel.Products.Add(newProduct); // Le agrego el nuevo producto 

            this.IsRunning = false;
            this.IsEnabled = true;
            await Application.Current.MainPage.Navigation.PopAsync();
        }



    }



        #endregion
    
}
