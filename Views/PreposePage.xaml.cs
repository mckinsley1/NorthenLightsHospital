using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NorthenLightsHospital.Views
{
    /// <summary>
    /// Interaction logic for PreposePage.xaml
    /// </summary>
    public partial class PreposePage : Page
    {
        HospitalEntities myBdd;
        public PreposePage()
        {
            InitializeComponent();
            myBdd = new HospitalEntities();

            //On rempli le combo box des assurances dans l'onglet patient
            cmbAssurancePatient.DataContext = myBdd.Assurances.ToList();

            //On rempli le combo box des medcin sur la page admission
            cmbMedecin.DataContext = myBdd.Medcins.ToList();

            //On rempli la liste de departement
            cmbDepartements.DataContext = (from d in myBdd.Departements select d).ToList();

            //On rempli la liste de type de lit
            cmbTypeLit.DataContext = (from t in myBdd.Type_lit select t).ToList();
        }

        //Code pour Effectuer l'ajout d'un patient a partir de l'onglet Patient
        private void btnAjouterPatient_Click(object sender, RoutedEventArgs e)
        {
            //On verifie si le patient existe deja
            if ((from p in myBdd.Patients where p.NSS == txtPatientNSS.Text select p).Any())
            {
                MessageBox.Show(String.Format("Le patient avec le NSS {0} existe deja dans notre systeme", txtPatientNSS.Text));
            }
            else //Si le patient existe pas deja dans le systeme on le rajoute
            {
                //On recolte tous les informations du formulaire
                try
                {
                    DateTime dateNaissance = (DateTime)datePickDateNaissance.SelectedDate;
                }
                catch (Exception)
                {
                    MessageBox.Show("Veuillez choisir une date de naissance svp!");
                }
                string nssPatient = txtPatientNSS.Text;
                string nomPatient = txtNomPatient.Text;
                string prenomPatient = txtPrenomPatient.Text;
                string adressePatient = txtAdressePatient.Text;
                string villePatient = txtVillePatient.Text;
                string provincePatient = txtProvincePatient.Text;
                string codePostal = txtCodePostalPatient.Text;
                string telephonePatient = txtTelephone.Text;
                Assurance assurance = cmbAssurancePatient.SelectedItem as Assurance;

                //Validation que aucun champs n'est vide
                if (nssPatient == "" || datePickDateNaissance.SelectedDate == null || nomPatient == "" || prenomPatient == "" || adressePatient == "" ||
                    villePatient == "" || provincePatient == "" || codePostal == "" || cmbAssurancePatient.SelectedItem == null || telephonePatient == "")
                {
                    MessageBox.Show("Tous les champs doivent etre rempli!", "Champs Vide!");
                }
                else
                {
                    //Instanciation du nouveau Patient
                    Patient nouveauPatient = new Patient
                    {
                        NSS = nssPatient,
                        Date_naissance = (DateTime)datePickDateNaissance.SelectedDate,
                        nom = nomPatient,
                        prenom = prenomPatient,
                        adresse = adressePatient,
                        ville = villePatient,
                        province = provincePatient,
                        code_postal = codePostal,
                        telephone = telephonePatient,
                        ID_Assurance = assurance.ID_Assurance
                    };

                    //On essaie d'ajouter le nouveau patient au niveau de la bdd
                    try
                    {
                        myBdd.Patients.Add(nouveauPatient);
                        myBdd.SaveChanges();
                        MessageBox.Show("Le nouveau patient a bien ete ajoute");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Le nouveau patient n'a pas ete enregistrer!", "Erreur");
                    }
                }
            }
        }

        //Code pour effectuer la recherche d'un patient
        private void Button_Click_Recherche(object sender, RoutedEventArgs e)
        {
            //On va chercher le NSS a chercher
            string nssRecherhce = txtNSSRecherche.Text;

            //On valide que le champ n'est pas vide
            if (nssRecherhce == "")
            {
                MessageBox.Show("Le champs NSS doit etre rempli", "Champ Vide!");
            }
            else
            {
                //Validation que le patient existe
                Boolean patientExiste = (from p in myBdd.Patients where p.NSS == nssRecherhce select p).Any();
                if (patientExiste)
                {
                    Patient patient = (from p in myBdd.Patients where p.NSS == nssRecherhce select p).First() as Patient;
                    txtNomPatientRecherche.Text = patient.nom;
                    txtPrenomPatientRecherche.Text = patient.prenom;
                    txtDateNaissanceRecherche.Text = patient.Date_naissance.ToString("MM / dd / yyyy");
                    txtNSSRechercheAffichage.Text = patient.NSS;

                    Assurance assurance = (from p in myBdd.Assurances where p.ID_Assurance == patient.ID_Assurance select p).First() as Assurance;
                    txtAssuranceRecherche.Text = assurance.nom_compagnie;
                }
                else
                {
                    //Si le patient n'existe pas on affiche un message et on met les textBox a vide
                    MessageBox.Show("Le patient n'existe pas dans le systeme.", "Patient non trouve!");
                    txtNomPatientRecherche.Text = "";
                    txtPrenomPatientRecherche.Text = "";
                    txtDateNaissanceRecherche.Text = "";
                    txtNSSRechercheAffichage.Text = "";
                    txtAssuranceRecherche.Text = "";
                }
            }
        }

        private void btnAjouterAdmission_Click(object sender, RoutedEventArgs e)
        {
            //On valide si le patient existe
            if ((from p in myBdd.Patients where p.NSS == txtAdmissionNSS.Text select p).Any())
            {
                labelNSSTrouve.Content = "";
                //On valide qu'il y a des lits disponible
                if (litDispo()) //Du type qu'il a choisi
                {
                    //On verifie s'il y a une chirurgie programmee
                    if ((bool)chkChirurgieProg.IsChecked)
                    {
                        if (cmbDepartements.Text != "Chirurgie")
                        {
                            chkChirurgieProg_Checked(sender, e);
                        }
                        else
                        {
                            //S'il y a une chirurgie programme on valide qu'il y a une date de selectionnee
                            if (datePickChirurgie == null)
                            {
                                labelDateChirurgie.Content = "Veuillez selectionner une date svp!";
                            }
                            else
                            {
                                creeAdmission();
                                Departement dept = cmbDepartements.SelectedItem as Departement;
                                Type_lit type_Lit = cmbTypeLit.SelectedItem as Type_lit;
                                txtFraisSupp.Content = String.Format("Voici vos frais supplementaire quotidien: {0}$", fraisSupp(VerifierAssurancePublic(txtAdmissionNSS.Text), dept, type_Lit));
                            }
                        }
                    }
                    else //S'il y a pas de chirurgie prog
                    {
                        creeAdmission();
                        Departement dept = cmbDepartements.SelectedItem as Departement;
                        Type_lit type_Lit = cmbTypeLit.SelectedItem as Type_lit;
                        txtFraisSupp.Content = String.Format("Voici vos frais supplementaire quotidien: {0}$", fraisSupp(VerifierAssurancePublic(txtAdmissionNSS.Text), dept, type_Lit));

                    }
                }
                else //S'il n'y a plus de lits disponible
                {
                    MessageBox.Show("On a plus de lit disponible a l'heure actuel", "Plus de place!");
                }
            }
            else //Si la patient n'existe pas
            {
                labelNSSTrouve.Content = "Aucun Patient trouve!";
            }
        }

        private bool litDispo()
        {
            //On trouve le nombre de lit dispo
            int nombreDeLitTotalDispo = (from l in myBdd.Lits where l.Occupe == false select l).Count();

            //On trouve le nombre d'admission actuel
            int nombreAdmissionActuel = (from a in myBdd.Admissions select a).Count();

            return nombreAdmissionActuel < nombreDeLitTotalDispo ? true : false;
        }

        //Selon le departement choisi on affiche seulement les lit disponible dans ce departement
        private void cmbDepartements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Departement dept = cmbDepartements.SelectedItem as Departement;

            if (dept != null)
            {
                if ((from l in myBdd.Lits where l.Occupe == false & l.ID_Departement == dept.ID_Departement & l.ID_Type == cmbTypeLit.Text select l).Count() > 0)
                {
                    cmbNumLit.DataContext = (from l in myBdd.Lits where l.Occupe == false & l.ID_Departement == dept.ID_Departement & l.ID_Type == cmbTypeLit.Text select l).ToList();
                }
                else
                {
                    MessageBox.Show(String.Format("Il n'y a plus de lit {0} disponible dans le departement selectionne. Veuillez selectionner un autre type de lit ou un autre departement.", cmbTypeLit.Text));
                    cmbNumLit.DataContext = "";
                }
            }

        }

        private void cmbTypeLit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Departement dept = cmbDepartements.SelectedItem as Departement;
            Type_lit type_Lit = cmbTypeLit.SelectedItem as Type_lit;
            if (dept != null)
            {
                if ((from l in myBdd.Lits where l.Occupe == false & l.ID_Departement == dept.ID_Departement & l.ID_Type == type_Lit.ID_Type select l).Count() > 0)
                {
                    cmbNumLit.DataContext = (from l in myBdd.Lits where l.Occupe == false & l.ID_Departement == dept.ID_Departement & l.ID_Type == type_Lit.ID_Type select l).ToList();
                }
                else
                {
                    MessageBox.Show(String.Format("Il n'y a plus de lit {0} disponible dans le departement selectionne. Veuillez selectionner un autre type de lit ou un autre departement.", cmbTypeLit.Text));
                    cmbNumLit.DataContext = "";
                }
            }

        }

        private void chkChirurgieProg_Checked(object sender, RoutedEventArgs e)
        {
            cmbDepartements.SelectedIndex = 5;
        }

        //On valide si le patient a une assurance prive
        private bool VerifierAssurancePublic(string nssRecherche)
        {
            Boolean isPublic = false;
            //On valide que le champ n'est pas vide
            if (nssRecherche == "")
            {
                MessageBox.Show("Le champs NSS doit etre rempli", "Champ Vide!");
            }
            else
            {
                //Validation que le patient existe
                if ((from p in myBdd.Patients where p.NSS == nssRecherche select p).Any())
                {
                    Patient patient = (from p in myBdd.Patients where p.NSS == nssRecherche select p).First() as Patient;

                    Assurance assurance = (from p in myBdd.Assurances where p.ID_Assurance == patient.ID_Assurance select p).First() as Assurance;

                    if (assurance.ID_Assurance == "5")
                    {
                        isPublic = true;
                    }
                    else
                    {
                        isPublic = false;
                    }
                }
                else
                {
                    MessageBox.Show("Le patient n'existe pas dans le system!");
                }
            }
            return isPublic;
        }

        private bool patientMineur(Patient patient)
        {
            Boolean isMineur = false;
            if ((DateTime.Today.Year - patient.Date_naissance.Year) < 16)
            {
                isMineur = true;
            }
            else isMineur = false;

            return isMineur;
        }

        private double fraisSupp(Boolean isPublic, Departement dept, Type_lit type_Lit)
        {
            double fraisSupp = 0;
            List<Lit> litStandard = (from l in myBdd.Lits where l.Occupe == false & l.ID_Departement == dept.ID_Departement & l.ID_Type == "Standard" select l).ToList();
            List<Lit> litSemi_Prive = (from l in myBdd.Lits where l.Occupe == false & l.ID_Departement == dept.ID_Departement & l.ID_Type == "Semi-Prive" select l).ToList();

            if (isPublic)
            {
                switch (type_Lit.ID_Type)
                {
                    case "Semi-Prive":
                        fraisSupp = litStandard.Count() <= 0 ? 267 : 0;
                        break;
                    case "Prive":
                        fraisSupp = litSemi_Prive.Count() <= 0 ? 571 : 0;
                        break;
                }
            }

            fraisSupp += (bool)chkTeleviseur.IsChecked ? 42.50 : 0;
            fraisSupp += (bool)chkTelephone.IsChecked ? 7.50 : 0;
            return fraisSupp;
        }

        private void creeAdmission()
        {
            Lit lit = new Lit();
            if (cmbNumLit.SelectedItem != null)
            {
                lit = cmbNumLit.SelectedItem as Lit;
                lit.Occupe = true;
            }

            //On retourne le medecin selectionne
            Medcin medcin = cmbMedecin.SelectedItem as Medcin;

            //On verifie si une admission actuel existe deja dans le system
            if ((from a in myBdd.Admissions where a.NSS == txtAdmissionNSS.Text & a.Numero_Lit != null select a).Any())
            {
                MessageBox.Show("Une admission pour ce patient existe deja dans le systeme");
            } else
            {
                Admission nouvelleAdmission = new Admission
                {
                    Chirurgie_programme = (bool)chkChirurgieProg.IsChecked,
                    Date_admission = DateTime.Now,
                    Date_chirurgie = datePickChirurgie.SelectedDate,
                    Televiseur = (bool)chkTeleviseur.IsChecked,
                    NSS = txtAdmissionNSS.Text,
                    Numero_Lit = lit.Numero_lit,
                    ID_Medcin = medcin.ID_Medcin
                };

                try
                {
                    myBdd.Admissions.Add(nouvelleAdmission);
                    myBdd.SaveChanges();
                    MessageBox.Show("Nouvelle admission ajouter avec succes");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }      
        }


        //On verifie si le patient a 6 ans et on met le dept a chirurgie
        private void txtAdmissionNSS_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((from p in myBdd.Patients where p.NSS == txtAdmissionNSS.Text select p).Any())
            {
                Patient patient = (from p in myBdd.Patients where p.NSS == txtAdmissionNSS.Text select p).First() as Patient;
                if (patientMineur(patient))
                {
                    cmbDepartements.SelectedIndex = 4;
                }
            }
        }

        //Bouton Logout
        private void Logout(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Login());
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
