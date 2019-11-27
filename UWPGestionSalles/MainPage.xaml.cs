using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GestionnaireBDD;
using ClassesMetier;
using Windows.UI.Popups;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPGestionSalles
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }
        GstBdd bdd;
        List<Place> lesPlaces;
        double montant;
        private async void btnReserver_Click(object sender, RoutedEventArgs e)
        {
            if(lstManifs.SelectedItem != null){
                if(lesPlaces.Count > 0)
                {
                    foreach (Place p in lesPlaces)
                    {
                        bdd.ReserverPlace(p.IdPlace, (lstManifs.SelectedItem as Manifestation).LaSalle.IdSalle, (lstManifs.SelectedItem as Manifestation).IdManif);
                    }
                    var dialog3 = new MessageDialog("Vos places ont bien été réservées");
                    await dialog3.ShowAsync();
                    gvPlaces.ItemsSource = null;
                    gvPlaces.ItemsSource = bdd.GetAllPlacesByIdManifestation((lstManifs.SelectedItem as Manifestation).IdManif, (lstManifs.SelectedItem as Manifestation).LaSalle.IdSalle);
                }
                else
                {
                    var dialog1 = new MessageDialog("Sélectionnez une place");
                    await dialog1.ShowAsync();
                }
            }
            else
            {
                var dialog = new MessageDialog("Sélectionnez une manifestation");
                await dialog.ShowAsync();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            bdd = new GstBdd();
            lstManifs.ItemsSource = bdd.GetAllManifestations();
            lstTarifs.ItemsSource = bdd.GetAllTarifs();
        }

        private void lstManifs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            montant = 0;
            txtTotal.Text = montant.ToString();
            lesPlaces = new List<Place>();
            gvPlaces.ItemsSource = null;
            gvPlaces.ItemsSource = bdd.GetAllPlacesByIdManifestation((lstManifs.SelectedItem as Manifestation).IdManif, (lstManifs.SelectedItem as Manifestation).LaSalle.IdSalle);
            txtNumSalle.Text = (lstManifs.SelectedItem as Manifestation).LaSalle.IdSalle.ToString();
            txtNbPlaces.Text = (lstManifs.SelectedItem as Manifestation).LaSalle.NbPlaces.ToString();
            txtNomSalle.Text = (lstManifs.SelectedItem as Manifestation).LaSalle.NomSalle;
        }

        private async void gvPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(gvPlaces.SelectedItem != null)
            {
                if ((gvPlaces.SelectedItem as Place).Occupee == true)
                {
                    var dialog4 = new MessageDialog("La place est déjà occupée!");
                    await dialog4.ShowAsync();
                }
                else
                {
                    if((gvPlaces.SelectedItem as Place).Etat == 'l')
                    {
                        (gvPlaces.SelectedItem as Place).Etat = 'r';
                        lesPlaces.Add(gvPlaces.SelectedItem as Place);
                        montant += Convert.ToDouble((gvPlaces.SelectedItem as Place).Prix);
                        txtTotal.Text = montant.ToString();
                    }
                    else
                    {
                        (gvPlaces.SelectedItem as Place).Etat = 'l';
                        lesPlaces.Remove(gvPlaces.SelectedItem as Place);
                        montant -= Convert.ToDouble((gvPlaces.SelectedItem as Place).Prix);
                        txtTotal.Text = montant.ToString();
                    }
                }
            }


        }
    }
}
