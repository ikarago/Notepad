using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    // NOTE - This dialog is still experimental and subject to (massive) changes. Use at your own risk 

    public sealed partial class UnsavedDialog : ContentDialog
    {
        public ContentDialogResult Result { get; set; }

        public UnsavedDialog()
        {
            this.InitializeComponent();
            Result = ContentDialogResult.None;
        }

        private void PrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogResult.Primary;
            Hide();
        }

        private void SecondaryButton_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogResult.Secondary;
            Hide();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogResult.None;
            Hide();
        }
    }
}
