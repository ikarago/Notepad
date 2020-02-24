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
    public class AutoRecoveryService
    {
        // Properties
        static readonly StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;

        public StorageFile AutoRecoveryFile { get; set; }
        public StorageFile CurrentFile { get; set; }
        public TextDataModel Data { get; set; }
        DateTimeOffset LastSavedTempFile { get; set; }


        // Constructor
        public AutoRecoveryService()
        {
            Initialize();
        }

        // Intitialize
        private async void Initialize()
        {
            Data = new TextDataModel();
            await InitializeForNewDocument();
        }


        /// <summary>
        /// Sets the AutoRecoveryService with the loaded data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="file"></param>
        //public void PrepareAutoRecoveryServiceWithData(TextDataModel data, StorageFile file = null)
        //{
        //    Data = data;
        //    CurrentFile = file;
        //}

        public async Task<bool> InitializeForNewDocument(TextDataModel data = null)
        {
            Debug.WriteLine("FileDataService - InitializeForNewDocument - START");
            // Delete the previously loaded AutoRecoveryFile if nessesary
            if (AutoRecoveryFile != null)
            {
                Debug.WriteLine("FileDataService - InitializeForNewDocument - Deleting old AutoRecovery File");
                await DeleteAutoRecoveryFile();
            }
            // Create a new AutoRecoveryFile for the File
            if (AutoRecoveryFile == null)
            {
                Debug.WriteLine("FileDataService - InitializeForNewDocument - Creating a new AutoRecovery File");
                // #TODO: Catch this in case somethis is crazy wrong like no write access to the Temp Folder
                AutoRecoveryFile = await tempFolder.CreateFileAsync("AutoRecoveryFile.txt", CreationCollisionOption.GenerateUniqueName);
            }
            // Set Data from the given parameter
            if (data != null)
            {
                Debug.WriteLine("FileDataService - InitializeForNewDocument - Setting Data");
                Data = data;
            }
            return true;
        }


        // Save to Temp File
        /// <summary>
        /// Saves the data in an AutoRecovery File in case the app crashes
        /// </summary>
        /// <param name="data">TextDataModel containing all the info</param>
        /// <returns>Returns the StorageFile with the location for later reference</returns>
        public async Task<StorageFile> SaveAutoRecoveryFile()
        {
            if (AutoRecoveryFile != null)
            {
                bool success = await FileDataService.Save(Data, AutoRecoveryFile);
                Debug.WriteLine("FileDataService - SaveAutoRecoveryFile - Success = " + success);
                return AutoRecoveryFile;
            }
            else
            {
                Debug.WriteLine("FileDataService - SaveAutoRecoveryFile - Something has gone horribly, horribly wrong");
                return null;
            }
        }


        // Load Temp file
        public async void LoadAutoRecoveryFiles()
        {
            // Get a list of AutoRecoveryFiles and open all of them via URI Activation
            var autoRecoveryFiles = await tempFolder.GetFilesAsync();
            foreach (var item in autoRecoveryFiles)
            {
                var uri = new Uri("notepad-uwp:" + item.Path);
                var success = await Windows.System.Launcher.LaunchUriAsync(uri);
                // Put a pause here?
            }

            await Task.Delay(1000);

            foreach (var item in autoRecoveryFiles)
            {
                await item.DeleteAsync();
            }

            // #TODO: Do something if there are more than 10 items in the list to prevent bufferoverflows

        }
        
        


        // Set Crash Bool to true


        // Set Crash bool to false



        public async Task<bool> CheckIfFileIsAutoRecoveryFile(StorageFile file)
        {
            var parentFolder = await file.GetParentAsync();
            if (parentFolder == tempFolder)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<bool> DeleteAutoRecoveryFile()
        {
            if (AutoRecoveryFile != null)
            {
                Debug.WriteLine("AutoRecoveryService - DeleteAutoRecoveryFile INTERNAL - Deleting Temp Storage File now");
                await AutoRecoveryFile.DeleteAsync();
                AutoRecoveryFile = null;
                return true;
            }
            else
            {
                Debug.WriteLine("FileDataService - DeleteAutoRecoveryFile INTERNAL - File is normal. Keep moving citizen :) ");
                return false;
            }
        }

        public async Task<bool> DeleteAutoRecoveryFile(StorageFile file)
        {
            if (await CheckIfFileIsAutoRecoveryFile(file))
            {
                Debug.WriteLine("AutoRecoveryService - DeleteAutoRecoveryFile - File is Auto Recovery File. Deleting Temp Storage File of it now");
                await file.DeleteAsync();
                return true;
            }
            else
            {
                Debug.WriteLine("FileDataService - DeleteAutoRecoveryFile - File is normal. Keep moving citizen :) ");
                return false;
            }
        }

        // Clear the Temporary Folder
        // #TODO: Improve this
        public async void ClearAllAutoRecoveryFiles()
        {
            var autoRecoveryFiles = await tempFolder.GetFilesAsync();
            foreach (var item in autoRecoveryFiles)
            {
                await item.DeleteAsync();
            }
            AutoRecoveryFile = null;
            await InitializeForNewDocument();
        }
    }
}
