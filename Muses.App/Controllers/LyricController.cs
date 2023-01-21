using System.IO;
using Microsoft.AspNetCore.Mvc;
using Muses.Domain.ViewModels;
using System.Net;
using System.Text;

namespace Muses.App.Controllers
{
    public class LyricController : Controller
    {
        [HttpGet]
        public IActionResult GetLyric()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult GetLyric(GetLyricViewModel getLyric)
        {
            if (ModelState.IsValid)
            {
                var data = string.Empty;
                var fileName = $"{getLyric.ArtistName}-{getLyric.SongName}.txt";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Transcripts", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        data = sr.ReadToEnd();
                        ViewBag.L = data;
                    }
                }
                else
                {
                    string urlAddress = $"https://www.google.com/search?q={getLyric.ArtistName}+{getLyric.SongName}+lyrics";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream receiveStream = response.GetResponseStream();
                        StreamReader readStream = null;
                        if (String.IsNullOrWhiteSpace(response.CharacterSet))
                            readStream = new StreamReader(receiveStream);
                        else
                            readStream = new StreamReader(receiveStream,
                                Encoding.GetEncoding(response.CharacterSet));
                        data = readStream.ReadToEnd();
                        response.Close();
                        readStream.Close();

                        string start = "</div></div></div></div><div class=\"hwc\"><div class=\"BNeawe tAd8D AP7Wnd\"><div><div class=\"BNeawe tAd8D AP7Wnd\">";
                        string end = "</div></div></div></div></div><div><span class=\"hwc\"><div class=\"BNeawe uEec3 AP7Wnd\">";


                        data = data.Substring(data.IndexOf(start) + start.Length);
                        data = data.Substring(0, data.IndexOf(end));
                        ViewBag.L = data;

                        // Saving the Transcript on the 
                        if (System.IO.File.Exists(filePath))
                            System.IO.Directory.CreateDirectory(filePath);

                        using (StreamWriter sw = System.IO.File.CreateText(filePath))
                        {
                            sw.WriteLine(data);
                        }

                    }
                }
            }
            return View(getLyric);
        }
    }
}
