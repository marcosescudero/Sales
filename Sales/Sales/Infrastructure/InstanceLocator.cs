
namespace Sales.Infrastructure
{
    using ViewModels;
    public class InstanceLocator
    {
        public MainViewModel Main { get; set; } // Main es el objeto en donde todas las pagina van a Bindar.

        public InstanceLocator()
        {
            this.Main = new MainViewModel();
        }
    }
}
