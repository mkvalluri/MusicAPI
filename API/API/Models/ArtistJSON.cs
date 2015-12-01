using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class ArtistJSON
    {
        public int ArtistId { get; set; }

        public string ArtistEchonestId { get; set; }

        public string ArtistName { get; set; }

        public string ArtistLocation { get; set; }

        public string ArtistImageLink { get; set; }

        public string ArtistMainGenre { get; set; }

        public List<GenreJSON> ArtistGenres { get; set; }

        public List<YearActiveJSON> ArtistActiveYears { get; set; }
        
        public double ArtistPopularity { get; set; }

        public ArtistJSON()
        {
            ArtistGenres = new List<GenreJSON>();
            ArtistActiveYears = new List<YearActiveJSON>();
        }
    }

    public class YearActiveJSON
    {
        public int Start { get; set; }

        public int End { get; set; }
    }

    public class GenreJSON
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string imageLink { get; set; }

        public double Relevance { get; set; }
    }

    public class ResponseJSON
    {
        public string Response { get; set; }

        public int ArtistsCount { get; set; }

        public int ArtistPopularitiesCount { get; set; }

        public int ArtistGenresCount { get; set; }

        public int ArtistActiveYearsCount { get; set; }

        public int GenresCount { get; set; }

        public float TimeTaken { get; set; }
    }

    /// <summary>
    /// Echonest API
    /// </summary>

    public class EStatus
    {
        public string version { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class EArtist
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class EAResponse
    {
        public EStatus status { get; set; }
        public List<EArtist> artists { get; set; }
    }

    public class EGRootObject
    {
        public EAResponse response { get; set; }
    }
}