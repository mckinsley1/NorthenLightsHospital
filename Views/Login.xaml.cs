using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NorthenLightsHospital.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        //Quand on click le bouton login on verifie le username et le mdp et on redirige a la bonne page
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usersername = txtUsername.Text;
            string password = txtPassword.Password;

            if (usersername == "admin" & password == "admin")
            {
                this.NavigationService.Navigate(new AdminPage());
            }

            if (usersername == "medecin" & password == "medecin")
            {
                this.NavigationService.Navigate(new MedcinPage());
            }


            if (usersername == "prepose" & password == "prepose")
            {
                this.NavigationService.Navigate(new PreposePage());
            }

        }

        //Quand on pese sur le bouton entree on verifie le username et le mdp et on redirige a la bonne page
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            string usersername = txtUsername.Text;
            string password = txtPassword.Password;

            if (e.Key == Key.Return)
            {
                if (usersername == "admin" & password == "admin")
                {
                    this.NavigationService.Navigate(new AdminPage());
                }

                if (usersername == "medcin" & password == "medcin")
                {
                    this.NavigationService.Navigate(new MedcinPage());
                }

                if (usersername == "prepose" & password == "prepose")
                {
                    this.NavigationService.Navigate(new PreposePage());
                }
            }
        }

        //Permet de bouger la fenetre
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Window window = Application.Current.MainWindow;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                window.DragMove();
            }
        }

        //Ferme l'application
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
