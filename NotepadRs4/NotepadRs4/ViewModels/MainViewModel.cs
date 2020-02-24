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

        private float _zoomFactor;
        public float ZoomFactor
        {
            get { return _zoomFactor; }
            set 
            { 
                SetProperty(ref _zoomFactor, value);
                UpdateZoomFactorPercentage();
            }
        }

        private string _selectedText;
        public string SelectedText
        {
            get { return _selectedText; }
            set { SetProperty(ref _selectedText, value); }
        }

        private int _zoomFactorPercentage;
        public int ZoomFactorPercentage
        {
            get { return _zoomFactorPercentage; }
            set { SetProperty(ref _zoomFactorPercentage, value); }
        }
        
        private bool _fileEdited;
        public bool FileEdited
        {
            get { return _fileEdited; }
            set { SetProperty(ref _fileEdited, value); }
        }

        // List for Share Data
        private IReadOnlyList<StorageFile> FilesToShare;

        private bool _isInAlwaysOnTopMode;


        /// <summary>
        /// UI Triggers
        /// </summary>
        private bool _uiTitlebarDetailsVisibility;
        public bool UITitlebarDetailsVisibility
        {
            get { return _uiTitlebarDetailsVisibility; }
            set { SetProperty(ref _uiTitlebarDetailsVisibility, value); }
        }

        private AppBarClosedDisplayMode _uiAppBarDisplayMode;
        public AppBarClosedDisplayMode UIAppBarDisplayMode
        {
            get { return _uiAppBarDisplayMode; }
            set { SetProperty(ref _uiAppBarDisplayMode, value); }
        }

        private bool _uiZoomFactorVisibility;
        public bool UIZoomFactorVisibility
        {
            get { return _uiZoomFactorVisibility; }
            set { SetProperty(ref _uiZoomFactorVisibility, value); }
        }

        // Capability buttons
        // Share Button
        private bool _uiShareButtonVisibility;
        public bool UIShareButtonVisibility
        {
            get { return _uiShareButtonVisibility; }
            set { SetProperty(ref _uiShareButtonVisibility, value); }
        }

        // Print Button
        private bool _uiPrintButtonVisibility;
        public bool UIPrintButtonVisibility
        {
            get { return _uiPrintButtonVisibility; }
            set { SetProperty(ref _uiPrintButtonVisibility, value); }
        }

        // Always on Top
        private bool _uiAlwaysOnTopButtonVisibility;
        public bool UIAlwaysOnTopButtonVisibility
        {
            get { return _uiAlwaysOnTopButtonVisibility; }
            set { SetProperty(ref _uiAlwaysOnTopButtonVisibility, value); }
        }
        private bool _uiCloseAlwaysOnTopButtonVisibility;
        public bool UICloseAlwaysOnTopButtonVisibility
        {
            get { return _uiCloseAlwaysOnTopButtonVisibility; }
            set { SetProperty(ref _uiCloseAlwaysOnTopButtonVisibility, value); }
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



        // Constructor
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
                bool success = await LoadDocument(file);
                if (!success)
                {
                    await PrepareNewDocument();
                }
            }
            else if (_data == null)
            {
                await PrepareNewDocument();
            }

            // Set UI and UX stuff
            _selectedText = "";
            ZoomFactor = 1;
            //UIZoomFactorVisibility = true;  // Shown on purpose. Gets hidden in 3 seconds after launch
            _isInAlwaysOnTopMode = false;
            UIAppBarDisplayMode = AppBarClosedDisplayMode.Compact;
            UITitlebarDetailsVisibility = true;
            SetEditedFalse();
            CheckDeviceCapabilities();
            SetUXToggles();
            UpdateZoomFactorPercentage();
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
                            Share();
                        });
                }
                return _shareCommand;
            }
        }

        private ICommand _toggleAlwaysOnTopCommand;
        public ICommand ToggleAlwaysOnTopCommand
        {
            get
            {
                if (_toggleAlwaysOnTopCommand == null)
                {
                    _toggleAlwaysOnTopCommand = new RelayCommand(
                        () =>
                        {
                            ToggleAlwaysOnTopMode();
                        });
                }
                return _toggleAlwaysOnTopCommand;
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

        private ICommand _searchOnlineCommand;
        public ICommand SearchOnlineCommand
        {
            get
            {
                if (_searchOnlineCommand == null)
                {
                    _searchOnlineCommand = new RelayCommand(
                        () =>
                        {
                            OnlineSearchService.SearchOnline(SelectedText);
                        });
                }
                return _searchOnlineCommand;
            }
        }

        private ICommand _searchWithBingCommand;
        public ICommand SearchWithBingCommand
        {
            get
            {
                if (_searchWithBingCommand == null)
                {
                    _searchWithBingCommand = new RelayCommand(
                        () =>
                        {
                            OnlineSearchService.SearchOnline(SelectedText, OnlineSearchProvider.Bing);
                        });
                }
                return _searchWithBingCommand;
            }
        }

        private ICommand _autoRecoveryCommand;
        public ICommand AutoRecoveryCommand
        {
            get
            {
                if (_autoRecoveryCommand == null)
                {
                    _autoRecoveryCommand = new RelayCommand(
                        () =>
                        {
                            App.RecoveryService.LoadAutoRecoveryFiles();
                        });
                }
                return _autoRecoveryCommand;
            }
        }

        private ICommand _clearAutoRecoveryFilesCommand;
        public ICommand ClearAutoRecoveryFilesCommand
        {
            get
            {
                if (_clearAutoRecoveryFilesCommand == null)
                {
                    _clearAutoRecoveryFilesCommand = new RelayCommand(
                        () =>
                        {
                            App.RecoveryService.ClearAllAutoRecoveryFiles();
                        });               
                }
                return _clearAutoRecoveryFilesCommand;
            }
        }



        // Methods
        // New
        private async void NewFile()
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
                        await PrepareNewDocument();
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
                    await PrepareNewDocument();
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
        private async Task<bool> SaveFile()
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
        private async Task<bool> SaveFileAs()
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
                App.RecoveryService.CurrentFile = File;
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





        private async Task<bool> LoadFile()
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
                        await LoadDocument();
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
                    await LoadDocument();
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
                await LoadDocument();
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
        private async Task<bool> PrepareNewDocument()
        {
            TextDataModel emptyData = new TextDataModel();
            emptyData.DocumentTitle = ResourceExtensions.GetLocalized("UnititledLabel");
            Data = emptyData;
            File = null;
            await App.RecoveryService.InitializeForNewDocument(Data);
            SetEditedFalse();
            RefreshTitlebarTitle();
            return true;
        }


        //private async void LoadDocument()
        //{
        //    // Just load
        //    TextDataModel model = await FileDataService.Load();
        //    //TextDataModel data = await FileDataService.Load();
        //    if (model != null)
        //    {
        //        Data = model;
        //        PreviousData = model;
        //        File = model.DataFile;
        //        App.RecoveryService.CurrentFile = File;
        //        RefreshTitlebarTitle();
        //        SetEditedFalse();
        //        ShowUXMessage(3);
        //        //return true;
        //    }
        //    else
        //    {
        //        Debug.WriteLine("Load File: Dialog cancelled");
        //        //return false;
        //    }
        //}

        /// <summary>
        /// Loads a document for use in this ViewModel
        /// </summary>
        /// <param name="file">StorageFile that needs to be loaded (leave empty to invoke Open File Dialog)</param>
        /// <returns>Bools indicating a successful load operation</returns>
        private async Task<bool> LoadDocument(StorageFile file = null)
        {
            TextDataModel model = await FileDataService.Load(file);

            if (model != null)
            {
                Data = model;
                PreviousData = model;
                RefreshTitlebarTitle();

                if (file != null && await App.RecoveryService.CheckIfFileIsAutoRecoveryFile(file))
                {
                    Debug.WriteLine("MainViewModel - LoadDocument - Loaded file is AutoRecovery file");
                    // Not setting the StorageFile here to prevent the user from accidentily save back to the apps Temp Folder
                    RefreshTitlebarTitle();
                    await App.RecoveryService.DeleteAutoRecoveryFile(model.DataFile);
                    await App.RecoveryService.InitializeForNewDocument();

                    SetEditedTrue();
                }
                else
                {
                    Debug.WriteLine("MainViewModel - LoadDocument - Loaded file is normal file");
                    File = model.DataFile;
                    await App.RecoveryService.InitializeForNewDocument(model);
                    App.RecoveryService.CurrentFile = model.DataFile;

                    ShowUXMessage(3);
                    SetEditedFalse();
                }

                return true;
            }
            else
            {
                Debug.WriteLine("Load File: Dialog cancelled");
                return false;
            }
        }

        private void Share()
        {
            ShareService shareService = new ShareService();
            if (SelectedText != "")
            {
                shareService.Share(SelectedText);
            }
            else { shareService.Share(Data); }
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
            if (DataTransferManager.IsSupported()) { UIShareButtonVisibility = true; }
            else { UIShareButtonVisibility = false; }

            // #TODO Check for the Print Button

            // Check for Always on Top capability
            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay)) 
            { 
                UIAlwaysOnTopButtonVisibility = true;
                UICloseAlwaysOnTopButtonVisibility = false;
            }
            else 
            { 
                UIAlwaysOnTopButtonVisibility = false;
                UICloseAlwaysOnTopButtonVisibility = false;
            }
        }

        /// <summary>
        /// Toggles the Always on Top-mode on and off
        /// </summary>
        private async void ToggleAlwaysOnTopMode()
        {
            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))    // Extra check, just in case
            {
                if (!_isInAlwaysOnTopMode)
                {
                    _isInAlwaysOnTopMode = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                    UIAppBarDisplayMode = AppBarClosedDisplayMode.Minimal;
                    UITitlebarDetailsVisibility = false;
                    UIAlwaysOnTopButtonVisibility = false;
                    UICloseAlwaysOnTopButtonVisibility = true;
                }
                else
                {
                    bool switched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
                    if (switched)
                    {
                        _isInAlwaysOnTopMode = false;
                        UIAppBarDisplayMode = AppBarClosedDisplayMode.Compact;
                        UITitlebarDetailsVisibility = true;
                        UIAlwaysOnTopButtonVisibility = true;
                        UICloseAlwaysOnTopButtonVisibility = false;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the title of the app
        /// </summary>
        private void RefreshTitlebarTitle()
        {
            ApplicationView.GetForCurrentView().Title = Data.DocumentTitle;
        }

        private async void UpdateZoomFactorPercentage()
        {
            double factor = Convert.ToDouble(ZoomFactor);
            int percentage = Convert.ToInt32(Math.Round(factor * 100, 0));
            ZoomFactorPercentage = percentage;

            if (percentage == 100)
            {
                await Task.Delay(3000);
                UIZoomFactorVisibility = false;
            }
            else { UIZoomFactorVisibility = true; }
        }

        // Set save status
        public void SetEditedFalse()
        {
            FileEdited = false;
            App.UnsavedChanges = false;
            //App.RecoveryService.DeleteAutoRecoveryFile();
        }
        public async void SetEditedTrue()
        {
            FileEdited = true;
            App.UnsavedChanges = true;
            App.RecoveryService.Data = Data;
            //await App.RecoveryService.SaveAutoRecoveryFile();
        }

        // Navigation and dialogs
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
