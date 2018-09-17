using NotepadRs4.Services;
using System;
using System.Collections.Generic;
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

namespace NotepadRs4.Views
{
    /// <summary>
    /// A dumping ground for various testing purposes
    /// </summary>
    public sealed partial class XamlPlayground : Page
    {
        public XamlPlayground()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
           base.OnNavigatedTo(e);
        }

        private async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            await testDialog.ShowAsync();
        }

        private void btnNavigateBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack == true)
            {
                NavigationService.GoBack();
            }
        }
    }
}
