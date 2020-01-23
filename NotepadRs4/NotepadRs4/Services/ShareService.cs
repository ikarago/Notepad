using NotepadRs4.Helpers;
using NotepadRs4.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Provider;

namespace NotepadRs4.Services
{
    public class ShareService
    {
        // Properties
        TextDataModel TextData { get; set; }
        List<StorageFile> TempShareFiles { get; set; }
        string TextToShare { get; set; }





        /// <summary>
        /// Checks whether the device supports the Share contracts
        /// </summary>
        /// <returns></returns>
        public bool CheckIfShareIsSupported()
        {
            bool result = DataTransferManager.IsSupported();
            Debug.WriteLine("ShareService - Is Share supported = " + result);
            return result;
        }

        /// <summary>
        /// Shares text with other apps
        /// </summary>
        /// <param name="textToShare">Text the user wants to share</param>
        public void Share(string textToShare)
        {
            Debug.WriteLine("ShareService - Share - Share STRING");
            TextToShare = textToShare;
            ShowShareUI();
        }

        /// <summary>
        /// Shares a the complete file with other apps
        /// </summary>
        /// <param name="data">The TextDataModel containing all the data</param>
        public async void Share(TextDataModel data)
        {
            Debug.WriteLine("ShareService - Share - Share FILE");
            TextData = data;
            if (data != null)
            {
                TempShareFiles = await GetShareStorageFiles(data);
            }
            ShowShareUI();
        }

        /// <summary>
        /// Sets and shows the Share Contract UI
        /// </summary>
        private void ShowShareUI()
        {
            Debug.WriteLine("ShareService - ShowShareUI - START");
            if (CheckIfShareIsSupported())
            {
                DataTransferManager.GetForCurrentView().DataRequested += ShareService_DataRequested;
                DataTransferManager.ShowShareUI();
            }
            else
            {
                Debug.WriteLine("ShareService - ShowShareUI - Share Contract is not supported");
            }
        }

        /// <summary>
        /// Gathers the data that is being requested for the Share Contract
        /// </summary>
        private void ShareService_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            Debug.WriteLine("ShareService - DataRequested - START");

            // Show the Share UI
            DataRequest request = args.Request;

            // Set the metadata
            request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;

            // If not null, share the document
            if (TempShareFiles != null)
            {
                request.Data.Properties.Description = (ResourceExtensions.GetLocalized("ShareDescription") + " " + TempShareFiles[0].Name);
                // Set the StorageItem
                request.Data.SetStorageItems(TempShareFiles);
                Debug.WriteLine("ShareService - DataRequested - FILE set");

            }
            else // Share the selected text
            {
                request.Data.Properties.Description = "Selected text: '" + TextToShare + "'";
                request.Data.SetText(TextToShare);
                Debug.WriteLine("ShareService - DataRequested - TEXT set");
            }
            Debug.WriteLine("ShareService - DataRequested - DONE");
        }


        /// <summary>
        /// Prepare the temp files to be shared in a ReadOnlyList
        /// </summary>
        /// #TODO Migrate part of this to the FileDataService (the creating of the temp file
        private async Task<List<StorageFile>> GetShareStorageFiles(TextDataModel textDataModel)
        {
            Debug.WriteLine("ShareService - GetShareStorageFiles - START");

            ApplicationData appData = ApplicationData.Current;
            List<StorageFile> filesToShare = new List<StorageFile>();
            StorageFile tempFile;


            // Save the current document to a temp folder
            // #TODO: Move this to the FileDataService
            // If the file being shared already has been saved use that name instead
            if (textDataModel.DataFile != null)
            {
                tempFile = await appData.TemporaryFolder.CreateFileAsync(textDataModel.DataFile.Name, CreationCollisionOption.ReplaceExisting);
            }
            else
            {
                tempFile = await appData.TemporaryFolder.CreateFileAsync((ResourceExtensions.GetLocalized("UnititledLabel") + ".txt"), CreationCollisionOption.ReplaceExisting);
            }

            // Write the data to the temp file
            // Prevent remote access to file until saving is done
            CachedFileManager.DeferUpdates(tempFile);
            //We need to make sure Data.Text isn't null.
            textDataModel.Text = textDataModel.Text ?? ""; //coalescing operator '??' returns left-hand operand's value if it isn't null, else right-hand's
            // Write the stuff to the file
            await FileIO.WriteTextAsync(tempFile, textDataModel.Text);
            // Let Windows know stuff is done
            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(tempFile);



            // Add the files to the List
            filesToShare.Add(tempFile);

            // Preperation is done, continue with the other stuff
            Debug.WriteLine("ShareService - GetShareStorageFiles - DONE");
            return filesToShare;
        }


    }
}
