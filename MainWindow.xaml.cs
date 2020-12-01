using NorthenLightsHospital.Views;
using System.Windows;

namespace NorthenLightsHospital
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(new Login());
        }
    }
}
