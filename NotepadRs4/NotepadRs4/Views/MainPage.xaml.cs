using System;
using System.Linq;
using System.Windows.Input;
using NotepadRs4.Helpers;
using NotepadRs4.ViewModels;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace NotepadRs4.Views
{
    public sealed partial class MainPage : Page
    {
        // Properties
        public MainViewModel ViewModel { get; } = new MainViewModel();
        public SettingsViewModel SettingViewModel { get; } = Singleton<SettingsViewModel>.Instance;
        private bool _isPageLoaded = false;


        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Extend the normal window to the Titlebar for the blur to reach there too
            CoreApplicationViewTitleBar coreTitlebar = CoreApplication.GetCurrentView().TitleBar;
            coreTitlebar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitlebar);

            // Set the draggable region
            Window.Current.SetTitleBar(AppTitleBar);
            coreTitlebar.LayoutMetricsChanged += CoreTitlebar_LayoutMetricsChanged;
            coreTitlebar.IsVisibleChanged += CoreTitlebar_IsVisibleChanged;

            // Put in triggers for the logo
            this.ActualThemeChanged += MainPage_ActualThemeChanged;
            CheckThemeForLogo();

            this.Loaded += MainPage_Loaded;
            this.LayoutUpdated += MainPage_LayoutUpdated;
        }



        private void CoreTitlebar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void CoreTitlebar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            // Do nothing for now, as we want to keep the titlebar visible, even in Full Screen mode
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            LeftPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayRightInset);
            btnCloseCompactOverlay.Margin = new Thickness(0, 0, 0, 0);

            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;
        }

        // Commands
        // Find and Replace
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

        // Font Options
        private ICommand _fontOptionsCommand;
        public ICommand FontOptionsCommand
        {
            get
            {
                if (_fontOptionsCommand == null)
                {
                    _fontOptionsCommand = new RelayCommand(
                        () =>
                        {
                            ShowHideFontOptionsBar();
                        });
                }
                return _fontOptionsCommand;
            }
        }
        private ICommand _fontOptionsCloseCommand;
        public ICommand FontOptionsCloseCommand
        {
            get
            {
                if (_fontOptionsCloseCommand == null)
                {
                    _fontOptionsCloseCommand = new RelayCommand(
                        () =>
                        {
                            CloseFontOptionsBar();
                        });
                }
                return _fontOptionsCloseCommand;
            }
        }
                                 

        // Overrides
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

#if DEBUG
            // Show Find & Replace-button
            cbtnFind.Visibility = Visibility.Visible;
            cbtnSeperator.Visibility = Visibility.Visible;

            // Show Font Options-button
            //cbtnFontOptions.Visibility = Visibility.Visible;

            // Show XAML Playground-button
            cbtnPlayground.Visibility = Visibility.Visible;
#endif
        }


        // Methods
        // Stuff for putting the focus on the content
        private void MainPage_LayoutUpdated(object sender, object e)
        {
            if (_isPageLoaded == true)
            {
                // Set focus on the main content so the user can start typing right away
                txtContent.SelectionStart = (txtContent.Text.Length);
                txtContent.Focus(FocusState.Programmatic);
                _isPageLoaded = false;
            }
        }
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _isPageLoaded = true;
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

        private void ShowHideFontOptionsBar()
        {
            if (gridFontOptions.Visibility == Visibility.Collapsed)
            {
                gridFontOptions.Visibility = Visibility.Visible;
            }
            else
            {
                gridFontOptions.Visibility = Visibility.Collapsed;
            }
        }
        private void CloseFontOptionsBar()
        {
            gridFontOptions.Visibility = Visibility.Collapsed;
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

        private void TxtContent_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            // #TODO Fix this so it won't affect the Edited bool when loading from explorer (with the Initialize stuff) as this stupid trigger constantly finds a way to sneak before it and get triggered when it's not supposed to
            ViewModel.SetEditedTrue();
            ViewModel.Data.Text = txtContent.Text;
        }

        private void txtContent_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //Getting current line
            string sub = txtContent.Text.Substring(0, txtContent.SelectionStart);
            ViewModel.Line = sub.Count(i => i == '\r') + 1;

            //Getting current pos
            int retIndex = sub.LastIndexOf('\r'); //we look for a return in the text
            retIndex = (retIndex < 0) ? 0 : retIndex; //if no return, start counting from the beginning (for line 1)
            string newsub = sub.Substring(retIndex); //substring from this point on to the end
            ViewModel.Col = newsub.Count(); //the value we need is simply the count of this new substring
        }

        private void svContent_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ViewModel.ZoomFactor = svContent.ZoomFactor;

        }

        private void txtContent_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab)
            {
                txtContent.SelectedText = ("\t");
                txtContent.Select(ViewModel.Col + 1, 0);
                e.Handled = true;                
            }

            if (IsCtrlPressed() & e.Key == (VirtualKey)187) //ctrl + +
            {
                svContent.ChangeView(0.0, 0.0, svContent.ZoomFactor + 0.1f);
            }
            if (IsCtrlPressed() & e.Key == (VirtualKey)189) //ctrl + -
            {
                svContent.ChangeView(0.0, 0.0, svContent.ZoomFactor - 0.1f);
            }
            if (IsCtrlPressed() & e.Key == (VirtualKey)48) //ctrl + 0
            {
                svContent.ChangeView(0.0, 0.0, 0.0f);
            }
        }

        private bool IsCtrlPressed()
        {
            var state = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control);
            return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
    }
}
