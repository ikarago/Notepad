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
        public static async Task<LoadDataModel> Load()
        {
            LoadDataModel model = null;
            TextDataModel data = null;

            model = new LoadDataModel();
            model.LoadSuccessful = false;

            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.ViewMode = PickerViewMode.List;
            picker.FileTypeFilter.Add("*");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                try
                {
                    data = new TextDataModel();

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

                    // Get Fast Access token
                    GetFaToken(file);
                    // #TODO: Store this token somewhere


                    model.TextModel = data;
                    model.File = file;
                    model.LoadSuccessful = true;

                    Debug.WriteLine("File " + file.Name + " has been loaded");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Loading failed");
                    Debug.WriteLine(ex);
                    throw;
                }
            }         

            return model;
        }

        // Load without prompt (for when loading from Explorer)
        public static async Task<LoadDataModel> LoadWithoutPrompt(StorageFile file)
        {
            LoadDataModel model = null;
            TextDataModel data = null;

            model = new LoadDataModel();
            model.LoadSuccessful = false;

            if (file != null)
            {
                try
                {
                    data = new TextDataModel();

                    // Textdata
                    data.Text = await FileIO.ReadTextAsync(file);
                    data.DocumentTitle = file.DisplayName + file.FileType;

                    // Get Fast Access token
                    GetFaToken(file);
                    // #TODO: Store this token somewhere


                    model.File = file;
                    model.TextModel = data;
                    model.LoadSuccessful = true;

                    Debug.WriteLine("File " + file.Name + " has been loaded");
                }
                catch
                {
                    Debug.WriteLine("Loading failed");
                }
            }

            return model;
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
