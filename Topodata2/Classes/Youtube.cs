using System.Linq;
using System.Text.RegularExpressions;

namespace Topodata2.Classes
{
    public static class Youtube
    {
        private const string YoutubeLinkRegex =
            "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";

        public static string GetVideoId(string url)
        {
            var regex = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
            foreach (
                var groupdata in
                    regex.Matches(url)
                        .Cast<Match>()
                        .SelectMany(
                            match =>
                                match.Groups.Cast<Group>()
                                    .Where(
                                        groupdata =>
                                            !groupdata.ToString().StartsWith("http://") &&
                                            !groupdata.ToString().StartsWith("https://") &&
                                            !groupdata.ToString().StartsWith("youtu") &&
                                            !groupdata.ToString().StartsWith("www."))))
            {
                return groupdata.ToString();
            }
            return string.Empty;
        }

        public static string GetEmbed(string url)
        {
            return "https://www.youtube.com/embed/" + GetVideoId(url);
        }

        public static string GetEmbedFromId(string id)
        {
            return "https://www.youtube.com/embed/" + id;
        }

        public static string GetImage(string url)
        {
            return "https://img.youtube.com/vi/" + GetVideoId(url) + "/hqdefault.jpg";
        }

        public static string GetImageFromId(string id)
        {
            return "https://img.youtube.com/vi/" + id + "/hqdefault.jpg";
        }

        public static string GetWatch(string url)
        {
            return "https://www.youtube.com/watch?v=" + GetVideoId(url);
        }

        public static string GetWatchFromId(string id)
        {
            return "https://www.youtube.com/watch?v=" + id;
        }
    }
}