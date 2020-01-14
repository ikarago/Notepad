using NotepadRs4.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;

namespace NotepadRs4.Services
{
    public static class FileDataService
    {
        // Save
        public static async Task<bool> Save(TextDataModel data, StorageFile file)
        {
            // #TODO Build this method
            if (file != null)
            {
                try
                {
                    // Prevent remote access to file until saving is done
                    CachedFileManager.DeferUpdates(file);
                    // Write the stuff to the file
                    await FileIO.WriteTextAsync(file, data.Text);

                    // Let Windows know stuff is done
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    return true;
                }
                catch { return false; }

            }
            return false;
        }

        // Save As
        public static async Task<StorageFile> SaveAs(TextDataModel data)
        {
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("Text Documents", new List<string>() { ".txt" });
            picker.FileTypeChoices.Add("All files", new List<string>() { "." });
            picker.DefaultFileExtension = ".txt";
            if (data.DocumentTitle != "")
            {
                picker.SuggestedFileName = data.DocumentTitle;
                // TODO: Get original file here as well
            }
            else
            {
                picker.SuggestedFileName = "Untitled";
            }
            

            StorageFile file = await picker.PickSaveFileAsync();

            if (file != null)
            {
                // Prevent remote access to file until saving is done
                CachedFileManager.DeferUpdates(file);

                // Write the stuff to the file
                await FileIO.WriteTextAsync(file, data.Text);

                // Get Fast Access token
                GetFaToken(file);
                // #TODO: Store this token somewhere

                // Let Windows know stuff is done
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                // DEBUG: Let programmer know what has happened
                if (status == FileUpdateStatus.Complete)
                {
                    Debug.WriteLine("File " + file.Name + " has been saved");
                }
                else
                {
                    Debug.WriteLine("File " + file.Name + " has NOT been saved");
                }
                
            }


            return file;

        }

        // Load
        public static async Task<TextDataModel> Load(StorageFile file = null)
        {
            Debug.WriteLine("FileDataService - Load - Loading File... START!");

            // If no file has been entered, try to get one from the FilePicker
            if (file == null)
            {
                Debug.WriteLine("FileDataService - Load - Opening File Picker...");
                file = await GetStorageFileFromFilePickerAsync();
            }

            if (file != null)
            {
                try
                {
                    TextDataModel dataModel = await GetTextDataModel(file);
                    Debug.WriteLine("FileDataService - Load - Loading File... SUCCESS!");
                    return dataModel;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("FileDataService - Load - Loading File... FAILED! Error:");
                    Debug.WriteLine(ex);
                    return null;
                }
            }
            else
            {
                Debug.WriteLine("FileDataService - Load - Loading File... FAILED! Error:");
                Debug.WriteLine("No readable StorageFile entered :(");
                return null;
            }
        }

        /// <summary>
        /// Gets a StorageFile from the File Picker
        /// </summary>
        /// <returns>StorageFile with the data</returns>
        private static async Task<StorageFile> GetStorageFileFromFilePickerAsync()
        {
            Debug.WriteLine("FileDataService - GetStorageFileFromFilePickerAsync - Picking File... START!");
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.ViewMode = PickerViewMode.List;
            picker.FileTypeFilter.Add("*");

            try
            {
                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    Debug.WriteLine("FileDataService - GetStorageFileFromFilePickerAsync - Picking File... SUCCESS!");
                    return file;
                }
                else 
                {
                    Debug.WriteLine("FilePicker cancelled by user");
                    throw new Exception();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("FileDataService - GetStorageFileFromFilePickerAsync - Picking File... FAILED! Error:");
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Loads the data in the StorageFile
        /// </summary>
        /// <param name="file">StorageFile containing the info that needs to be loaded</param>
        /// <returns>Returns loaded TextDataModel containing all info. Will return null if loading has failed</returns>
        private static async Task<TextDataModel> GetTextDataModel(StorageFile file)
        {
            Debug.WriteLine("FileDataService - GetTextDataModel - Loading File... START!");

            TextDataModel data = null;

            try
            {
                data = new TextDataModel();

                // #TODO: Abstract this further with an seperate LoadingLogic method ---> How to set Exceptions for it?
                // Get the buffer of the file so we can also read it when encoded in different encoding from UTF8
                // Fix with the help of Fred Bao's answer on https://social.msdn.microsoft.com/Forums/sqlserver/en-US/0f3cd056-a2e3-411b-8e8a-d2109255359a/uwpc-reading-ansi-text-file
                IBuffer buffer = await FileIO.ReadBufferAsync(file);
                DataReader dataReader = DataReader.FromBuffer(buffer);
                byte[] fileContent = new byte[dataReader.UnconsumedBufferLength];
                dataReader.ReadBytes(fileContent);
                string readText = Encoding.UTF8.GetString(fileContent, 0, fileContent.Length);
                // #TODO: Get the current encoding and display it in the TextDataModel


                // Textdata
                data.Text = readText;
                data.DocumentTitle = file.DisplayName + file.FileType;
                data.DataFile = file;

                // Set the Fast Access Token
                SetFaToken(file);                 

                Debug.WriteLine("FileDataService - GetTextDataModel - Loading File... SUCCESS!");
                Debug.WriteLine("File " + file.Name + " has been loaded");
            }
            catch (Exception ex)
            {
                data = null;
                Debug.WriteLine("FileDataService - GetTextDataModel - Loading File... FAILED! Error:");
                Debug.WriteLine(ex);
            }

            return data;
        }

        private static void SetFaToken(StorageFile file)
        {
            // Get Fast Access token
            GetFaToken(file);
            // #TODO: Store this token somewhere
        }

        private static string GetFaToken(StorageFile file)
        {
            // Check if the maximum amount of Fast Access List items has been met
            if (Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Entries.Count >= Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.MaximumItemsAllowed)
            {
                // If so, clear the whole list (should only happen rarely)
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Clear();
            }

            // Get the Fast Access List Token
            string faToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);

            return faToken;
        }
    }
}
