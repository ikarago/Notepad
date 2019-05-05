using NotepadRs4.Helpers;
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

namespace NotepadRs4.Views.Dialogs
{
    /// <summary>
    /// Results returned from a ExitConfirmationDialog:
    /// Nothing - No action has been taken (yet) by the user.
    /// Save - User wants to save the current item.
    /// Discard - User wants to discard the current item.
    /// Cancel - User wants to cancel the action they've clicked on.
    /// DialogClosed - User closed the dialog and gave no answer. Handle as 'Cancel' to the user, this option is there for debugging
    /// </summary>
    public enum ExitConfirmationDialogResult
    {
        Nothing,
        Save,
        Discard,
        Cancel,
        DialogClosed
    }

    public sealed partial class ExitConfirmationDialog : ContentDialog
    {
        // Properties
        public ExitConfirmationDialogResult Result { get; set; }


        // Constructor
        public ExitConfirmationDialog()
        {
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            this.InitializeComponent();
            Result = ExitConfirmationDialogResult.Nothing;
        }


        // Commands
        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        () =>
                        {
                            Save();
                        });
                }
                return _saveCommand;
            }
        }

        private ICommand _discardCommand;
        public ICommand DiscardCommand
        {
            get
            {
                if (_discardCommand == null)
                {
                    _discardCommand = new RelayCommand(
                        () =>
                        {
                            Discard();
                        });
                }
                return _discardCommand;
            }
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(
                        () =>
                        {
                            Cancel();
                        });
                }
                return _cancelCommand;
            }
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(
                        () =>
                        {
                            Close();
                        });
                }
                return _closeCommand;
            }
        }




        // Methods
        private void Save()
        {
            Result = ExitConfirmationDialogResult.Save;
            Hide();
        }

        private void Discard()
        {
            Result = ExitConfirmationDialogResult.Discard;
            Hide();
        }

        private void Cancel()
        {
            Result = ExitConfirmationDialogResult.Cancel;
            Hide();
        }

        private void Close()
        {
            Result = ExitConfirmationDialogResult.DialogClosed;
            Hide();
        }
    }
}
