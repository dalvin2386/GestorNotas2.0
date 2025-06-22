using GestorNotas2._0.Views;

namespace GestorNotas2._0
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainView());
        }
    }
}
