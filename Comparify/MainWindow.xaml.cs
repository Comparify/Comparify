using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//benötigte using-Direktive für StreamReader
using System.IO;
//benötigte using-Direktive für Http-Klassen
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Diagnostics;
//using Direktive für ListViewItems
using Comparify.ListViewItems;
using Comparify.SubstringExtensions;

namespace Comparify
{    

    public class Produkt {
        public string Bild { get; set; }
        public string Titel { get; set; }
        public string Link { get; set; }
        public string Preis { get; set; }
    }

    public partial class MainWindow : Window
    {
        #region HTTP Request
        public string hole_web_content(string url)
    {
        Uri uri = new Uri(url);
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
        request.Method = WebRequestMethods.Http.Get;
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string output = reader.ReadToEnd();
        response.Close();
 
        return output;
    }
        #endregion

        #region ListViewItemsCollections

        public ObservableCollection<ListViewItemsData> ListViewItemsCollections 
        { 
            get 
            { 
                return _ListViewItemsCollections; 
            } 
        }
        readonly ObservableCollection<ListViewItemsData> _ListViewItemsCollections = new ObservableCollection<ListViewItemsData>();

        #endregion

        #region Initialisierung

        public MainWindow()
        {
            InitializeComponent();

            ErgebnisListView.ItemsSource = ListViewItemsCollections;
        }

        #endregion

        #region Funktionen
        private void SucheProdukte(string SucheNachText)
        {
            List<string> WebseitenListe = new List<string>();

            List<Produkt> GridProdukte = new List<Produkt>();

            #region Checkboxaktivierung
            //Einfügen der gewünschten Webseiten in die Liste
            if (AmazonCheckBox.IsChecked == true)
            {
                WebseitenListe.Add("https://amazon.de/s?k=");
            }
            //Aufgabe 1: Ebay-Checkbox einfügen
            #endregion

            for (int j = 0; j < WebseitenListe.Count; j++)
            {
                string BildURL = "";
                string URI = WebseitenListe[j] + SucheNachText;
                string AktuelleWebseite = SubstringExtension.Before(WebseitenListe[j].Substring(8), "/");
                string data = hole_web_content(URI);

                //Seitenabhängige Webseitenanalyse
                if (WebseitenListe[j].Contains("amazon.de"))
                {
                    BildURL = SubstringExtension.Between(data, "<span data-component-type=\"s-product-image\" class=\"rush-component\">", "");
                }
                else if (WebseitenListe[j].Contains("www.ebay.de"))
                {
                    BildURL = SubstringExtension.Between(data, "srp-river-results", "srp-main-below-river");
                }

                int SuchVariable = 3; // Amazon SuchVariable, falls gesponsertes Produkt.

                //Amazon.de Produkt-Titel, -Bild, -Link
                if (WebseitenListe[j].Contains("amazon.de"))
                {
                    for (int dii = 1; dii <= SuchVariable; dii++)
			        {
                        string Produktinfos = SubstringExtension.Before(BildURL, "data-image-index=\""+ dii +"\"");
                        Produktinfos = Produktinfos.After("<a class=\"a-link-normal s-no-outline\"");
                        Produkt Produkt = new Produkt();

                        for (int k = 0; k < Produktinfos.Length - 10; k++)
                        {
                            if (Produktinfos[k] == 'h' && Produktinfos[k + 1] == 'r' && Produktinfos[k + 2] == 'e' && Produktinfos[k + 3] == 'f')
                            {
                                k += 6;
                                string _produktlink = Produktinfos.Substring(k);
                                Produkt.Link = AktuelleWebseite + SubstringExtension.Before(_produktlink, "/ref");
                            }
                            if (Produktinfos[k] == 'i' && Produktinfos[k + 1] == 'm' && Produktinfos[k + 2] == 'g')
                            {
                                k += 9;
                                string _produktbild = Produktinfos.Substring(k);
                                if (_produktbild[0] == '=') 
                                {
                                    _produktbild = _produktbild.Substring(16);
                                    Produkt.Bild = SubstringExtension.Before(_produktbild, "\"");
                                }
                                else 
                                {
                                    Produkt.Bild = SubstringExtension.Before(_produktbild, "\"");
                                }
                            }
                            if (Produkt.Preis == null && Produkt.Link != null && Produkt.Link.Contains("/dp/")) 
                            {
                                string Produktpreisinfos = SubstringExtension.After(BildURL, Produktinfos);
                                string test = SubstringExtension.Before(Produktpreisinfos, "</span><span class=\"a-price-symbol\">€</span></span></span>");
                                Produkt.Preis = SubstringExtension.After(test, "<span class=\"a-price-whole\">");
                            }
                            if (Produktinfos[k] == 'a' && Produktinfos[k + 1] == 'l' && Produktinfos[k + 2] == 't')
                            {
                                k += 5;
                                string _produkttitel = Produktinfos.Substring(k);
                                Produkt.Titel = SubstringExtension.Before(_produkttitel, "\"");
                                break;
                            }
                        }

                        if (Produkt != null && Produkt.Titel != null && Produkt.Link != null && Produkt.Bild != null && Produkt.Preis != null && Produkt.Link != "" && Produkt.Bild != "" && Produkt.Titel != "" && !Produkt.Titel.Contains("Gesponsert"))
                        {
                            GridProdukte.Add(Produkt);
                        } 
                        else
                        {
                            SuchVariable += 1;
                        }
                    }
                }
                else if (WebseitenListe[j].Contains("www.ebay.de"))
                {
                    //Ebay
                    for (int iid = 2; iid <= SuchVariable + 1; iid++)
			        {
                        Produkt Produkt = new Produkt();

                        //Aufgabe 3: Suche in BildURL "data-view=mi:1686|iid" und weise nachfolgenden Code der neuen Variable: "Produktinfos" zu.

                        // ÜBER MIR DIE VARIABLE ANLEGEN
/*                      
                        string subProduktinfos = SubstringExtension.Before(Produktinfos, "<div class=\"s-item__info clearfix\">");
                        string _produkttitel = SubstringExtension.After(subProduktinfos, "alt=\"");
                        Produkt.Titel = SubstringExtension.Before(_produkttitel, "\"");
                        string _produktbild = SubstringExtension.After(subProduktinfos, "src=");
                        Produkt.Bild = _produktbild.Before(".jpg") + ".jpg";
                        string _produktlink = SubstringExtension.After(subProduktinfos, "href=");
                        string _produktlink2 = SubstringExtension.Before(_produktlink, " ");
                        Produkt.Link = SubstringExtension.After(_produktlink2, "https://");
                        if (Produkt.Preis == null)
                        {
                            int produkt_index = SubstringExtension.BeforeIndexOf(Produktinfos, "</div><div class=\"s-item__details clearfix\"><div class=\"s-item__detail s-item__detail--primary\"><span class=s-item__price>");
                            string produkt_sektion = Produktinfos.Substring(produkt_index);
                            produkt_sektion = produkt_sektion.Before("</span></div><div class=\"s-item__detail s-item__detail--primary\">");
                            string preis = produkt_sektion.After("price>");
                            Produkt.Preis = preis;
                        }
*/

                        //Aufgabe 2: Abfrage, ob ein Produkt existiert, ein Bild, Titel und Preis vorhanden und nicht leer ist.
                        GridProdukte.Add(Produkt);
                    }
                }
                AddToListe(GridProdukte);
                GridProdukte = new List<Produkt>();
            }
        }

