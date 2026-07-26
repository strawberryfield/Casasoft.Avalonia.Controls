// copyright (c) 2020-2026 Roberto Ceccarelli - Casasoft
// http://strawberryfield.altervista.org
//
// This file is part of Casasoft Contemporary Carte de Visite Tools
// https://github.com/strawberryfield/Contemporary_CDV
//
// Casasoft CCDV Tools is free software:
// you can redistribute it and/or modify it
// under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Casasoft CCDV Tools is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// You should have received a copy of the GNU AGPL v.3
// along with Casasoft CCDV Tools.
// If not, see <http://www.gnu.org/licenses/>.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageMagick;
using System.IO;

namespace Casasoft.Avalonia.Controls;

/// <summary>
/// Displays a <see cref="MagickImage"/> preview. Replacement for the WPF
/// <c>Casasoft.Xaml.Controls.ImageViewer</c>, which relied on
/// <c>Magick.NET.SystemWindowsMedia</c>'s <c>MagickImage.ToBitmapSource()</c>
/// extension. Avalonia has no equivalent extension, so <see cref="SetImage"/>
/// converts through an in-memory PNG stream instead.
/// </summary>
/// <remarks>
/// Call sites that previously did <c>image.Source = bm.ToBitmapSource();</c>
/// become <c>image.SetImage(bm);</c> after porting.
/// </remarks>
public partial class ImageViewer : UserControl
{
    public static readonly StyledProperty<Bitmap?> SourceProperty =
        AvaloniaProperty.Register<ImageViewer, Bitmap?>(nameof(Source));

    /// <summary>
    /// Gets or sets the currently displayed bitmap in the image viewer.
    /// </summary>
    /// <remarks>
    /// This property is backed by the <see cref="SourceProperty"/> styled property, which allows
    /// it to be used as a bindable property in XAML markup. When set to <c>null</c>, the viewer
    /// displays as empty without any image content.
    /// </remarks>
    /// <value>
    /// A <see cref="Bitmap"/> object representing the image to display, or <c>null</c> if no image
    /// is currently displayed in the viewer.
    /// </value>
    public Bitmap? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageViewer"/> control.
    /// </summary>
    /// <remarks>
    /// Calls <see cref="InitializeComponent"/> to load the control's XAML, create and
    /// wire up child controls (for example the internal <c>img</c> element) and apply styles.
    /// After construction the control is ready for use and its <see cref="Source"/> property
    /// may be set (or <see cref="SetImage"/> called) to display image content.
    /// </remarks>
    public ImageViewer()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty)
        {
            img.Source = change.GetNewValue<Bitmap?>();
        }
    }

    /// <summary>
    /// Converts <paramref name="image"/> to an Avalonia <see cref="Bitmap"/> (via an
    /// in-memory PNG round-trip) and displays it. Pass <c>null</c> to clear the viewer.
    /// </summary>
    /// <param name="image">Source image, or null to clear the display.</param>
    public void SetImage(MagickImage? image)
    {
        if (image is null)
        {
            Source = null;
            return;
        }

        using MemoryStream ms = new();
        image.Write(ms, MagickFormat.Png);
        ms.Position = 0;
        Source = new Bitmap(ms);
    }

    /// <summary>
    /// Clears the currently displayed image.
    /// </summary>
    public void Clear() => Source = null;
}
