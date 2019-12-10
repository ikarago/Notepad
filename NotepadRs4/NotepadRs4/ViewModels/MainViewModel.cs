using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using NotepadRs4.Helpers;
using NotepadRs4.Models;
using NotepadRs4.Services;
using NotepadRs4.Views.Dialogs;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NotepadRs4.ViewModels
{
    public class MainViewModel : NotificationBase
    {
        // Properties
        private TextDataModel _data;
        public TextDataModel Data
        {
            get { return _data; }
            set { SetProperty(ref _data, value); }
        }

        // Previous Data is kept temporarily for comparing the unaltered data with the current state.
        private TextDataModel _previousData;
        public TextDataModel PreviousData
        {
            get { return _previousData; }
            set { SetProperty(ref _previousData, value); }
        }

        private StorageFile _file;
        public StorageFile File
        {
            get { return _file; }
            set { SetProperty(ref _file, value); }
        }

        private int _line;
        public int Line
        {
            get { return _line; }
            set { SetProperty(ref _line, value); }
        }

        private int _col;
        public int Col
        {
            get { return _col; }
            set { SetProperty(ref _col, value); }
        }

        private bool _fileEdited;
        public bool FileEdited
        {
            get { return _fileEdited; }
            set { SetProperty(ref _fileEdited, value); }
        }

        // List for Share Data
        private IReadOnlyList<StorageFile> FilesToShare;



        // Capability buttons
        // Share Button
        private Visibility _uiShareButtonVisibility;
        public Visibility UiShareButtonVisibility
        {
            get { return _uiShareButtonVisibility; }
            set { SetProperty(ref _uiShareButtonVisibility, value); }
        }

        // Print Button
        private Visibility _uiPrintButtonVisibility;
        public Visibility UiPrintButtonVisibility
        {
            get { return _uiPrintButtonVisibility; }
            set { SetProperty(ref _uiPrintButtonVisibility, value); }
        }


        // UI Notification triggers
        // Save Successful
        private Visibility _uxNotificationSaveSuccessful;
        public Visibility UXNotificationSaveSuccessful
        {
            get { return _uxNotificationSaveSuccessful; }
            set { SetProperty(ref _uxNotificationSaveSuccessful, value); }
        }
        // Save Failed
        private Visibility _uxNotificationSaveFailed;
        public Visibility UXNotificationSaveFailed
        {
            get { return _uxNotificationSaveFailed; }
            set { SetProperty(ref _uxNotificationSaveFailed, value); }
        }
        // Load Successful
        private Visibility _uxNotificationLoadSuccessful;
        public Visibility UXNotificationLoadSuccessful
        {
            get { return _uxNotificationLoadSuccessful; }
            set { SetProperty(ref _uxNotificationLoadSuccessful, value); }
        }
        // Load Failed
        private Visibility _uxNotificationLoadFailed;
        public Visibility UXNotificationLoadFailed
        {
            get { return _uxNotificationLoadFailed; }
            set { SetProperty(ref _uxNotificationLoadFailed, value); }
        }



        // Main
        public MainViewModel()
        {
            Initialize();
        }

        /// <summary>
        /// Initilize the ViewModel
        /// </summary>
        /// <param name="file">File that needs to be loaded. Leave empty for a new document</param>
        public async void Initialize(StorageFile file = null)
        {
            if (file != null)
            {
                LoadDataModel model = await FileDataService.LoadWithoutPrompt(file);
                if (model.LoadSuccessful == true)
                {
                    Data = model.TextModel;
                    PreviousData = model.TextModel;
                    File = model.File;
                    RefreshTitlebarTitle();
                }
                else // If loading fails fall back by preparing a new, empty document instead
                {
                    PrepareNewDocument();
                }
            }
            else if (_data == null)
            {
                PrepareNewDocument();
            }

            // Set UI and UX stuff
            SetEditedFalse();
            CheckDeviceCapabilities();
            SetUXToggles();
        }


        // Commands
        private ICommand _newFileCommand;
        public ICommand NewFileCommand
        {
            get
            {
                if (_newFileCommand == null)
                {
                    _newFileCommand = new RelayCommand(
                        () =>
                        {
                            NewFile();
                        });
                }
                return _newFileCommand;
            }
        }

        private ICommand _saveFileCommand;
        public ICommand SaveFileCommand
        {
            get
            {
                if (_saveFileCommand == null)
                {
                    _saveFileCommand = new RelayCommand(
                        async () =>
                        {
                            await SaveFile();
                        });
                }
                return _saveFileCommand;
            }
        }

        private ICommand _saveFileAsCommand;
        public ICommand SaveFileAsCommand
        {
            get
            {
                if (_saveFileAsCommand == null)
                {
                    _saveFileAsCommand = new RelayCommand(
                        async () =>
                        {
                            await SaveFileAs();
                        });
                }
                return _saveFileAsCommand;
            }
        }

        private ICommand _loadFileCommand;
        public ICommand LoadFileCommand
        {
            get
            {
                if (_loadFileCommand == null)
                {
                    _loadFileCommand = new RelayCommand(
                        async () =>
                        {
                            await LoadFile();
                        });
                }
                return _loadFileCommand;
            }
        }

        private ICommand _printCommand;
        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new RelayCommand(
                        async () =>
                        {
                            // #TODO
                        });
                }
                return _printCommand;
            }
        }

        private ICommand _shareCommand;
        public ICommand ShareCommand
        {
            get
            {
                if (_shareCommand == null)
                {
                    _shareCommand = new RelayCommand(
                        () =>
                        {
                            ShareDocument();
                        });
                }
                return _shareCommand;
            }
        }

        private ICommand _playgroundCommand;
        public ICommand PlaygroundCommand
        {
            get
            {
                if (_playgroundCommand == null)
                {
                    _playgroundCommand = new RelayCommand(
                        () =>
                        {
                            GoToPlayground();
                        });
                }
                return _playgroundCommand;
            }
        }

        private ICommand _settingsCommand;
        public ICommand SettingsCommand
        {
            get
            {
                if (_settingsCommand == null)
                {
                    _settingsCommand = new RelayCommand(
                        () =>
                        {
                            ShowSettingsDialog();
                        });
                }
                return _settingsCommand;
            }
        }

        private ICommand _aboutCommand;
        public ICommand AboutCommand
        {
            get
            {
                if (_aboutCommand == null)
                {
                    _aboutCommand = new RelayCommand(
                        () =>
                        {
                            ShowAboutDialog();
                        });
                }
                return _aboutCommand;
            }
        }



        // Methods
        // New
        public async void NewFile()
        {
            if (_data.Text != "")
            {
                // Show dialog
                var answer = await SaveBeforeClosing();
                // Result Save
                if (answer == UnsavedDialogResult.Save)
                {
                    // Save & then open a new file
                    Debug.WriteLine("New File: Save & open new file");

                    // TODO: Change between Save and SaveAs methods when it has been saved before

                    bool saveSuccessful = await SaveFileAs();
                    if (saveSuccessful == true)
                    {
                        // Show Save Successful Notification
                        Debug.WriteLine("New File: Saving Successful");
                        ShowUXMessage(1);
                        // Set up the new empty document
                        PrepareNewDocument();
                    }
                    else
                    {
                        // Show Save Failed Notification
                        Debug.WriteLine("New File: Saving Failed");
                        ShowUXMessage(2);
                    }
                }
                // Result Discard
                if (answer == UnsavedDialogResult.Discard)
                {
                    // Discard changes and open a new file
                    Debug.WriteLine("New File: Discard changes");
                    PrepareNewDocument();
                }
                // Result Cancelled
                else
                {
                    // Close the dialog and do nothing
                    Debug.WriteLine("New File: Dialog cancelled");
                }

                // or
                // #TODO: Create a new window with an empty file if a file is currently open
            }
        }

        // Save
        public async Task<bool> SaveFile()
        {
            if (File == null)
            {
                return await SaveFileAs();
            }
            else
            {
                bool success = await FileDataService.Save(Data, File);

                if (success == true)
                {
                    // Show Save Successful Notification
                    Debug.WriteLine("Save File: Saving Successful");
                    ShowUXMessage(1);
                    SetEditedFalse();
                    return true;
                }
                else
                {
                    // Show Save Failed Notification
                    Debug.WriteLine("Save File: Saving Failed");
                    ShowUXMessage(2);
                    return false;
                }
            }
        }

        // Save As
        public async Task<bool> SaveFileAs()
        {
            // #TODO: Check whether the user cancelled the action or it has actually failed
            StorageFile tempFile = await FileDataService.SaveAs(_data);

            if (tempFile != null)
            {
                File = tempFile;
                // Create a temp TextDataModel to make the changes in
                TextDataModel data = new TextDataModel();
                data.Text = Data.Text;
                data.DocumentTitle = File.DisplayName + File.FileType;
                // Write the changes back to the Data property since it doesn't register single changed items otherwise
                Data = data;
                PreviousData = data;
                RefreshTitlebarTitle();

                // Show Save Successful Notification
                Debug.WriteLine("Save File As File: Saving Successful");
                ShowUXMessage(1);
                SetEditedFalse();

                return true;
            }
            else
            {
                // Show Save Failed Notification
                Debug.WriteLine("Save File As: Saving Failed");
                ShowUXMessage(2);
                return false;
            }
        }

        // Load
        public async Task<bool> LoadFile()
        {
            if (_data.Text != "" && FileEdited == true)
            {
                // Show dialog
                var answer = await SaveBeforeClosing();
                // Save result
                if (answer == UnsavedDialogResult.Save)
                {
                    // Save & then open load file
                    Debug.WriteLine("Load File: Save & open new file");
                    // TODO: Change between Save and SaveAs methods when it has been saved before

                    bool saveSuccessful = await SaveFileAs();
                    if (saveSuccessful == true)
                    {
                        // Show Save Successful Notification
                        Debug.WriteLine("Load File: Saving Successful");
                        ShowUXMessage(1);
                        // Load the other document
                        LoadDocument();
                        return true;
                    }
                    else
                    {
                        // Show Save Failed Notification
                        Debug.WriteLine("Load File: Saving Failed");
                        ShowUXMessage(2);
                        return false;
                    }
                }
                // Discard Result
                if (answer == UnsavedDialogResult.Discard)
                {
                    // Discard changes and open a new file
                    Debug.WriteLine("Load File: Discard changes");
                    LoadDocument();
                    return true;
                }
                // Cancel result
                else
                {
                    // Close the dialog and do nothing
                    Debug.WriteLine("Load File: Dialog cancelled");
                    return false;
                }
            }
            // If no changes have been made in the first place, just load the document
            else
            {
                LoadDocument();
                return true;
            }
        }

        // Drop Text
        public async Task<bool> DropText(DragEventArgs e)
        {
            bool success = false;
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                string tempText = await e.DataView.GetTextAsync();

                // Create a temporary datamodel to swap out so the ViewModel can track the change
                TextDataModel textDataModel = new TextDataModel();
                textDataModel.DocumentTitle = _data.DocumentTitle;
                textDataModel.Text = (_data.Text + '\n' + '\n' + tempText);

                Data = textDataModel;
                success = true;
            }

            return success;
        }


        // Preperation Methods
        /// <summary>
        /// Sets up a new document for use in this ViewModel
        /// </summary>
        private void PrepareNewDocument()
        {
            TextDataModel emptyData = new TextDataModel();
            emptyData.DocumentTitle = ResourceExtensions.GetLocalized("UnititledLabel");
            Data = emptyData;
            File = null;
            SetEditedFalse();
            RefreshTitlebarTitle();
        }

        /// <summary>
        /// Loads a document for use in this ViewModel
        /// </summary>
        private async void LoadDocument()
        {
            // Just load
            LoadDataModel model = await FileDataService.Load();
            //TextDataModel data = await FileDataService.Load();
            if (model.LoadSuccessful == true)
            {
                Data = model.TextModel;
                PreviousData = model.TextModel;
                File = model.File;
                RefreshTitlebarTitle();
                SetEditedFalse();
                ShowUXMessage(3);
                //return true;
            }
            else
            {
                Debug.WriteLine("Load File: Dialog cancelled");
                //return false;
            }
        }

        /// <summary>
        /// Shares the current opened document via the Share capabilities in Windows
        /// </summary>
        private async void ShareDocument()
        {
            // Get the data from the ReadOnlyList
            await GetShareStorageFiles();
            // #TODO Show/hide the Share button depending on whether the device supports sharing
            DataTransferManager.GetForCurrentView().DataRequested += MainViewModel_ShareDataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void MainViewModel_ShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            // Show the Share UI
            DataRequest request = args.Request;

            // Set the metadata
            request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;
            request.Data.Properties.Description = (ResourceExtensions.GetLocalized("ShareDescription") + " " + FilesToShare[0].Name);

            // Set the StorageItem
            request.Data.SetStorageItems(FilesToShare);
        }

        /// <summary>
        /// Prepare the temp files to be shared in a ReadOnlyList
        /// </summary>
        private async Task<bool> GetShareStorageFiles()
        {
            // Save the current document to a temp folder
            ApplicationData appData = ApplicationData.Current;
            StorageFile tempFile;

            // If the file being shared already has been saved use that name instead
            // #TODO: Instead, save the current file as well?
            if (File != null)
            {
                tempFile = await appData.TemporaryFolder.CreateFileAsync(File.Name, CreationCollisionOption.ReplaceExisting);
            }
            else
            {
                tempFile = await appData.TemporaryFolder.CreateFileAsync((ResourceExtensions.GetLocalized("UnititledLabel") + ".txt"), CreationCollisionOption.ReplaceExisting);
            }

            // Write the data to the temp file
            // Prevent remote access to file until saving is done
            CachedFileManager.DeferUpdates(tempFile);
            //We need to make sure Data.Text isn't null.
            Data.Text = Data.Text ?? ""; //coalescing operator '??' returns left-hand operand's value if it isn't null, else right-hand's
            // Write the stuff to the file
            await FileIO.WriteTextAsync(tempFile, Data.Text);
            // Let Windows know stuff is done
            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(tempFile);

            // Create a List and set add the files to it
            List<StorageFile> filesToShare = new List<StorageFile>();
            filesToShare.Add(tempFile);
            // Set the ReadOnlyList with the new data
            FilesToShare = filesToShare;

            // Preperation is done, continue with the other stuff
            return true;
        }

        /// <summary>
        /// Sets up the UX Toggles to their default state
        /// </summary>
        private void SetUXToggles()
        {
            UXNotificationSaveSuccessful = Visibility.Collapsed;
            UXNotificationSaveFailed = Visibility.Collapsed;
            UXNotificationLoadSuccessful = Visibility.Collapsed;
            UXNotificationLoadFailed = Visibility.Collapsed;
        }

        /// <summary>
        /// Checks whether the device running is capable of using certain capabilities and if not hides them
        /// </summary>
        private void CheckDeviceCapabilities()
        {
            // Check for the Share Button
            if (DataTransferManager.IsSupported()) { UiShareButtonVisibility = Visibility.Visible; }
            else { UiShareButtonVisibility = Visibility.Collapsed; }

            // #TODO Check for the Print Button
        }

        /// <summary>
        /// Sets the title of the app
        /// </summary>
        private void RefreshTitlebarTitle()
        {
            ApplicationView.GetForCurrentView().Title = Data.DocumentTitle;
        }


        // Set save status
        public void SetEditedFalse()
        {
            FileEdited = false;
            App.UnsavedChanges = false;
        }
        public void SetEditedTrue()
        {
            FileEdited = true;
            App.UnsavedChanges = true;
        }

        // Navigation and dialogs
        /// <summary>
        /// Navigates to the Playground Page
        /// </summary>
        private void GoToPlayground()
        {
            NavigationService.Navigate(typeof(Views.XamlPlayground));
        }

        /// <summary>
        /// Opens the Settings Dialog
        /// </summary>
        private async void ShowSettingsDialog()
        {
            var dialog = new SettingsDialog();
            await dialog.ShowAsync();
        }

        /// <summary>
        /// Opens the About Dialog
        /// </summary>
        private async void ShowAboutDialog()
        {
            var dialog = new AboutDialog();
            await dialog.ShowAsync();
        }

        /// <summary>
        /// Display a Save before Closing dialog
        /// </summary>
        /// <returns>Returns UnsavedDialogResult</returns>
        private async Task<UnsavedDialogResult> SaveBeforeClosing()
        {
            UnsavedDialog dialog = new UnsavedDialog();
            await dialog.ShowAsync();
            var answer = dialog.Result;
            return answer;
        }

        /// <summary>
        /// Toggle the Visibility-state of a UX message in the View
        /// </summary>
        /// <param name="message">
        /// 1 = Save Successful,
        /// 2 = Save Failed,
        /// 3 = Load Successful,
        /// 4 = Load Failed</param>
        private async void ShowUXMessage(int message)
        {
            if (message == 1)
            {
                UXNotificationSaveSuccessful = Visibility.Visible;
                await Task.Delay(TimeSpan.FromSeconds(3));
                UXNotificationSaveSuccessful = Visibility.Collapsed;
            }
            else if (message == 2)
            {
                UXNotificationSaveFailed = Visibility.Visible;
                await Task.Delay(TimeSpan.FromSeconds(3));
                UXNotificationSaveFailed = Visibility.Collapsed;
            }
            else if (message == 3)
            {
                UXNotificationLoadSuccessful = Visibility.Visible;
                await Task.Delay(TimeSpan.FromSeconds(3));
                UXNotificationLoadSuccessful = Visibility.Collapsed;
            }
            else if (message == 4)
            {
                UXNotificationLoadFailed = Visibility.Visible;
                await Task.Delay(TimeSpan.FromSeconds(3));
                UXNotificationLoadFailed = Visibility.Collapsed;
            }
        }
    }
}
