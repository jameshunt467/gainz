namespace gainz
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        public static class Constants
        {
            public const string LogTag = "gainzLog";
        }
    }
}
