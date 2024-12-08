using Newtonsoft.Json;
using System.Collections.ObjectModel;
using TankkausApp.Models;
using System.Net.Http;
using System.Text;
using System;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Globalization;
using System.Text.RegularExpressions;
using Microcharts.Maui;
using SkiaSharp;
using Microcharts;
//using static CoreFoundation.DispatchSource;
//using JsonSerializer = Newtonsoft.Json.JsonSerializer;



namespace TankkausApp
{
    public partial class MainPage : TabbedPage
    {
        private string valittuRekisterinumero;
        public MainPage()
        {
            InitializeComponent();
            tallenna.IsEnabled = false;
            haku.IsEnabled = false;
        }
        
        

        private void inputKenttaRek_TextChanged(object sender, TextChangedEventArgs e)
        {
            var syöte = (Entry)sender;
            string isotKirjaimet = e.NewTextValue.ToUpper();

            // Tarkistaa ja lisää väliviiva automaattisesti kolmannen kirjaimen jälkeen
            if (isotKirjaimet.Length == 3 && !isotKirjaimet.Contains("-"))
            {
                isotKirjaimet += "-";
            }

            // Aseta muunnettu teksti Rekisterinumero-kenttään vain, jos se on muuttunut
            if (syöte.Text != isotKirjaimet)
            {
                syöte.Text = isotKirjaimet;

                //Asettaa kursorin tekstin loppuun
                //Tämä ei toiminut mobiililaitteilla
                //syöte.CursorPosition = syöte.Text.Length;

                //Asettaa kursorin automaattisesti rivin loppuun kun - on lisätty
                //Toimii myös mobiililaitteilla
                Dispatcher.Dispatch(() =>
                {
                    syöte.CursorPosition = syöte.Text.Length;
                });
            }

            // Tarkistaa rekisterinumeron syötteen "ABC-123" ja muuta tekstin väri sen mukaisesti
            //Kommenteissa perinteistenkilpien tarkistus, mutta suomessa sallitut erikoiskilvet voivat sisältää myös vai 2 kirjainta ja 1-3 numeroa
            //string sallittuSyote = @"^[A-Z]{3}-\d{3}$";
            string sallittuSyote = @"^[A-Z]{2,3}-\d{1,3}$";
            if (Regex.IsMatch(isotKirjaimet, sallittuSyote))
            {
                syöte.TextColor = Colors.Black; // Oikea syöte
                tallenna.IsEnabled = true;
            }
            else
            {
                syöte.TextColor = Colors.Red; // Virheellinen syöte
                tallenna.IsEnabled = false;
            }
        }


        //Tankkausten hakukentän muotoilu
        private void inputKenttaRekhaku_TextChanged(object sender, TextChangedEventArgs e)
        {
            var syöte = (Entry)sender;
            string isotKirjaimet = e.NewTextValue.ToUpper();

            // Tarkistaa ja lisää väliviiva automaattisesti kolmannen kirjaimen jälkeen
            if (isotKirjaimet.Length == 3 && !isotKirjaimet.Contains("-"))
            {
                isotKirjaimet += "-";
            }

            // Aseta muunnettu teksti Rekisterinumero-kenttään vain, jos se on muuttunut
            if (syöte.Text != isotKirjaimet)
            {
                syöte.Text = isotKirjaimet;

                //Asettaa kursorin tekstin loppuun
                //Tämä ei toiminut mobiililaitteilla
                //syöte.CursorPosition = syöte.Text.Length;

                //Asettaa kursorin automaattisesti rivin loppuun kun - on lisätty
                //Toimii myös mobiililaitteilla
                Dispatcher.Dispatch(() =>
                {
                    syöte.CursorPosition = syöte.Text.Length;
                });
            }

            // Tarkistaa rekisterinumeron syötteen "ABC-123" ja muuta tekstin väri sen mukaisesti
            //Kommenteissa perinteistenkilpien tarkistus, mutta suomessa sallitut erikoiskilvet voivat sisältää myös vai 2 kirjainta ja 1-3 numeroa
            //string sallittuSyote = @"^[A-Z]{3}-\d{3}$";
            string sallittuSyote = @"^[A-Z]{2,3}-\d{1,3}$";
            if (Regex.IsMatch(isotKirjaimet, sallittuSyote))
            {
                syöte.TextColor = Colors.Black; // Oikea syöte
                haku.IsEnabled = true;
            }
            else
            {
                syöte.TextColor = Colors.Red; // Virheellinen syöte
                haku.IsEnabled = false;
            }
        }




