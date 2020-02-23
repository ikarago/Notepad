using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using NotepadRs4.Helpers;
using NotepadRs4.ViewModels;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
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
            this.Unloaded += MainPage_OnUnloaded;
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
            //cbtnFind.Visibility = Visibility.Visible;
            //cbtnSeperator.Visibility = Visibility.Visible;

            // Show Font Options-button
            //cbtnFontOptions.Visibility = Visibility.Visible;
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
            txtContent.SelectionFlyout.Opening += Menu_Opening;
            txtContent.ContextFlyout.Opening += Menu_Opening;

            _isPageLoaded = true;
        }

        private void MainPage_OnUnloaded(object sender, RoutedEventArgs e)
        {
            txtContent.SelectionFlyout.Opening -= Menu_Opening;
            txtContent.ContextFlyout.Opening -= Menu_Opening;
        }

        private void Menu_Opening(object sender, object e)
        {
            if (!(sender is TextCommandBarFlyout myFlyout) || myFlyout.Target != txtContent) return;
            if (ViewModel.SelectedText != "")
            {
                AddMenuItems(myFlyout.PrimaryCommands);
            }
        }

        private void AddMenuItems(IObservableVector<ICommandBarElement> primaryCommands)
        {
            // Search Local button
            if (!primaryCommands.Any(b => b is AppBarButton button && button.Name == "Search"))
            {
                var appBarButton = new AppBarButton
                {
                    Name = "Search",
                    Icon = new SymbolIcon(Symbol.Find),
                    Label = "Search",
                    Visibility = Visibility.Collapsed
                };
                primaryCommands.Add(appBarButton);
            }

            // Search Online Buttons
            if (!primaryCommands.Any(b => b is AppBarButton button && button.Name == "SearchOnline"))
            {
                var appBarButton = new AppBarButton
                {
                    Name = "SearchOnline",
                    Icon = new SymbolIcon((Symbol)0xF6FA),
                    Label = "Search Online",
                    Visibility = Visibility.Collapsed,
                    Command = ViewModel.SearchOnlineCommand
                };
                // Set the tooltip
                var toolTip = new ToolTip();
                toolTip.Content = "Search Online";
                ToolTipService.SetToolTip(appBarButton, toolTip);
                // Add the button
                primaryCommands.Add(appBarButton);
            }

            // Search with Bing
            if (!primaryCommands.Any(b => b is AppBarButton button && button.Name == "Bing"))
            {
                var iconBing = new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Icons/BingIcon.png") };

                var appBarButton = new AppBarButton
                {
                    Name = "Bing",
                    Icon = iconBing,
                    Label = "Search with Bing",
                    Command = ViewModel.SearchWithBingCommand
                };
                // Set the tooltip
                var toolTip = new ToolTip();
                toolTip.Content = "Search with Bing";
                ToolTipService.SetToolTip(appBarButton, toolTip);
                // Add the button
                primaryCommands.Add(appBarButton);
            }
            
            // Share button
            if (!primaryCommands.Any(b => b is AppBarButton button && button.Name == "Share") && ViewModel.UIShareButtonVisibility == true)
            {
                var appBarButton = new AppBarButton
                {
                    Name = "Share",
                    Icon = new SymbolIcon(Symbol.Share),
                    Label = "Share",
                    Command = ViewModel.ShareCommand,
                };
                // Set the tooltip
                var toolTip = new ToolTip();
                toolTip.Content = "Share";
                ToolTipService.SetToolTip(appBarButton, toolTip);
                // Add the button
                primaryCommands.Add(appBarButton);
            }
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

            ViewModel.SelectedText = txtContent.SelectedText;
        }

        private void svContent_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ViewModel.ZoomFactor = svContent.ZoomFactor;

        }

        private void txtContent_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            //Debug.WriteLine("Inputted Virtual Key: " + e.Key.ToString() + " + Original Virtual Key: " + e.OriginalKey.ToString());
            // Text Tab
            if (e.Key == VirtualKey.Tab)
            {
                txtContent.SelectedText = ("\t");
                txtContent.Select(ViewModel.Col + 1, 0);
                e.Handled = true;                
            }

            // Zoom
            if (IsCtrlPressed() & e.Key == VirtualKey.Add) //ctrl + + --> = (VirtualKey)187
            {
                svContent.ChangeView(0.0, 0.0, svContent.ZoomFactor + 0.1f);
            }
            // #TODO Find out why this doesn't register and instead inserts a invisible character
            if (IsCtrlPressed() & e.Key == VirtualKey.Subtract) //ctrl + - --> = (VirtualKey)189
            {
                svContent.ChangeView(0.0, 0.0, svContent.ZoomFactor - 0.1f);
            }
            if (IsCtrlPressed() & e.Key == VirtualKey.Number0) //ctrl + 0 --> = (VirtualKey)48
            {
                svContent.ChangeView(0.0, 0.0, 0.0f);
            }

            // Shortcut routines for CommandBar items PiP mode
            // Save As
            if ((IsCtrlPressed() & IsShiftPressed() & e.Key == VirtualKey.S) && ViewModel.UIAppBarDisplayMode != AppBarClosedDisplayMode.Compact)
            {
                ViewModel.SaveFileAsCommand.Execute(null);
            }
            // Save
            else if ((IsCtrlPressed() & e.Key == VirtualKey.S) && ViewModel.UIAppBarDisplayMode != AppBarClosedDisplayMode.Compact)
            {
                ViewModel.SaveFileCommand.Execute(null);
            }

            // Open
            if ((IsCtrlPressed() & e.Key == VirtualKey.O) && ViewModel.UIAppBarDisplayMode != AppBarClosedDisplayMode.Compact)
            {
                ViewModel.LoadFileCommand.Execute(null);
            }
            // New File
            if ((IsCtrlPressed() & e.Key == VirtualKey.N) && ViewModel.UIAppBarDisplayMode != AppBarClosedDisplayMode.Compact)
            {
                ViewModel.NewFileCommand.Execute(null);
            }
            // Share
            if ((IsCtrlPressed() & e.Key == VirtualKey.H) && ViewModel.UIAppBarDisplayMode != AppBarClosedDisplayMode.Compact)
            {
                ViewModel.ShareCommand.Execute(null);
            }
            // Find & Replace
            /*if ((IsCtrlPressed() & e.Key == VirtualKey.F) && ViewModel.UIAppBarDisplayMode != AppBarClosedDisplayMode.Compact)
            {
                ViewModel.SaveFileCommand.Execute(null);
            }*/
            // Print
            /*if ((IsCtrlPressed() & e.Key == VirtualKey.P) && ViewModel.UIAppBarDisplayMode != AppBarClosedDisplayMode.Compact)
            {
                ViewModel.PrintCommand.Execute(null);
            }*/
        }

        private bool IsCtrlPressed()
        {
            var state = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control);
            return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
        private bool IsShiftPressed()
        {
            var state = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift);
            return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }

        private void cbtnRestoreTip_Click(object sender, RoutedEventArgs e)
        {
            tipRecovery.IsOpen = true;
        }
    }
}
