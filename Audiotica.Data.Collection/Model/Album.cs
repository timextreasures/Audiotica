﻿#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using Audiotica.Core.Common;
using Audiotica.Core.Utilities;
using Audiotica.Data.Collection.SqlHelper;

#endregion

namespace Audiotica.Data.Collection.Model
{
    public class Album : BaseEntry
    {
        private BitmapImage _artwork;

        public Album()
        {
            Songs = new OptimizedObservableCollection<Song>();
            AddableTo = new ObservableCollection<AddableCollectionItem>();
        }

        public string ProviderId { get; set; }

        [SqlProperty(ReferenceTo = typeof (Artist))]
        public long PrimaryArtistId { get; set; }


        public string Name { get; set; }

        public string Genre { get; set; }

        public DateTime ReleaseDate { get; set; }

        public OptimizedObservableCollection<Song> Songs { get; set; }

        public BitmapImage Artwork { get { return _artwork; } set { Set(ref _artwork, value); } }

        public Artist PrimaryArtist { get; set; }

        public bool HasArtwork { get; set; }

        public ObservableCollection<AddableCollectionItem> AddableTo { get; set; }
    }
}