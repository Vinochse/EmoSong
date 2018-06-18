using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace EmoSong
{
    public partial class VideoPage : ContentPage
    {
        public VideoPage(List<PlayList> Items)
        {
            InitializeComponent();

            list.ItemsSource = Items;
        }

        void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            PlayList p = (PlayList)e.Item;

            try
            {
                Device.OpenUri(new Uri(p.SpotifyURL));
            }
            catch (Exception)
            {
                DisplayAlert("Spotify Not Installed", "You must install spotify to play the song.", "Got It!");
            }

            ((ListView)sender).SelectedItem = null;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();

            Navigation.PushModalAsync(new MainPage());

            return true;
        }
    }
}