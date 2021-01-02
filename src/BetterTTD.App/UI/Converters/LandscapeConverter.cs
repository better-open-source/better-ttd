using System;
using System.Globalization;
using Avalonia.Data.Converters;
using BetterTTD.Domain.Enums;

namespace BetterTTD.App.UI.Converters
{
    public class LandscapeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Landscape landscape)
            {
                return landscape switch
                {
                    Landscape.LANDSCAPE_TEMPERATE => "Temperate",
                    Landscape.LANDSCAPE_ARCTIC => "Arctic",
                    Landscape.LANDSCAPE_TROPIC => "Tropic",
                    Landscape.LANDSCAPE_TOYLAND => "Toyland",
                    Landscape.NUM_LANDSCAPE => "Unknown",
                    _ => throw new ArgumentOutOfRangeException(nameof(landscape))
                };
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string landscape)
            {
                return landscape switch
                {
                    "Temperate" => Landscape.LANDSCAPE_TEMPERATE,
                    "Arctic" => Landscape.LANDSCAPE_ARCTIC,
                    "Tropic" => Landscape.LANDSCAPE_TROPIC,
                    "Toyland" => Landscape.LANDSCAPE_TOYLAND,
                    _ => throw new ArgumentOutOfRangeException(nameof(landscape))
                };
            }

            return Landscape.NUM_LANDSCAPE;
        }
    }
}