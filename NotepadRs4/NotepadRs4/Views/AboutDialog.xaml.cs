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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NotepadRs4.Views
{
    public sealed partial class AboutDialog : ContentDialog
    {
        public AboutViewModel ViewModel { get; } = new AboutViewModel();

        public AboutDialog()
        {
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            this.InitializeComponent();
        }

        // #TODO: Put this stuff in the ViewModel
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

        private void CloseDialog()
        {
            this.Hide();
        }
    }
}
