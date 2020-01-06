using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Uwp.Helpers;
using NotepadRs4.Helpers;
using NotepadRs4.Services;

using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NotepadRs4.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
    public class SettingsViewModel : Observable
    {
        // Properties
        private bool _hasInstanceBeenInitialized = false;
        private bool _haveSettingDefaultsBeenSet = false;

        private ElementTheme _elementTheme = ThemeSelectorService.Theme;
        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;
        public string VersionDescription
        {
            get { return _versionDescription; }
            set { Set(ref _versionDescription, value); }
        }

        // Textbox properties
        /// <summary>
        /// Text Wrapping
        /// </summary>
        private TextWrapping _textWrapping;
        public TextWrapping TextWrapping
        {
            get { return _textWrapping; }
            set
            {
                if (value != _textWrapping)
                {
                    Task.Run(async () => await Windows.Storage.ApplicationData.Current.LocalSettings.SaveAsync(nameof(TextWrapping), value));
                }

                Set(ref _textWrapping, value);
            }
        }

        /// <summary>
        /// Spell Checker
        /// </summary>
        private bool _isSpellCheckerEnabled;
        public bool IsSpellCheckerEnabled
        {
            get { return _isSpellCheckerEnabled; }
            set
            {
                if (value != _isSpellCheckerEnabled)
                {
                    Task.Run(async () => await Windows.Storage.ApplicationData.Current.LocalSettings.SaveAsync(nameof(IsSpellCheckerEnabled), value));
                }

                Set(ref _isSpellCheckerEnabled, value);
            }
        }

        /// <summary>
        /// Status Bar
        /// </summary>
        private bool _isStatusBarEnabled;
        public bool IsStatusBarEnabled
        {
            get { return _isStatusBarEnabled; }
            set
            {
                if (value != _isStatusBarEnabled)
                {
                    Task.Run(async () => await Windows.Storage.ApplicationData.Current.LocalSettings.SaveAsync(nameof(IsStatusBarEnabled), value));
                }

                Set(ref _isStatusBarEnabled, value);
            }
        }

        /// <summary>
        /// List of Fonts available on the system
        /// </summary>
        public ObservableCollection<string> Fonts
        {
            get
            {
                ObservableCollection<string> list = new ObservableCollection<string>();

                // Get the array of fonts
                var fonts = Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies();

                // Put the individual fonts in the list
                foreach (string font in fonts)
                {
                    list.Add(font);
                }

                return list;
            }
        }

        private string _selectedFontFamily;
        public string SelectedFontFamily
        {
            get { return _selectedFontFamily; }
            set
            {
                if (value != _selectedFontFamily)
                {
                    // #TODO Save the new value to Settings
                }

                Set(ref _selectedFontFamily, value);
            }
        }

        private ObservableCollection<int> _fontSizes;
        public ObservableCollection<int> FontSizes
        {
            get { return _fontSizes; }
            set { Set(ref _fontSizes, value); }
        }

        private int _selectedFontSize;
        public int SelectedFontSize
        {
            get { return _selectedFontSize; }
            set
            {
                if (value != _selectedFontSize)
                {
                    // #TODO Save the new value to Settings
                    /*if (!FontSizes.Contains(value))
                    {
                        FontSizes.Add(value);
                    }*/
                }

                Set(ref _selectedFontSize, value);
            }
        }

        // #TODO: Bold property
        // #TODO: Italic property
        

        // Constructor
        public SettingsViewModel()
        {
        }

        // Initialize
        public void Initialize()
        {
            GetSettingValues();
            VersionDescription = GetVersionDescription();

            FontSizes = new ObservableCollection<int>
            {
                8,
                9,
                10,
                11,
                12,
                14,
                16,
                18,
                20,
                22,
                24,
                26,
                28,
                36,
                48,
                72
            };

            // #TODO - TEMP Set the default fontsize and family for the session
            SelectedFontFamily = FontFamily.XamlAutoFontFamily.Source;
            SelectedFontSize = 14;
        }


        // Commands
        private ICommand _switchThemeCommand;
        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            if (_hasInstanceBeenInitialized)
                            {
                                ElementTheme = param;
                                await ThemeSelectorService.SetThemeAsync(param);
                            }
                        });
                }

                return _switchThemeCommand;
            }
        }


        // Methods
        public async Task EnsureInstanceInitializedAsync()
        {
            if (!_hasInstanceBeenInitialized)
            {
                Initialize();

                _hasInstanceBeenInitialized = true;
            }
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async void GetSettingValues()
        {
            // Reworked this with try/catches so it's not dependent on the first run bool anymore and checks stuff dynamically
            // The purpose of this is; try to read the data; if it fails set a default value on the public property so it'll be saved back to the backend settings automatically

            /// Text Wrapping
            try { TextWrapping = await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<TextWrapping>(nameof(TextWrapping)); }
            catch { TextWrapping = TextWrapping.Wrap; }

            /// Spell Checkers
            try { IsSpellCheckerEnabled = await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<bool>(nameof(IsSpellCheckerEnabled)); }
            catch { IsSpellCheckerEnabled = false; }

            /// Status bar
            try { IsStatusBarEnabled = await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<bool>(nameof(IsStatusBarEnabled)); }
            catch { IsStatusBarEnabled = true; }
        }

        private void AddCustomFontSize(int value)
        {
            FontSizes.Add(value);
            // Automatically set as selected font size
            SelectedFontSize = value;
        }
    }
}
