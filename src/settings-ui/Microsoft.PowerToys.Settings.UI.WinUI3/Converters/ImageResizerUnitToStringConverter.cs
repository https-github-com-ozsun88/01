﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Windows.ApplicationModel.Resources;
using Microsoft.UI.Xaml.Data;

namespace Microsoft.PowerToys.Settings.UI.WinUI3.Converters
{
    public sealed class ImageResizerUnitToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var toLower = false;
            if ((string)parameter == "ToLower")
            {
                toLower = true;
            }

            string targetValue = string.Empty;
            switch (value)
            {
                case 0: targetValue = ResourceLoader.GetForViewIndependentUse().GetString("ImageResizer_Unit_Centimeter"); break;
                case 1: targetValue = ResourceLoader.GetForViewIndependentUse().GetString("ImageResizer_Unit_Inch"); break;
                case 2: targetValue = ResourceLoader.GetForViewIndependentUse().GetString("ImageResizer_Unit_Percent"); break;
                case 3: targetValue = ResourceLoader.GetForViewIndependentUse().GetString("ImageResizer_Unit_Pixel"); break;
            }

            if (toLower)
            {
                targetValue = targetValue.ToLower(CultureInfo.CurrentCulture);
            }

            return targetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
