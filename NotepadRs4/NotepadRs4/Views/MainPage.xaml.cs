using System;
using NotepadRs4.Helpers;
using NotepadRs4.ViewModels;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace NotepadRs4.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();
        public SettingsViewModel SettingViewModel { get; } = Singleton<SettingsViewModel>.Instance;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is StorageFile)
            {
                StorageFile file = e.Parameter as StorageFile;
                ViewModel.InitializeByFileActivation(file);
            }
            else
            {
                ViewModel.Initialize();
            }

        }

        // TODO: Check if this can be done soly by the ViewModel
        private async void txtContent_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            await ViewModel.DropText(e);
            // Set the cursor at the end of the added text
            txtContent.Select(txtContent.Text.Length, 0);

        }
        // TODO: Check if this can be done soly by the ViewModel
        private void txtContent_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        }
    }
}
