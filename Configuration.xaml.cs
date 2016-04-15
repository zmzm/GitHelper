using System.Windows;

namespace GitSharp.Demo
{
    public partial class Configuration : Window
    {

        public Configuration()
        {
            InitializeComponent();
        }
        public void Init(GitSharp.Repository repository)
        {
            configurationList.ItemsSource = repository.Config;
        }

        private void OnLoadConfiguration(object sender, RoutedEventArgs e)
        {
        }

        private void OnSaveConfiguration(object sender, RoutedEventArgs e)
        {
        }
    }
}
