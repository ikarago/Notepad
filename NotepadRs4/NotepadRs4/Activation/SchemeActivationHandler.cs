using System;
using System.Diagnostics;
using System.Threading.Tasks;

using NotepadRs4.Services;
using NotepadRs4.Views;

using Windows.ApplicationModel.Activation;
using Windows.Storage;

namespace NotepadRs4.Activation
{
    internal class SchemeActivationHandler : ActivationHandler<ProtocolActivatedEventArgs>
    {
        protected override async Task HandleInternalAsync(ProtocolActivatedEventArgs args)
        {
            StorageFile dataFile = null;
            try
            {
                string path = args.Uri.LocalPath;
                StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                dataFile = file;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (dataFile != null)
            {
                NavigationService.Navigate(typeof(Views.MainPage), dataFile);
            }
            else
            {
                NavigationService.Navigate(typeof(Views.MainPage));
            }

            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(ProtocolActivatedEventArgs args)
        {
            // If your app has multiple handlers of ProtocolActivationEventArgs
            // use this method to determine which to use. (possibly checking args.Uri.Scheme)
            return true;
        }
    }
}
