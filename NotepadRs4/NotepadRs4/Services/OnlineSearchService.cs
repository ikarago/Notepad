using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotepadRs4.Services
{
    public enum OnlineSearchProvider
    {
        Default,
        Bing,
        DuckDuckGo,
        Google
    }

    public static class OnlineSearchService
    {
        /// <summary>
        /// Opens the users default webbrowser and searches for the given text
        /// </summary>
        /// <param name="textToSearch">The text that needs to be searched for online></param>
        /// <param name="searchProvider">Set the Search Provider to search the text with</param>
        public static void SearchOnline(string textToSearch, OnlineSearchProvider searchProvider = OnlineSearchProvider.Default)
        {
            switch (searchProvider)
            {
                case OnlineSearchProvider.Default:
                    {
                        // #TODO Check the default search provider in the settings
                        SearchWithBing(textToSearch);

                        break;
                    }
                case OnlineSearchProvider.Bing:
                    {
                        SearchWithBing(textToSearch);
                        break;
                    }
                case OnlineSearchProvider.DuckDuckGo:
                    {
                        SearchWithDuckDuckGo(textToSearch);
                        break;
                    }
                case OnlineSearchProvider.Google:
                    {
                        SearchWithGoogle(textToSearch);
                        break;
                    }
            }
        }


        /// <summary>
        /// Launches a search with Bing
        /// </summary>
        /// <param name="textToSearch">The text that needs to be searched online for</param>
        private static async void SearchWithBing(string textToSearch)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri($"https://www.bing.com/search?q={Uri.EscapeDataString(textToSearch)}"));
        }

        /// <summary>
        /// Launches a search with DuckDuckGo
        /// </summary>
        /// <param name="textToSearch">The text that needs to be searched online for</param>
        private static async void SearchWithDuckDuckGo(string textToSearch)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri($"https://duckduckgo.com/?q={Uri.EscapeDataString(textToSearch)}"));
        }

        /// <summary>
        /// Launches a search with Google
        /// </summary>
        /// <param name="textToSearch">The text that needs to be searched online for</param>
        private static async void SearchWithGoogle(string textToSearch)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri($"https://www.google.com/search?q={Uri.EscapeDataString(textToSearch)}"));
        }

        // #TODO: Settings getter
    }
}
