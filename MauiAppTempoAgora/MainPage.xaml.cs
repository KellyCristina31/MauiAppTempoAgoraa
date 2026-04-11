using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System.Diagnostics;
namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked_Previsão(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";

                        dados_previsao = $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Nascer do Sol: {t.sunrise} \n" +
                                         $"Por do Sol: {t.sunset} \n" +
                                         $"Temp Máx: {t.temp_max} ºC \n" +
                                         $"Temp Min: {t.temp_min} ºC \n" +
                                         $"Descrição do tempo: {t.description} \n" +
                                         $"Velocidade do vento: {t.speed} m/s \n" +
                                         $"Visibilidade: {t.visibility} m \n";

                        lbl_res.Text = dados_previsao;
                        string mapa = $"https://embed.windy.com/embed.html?" +
                                     $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
                                     $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                                     $"&lat={t.lat.ToString().Replace(",", ".")}&lon={t.lon.ToString().Replace(",", ".")}";

                        wv_mapa.Source = mapa;

                        Debug.WriteLine(mapa);


                    }
                    else
                    {

                        lbl_res.Text = "Cidade não encontrada ou falha na escrita";
                    }
                }
                else
                {
                    lbl_res.Text = "Preencha a cidade.";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {

            try
            {
                GeolocationRequest request = new GeolocationRequest(
                    GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(10));
                Location? local = await Geolocation.Default.GetLocationAsync(request);
                if (local != null)
                {
                    string dados_localizacao = $"Latitude: {local.Latitude} \n" +
                                              $"Longitude: {local.Longitude} \n" +
                                              $"Altitude: {local.Altitude} m \n" +
                                              $"Precisão: {local.Accuracy} m \n" +
                                              $"Tempo da última atualização: {local.Timestamp} \n";
                    lbl_res.Text = dados_localizacao;
                }
                else
                {
                    lbl_res.Text = "Não foi possível obter a localização.";
                }

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não Suporta", fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Erro: Localização Desabilitada", fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Erro: Permissão da Localização", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }

        }
        private async void GetCidade(double lat, double lon)
        {
            try
            {
                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);

                Placemark? place = places.FirstOrDefault();

                if (place != null)
                {
                    txt_cidade.Text = place.Locality;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro: Obtenção do nome da Cidade", ex.Message, "OK");
            }
        }
    }
}
