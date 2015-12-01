namespace API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ActiveYear
    {
        public int Id { get; set; }

        public int Start { get; set; }

        public int End { get; set; }

        public int ArtistId { get; set; }

        public virtual Artist Artist { get; set; }
    }
}
