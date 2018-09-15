
namespace Sales.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Services;
    using Xamarin.Forms;

    public class ProductsViewModel : BaseViewModel
    {
        #region Attributes
        private string filter;
        private ApiService apiService;
        private DataService dataService;

        private bool isRefreshing;
        private ObservableCollection<ProductItemViewModel> products;
        #endregion

        #region Properties
        public string Filter
        {
            get { return this.filter; }
            set
            {
                this.filter = value;
                this.RefreshList();
            }
        }
        public List<Product> MyProducts { get; set; }

        public ObservableCollection<ProductItemViewModel> Products
        {
            get { return this.products; }
            set { SetValue(ref this.products, value); }
        }

        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { SetValue(ref this.isRefreshing, value); }
        }
        #endregion

        #region Constructors
        public ProductsViewModel()
        {
            instance = this;
            this.apiService = new ApiService();
            this.dataService = new DataService();
            this.LoadProducts();
            this.IsRefreshing = false;
        }
        #endregion

        #region Singleton
        private static ProductsViewModel instance; // Atributo
        public static ProductsViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new ProductsViewModel(); 
            }
            
            return instance;
        }
        #endregion

        #region Methods
        private async void LoadProducts()
        {
            this.IsRefreshing = true;

            var connection = await apiService.CheckConnection();
            if (connection.IsSuccess)
            {
                var answer = await this.LoadProductsFromAPI();
                if (answer)
                {
                    this.SaveProductsToDB();
                }
            } else
            {
                await this.LoadProductsFromDB();
            }

            if (this.MyProducts == null || this.MyProducts.Count == 0)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.NoProductsMessage, Languages.Accept);
                return;
            }

            this.RefreshList();
            this.IsRefreshing = false;

        }

        private async Task LoadProductsFromDB()
        {
            this.MyProducts = await this.dataService.GetAllProducts();
        }

        private async Task SaveProductsToDB()
        {
            await this.dataService.DeleteAllProducts();
            this.dataService.Insert(this.MyProducts);
        }

        private async Task<bool> LoadProductsFromAPI()
        {
            //var response = await this.apiService.GetList<Product>("http://200.55.241.235", "/SalesAPI/api", "/Products");
            var url = Application.Current.Resources["UrlAPI"].ToString(); // Obtengo la url del diccionario de recursos.
            var prefix = Application.Current.Resources["UrlPrefix"].ToString(); // Obtengo el prefijo del diccionario de recursos.
            var controller = Application.Current.Resources["UrlProductsController"].ToString(); // Obtengo el controlador del diccionario de recursos.

            var response = await this.apiService.GetList<Product>(url, prefix, controller, Settings.TokenType, Settings.AccessToken);

            if (!response.IsSuccess)
            {
                return false;
            }
            this.MyProducts = (List<Product>)response.Result; // hay que castearlo
            return true;
        }

        public void RefreshList()
        {
            // Expresion válida pero de BAJA PERFORMANCE.!!!
            //var myList = new List<ProductItemViewModel>();
            //foreach (var item in list)
            //{
            //    myList.Add(new ProductItemViewModel
            //    {
            //    });
            //}

            if (string.IsNullOrEmpty(this.Filter))
            {
                // Expresion Lamda (ALTA PERFORMANCE)
                var myListProductItemViewModel = this.MyProducts.Select(p => new ProductItemViewModel
                {
                    Description = p.Description,
                    ImageArray = p.ImageArray,
                    ImagePath = p.ImagePath,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    ProductId = p.ProductId,
                    PublishOn = p.PublishOn,
                    Remarks = p.Remarks,
                });
                this.Products = new ObservableCollection<ProductItemViewModel>(
                    myListProductItemViewModel.OrderBy(p => p.Description));
            }
            else
            {
                var myListProductItemViewModel = this.MyProducts.Select(p => new ProductItemViewModel
                {
                    Description = p.Description,
                    ImageArray = p.ImageArray,
                    ImagePath = p.ImagePath,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    ProductId = p.ProductId,
                    PublishOn = p.PublishOn,
                    Remarks = p.Remarks,
                }).Where(p => p.Description.ToLower().Contains(this.Filter.ToLower())).ToList();

                this.Products = new ObservableCollection<ProductItemViewModel>(
                    myListProductItemViewModel.OrderBy(p => p.Description));
            }
        }
        #endregion

        #region Commands
        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(RefreshList);
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadProducts);
            }
        } 
        #endregion
    }
}