        private bool GridBeinhaltetKeinItem(List<Produkt> pListe, string pLink) 
        {
            foreach (var item in pListe)
	        {
                if (item.Link.CompareTo(pLink) <= 2)
                {
                    return false;
                }
	        }

            return true;
        }

        private void AddToListe(List<Produkt> GridProdukte)
        {
            for (int i = 0; i < GridProdukte.Count; i++)
			{
                string Plattformbild = "";
                if (GridProdukte[i].Link.Contains("amazon")) 
                {
                    Plattformbild = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..\\..\\Logos\\amazon_logo.jpg"));
                }
                else 
                {
                    Plattformbild = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..\\..\\Logos\\ebay_logo.jpg"));
                }

                if (GridProdukte[i].Preis.Contains("EUR")) 
                {
                    GridProdukte[i].Preis = GridProdukte[i].Preis.After("EUR");

                    if (GridProdukte[i].Preis.Contains("</"))
                    {
                        GridProdukte[i].Preis = GridProdukte[i].Preis.Before("</");
                    }
                }


                ListViewItemsCollections.Add(new ListViewItemsData()
                {
                    GridViewColumn_Bild = GridProdukte[i].Bild,
                    GridViewColumn_Titel = GridProdukte[i].Titel,
                    GridViewColumn_Webseite = GridProdukte[i].Link,
                    GridViewColumn_Preis = GridProdukte[i].Preis,
                    GridViewColumn_Plattform = Plattformbild
                });
			}
        }
        
        #endregion

        #region Buttons
        private void SuchButton_Click(object sender, RoutedEventArgs e)
        {
            LeereTabelle();
            SucheProdukte(SuchBox.Text);
        }
        #endregion

        #region CheckBoxen

        private void AmazonCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void EbayCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region TextBoxen
        private void SuchBox_TextGeändert(object sender, TextChangedEventArgs e)
        {

        }

        #endregion

        #region Aufruf eines ListView-Items.
        private void ErgebnisListView_AuswahlGeändert(object sender, SelectionChangedEventArgs e)
        {
            ListViewItemsData selitem = (ListViewItemsData)ErgebnisListView.SelectedItem;
            if (selitem != null) 
            {
                Process.Start("https://" +selitem.GridViewColumn_Webseite+"");
            }
        }
        #endregion

        #region Tabelle leeren
        private void LeereTabelle() 
        {
            ListViewItemsCollections.Clear();
        }
        #endregion
    }
}
