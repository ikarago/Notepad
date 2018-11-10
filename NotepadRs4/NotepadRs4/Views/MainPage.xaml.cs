using System;
using System.Windows.Input;
using NotepadRs4.Helpers;
using NotepadRs4.ViewModels;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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

            // Put in triggers for the logo
            this.ActualThemeChanged += MainPage_ActualThemeChanged;
            CheckThemeForLogo();
        }

        // Commands
        private ICommand _findCommand;
        public ICommand FindCommand
        {
            get
            {
                if (_findCommand == null)
                {
                    _findCommand = new RelayCommand(
                        () =>
                        {
                            ShowHideFindBar();
                        });
                }
                return _findCommand;
            }
        }

        private ICommand _findBarCloseCommand;
        public ICommand FindBarCloseCommand
        {
            get
            {
                if (_findBarCloseCommand == null)
                {
                    _findBarCloseCommand = new RelayCommand(
                        () =>
                        {
                            CloseFindBar();
                        });
                }
                return _findBarCloseCommand;
            }
        }
                                 


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is StorageFile)
            {
                StorageFile file = e.Parameter as StorageFile;
                ViewModel.Initialize(file);
            }
            else
            {
                ViewModel.Initialize();
            }

            SetStatusBar();

#if DEBUG
            // Show Find & Replace-buttons
            cbtnFind.Visibility = Visibility.Visible;
            cbtnSeperator.Visibility = Visibility.Visible;

            // Show XAML Playground-button
            cbtnPlayground.Visibility = Visibility.Visible;
#endif
        }

        // #TODO: Check if this can be done soly by the ViewModel
        private async void txtContent_Drop(object sender, DragEventArgs e)
        {
            await ViewModel.DropText(e);
            // Set the cursor at the end of the added text
            txtContent.Select(txtContent.Text.Length, 0);

        }
        // #TODO: Check if this can be done soly by the ViewModel
        private void txtContent_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        }



        // Methods
        private void ShowHideFindBar()
        {
            if (gridFind.Visibility == Visibility.Collapsed)
            {
                gridFind.Visibility = Visibility.Visible;
            }
            else
            {
                gridFind.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseFindBar()
        {
            gridFind.Visibility = Visibility.Collapsed;
        }

        private void MainPage_ActualThemeChanged(FrameworkElement sender, object args)
        {
            CheckThemeForLogo();
        }

        private void CheckThemeForLogo()
        {
            // Change the displayed logo
            if (ActualTheme == ElementTheme.Dark)
            {
                BitmapImage image = new BitmapImage(new Uri("ms-appx:///Assets/Logo/contrast-black/Square44x44Logo.altform-unplated_targetsize-256.png"));
                imgAppIcon.Source = image;
            }
            else if (ActualTheme == ElementTheme.Light)
            {
                BitmapImage image = new BitmapImage(new Uri("ms-appx:///Assets/Logo/contrast-white/Square44x44Logo.altform-unplated_targetsize-256.png"));
                imgAppIcon.Source = image;
            }
        }

        // Set the StatusBar on Windows 10 Mobile devices so it doesn't look buttugly on those devices
        private async void SetStatusBar()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                SolidColorBrush solidColorBrush = (SolidColorBrush)App.Current.Resources["SystemControlBackgroundAccentBrush"];
                statusbar.BackgroundColor = solidColorBrush.Color;
                statusbar.BackgroundOpacity = 1;
                statusbar.ForegroundColor = Colors.White;

                await statusbar.ShowAsync();
            }
        }
    }
}
