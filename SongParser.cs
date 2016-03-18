using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoadBadBonnSiteConsoleApp
{
    class SongParser
    {
        public static IEnumerable<Song> GetSongs(string url, string identifier)
        {
            var artistPages = GetArtistPage(url, identifier);

            var songs = new List<Song>();

            foreach(var subSite in artistPages)
            {
                songs.AddRange(GetSubSiteVideo(subSite));
            }

            return songs;
        }

        private static IEnumerable<string> GetArtistPage(string url, string identifier)
        {
            var subSites = new List<string>();
            var web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href] "))
            {
                HtmlAttribute att = link.Attributes["href"];
                if (att.Value.Contains(identifier))
                {
                    var result = Regex.Match(att.Value, @"\d+$").Value;
                    if (!string.IsNullOrEmpty(result))
                    {
                        subSites.Add(att.Value);
                    }
                }
            }

            return subSites;
        }

        private static IEnumerable<Song> GetSubSiteVideo(string url)
        {
            var videos = new List<Song>();

            var web = new HtmlWeb();
            var host = "http://kilbi.badbonn.ch/2016";
            var address = string.Format("{0}{1}", host, url.Substring(2));
            HtmlDocument doc = web.Load(address);

            var dataVideoIdIndex = doc.DocumentNode.OuterHtml.IndexOf("data-video-id");

            if (dataVideoIdIndex > 0)
            {
                var videoId = doc.DocumentNode.OuterHtml.Substring(dataVideoIdIndex + 15, 11);


                videos.Add(new Song
                {
                    YoutubeUrl = "https://youtu.be/" + videoId,//att.Value,
                    VideoId = videoId
                });
            }
 
            return videos;
        }
    }
}
