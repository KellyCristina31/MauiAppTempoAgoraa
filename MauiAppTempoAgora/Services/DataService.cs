using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "cc64d9106b9c2d5bc4af0886de8d2116";

            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={cidade}&units=metric&appid={chave}&lang=pt_br";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage resp = await client.GetAsync(url);

                if (resp.IsSuccessStatusCode)
                {
                    string json = await resp.Content.ReadAsStringAsync();
                    var rascunho = JObject.Parse(json);

                    // Conversão correta do UNIX timestamp
                    long sunriseUnix = (long)rascunho["sys"]["sunrise"];
                    long sunsetUnix = (long)rascunho["sys"]["sunset"];

                    DateTime sunrise = DateTimeOffset.FromUnixTimeSeconds(sunriseUnix).LocalDateTime;
                    DateTime sunset = DateTimeOffset.FromUnixTimeSeconds(sunsetUnix).LocalDateTime;

                    t = new()
                    {
                        lat = (double)rascunho["coord"]["lat"],
                        lon = (double)rascunho["coord"]["lon"],
                        description = (string)rascunho["weather"][0]["description"],
                        main = (string)rascunho["weather"][0]["main"],
                        temp_min = (double)rascunho["main"]["temp_min"],
                        temp_max = (double)rascunho["main"]["temp_max"],
                        visibility = (int)rascunho["visibility"],
                        speed = (double)rascunho["wind"]["speed"],
                        sunrise = sunrise,
                        sunset = sunset
                    };
                }
            }
            return t;
        }
    }
}
