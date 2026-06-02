using System.Windows;

namespace Demo
{
    public partial class UserWindow : Window
    {
        public UserWindow() => InitializeComponent();
        private void Back_Click(object sender, RoutedEventArgs e) { new MainWindow().Show(); Close(); }
    }
}