        private async void tallenna_Clicked(object sender, EventArgs e)
        {
            //Tarkistetaan onko kentissä null arvoja ja korvataan pilkut pisteillä
            var litratText = !string.IsNullOrWhiteSpace(inputKenttaLitrat.Text)
                             ? inputKenttaLitrat.Text.Replace(",", ".")
                             : null;
            var eurotText = !string.IsNullOrWhiteSpace(inputKenttaEurot.Text)
                            ? inputKenttaEurot.Text.Replace(",", ".")
                            : null;
            var kilometritText = !string.IsNullOrWhiteSpace(inputKenttaKilometrit.Text)
                                 ? inputKenttaKilometrit.Text.Replace(",", ".")
                                 : null;

            // Alustetaan muuttujat null-arvoilla
            decimal? tankatutLitrat = null;
            decimal? tankatutEurot = null;
            decimal? kilometrit = null;

            //Tarkistetaan kenttien arvot jos ne eivät ole tyhjiä
            if (!string.IsNullOrWhiteSpace(litratText))
            { 

                if (decimal.TryParse(litratText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal muutetutLitrat))
                {
                    tankatutLitrat = muutetutLitrat;
                }
                else { 

                await DisplayAlert("Virheellinen syöte kohdassa: Tankatut litrat!", "Syötä kelvollinen luku esim. 35.50", "Ok");
                return;
                }
            }

            if (!string.IsNullOrWhiteSpace(eurotText))
            {

                if (decimal.TryParse(eurotText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal muutetutEurot))
                {
                    tankatutEurot = muutetutEurot;
                }
                else
                {

                    await DisplayAlert("Virheellinen syöte kohdassa: Tankatut eurot!", "Syötä kelvollinen luku esim. 60.00", "Ok");
                    return;
                }
            }
            // Tarkistetaan kilometrimäärä, jos syöte ei ole tyhjä
            if (!string.IsNullOrWhiteSpace(kilometritText))
            {
                if (decimal.TryParse(kilometritText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal muutetutKilometrit))
                {
                    kilometrit = muutetutKilometrit;
                }
                else
                {
                    await DisplayAlert("Virheellinen syöte kohdassa: Mittari lukema", "Syötä kelvollinen kilometrimäärä , esim. 150000", "OK");
                    return;
                }
            }


            // Luo uuden Tankkaus-objektin ja täyttää tiedot
            var uusiTankkaus = new Tankkaus



            {
                RekisteriNum = inputKenttaRek.Text.Trim(),
                TankatutLitrat = tankatutLitrat,
                TankatutEurot = tankatutEurot,
                AjoKilometrit = kilometrit,
                PaivaMaara = DateTime.Today
            };
            //Luo HTTPCleint olion
            HttpClient client = new HttpClient();

            //Decimaalin piste erottimen hyväksyntä
            var jsonSettings = new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture,
                FloatFormatHandling = FloatFormatHandling.DefaultValue,
                FloatParseHandling = FloatParseHandling.Decimal
            };


