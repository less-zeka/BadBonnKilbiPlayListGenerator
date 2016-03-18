using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadBonnSiteConsoleApp
{
    public class Youtube
    {
        private static string ApplicationName = "BadBonn";

        public async static Task AddSongsToPlaylist(IEnumerable<Song> songs, string playListName)
        {

            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows for full read/write access to the
                    // authenticated user's account.
                    new[] { YouTubeService.Scope.Youtube },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(ApplicationName)
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            var playList = await CreatePlayList(youtubeService, playListName);
            await AddSongs(youtubeService, playList, songs);           
        }

        private static async Task AddSongs(YouTubeService youtubeService, Playlist playList, IEnumerable<Song> songs)
        {
            foreach (var song in songs)
            {
                var newPlaylistItem = new PlaylistItem();
                newPlaylistItem.Snippet = new PlaylistItemSnippet();
                newPlaylistItem.Snippet.PlaylistId = playList.Id;
                newPlaylistItem.Snippet.ResourceId = new ResourceId();
                newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
                newPlaylistItem.Snippet.ResourceId.VideoId = song.YoutubeUrl.Substring(17, 11);

                newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();
            }
        }

        private static async Task<Playlist> CreatePlayList(YouTubeService youtubeService, string playListName)
        {
            var playlist = new Playlist();
            playlist.Snippet = new PlaylistSnippet();
            playlist.Snippet.Title = playListName;
            playlist.Snippet.Description = "A playlist created by Less n love";
            playlist.Status = new PlaylistStatus();
            playlist.Status.PrivacyStatus = "public";
            return await youtubeService.Playlists.Insert(playlist, "snippet,status").ExecuteAsync();
        }
    }
}