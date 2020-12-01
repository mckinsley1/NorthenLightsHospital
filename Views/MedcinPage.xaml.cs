using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NorthenLightsHospital.Views
{
    /// <summary>
    /// Interaction logic for MedcinPage.xaml
    /// </summary>
    public partial class MedcinPage : Page
    {
        HospitalEntities myBdd;

        public MedcinPage()
        {
            InitializeComponent();

            //On cree l'instance de la bdd
            myBdd = new HospitalEntities();

            //On remplie la liste avec les Admission qui ont pas de date de conge
            lstAdmissionMedcin.DataContext = (from ad in myBdd.Admissions where ad.Date_du_conge == null select ad).ToList();
        }

        //Popule les champs de texte par rapport au medecin selectionnee
        private void lstPatientMedcin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Admission admission = lstAdmissionMedcin.SelectedItem as Admission;
            Patient patient = (from p in myBdd.Patients where p.NSS == admission.NSS select p).First() as Patient;

            txtNom.Text = patient.nom;
            txtPrenom.Text = patient.prenom;
            txtDateNaissance.Text = patient.Date_naissance.ToString("MM / dd / yyyy");
            txtDateAdmission.Text = admission.Date_admission.ToString("MM / dd / yyyy");
        }


        private void btnDonnerConge_Click(object sender, RoutedEventArgs e)
        {
            //Capter l'admission selectionnee
            Admission admissionSelectionnee = lstAdmissionMedcin.SelectedItem as Admission;

            //On met la date du conge a la date d'aujourd'hui
            admissionSelectionnee.Date_du_conge = DateTime.Now;

            //On cree une variable pour garder le numero du lit a liberer
            int? numLit = admissionSelectionnee.Numero_Lit;

            //On met le numero du lit de l'admission a null
            admissionSelectionnee.Numero_Lit = null;

            //On va chercher le lit qui doit etre liberer
            Lit litALiberer = (from l in myBdd.Lits where l.Numero_lit == numLit select l).First() as Lit;
            litALiberer.Occupe = false;

            //On essaie d'enregistrer a la bdd
            try
            {
                myBdd.SaveChanges();

                //On refresh la liste d'admission
                lstAdmissionMedcin.DataContext = (from ad in myBdd.Admissions where ad.Date_du_conge == null select ad).ToList();
            }
            catch (Exception)
            {
                MessageBox.Show("Erreur! Les modifications n'ont pas ete enregistrer", "Erreur");
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

        //Ramene a la fenetre de Login
        private void btnLogoutMedcin_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Login());
        }
    }
}
