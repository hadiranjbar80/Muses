using System.IO;
using Microsoft.AspNetCore.Mvc;
using Muses.Domain.ViewModels;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;

namespace Muses.App.Controllers
{
    public class LyricController : BaseController
    {
        private string transcript = string.Empty;
        private string rawData = string.Empty;
        private string songName = string.Empty;
        private string artistName = string.Empty;
        private string fileName = string.Empty;
        private string filePath = string.Empty;

        public LyricController(IConfiguration configuration) : base(configuration)
        {
        }

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

                string[] files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Transcripts"));

                foreach (var file in files)
                {
                    if (file.ToLower().Contains(getLyric.ArtistName) && file.ToLower().Contains(getLyric.SongName))
                    {
                        filePath = file;
                    }
                }

                if (System.IO.File.Exists(filePath))
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        transcript = sr.ReadToEnd();
                        string fileName = filePath.Substring(60);
                        TempData["Transcript"] = transcript;
                        TempData["ArtistName"] = fileName.Split("-")[0];
                        TempData["SongName"] = fileName.Split("-")[1].Split(".")[0];
                    }
                    return RedirectToAction("ShowTranscript");
                }
                else
                {
                    if (CheckInternetConnection())
                    {
                        string urlAddress = $"https://www.google.com/search?q={getLyric.ArtistName}+{getLyric.SongName}+lyrics";
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);

                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        Stream receiveStream = response.GetResponseStream();
                        StreamReader readStream = null;
                        if (String.IsNullOrWhiteSpace(response.CharacterSet))
                            readStream = new StreamReader(receiveStream);
                        else
                            readStream = new StreamReader(receiveStream,
                                Encoding.GetEncoding(response.CharacterSet));
                        rawData = readStream.ReadToEnd();
                        response.Close();
                        readStream.Close();

                        // Saving the Transcript on the 
                        transcript = GetTranscript(rawData);
                        string fileName = CreateFileName(rawData);
                        if (transcript == "" || fileName == "Not found!")
                        {
                            Notify("No lyric found!", "Error", NotificationType.error);
                            
                        }
                        else
                        {
                            TempData["Transcript"] = transcript;

                            filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Transcripts", fileName);
                            if (System.IO.File.Exists(filePath))
                                System.IO.Directory.CreateDirectory(filePath);

                            using (StreamWriter sw = System.IO.File.CreateText(filePath))
                            {
                                sw.WriteLine(transcript);
                            }

                            return RedirectToAction("ShowTranscript");
                        }
                    }
                    else
                    {
                        Notify("No internet connection!", "Connection error", NotificationType.error);
                    }

                }
            }
            return View(getLyric);

            string CreateFileName(string actualDate)
            {
                string start = "/ </span><span><span class=\"BNeawe s3v9rd AP7Wnd\">";
                string end = "</span></span></div><div class=\"Q0HXG\"></div>";

                artistName = actualDate.IndexOf(start) == -1 ? "" : actualDate.Substring(rawData.IndexOf(start) + start.Length);
                if (artistName != "")
                    artistName = artistName.Substring(0, artistName.IndexOf(end));

                start = "<span><span class=\"BNeawe tAd8D AP7Wnd\">";
                end = "</span></span><span class=\"BNeawe s3v9rd AP7Wnd\">";

                songName = actualDate.IndexOf(start) == -1 ? "" : actualDate.Substring(rawData.IndexOf(start) + start.Length);
                if (songName != "")
                    songName = songName.Substring(0, songName.IndexOf(end));

                var fileName = $"{artistName}-{songName}.txt";
                TempData["ArtistName"] = artistName;
                TempData["SongName"] = songName;
                if (fileName.Trim() == "")
                {
                    return "Not found!";
                }

                return fileName;
            }

            string GetTranscript(string rawData)
            {

                string start = "</div></div></div></div><div class=\"hwc\"><div class=\"BNeawe tAd8D AP7Wnd\"><div><div class=\"BNeawe tAd8D AP7Wnd\">";
                string end = "</div></div></div></div></div><div><span class=\"hwc\"><div class=\"BNeawe uEec3 AP7Wnd\">";

                rawData = rawData.IndexOf(start) == -1 ? "" : rawData.Substring(rawData.IndexOf(start) + start.Length);
                if (rawData != "")
                    rawData = rawData.Substring(0, rawData.IndexOf(end));
                return rawData;
            }

            bool CheckInternetConnection()
            {
                try
                {
                    using (var client = new WebClient())
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }

            }
        }

        [HttpGet]
        public IActionResult ShowTranscript()
        {
            var result = new ShowLyricResult()
            {
                Transcript = TempData["Transcript"].ToString(),
                ArtistName = TempData["ArtistName"].ToString(),
                SongName = TempData["SongName"].ToString()
            };

            return View(result);
        }
    }
}
