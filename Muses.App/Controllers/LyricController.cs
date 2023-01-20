using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace Muses.App.Controllers
{
    public class LyricController : Controller
    {
        public IActionResult Getlyric()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Getlyric(string artist,string songName)
        {
            string urlAddress = $"https://www.google.com/search?q={artist}+{songName}+lyrics";
            var actualData = string.Empty;
            var dataHtml = string.Empty;
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

                data = data.Substring(data.IndexOf(start) + start.Length);
                data = data.Substring(0, data.IndexOf(end));
                ViewBag.L = data;
            }

            return View();
        }
    }
}
