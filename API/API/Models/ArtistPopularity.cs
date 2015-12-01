namespace API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ArtistPopularity
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public double Popularity { get; set; }

        public int Artist_Id { get; set; }

        public int MainGenre { get; set; }

        public virtual Artist Artist { get; set; }
    }
}
