namespace API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ArtistGenre
    {
        public int Id { get; set; }

        public double Weight { get; set; }

        public double Frequency { get; set; }

        public int Genre_Id { get; set; }

        public int Artist_Id { get; set; }

        public virtual Artist Artist { get; set; }

        public virtual Genre Genre { get; set; }
    }
}
