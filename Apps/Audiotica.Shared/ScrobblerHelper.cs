﻿#region

using System;
using System.Linq;
using System.Threading.Tasks;
using Audiotica.Core.Common;
using Audiotica.Data;
using Audiotica.Data.Collection.Model;
using IF.Lastfm.Core.Objects;

#endregion

namespace Audiotica
{
    public static class ScrobblerHelper
    {
        public static Artist ToArtist(this LastArtist lastArtist)
        {
            return new Artist
            {
                Name = lastArtist.Name,
                ProviderId = !string.IsNullOrEmpty(lastArtist.Mbid) ? ("mbid." + lastArtist.Mbid) : ("lastid." + lastArtist.Id),
            };
        }

        public static Album ToAlbum(this LastAlbum lastAlbum)
        {
            var album = new Album
            {
                ProviderId = !string.IsNullOrEmpty(lastAlbum.Mbid) ? ("mbid." + lastAlbum.Mbid) : ("lastid." + lastAlbum.Id),
                Name = lastAlbum.Name,
                ReleaseDate = lastAlbum.ReleaseDateUtc,
                Genre = lastAlbum.TopTags != null ? lastAlbum.TopTags.First().Name : ""
            };

            return album;
        }

        public static Song ToSong(this LastTrack track)
        {
            var song = new Song
            {
                ProviderId = !string.IsNullOrEmpty(track.Mbid) ? ("mbid." + track.Mbid) : ("lastid." + track.Id),
                Name = track.Name
            };
            return song;
        }

        public static async Task SaveTrackAsync(LastTrack track)
        {
            var url = await Mp3MatchEngine.FindMp3For(track);

            if (string.IsNullOrEmpty(url))
                CurtainPrompt.ShowError("No match found");

            else
            {
                var song = await PrepareTrackForDownloadAsync(track);
                song.AudioUrl = url;
                try
                {
                    await App.Locator.CollectionService.AddSongAsync(song, track.Images != null ? track.Images.Largest.AbsoluteUri : null);
                    CurtainPrompt.Show("Song saved");
                }
                catch (Exception e)
                {
                    CurtainPrompt.ShowError(e.Message);
                }
            }
        }

        public static async Task<Song> PrepareTrackForDownloadAsync(LastTrack track)
        {
            track = await App.Locator.ScrobblerService.GetDetailTrack(track.Name, track.ArtistName);
            var song = track.ToSong();
            LastArtist artist;

            if (!string.IsNullOrEmpty(track.AlbumName))
            {
                var lastAlbum = await App.Locator.ScrobblerService.GetDetailAlbum(track.AlbumName, track.ArtistName);
                artist = await App.Locator.ScrobblerService.GetDetailArtistByMbid(track.ArtistMbid ?? track.ArtistName);
                song.Album = lastAlbum.ToAlbum();
                song.Album.PrimaryArtist = artist.ToArtist();
                track.Images = lastAlbum.Images;
            }

            else
                artist = await App.Locator.ScrobblerService.GetDetailArtist(track.ArtistName);

            song.Artist = artist.ToArtist();
            song.ArtistName = artist.Name;

            return song;
        }
    }
}