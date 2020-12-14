using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Movie_api.Controllers
{
    [Produces("application/json")]
    [Route("api/Movies/stats")]
    public class MovieController : Controller
    {
        // GET: api/Movie/stats
        [HttpGet()]
        public JsonResult Get()
        {
            return Json(CreateData());
        }

        private List<Metadata> ReadMovies()
        {
            List<Metadata> result = new List<Metadata>();

            StreamReader sr = new StreamReader("./CSVs/Metadata.csv");
            string line;
            string[] row = new string[6];
            while ((line = sr.ReadLine()) != null)
            {
                row = line.Split(',');
                if (row.Contains(""))
                    continue;

                if (result.Where(x => x.MovieID == row[1] && x.Language == row[3]).Count() > 0)
                    result.Remove(result.Single(x => x.MovieID == row[1] && x.Language == row[3]));

                result.Add(new Metadata
                {
                    MovieID = row[1],
                    Title = row[2],
                    Language = row[3],
                    Duration = row[4],
                    ReleaseYear = row[5]
                });
            }

            return result;
        }

        private List<MovieStat> CreateData()
        {
            List<Metadata> movies = ReadMovies().Where(x => x.Language == "EN").ToList();

            List<MovieStat> result = new List<MovieStat>();
            List<StatData> data = new List<StatData>();

            StreamReader sr = new StreamReader("./CSVs/stats.csv");
            string line;
            string[] row = new string[2];
            while ((line = sr.ReadLine()) != null)
            {
                row = line.Split(',');
                
                Int32.TryParse(row[0], out int movie);
                if (movie == 0)
                    continue;

                if (data.Where(x => x.MovieID == row[0]).Count() == 0)
                {
                    string[] time = new string[1];
                    time[0] = row[1];
                    data.Add(new StatData
                    {
                        MovieID = row[0],
                        Count = 1,
                        WatchDurationMs = new List<string>(time)
                    });
                }
                else
                {
                    data.Find(x => x.MovieID == row[0]).Count++;
                    data.Find(x => x.MovieID == row[0]).WatchDurationMs.Add(row[1]);
                }
            }

            foreach(StatData sd in data)
            {
                if (movies.Find(x => x.MovieID == sd.MovieID) == null)
                    continue;

                double duration = 0;
                foreach (string time in sd.WatchDurationMs)
                    duration += Int32.Parse(time);

                duration /= sd.Count;
                duration /= 1000;

                result.Add(new MovieStat
                {
                    MovieID = sd.MovieID,
                    Title = movies.Find(x => x.MovieID == sd.MovieID).Title,
                    AverageWatchDurationS = (int)duration,
                    Watches = sd.Count,
                    ReleaseYear = movies.Find(x => x.MovieID == sd.MovieID).ReleaseYear
                });
            }

            return result;
        }
    }
}
