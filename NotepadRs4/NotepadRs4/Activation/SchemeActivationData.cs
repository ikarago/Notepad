using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using Windows.Storage;

namespace NotepadRs4.Activation
{
    public class SchemeActivationData
    {
        private const string ProtocolName = "notepad-uwp";

        public Type PageType { get; private set; }

        public Uri Uri { get; private set; }

        //public Dictionary<string, string> Parameters { get; private set; } = new Dictionary<string, string>();
        public object Parameters { get; private set; }

        public bool IsValid => PageType != null;

        public SchemeActivationData(Uri activationUri)
        {
            // #TODO Rework this to check the validity of the given file

            // #TODO If valid then launch the MainPage with the StorageFile as parameter

            GetStorageFileFromUri(activationUri);
            if (Parameters != null)
            {
                PageType = typeof(Views.MainPage);
            }

            if (!IsValid || string.IsNullOrEmpty(activationUri.Query))
            {
                return;
            }




            //PageType = SchemeActivationConfig.GetPage(activationUri.AbsolutePath);

            //if (!IsValid || string.IsNullOrEmpty(activationUri.Query))
            //{
            //    return;
            //}

            //var uriQuery = HttpUtility.ParseQueryString(activationUri.Query);
            //foreach (var paramKey in uriQuery.AllKeys)
            //{
            //    Parameters.Add(paramKey, uriQuery.Get(paramKey));
            //}
        }


        private async void GetStorageFileFromUri(Uri activationUri)
        {
            try
            {
                string path = activationUri.LocalPath;
                StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                Parameters = file;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            
        }

        public SchemeActivationData(Type pageType, Dictionary<string, string> parameters = null)
        {
            PageType = pageType;
            Parameters = parameters;
            Uri = BuildUri();
        }

        private Uri BuildUri()
        {
            var pageKey = SchemeActivationConfig.GetPageKey(PageType);
            var uriBuilder = new UriBuilder($"{ProtocolName}:{pageKey}");
            var query = HttpUtility.ParseQueryString(string.Empty);

            //foreach (var parameter in Parameters)
            //{
            //    query.Set(parameter.Key, parameter.Value);
            //}

            uriBuilder.Query = query.ToString();
            return new Uri(uriBuilder.ToString());
        }
    }
}
