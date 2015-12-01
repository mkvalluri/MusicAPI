namespace API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Album
    {
        public int Id { get; set; }

        public string AlbumID { get; set; }

        public string AlbumName { get; set; }

        public int? AlbumReleaseDate { get; set; }

        public double? AlbumRating { get; set; }

        public int ArtistId { get; set; }

        public virtual Artist Artist { get; set; }
    }
}
