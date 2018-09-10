//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

// Based on the BooleanToVisibiltyConverter from the UWP RSS Reader sample
// https://github.com/Microsoft/Windows-appsample-rssreader/blob/master/RssReader/Common/BooleanToVisibilityConverter.cs

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NotepadRs4.Helpers
{
    /// <summary>
    /// Converts between bool and TextWrapping values
    /// </summary>
    public class BoolToTextWrappingConverter : IValueConverter
    {
        /// <summary>
        /// Converts TextWrapping.Wrap values to 'true' and TextWrapping.NoWrap to 'false'
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language) =>
            (TextWrapping)value == TextWrapping.NoWrap ^ (parameter as string ?? string.Empty).Equals("Reverse");

        /// <summary>
        /// Converts 'true' values to TextWrapping.Wrap and 'false' values to TextWrapping.NoWrap
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            (bool)value ^ (parameter as string ?? string.Empty).Equals("Reverse") ?
                TextWrapping.NoWrap : TextWrapping.Wrap;
    }
}
