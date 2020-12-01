using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NorthenLightsHospital.Views
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        //Declaration de la bdd pour avoir un access partout
        HospitalEntities myBdd;

        public AdminPage()
        {
            //Initialisation de la bdd et remplisage de la liste de medcin
            InitializeComponent();
            myBdd = new HospitalEntities();

            //On rempli la liste avec les donnees initials
            lstMedcinAdmin.DataContext = myBdd.Medcins.ToList();
        }

        private void btnAjouterMedcin_Click(object sender, RoutedEventArgs e)
        {
            string nomMedcin = txtNomMedcin.Text;
            string prenomMedcin = txtPrenomMedcin.Text;

            //Validation si les champs ne sont pas vide
            if (nomMedcin != "" & prenomMedcin != "")
            {
                //On cree la nouvelle instance d'un Medcin et on rempli les parametres necessaires
                Medcin medcin = new Medcin
                {
                    nom = nomMedcin,
                    prenom = prenomMedcin
                };

                //Validation de l'enregistrement au niveau de la bdd
                try
                {
                    //Ajout du medcin dans la bdd
                    myBdd.Medcins.Add(medcin);
                    myBdd.SaveChanges();

                    //On refresh la liste avec les nouvelles donnee
                    lstMedcinAdmin.DataContext = myBdd.Medcins.ToList();

                    //On vide les champs apres l'operation
                    txtNomMedcin.Text = "";
                    txtPrenomMedcin.Text = "";
                    lstMedcinAdmin.UnselectAll();
                }
                catch (Exception)
                {
                    MessageBox.Show("Erreur d'entregistrement. L'ajout n'a pas pu etre apporter", "Erreur d'enregistrement!");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs doivent etre rempli!", "Champs Vide!");
            }
        }

        private void btnModifierMedcin_Click(object sender, RoutedEventArgs e)
        {
            //Capter le medcin choisi pour ensuite faire les modification
            Medcin medcinSelectionee = lstMedcinAdmin.SelectedItem as Medcin;

            //Verifier si les champs sont pas vide
            if (txtNomMedcin.Text != "" & txtPrenomMedcin.Text != "")
            {
                medcinSelectionee.nom = txtNomMedcin.Text;
                medcinSelectionee.prenom = txtPrenomMedcin.Text;

                //On essaie la modification du medcin selectionne, si on rencontre une erreur on affiche le message d'erreur a l'utlisateur
                try
                {
                    myBdd.SaveChanges();
                    lstMedcinAdmin.DataContext = myBdd.Medcins.ToList();

                    //On vide les champs apres l'operation
                    txtNomMedcin.Text = "";
                    txtPrenomMedcin.Text = "";
                    lstMedcinAdmin.UnselectAll();

                }
                catch (Exception)
                {
                    MessageBox.Show("La modification n'a pas ete complete", "Erreur de modification");
                }

            }
            else
            {
                MessageBox.Show("Tous les champs doivent etre rempli!", "Champs Vide!");
            }

        }

        private void lstMedcinAdmin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Capter le medcin choisi pour ensuite faire les modification
            Medcin medcinSelectionee = lstMedcinAdmin.SelectedItem as Medcin;

            //S'il y a un medcin selectionne remplir les champs avec les informations du medcin
            if (lstMedcinAdmin.SelectedItem != null)
            {
                txtNomMedcin.Text = medcinSelectionee.nom;
                txtPrenomMedcin.Text = medcinSelectionee.prenom;
            }
        }

        private void btnSuppMedcin_Click(object sender, RoutedEventArgs e)
        {
            //Capter le medcin choisi pour ensuite faire les modification
            Medcin medcinSelectionee = lstMedcinAdmin.SelectedItem as Medcin;

            //Essaie de supprimer le medcin, si on rencontre un erruer on affiche un message pour laisser savoir a l'utilisateur.
            try
            {
                myBdd.Medcins.Remove(medcinSelectionee);
                myBdd.SaveChanges();
                lstMedcinAdmin.DataContext = myBdd.Medcins.ToList();

                //On vide les champs apres l'operation
                txtNomMedcin.Text = "";
                txtPrenomMedcin.Text = "";
                lstMedcinAdmin.UnselectAll();
            }
            catch (Exception)
            {
                MessageBox.Show("La supression n'a pas ete complete.", "Erreur de supression!");
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

        //Ramene a la page de login
        private void btnLogoutMedcin_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Login());
        }
    }
}
