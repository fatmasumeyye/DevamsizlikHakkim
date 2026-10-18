namespace DevamsizlikHakkim
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(YeniDersPage), typeof(YeniDersPage));
            Routing.RegisterRoute(nameof(DersDetayPage), typeof(DersDetayPage));
        }
    }
}
