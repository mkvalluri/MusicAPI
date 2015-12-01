using API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class InformationController : ApiController
    {

        static Data dbCon = new Data();
        static List<ArtistPopularity> artistPopularities = new List<ArtistPopularity>();
        static List<ArtistGenre> artistGenres = new List<ArtistGenre>();
        static List<ArtistJSON> artistInfos = new List<ArtistJSON>();
        static List<ActiveYear> artistActiveYears = new List<ActiveYear>();
        static List<Genre> genres = new List<Genre>();

        [Route("api/InitRepo")]
        [HttpGet]
        public ResponseJSON LoadData()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                artistInfos = dbCon.Artists.Select(a => new ArtistJSON() { ArtistId = a.Id, ArtistName = a.ArtistName, ArtistEchonestId = a.EchonestID, ArtistImageLink = a.ArtistImageURL, ArtistLocation = a.ArtistLocation }).ToList();
                artistPopularities = dbCon.ArtistPopularities.Where(a => a.Year >= 1960 && a.Year <= 2014).ToList();
                artistGenres = dbCon.ArtistGenres.ToList();
                artistActiveYears = dbCon.ActiveYears.ToList();
                genres = dbCon.Genres.ToList();

                artistInfos.ForEach(a =>
                {
                    a.ArtistGenres = artistGenres
                    .Where(ar => ar.Artist_Id == a.ArtistId)
                    .Select(g => new GenreJSON() { Id = g.Genre_Id, Name = g.Genre.GenreName, imageLink = "http://dummyimage.com/300x300&text=" + g.Genre.GenreName, Relevance = g.Frequency * g.Weight })
                    .OrderByDescending(o => o.Relevance)
                    .ToList();

                    if (a.ArtistGenres.Count > 0)
                        a.ArtistMainGenre = a.ArtistGenres.FirstOrDefault().Name;
                    else
                        a.ArtistMainGenre = "";

                    a.ArtistActiveYears = artistActiveYears
                    .Where(ar => ar.ArtistId == a.ArtistId)
                    .Select(g => new YearActiveJSON() { Start = g.Start, End = g.End })
                    .OrderBy(o => o.Start)
                    .ToList();
                });


                return new ResponseJSON() { Response = "Success.", ArtistPopularitiesCount = artistPopularities.Count, ArtistGenresCount = artistGenres.Count, ArtistsCount = artistInfos.Count, ArtistActiveYearsCount = artistActiveYears.Count, TimeTaken = stopwatch.ElapsedMilliseconds, GenresCount = genres.Count };
            }
            catch (Exception e)
            {
                return new ResponseJSON() { Response = e.InnerException.ToString() };
            }
        }

        [Route("api/TopArtists")]
        [HttpGet]
        public List<ArtistJSON> GetTopArtists(int startYear, int endYear)
        {
            var returnList = new List<ArtistJSON>();
            var dataList = new List<ArtistPopularity>();

            if (artistPopularities.Count > 0)
            {
                dataList = artistPopularities.Where(a => a.Year >= startYear && a.Year <= endYear).ToList();
                returnList = dataList.GroupBy(r => r.Artist_Id).Select(r => new KeyValuePair<int, double>(r.Key, r.Sum(p => p.Popularity))).ToList().OrderByDescending(o => o.Value).ToList().GetRange(0, 10)
                    .Select(a => new ArtistJSON() { ArtistId = a.Key, ArtistPopularity = a.Value / (endYear - startYear) }).ToList();
            }

            return PopulateArtistDetails(returnList);
        }

        [Route("api/TopGenresByDecade")]
        [HttpGet]
        public List<GenreJSON> GetTopGenresByDecade(int startYear, int endYear)
        {
            var returnList = new List<GenreJSON>();
            var dataList = new List<ArtistPopularity>();

            if (artistGenres.Count > 0)
            {
                dataList = artistPopularities.Where(a => a.Year >= startYear && a.Year <= endYear).ToList();
                returnList = dataList.GroupBy(r => r.MainGenre).Select(r => new KeyValuePair<int, double>(r.Key, r.Sum(p => p.Popularity))).ToList().OrderByDescending(o => o.Value).ToList().GetRange(0, 10)
                    .Select(a => new GenreJSON() { Id = a.Key, Relevance = a.Value / (endYear - startYear) }).ToList();
            }

            return PopulateGenreDetails(returnList);
        }

        [Route("api/TopGenres")]
        [HttpGet]
        public Dictionary<int, List<GenreJSON>> GetTopGenres(int startYear, int endYear)
        {
            var returnList = new Dictionary<int, List<GenreJSON>>();
            var dataList = new List<ArtistPopularity>();

            if (artistGenres.Count > 0)
            {
                for (int i = startYear; i <= endYear; i++)
                {
                    dataList = artistPopularities.Where(a => a.Year == i).ToList();
                    returnList.Add(i, PopulateGenreDetails(dataList.GroupBy(r => r.MainGenre).Select(r => new KeyValuePair<int, double>(r.Key, r.Sum(p => p.Popularity))).ToList().OrderByDescending(o => o.Value).ToList().GetRange(0, 10)
                        .Select(a => new GenreJSON() { Id = a.Key, Relevance = a.Value }).ToList()));
                }
            }

            return returnList;
        }

        [Route("api/SimilarArtists")]
        [HttpGet]
        public List<ArtistJSON> GetSimilarArtists(string artistName)
        {
            List<ArtistJSON> artistList = new List<ArtistJSON>();
            WebClient wc = new WebClient();
            var result = wc.DownloadString("http://developer.echonest.com/api/v4/artist/similar?api_key=L6L1RWYT1A0EWKHJF&format=json&results=10&start=0&name=" + artistName);
            var data = JsonConvert.DeserializeObject<EGRootObject>(result);
            if (data.response.artists.Count > 0)
            {
                artistList = GetArtistsInfo(data.response.artists.Select(a => a.id).ToList());
            }
            return artistList;
        }

        [Route("api/TopArtistsByGenre")]
        [HttpGet]
        public List<ArtistJSON> GetTopArtistsByGenre(string genreName)
        {
            List<ArtistJSON> artistList = new List<ArtistJSON>();
            WebClient wc = new WebClient();
            var result = wc.DownloadString("http://developer.echonest.com/api/v4/genre/artists?api_key=L6L1RWYT1A0EWKHJF&format=json&results=10&name=" + genreName);
            var data = JsonConvert.DeserializeObject<EGRootObject>(result);
            if (data.response.artists.Count > 0)
            {
                artistList = GetArtistsInfo(data.response.artists.Select(a => a.id).ToList());
            }
            return artistList;
        }

        [Route("api/ArtistsByGenre")]
        [HttpGet]
        public List<ArtistJSON> GetArtistsByGenre(string genreName)
        {
            List<ArtistJSON> artistList = new List<ArtistJSON>();
            artistList = GetArtistsInfo(artistGenres.Where(g => g.Genre.GenreName.Contains(genreName)).Select(s => s.Artist.EchonestID).ToList().GetRange(0, 30));

            if (artistList != null && artistList.Count > 0)
                return artistList;
            else
            {
                artistList = new List<ArtistJSON>();
                artistList.Add(new ArtistJSON() { ArtistActiveYears = null, ArtistEchonestId = null, ArtistGenres = null, ArtistId = -1, ArtistImageLink = null, ArtistLocation = null, ArtistMainGenre = null, ArtistName = null, ArtistPopularity = -1 });
                return artistList;
            }
        }

        [Route("api/SearchArtistsByName")]
        [HttpGet]
        public List<ArtistJSON> GetArtists(string artistName)
        {
            List<ArtistJSON> artistList = new List<ArtistJSON>();
            artistList = artistInfos.Where(a => a.ArtistName.Contains(artistName)).ToList();
            
            if (artistList != null && artistList.Count > 0)
                return artistList;
            else
            {
                artistList = new List<ArtistJSON>();
                artistList.Add(new ArtistJSON() { ArtistActiveYears = null, ArtistEchonestId = null, ArtistGenres = null, ArtistId = -1, ArtistImageLink = null, ArtistLocation = null, ArtistMainGenre = null, ArtistName = null, ArtistPopularity = -1 });
                return artistList;
            }
        }

        [Route("api/SearchArtistsByYearRange")]
        [HttpGet]
        public List<ArtistJSON> GetArtistsByYearRange(int startYear, int endYear)
        {
            var artistList = artistInfos.Where(a => a.ArtistActiveYears.Count > 0 && a.ArtistActiveYears.FirstOrDefault().Start >= startYear && a.ArtistActiveYears.LastOrDefault().Start <= endYear).ToList().GetRange(0, 10);
            if (artistList != null && artistList.Count > 0)
                return artistList;
            else
            {
                artistList = new List<ArtistJSON>();
                artistList.Add(new ArtistJSON() { ArtistActiveYears = null, ArtistEchonestId = null, ArtistGenres = null, ArtistId = -1, ArtistImageLink = null, ArtistLocation = null, ArtistMainGenre = null, ArtistName = null, ArtistPopularity = -1 });
                return artistList;
            }
        }

        #region Private Functions

        private List<ArtistJSON> GetArtistsInfo(List<string> artistList)
        {
            List<ArtistJSON> returnArtistList = new List<ArtistJSON>();

            artistList.ForEach(a =>
            {
                if(artistInfos.Where(ar => ar.ArtistEchonestId == a).FirstOrDefault() != null)
                    returnArtistList.Add(artistInfos.Where(ar => ar.ArtistEchonestId == a).FirstOrDefault());
            });

            return returnArtistList;
        }

        private List<ArtistJSON> PopulateArtistDetails(List<ArtistJSON> artistList)
        {
            artistList.ForEach(a =>
            {
                var tempA = artistInfos.Where(ta => ta.ArtistId == a.ArtistId).FirstOrDefault();
                a.ArtistName = tempA.ArtistName;
                a.ArtistImageLink = tempA.ArtistImageLink;
                a.ArtistLocation = tempA.ArtistLocation;
                a.ArtistMainGenre = tempA.ArtistMainGenre;
                a.ArtistGenres = tempA.ArtistGenres;
                a.ArtistActiveYears = tempA.ArtistActiveYears;
            });
            return artistList;
        }

        private List<GenreJSON> PopulateGenreDetails(List<GenreJSON> genreList)
        {
            genreList.ForEach(g =>
            {
                var tempG = genres.Where(tg => tg.Id == g.Id).FirstOrDefault();
                g.Name = tempG.GenreName;
                g.imageLink = "http://dummyimage.com/300x300&text=" + g.Name;
            });
            return genreList;
        }

        #endregion Private Functions
    }
}
