using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace LoadBadBonnSiteConsoleApp
{
    class Program
    {
        static void Main()
        {
            //Program config
            var day = "Donnerstag";
            day = "Freitag";
            day = "Samstag";

            //source config
            var url = "http://kilbi.badbonn.ch/2016/_rubric/index.php?rubric=Programm";
            var identifier = string.Format("detail.php?rubric={0}&nr=", day);

            //target config
            var playListName = "Bad Bonn Kilbi 2016 - " +day;
            var songs = SongParser.GetSongs(url, identifier);

            //1 2 1 2... Action!
            Youtube.AddSongsToPlaylist(songs, playListName);

            Console.WriteLine("Your welcome!");

            Console.Read();   
        }
    }
}