            // Muuntaa objektin JSON-muotoon
            var json = JsonConvert.SerializeObject(uusiTankkaus, jsonSettings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");



                      
                // Asettaa API-osoittene
                client.BaseAddress = new Uri("https://tankkausrestapi.azurewebsites.net");

            try
            {
                // Lähetä POST-pyyntö
                HttpResponseMessage response = await client.PostAsync("/api/tankkaus", content);

                // Tarkistaa, onnistuiko pyyntö
                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Onnistui", "Tankkaus tallennettu onnistuneesti!", "OK");
                    //Kenttien tyhjennys
                    inputKenttaRek.Text = "";
                    inputKenttaLitrat.Text = "";
                    inputKenttaEurot.Text = "";
                    inputKenttaKilometrit.Text = "";
                }
                else
                {
                    await DisplayAlert("Virhe", "Tallennus epäonnistui. Yritä uudelleen.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Käsittele mahdolliset virheet
                await DisplayAlert("Virhe", $"Tallennuksessa tapahtui virhe: {ex.Message}", "OK");
            }      


        }

        private async void haku_Clicked(object sender, EventArgs e)
        {

            //Napin pyöritys
            var button = (Button)sender;
            await button.RotateTo(360, 1000, Easing.CubicOut);//Nappi pyörii 360 astetta 1 sekunnissa
            await button.RotateTo(0, 0); //Nappi palautuu alkuperäiseen asentoon

            // Tarkistetaan, että rekisterinumero on syötetty
            string rekisterinumero = inputKenttaRekhaku.Text?.Trim();
            if (string.IsNullOrEmpty(rekisterinumero))
            {
                await DisplayAlert("Virhe", "Syötä rekisterinumero hakua varten.", "OK");
                return;
            }
            //Tallennetaan rekisterinumero muuttujaan, jota käytetään kaavio tietojen hakuun
            valittuRekisterinumero = rekisterinumero;

            // Luodaan HttpClient
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://tankkausrestapi.azurewebsites.net");

            try
            {
                // Lähetetään GET-pyyntö backendille rekisterinumero parametrina
                HttpResponseMessage response = await client.GetAsync($"/api/tankkaus/{rekisterinumero}");

                // Tarkistetaan, onnistuiko pyyntö
                if (response.IsSuccessStatusCode)
                {
                    // Luetaan vastauksen sisältö
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserialisoidaan vastauksen JSON Tankkaus-objektien listaksi
                    var tankkaukset = JsonConvert.DeserializeObject<List<Tankkaus>>(jsonResponse);

                    // Lasketaan litrahinta ja lisätään jokaiseen tankkaukseen
                    foreach (var tankkaus in tankkaukset)
                    {
                        if (tankkaus.TankatutLitrat > 0 && tankkaus.TankatutEurot > 0)
                        {
                            tankkaus.Litrahinta = tankkaus.TankatutEurot / tankkaus.TankatutLitrat;
                        }
                        else 
                        {
                            tankkaus.Litrahinta = 0; //Jos tankkaus euroja tai litroja ei ole ilmoitettu litrahinta on 0
                        }
                    }
                    string reknum = inputKenttaRekhaku.Text;
                    rekisteriLabel.Text = $"Tankkaukset ajoneuvoon, jonka rekisterinumero on: {reknum}";

                    //----------------------------Kaavion testausta-------------------------


                    var kuukausiSaldo = tankkaukset
                        .Where(t => t.PaivaMaara != null)
                     .GroupBy(t => t.PaivaMaara.Value.ToString("yyyy-MM"))
                      .Select(g => new
                      {
                      Kuukausi = g.Key,
                     KokonaisSaldo = g.Sum(t => t.TankatutEurot)
                     })
                     .ToList();

                    // Käytetään kuukausiSaldoa kaavion näyttämiseen
                    var kaavioData = kuukausiSaldo.Select(s => new ChartEntry((float)s.KokonaisSaldo)
                    {
                        Label = s.Kuukausi,
                        ValueLabel = $"{s.KokonaisSaldo}€",
                        Color = SKColor.Parse("#FF5722") // Valitse väri
                    }).ToList();

                    TankkausKaavio.Chart = new LineChart
                    {
                        Entries = kaavioData,
                        LineMode = LineMode.Spline,

                        BackgroundColor = SKColor.Parse("#f0f0f0"), // Taustaväri
                        PointSize = 10, // Pisteiden koko
                        IsAnimated = true,
                    };



                    //----------------------------kaavion testi loppuu------------

                    // Päivitetään listan sisältö
                    if (tankkaukset != null && tankkaukset.Count > 0)
                    {
                        TankkausLista.ItemsSource = tankkaukset;
                        TankkausLista.IsVisible = true;
                        inputKenttaRekhaku.Text = "";
                    }
                    else
                    {
                        await DisplayAlert("Hakutulokset", "Ei tankkauksia löytynyt annetulle rekisterinumerolle.", "OK");
                        TankkausLista.IsVisible = false;
                        rekisteriLabel.Text = "";
                    }
                }
                else
                {
                    await DisplayAlert("Virhe", "Haku epäonnistui. Tarkista rekisterinumero ja yritä uudelleen.", "OK");
                    TankkausLista.IsVisible = false;
                    rekisteriLabel.Text = "";
                }
            }
            catch (Exception ex)
            {
                // Käsittele mahdolliset virheet
                await DisplayAlert("Virhe", $"Haussa tapahtui virhe: {ex.Message}", "OK");
                TankkausLista.IsVisible = false;
                rekisteriLabel.Text = "";
            }
        }

  

    }

}
