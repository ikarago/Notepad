using NotepadRs4.Activation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace NotepadRs4.Services
{
    internal class FileAssociationService : ActivationHandler<FileActivatedEventArgs>
    {
        protected override async Task HandleInternalAsync(FileActivatedEventArgs args)
        {
            var file = args.Files.FirstOrDefault();
            // #TODO: Open the loaded file in a new window (if there is another file open)
            NavigationService.Navigate(typeof(Views.MainPage), file);
            await Task.CompletedTask;  
        }
    }
}
