using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace EmoSong
{
    public partial class MainPage : ContentPage
    {
        public List<PlayList> Items = new List<PlayList>();

        public MainPage()
        {
            InitializeComponent();
        }

        async void btn_Clicked(object sender, EventArgs e)
        {
            try
            {
                String mood = string.Empty;
                String range = string.Empty;

                String Image = await Utilities.getCamera();
                MessageLabel.IsVisible = true;
                PictureButton.IsVisible = false;
                String res = await App.RManager.ReadEmotion(Convert.FromBase64String(Image));

                if (res.Contains("happiness"))
                {
                    mood = "happy";
                    range = "max_danceability=0.9&max_valence=0.96&max_energy=0.97";
                }
                else if (res.Contains("sad"))
                {
                    mood = "sad";
                    range = "max_danceability=0.36&max_valence=0.36&max_energy=0.47";
                }
                else if (res.Contains("anger"))
                {
                    mood = "angry";
                    range = "max_danceability=0.3&max_valence=0.45&max_energy=0.85";
                }
                else if (res.Contains("contempt"))
                {
                    mood = "contempt";
                    range = "max_danceability=0.2&max_valence=0.20&max_energy=0.30";
                }
                else if (res.Contains("disgust"))
                {
                    mood = "disgust";
                    range = "max_danceability=0.5&max_valence=0.25&max_energy=0.85";
                }
                else if (res.Contains("fear"))
                {
                    mood = "afraid";
                    range = "max_danceability=0.6&max_valence=0.35&max_energy=0.20";
                }
                else if (res.Contains("surprise"))
                {
                    mood = "surprise";
                    range = "max_danceability=0.9&max_valence=0.95&max_energy=0.95";
                }
                else if (res.Contains("neutral"))
                {
                    mood = "normal";
                    range = "max_danceability=0.6&max_valence=0.75&max_energy=0.85";
                }
                else
                {
                    mood = "normal";
                    range = "max_danceability=0.6&max_valence=0.75&max_energy=0.85";
                }

                MessageLabel.Text = "Ah, we have noticed that you seem " + mood + ".\n\n" + "Please wait, we are looking for songs which suits your mood...";

                await Playlists(range);

                await Navigation.PushModalAsync(new VideoPage(Items));
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Exception");
            }
        }

        public async Task<List<PlayList>> Playlists(string range)
        {
            //Get Access Token
            String access_token = string.Empty;

            using (var client = new HttpClient())
            {
                var requestBody = "grant_type=client_credentials&redirect_uri=www.example.com&client_id=91d799dfba6b44a9a9d5e256c1b23010&client_secret=6932a95547e64d9785e7b496d2c61005";

                client.BaseAddress = new Uri("https://accounts.spotify.com/api/token");

                var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.PostAsync("https://accounts.spotify.com/api/token", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    using (HttpContent content = response.Content)
                    {
                        string result = await content.ReadAsStringAsync();
                        var jObj = JObject.Parse(result);
                        access_token = Convert.ToString(jObj["access_token"]);
                    }
                }
            }

            //Get Playlist for mood
            if (!String.IsNullOrEmpty(access_token))
            {
                using (var client = new HttpClient())
                {
                    String URL = "https://api.spotify.com/v1/recommendations?limit=20&seed_genres=indie-pop&" + range;
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

                    HttpResponseMessage response = await client.GetAsync(URL);
                    if (response.IsSuccessStatusCode)
                    {
                        using (HttpContent content = response.Content)
                        {
                            string result = await content.ReadAsStringAsync();
                            var jObj = JObject.Parse(result);
                            for (int i = 0; i < Enumerable.Count(jObj["tracks"]); i++)
                            {
                                try
                                {
                                    Items.Add(new PlayList(Convert.ToString(jObj["tracks"][i]["name"]), Convert.ToString(jObj["tracks"][i]["uri"]), Convert.ToString(jObj["tracks"][i].SelectToken("album").SelectToken("images")[1].SelectToken("url"))));
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                }
            }

            return Items;
        }
    }

    public class PlayList
    {
        public String SongName { get; set; }
        public String SpotifyURL { get; set; }
        public String Image { get; set; }

        public PlayList(String name, String spotifyurl, String image)
        {
            SongName = name;
            SpotifyURL = spotifyurl;
            Image = image;
        }
    }
}