// copyright (c) 2026 Roberto Ceccarelli - Casasoft
// http://strawberryfield.altervista.org
//
// This file is part of Casasoft Avalonia Controls
// https://github.com/strawberryfield/AvaloniaControls
//
// Casasoft Avalonia Controls is free software:
// you can redistribute it and/or modify it
// under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Casasoft Avalonia Controls is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// You should have received a copy of the GNU AGPL v.3
// along with Casasoft Avalonia Controls.
// If not, see <http://www.gnu.org/licenses/>.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageMagick;
using System.IO;

namespace Casasoft.Avalonia.Controls;

/// <summary>
/// A lightweight Avalonia control that displays an <see cref="ImageMagick.MagickImage"/> as an Avalonia <see cref="Avalonia.Media.Imaging.Bitmap"/>.
/// </summary>
/// <remarks>
/// This control replaces the previous WPF implementation that relied on
/// <c>Magick.NET.SystemWindowsMedia</c>'s <c>MagickImage.ToBitmapSource()</c> extension.
/// Because Avalonia does not provide that extension, <see cref="SetImage"/> performs an
/// in-memory PNG round-trip: the <see cref="MagickImage"/> is written to a <see cref="MemoryStream"/>
/// in PNG format and then loaded into an Avalonia <see cref="Bitmap"/> for display.
///
/// Typical usage:
/// <code>
/// // convert a MagickImage and show it
/// var magick = new MagickImage("photo.jpg");
/// imageViewer.SetImage(magick);
///
/// // clear the viewer
/// imageViewer.Clear();
/// </code>
///
/// The control expects a child image element in the corresponding XAML with the name <c>img</c>.
/// </remarks>
public partial class ImageViewer : UserControl
{
    /// <summary>
    /// Backing styled property for <see cref="Source"/>.
    /// </summary>
    public static readonly StyledProperty<Bitmap?> SourceProperty =
        AvaloniaProperty.Register<ImageViewer, Bitmap?>(nameof(Source));

    /// <summary>
    /// Gets or sets the currently displayed bitmap.
    /// </summary>
    /// <remarks>
    /// This property is a styled property so it can be bound to in XAML.
    /// Setting this property to <c>null</c> clears the displayed image.
    /// When the property changes internally the control updates the visual image element
    /// (the internal element named <c>img</c> declared in the control's XAML).
    /// </remarks>
    /// <value>
    /// A <see cref="Bitmap"/> instance to display, or <c>null</c> if no image is shown.
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
    /// Calls <see cref="InitializeComponent"/> to load the control's XAML, create and wire up child controls,
    /// and apply styles. After construction the <see cref="Source"/> property may be set or
    /// <see cref="SetImage(MagickImage?)"/> may be called to present image content.
    /// </remarks>
    public ImageViewer()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Monitors property changes and forwards <see cref="Source"/> updates to the visual image element.
    /// </summary>
    /// <param name="change">Details about the changed property.</param>
    /// <remarks>
    /// This override listens for changes to the <see cref="SourceProperty"/> and sets the
    /// internal <c>img.Source</c> accordingly. The <c>img</c> element is expected to be present
    /// in the control's XAML (typically an <see cref="global::Avalonia.Controls.Image"/> named <c>img</c>).
    /// </remarks>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty)
        {
            img.Source = change.GetNewValue<Bitmap?>();
        }
    }

    /// <summary>
    /// Converts a <see cref="MagickImage"/> into an Avalonia <see cref="Bitmap"/> and displays it.
    /// </summary>
    /// <param name="image">
    /// The source <see cref="MagickImage"/> to display. Pass <c>null</c> to clear the viewer.
    /// </param>
    /// <remarks>
    /// The conversion is performed by writing the <paramref name="image"/> to an in-memory PNG stream
    /// and creating an Avalonia <see cref="Bitmap"/> from that stream. The method disposes the temporary
    /// stream before returning; <see cref="Bitmap"/> already reads the required data during construction.
    ///
    /// This method performs work on the calling thread; if called from a non-UI thread the caller must
    /// ensure thread affinity where required by Avalonia or marshal back to the UI thread before calling.
    /// </remarks>
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
    /// Clears any image currently displayed by the control.
    /// </summary>
    public void Clear() => Source = null;
}
