using NotepadRs4.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NotepadRs4.Controls
{
    public sealed partial class AboutDialogControl : ContentDialog
    {
        // Properties
        // Version control
        private string AboutAppName { get; set; }
        private string AboutAppVersionNumber { get; set; }
        public string AboutDescription { get; set; }
        private bool AboutDescriptionVisibility
        {
            get
            {
                if (AboutDescription != null)
                { return true; }
                else { return false; }
            }
        }


        // Translators
        public string TranslatorsTitle { get; set; }
        public string TranslatorsDescription { get; set; }
        public List<string> Translators { get; set; }

        private bool TranslatorsVisibility
        {
            get
            {
                if (TranslatorsTitle != null && Translators.Count != 0)
                { return true; }
                else { return false; }
            }
        }
        private bool TranslatorsDescriptionVisibility
        {
            get
            {
                if (TranslatorsDescription != null)
                { return true; }
                else { return false; }
            }
        }

        // Special thanks


        // Acknowlegdements


        // Links


        // Support us


        // Visibilities



        // Constructor
        public AboutDialogControl()
        {
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            this.InitializeComponent();

            Initialize();
        }

        // Initialize
        public void Initialize()
        {
            AboutAppName = GetAppName();
            AboutAppVersionNumber = GetVersionNumber();
            Translators = new List<string>();
        }




        // Commands
        private ICommand _closeDialogCommand;
        public ICommand CloseDialogCommand
        {
            get
            {
                if (_closeDialogCommand == null)
                {
                    _closeDialogCommand = new RelayCommand(
                        () =>
                        {
                            Hide();
                        });
                }
                return _closeDialogCommand;
            }
        }





        // Methods
        private string GetAppName()
        {
            var package = Package.Current;
            string appName = package.DisplayName;

            return appName;
        }

        private string GetVersionNumber()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
