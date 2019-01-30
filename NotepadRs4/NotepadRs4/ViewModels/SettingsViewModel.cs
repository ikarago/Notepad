using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Uwp.Helpers;
using NotepadRs4.Helpers;
using NotepadRs4.Services;

using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private string _versionDescription;
        public string VersionDescription
        {
            get { return _versionDescription; }
            set { Set(ref _versionDescription, value); }
        }

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

        // #TODO: Create a switch TextWrapping command
        /*private ICommand _switchTextWrapping;
        public ICommand SwitchTextWrapping
        {
            get
            {
                if (_switchTextWrapping == null)
                {
                    _switchTextWrapping = new RelayCommand<TextWrapping>(
                        async (param) =>
                        {
                            if (_hasInstanceBeenInitialized)
                            {
                                // Check true or false and set the new value
                            }
                        });
                }
                return _switchTextWrapping;
            }
        }*/
        

        // Constructor
        public SettingsViewModel()
        {
        }

        // Initialize
        public void Initialize()
        {
            GetSettingValues();
            VersionDescription = GetVersionDescription();
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
            // Set the default settings on the first run
            if (SystemInformation.IsFirstRun && _haveSettingDefaultsBeenSet == false)
            {
                TextWrapping = TextWrapping.Wrap;
                _haveSettingDefaultsBeenSet = true;
            }
            else
            {
                TextWrapping = await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<TextWrapping>(nameof(TextWrapping));
            }
        }

        /*
        private void CloseDialog(ContentDialog dialog)
        {
            if (dialog != null)
            {
                dialog.Hide();
            }
        } */
    }
}
