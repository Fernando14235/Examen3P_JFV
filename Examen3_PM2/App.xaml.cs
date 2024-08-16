namespace Examen3_PM2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Views.viewNote());
        }
    }
}
