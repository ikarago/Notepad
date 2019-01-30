using NotepadRs4.Helpers;
using NotepadRs4.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NotepadRs4.Views.Dialogs
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        // Properties
        public SettingsViewModel ViewModel { get; } = Singleton<SettingsViewModel>.Instance;


        // Constructor
        public SettingsDialog()
        {
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            this.InitializeComponent();
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
                            CloseDialog();
                        });
                }
                return _closeDialogCommand;
            }
        }


        // Methods
        private void CloseDialog()
        {
            this.Hide();
        }
    }
}
