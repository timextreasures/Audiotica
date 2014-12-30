﻿#region

using System;
using System.Linq;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Audiotica.Data.Collection.Model;
using Audiotica.Data.Spotify.Models;
using Audiotica.ViewModel;
using IF.Lastfm.Core.Objects;

#endregion

namespace Audiotica.View
{
    public sealed partial class HomePage
    {
        private bool _currentlyPreparing;

        public HomePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (Frame.CanGoBack)
            {
                Frame.BackStack.RemoveAt(0);
            }
        }

        //TODO [Harry,20140908] move this to view model with RelayCommand
        private async void TopSongsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var chartTrack = e.ClickedItem as ChartTrack;
            if (chartTrack == null) return;

            await CollectionHelper.SaveTrackAsync(chartTrack);
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var artist = ((Grid) sender).DataContext as LastArtist;
            Frame.Navigate(typeof (SpotifyArtistPage), "name." + artist.Name);
        }

        private void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
        }

        private async void MostPlayedGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_currentlyPreparing) return;
            _currentlyPreparing = true;

            var clickedSong = e.ClickedItem as Song;
            var vm = DataContext as MainViewModel;
            var index = vm.MostPlayed.IndexOf(clickedSong);

            await App.Locator.CollectionService.ClearQueueAsync().ConfigureAwait(false);

            foreach (var song in vm.MostPlayed.ToList())
            {
                await App.Locator.CollectionService.AddToQueueAsync(song);
            }

            App.Locator.AudioPlayerHelper.PlaySong(App.Locator.CollectionService.PlaybackQueue[index]);
            _currentlyPreparing = false;
        }

        private void RecommendationListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var artist = e.ClickedItem as LastArtist;
            Frame.Navigate(typeof(SpotifyArtistPage), "name." + artist.Name);
        }
    }
}