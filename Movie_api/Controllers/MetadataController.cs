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
    [Route("api/Metadata")]
    public class MetadataController : Controller
    {
        // GET: api/Metadata/5
        [HttpGet("{id}", Name = "Get")]
        public JsonResult Get(int id)
        {
            List<Metadata> result = Read(id.ToString());

            if (result.Count == 0)
                return Json(new { error404 = "NOT FOUND" });

            return Json(result);
        }
        
        // POST: api/Metadata
        [HttpPost]
        public void Post([FromBody]Metadata value)
        {
            Data.DB.Add(value);
        }

        private List<Metadata> Read(string id)
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

                if(row[1] == id)
                {
                    if (result.Where(x => x.Language == row[3]).Count() > 0)
                        result.Remove(result.Single(x => x.Language == row[3]));

                    result.Add(new Metadata
                    {
                        MovieID = row[1],
                        Title = row[2],
                        Language = row[3],
                        Duration = row[4],
                        ReleaseYear = row[5]
                    });
                }
            }

            return result;
        }
    }
}
