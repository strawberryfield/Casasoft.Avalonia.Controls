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

namespace Casasoft.Avalonia.Controls;

/// <summary>
/// <see cref="FileTextBox"/> with an attached caption label above it. Replacement
/// for the WPF <c>Casasoft.Xaml.Controls.FileTextBoxLabel</c> used e.g. for
/// front/back/magnetic-band images and the script file picker.
/// </summary>
public partial class FileTextBoxLabel : UserControl
{
    /// <summary>
    /// Backing StyledProperty for the <see cref="Caption"/> property.
    /// Represents the text displayed in the caption label above the inner <c>FileTextBox</c>.
    /// Defaults to an empty string.
    /// </summary>
    public static readonly StyledProperty<string> CaptionProperty =
        AvaloniaProperty.Register<FileTextBoxLabel, string>(nameof(Caption), string.Empty);

    /// <summary>
    /// Gets or sets the caption displayed above the file text box.
    /// This is a styled (Avalonia) property and supports data binding and styling.
    /// </summary>
    /// <remarks>
    /// Changing this property updates the visual label content (see <c>lblCaption</c>).
    /// </remarks>
    public string Caption
    {
        get => GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    /// <summary>
    /// Backing StyledProperty for the <see cref="Value"/> property.
    /// Stores the path or value of the selected file. Defaults to an empty string.
    /// </summary>
    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<FileTextBoxLabel, string>(nameof(Value), string.Empty);

    /// <summary>
    /// Gets or sets the current value (file path) shown by the inner <c>FileTextBox</c>.
    /// This property is synchronized with the inner control so that changes from the UI
    /// or programmatically are reflected here and vice-versa.
    /// </summary>
    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Backing StyledProperty for the <see cref="OpenFileDialogFilter"/> property.
    /// Specifies the file dialog filter string used when browsing for files.
    /// Defaults to an empty string.
    /// </summary>
    public static readonly StyledProperty<string> OpenFileDialogFilterProperty =
        AvaloniaProperty.Register<FileTextBoxLabel, string>(nameof(OpenFileDialogFilter), string.Empty);

    /// <summary>
    /// Gets or sets the filter string applied to the OpenFileDialog used by the inner <c>FileTextBox</c>.
    /// Example: <c>"Image Files (*.png;*.jpg)|*.png;*.jpg"</c>.
    /// </summary>
    public string OpenFileDialogFilter
    {
        get => GetValue(OpenFileDialogFilterProperty);
        set => SetValue(OpenFileDialogFilterProperty, value);
    }

    /// <summary>
    /// Backing StyledProperty for the <see cref="OpenFileDialogTitle"/> property.
    /// The title text shown on the open file dialog. Defaults to "Seleziona file".
    /// </summary>
    public static readonly StyledProperty<string> OpenFileDialogTitleProperty =
        AvaloniaProperty.Register<FileTextBoxLabel, string>(nameof(OpenFileDialogTitle), "Seleziona file");

    /// <summary>
    /// Gets or sets the title for the OpenFileDialog shown by the inner <c>FileTextBox</c>.
    /// </summary>
    public string OpenFileDialogTitle
    {
        get => GetValue(OpenFileDialogTitleProperty);
        set => SetValue(OpenFileDialogTitleProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FileTextBoxLabel"/>.
    /// </summary>
    /// <remarks>
    /// Calls <see cref="InitializeComponent"/> to load the XAML portion of the control
    /// and wires up a PropertyChanged handler on the inner <c>FileTextBox</c> (named <c>ftb</c>)
    /// so that when the inner control's <c>Value</c> changes (for example via the Browse button),
    /// this control's <see cref="Value"/> is updated (bubbled) as well.
    /// </remarks>
    public FileTextBoxLabel()
    {
        InitializeComponent();

        // Bubble the inner control's Value up to this control's own Value property
        // (e.g. after the user picks a file via the browse button).
        ftb.PropertyChanged += (_, args) =>
        {
            if (args.Property == FileTextBox.ValueProperty)
                SetCurrentValue(ValueProperty, ftb.Value);
        };
    }

    /// <summary>
    /// Handles changes to the control's styled properties and forwards values
    /// to the corresponding elements of the visual tree (inner controls).
    /// </summary>
    /// <param name="change">Provides information about the property change.</param>
    /// <remarks>
    /// The override keeps the visual state in sync:
    /// - When <see cref="CaptionProperty"/> changes, updates <c>lblCaption.Content</c>.
    /// - When <see cref="ValueProperty"/> changes, updates the inner <c>ftb.Value</c>.
    /// - When <see cref="OpenFileDialogFilterProperty"/> or <see cref="OpenFileDialogTitleProperty"/> change,
    ///   forwards the new values to the inner <c>FileTextBox</c>.
    /// </remarks>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CaptionProperty)
        {
            lblCaption.Content = change.GetNewValue<string>();
        }
        else if (change.Property == ValueProperty)
        {
            string newValue = change.GetNewValue<string>() ?? string.Empty;
            if (ftb.Value != newValue) ftb.Value = newValue;
        }
        else if (change.Property == OpenFileDialogFilterProperty)
        {
            ftb.OpenFileDialogFilter = change.GetNewValue<string>();
        }
        else if (change.Property == OpenFileDialogTitleProperty)
        {
            ftb.OpenFileDialogTitle = change.GetNewValue<string>();
        }
    }
}
