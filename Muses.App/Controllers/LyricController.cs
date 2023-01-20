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
            if(ModelState.IsValid)
            {
                string urlAddress = $"https://www.google.com/search?q={getLyric.ArtistName}+{getLyric.SongName}+lyrics";
                var data = string.Empty;
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

                    if (!string.IsNullOrEmpty(data))
                    {
                        data = data.Substring(data.IndexOf(start) + start.Length);
                        data = data.Substring(0, data.IndexOf(end));
                        ViewBag.L = data;
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            return View(getLyric);
        }
    }
}
