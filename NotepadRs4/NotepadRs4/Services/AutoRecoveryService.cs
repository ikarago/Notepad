using NotepadRs4.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace NotepadRs4.Services
{
    public static class AutoRecoveryService
    {
        // Properties
        static readonly StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;





        // Save to Temp File
        /// <summary>
        /// Saves the data in an AutoRecovery File in case the app crashes
        /// </summary>
        /// <param name="data">TextDataModel containing all the info</param>
        /// <returns>Returns the StorageFile with the location for later reference</returns>
        public static async Task<StorageFile> SaveAutoRecoveryFile(TextDataModel data)
        {
            StorageFile file = await tempFolder.CreateFileAsync("AutoRecoveryFile.txt", CreationCollisionOption.GenerateUniqueName);

            if (file != null)
            {
                bool success = await FileDataService.Save(data, file);
                Debug.WriteLine("FileDataService - SaveAutoRecoveryFile - Success = " + success);
                return file;
            }
            else
            {
                Debug.WriteLine("FileDataService - SaveAutoRecoveryFile - Something has gone horribly, horribly wrong");
                return null;
            }
        }


        // Load Temp file
        public static async void LoadAutoRecoveryFiles()
        {
            // Get a list of AutoRecoveryFiles and open all of them via URI Activation
            var autoRecoveryFiles = await tempFolder.GetFilesAsync();
            foreach (var item in autoRecoveryFiles)
            {
                var uri = new Uri("notepad-uwp:" + item.Path);
                var success = await Windows.System.Launcher.LaunchUriAsync(uri);
                // Put a pause here?
            }

            // #TODO: Do something if there are more than 10 items in the list to prevent bufferoverflows

        }
        
        


        // Set Crash Bool to true


        // Set Crash bool to false



        public static async Task<bool> CheckIfFileIsAutoRecoveryFile(StorageFile file)
        {
            var parentFolder = await file.GetParentAsync();
            if (parentFolder != tempFolder)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public static async void DeleteAutoRecoveryFile(StorageFile file)
        {
            if (await CheckIfFileIsAutoRecoveryFile(file))
            {
                Debug.WriteLine("AutoRecoveryService - AutoRecoveryCleanup - File is Auto Recovery File. Deleting Temp Storage File of it now");
                await file.DeleteAsync();
            }
            else
            {
                Debug.WriteLine("FileDataService - AutoRecoveryCleanup - File is normal. Keep moving citizen :) ");
            }
        }

        // Clear the Temporary Folder
        public static async void ClearAllAutoRecoveryFiles()
        {
            var autoRecoveryFiles = await tempFolder.GetFilesAsync();
            foreach (var item in autoRecoveryFiles)
            {
                await item.DeleteAsync();
            }
        }
    }
}
