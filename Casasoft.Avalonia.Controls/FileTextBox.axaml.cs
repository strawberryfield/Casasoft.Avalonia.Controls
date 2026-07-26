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
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Casasoft.Avalonia.Controls;

/// <summary>
/// Text box with an attached "browse for file" button. Direct replacement for the
/// WPF <c>Casasoft.Xaml.Controls.FileTextBox</c> used throughout the CCDV forms
/// (e.g. <c>filename1</c>..<c>filename8</c> slots, front/back/border images, ...).
/// </summary>
/// <remarks>
/// <see cref="OpenFileDialogFilter"/> keeps the exact same syntax used by the WPF
/// XAML already in the codebase (e.g.
/// <c>"Image files (*.jpg;*.jpeg;*.png;*.psd)|*.jpg;*.jpeg;*.png;*.psd|All files (*.*)|*.*"</c>),
/// so those attribute values can be copied verbatim into the ported .axaml files.
/// </remarks>
public partial class FileTextBox : UserControl
{
    /// <summary>
    /// Backing styled property for <see cref="Value"/>.
    /// Holds the currently selected path or filename. Two-way: typing in the box
    /// or picking a file both update this property.
    /// </summary>
    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<FileTextBox, string>(nameof(Value), string.Empty);

    /// <summary>
    /// Currently selected path or filename. This is a two-way bindable property:
    /// - When the user types in the text box, the property is updated.
    /// - When the property is set programmatically, the text box is updated.
    /// Always non-null (empty string when no value).
    /// </summary>
    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Backing styled property for <see cref="OpenFileDialogFilter"/>.
    /// Stores a WPF-style pipe-delimited file filter string.
    /// </summary>
    public static readonly StyledProperty<string> OpenFileDialogFilterProperty =
        AvaloniaProperty.Register<FileTextBox, string>(nameof(OpenFileDialogFilter), string.Empty);

    /// <summary>
    /// File filter using the same pipe-delimited syntax as WPF's
    /// <c>OpenFileDialog.Filter</c>: <c>"Description|*.ext1;*.ext2|Description2|*.ext3"</c>.
    /// May be empty. This value is parsed by <see cref="ParseWpfFilter(string?)"/>.
    /// </summary>
    public string OpenFileDialogFilter
    {
        get => GetValue(OpenFileDialogFilterProperty);
        set => SetValue(OpenFileDialogFilterProperty, value);
    }

    /// <summary>
    /// Backing styled property for <see cref="OpenFileDialogTitle"/>.
    /// Controls the title shown on the file picker dialog.
    /// </summary>
    public static readonly StyledProperty<string> OpenFileDialogTitleProperty =
        AvaloniaProperty.Register<FileTextBox, string>(nameof(OpenFileDialogTitle), "Seleziona file");

    /// <summary>
    /// Title shown on the file picker dialog. Defaults to "Seleziona file".
    /// </summary>
    public string OpenFileDialogTitle
    {
        get => GetValue(OpenFileDialogTitleProperty);
        set => SetValue(OpenFileDialogTitleProperty, value);
    }

    /// <summary>
    /// Internal flag used to avoid feedback loops when synchronizing the
    /// <see cref="Value"/> property and the internal text box control.
    /// When true, changes originating from code that are synchronizing the UI
    /// should not re-apply the same value back to the property.
    /// </summary>
    private bool _suppressTextSync;

    /// <summary>
    /// Initializes a new instance of <see cref="FileTextBox"/>.
    /// Wires up the internal text box change handler so typed text updates
    /// the <see cref="Value"/> property unless synchronization is suppressed.
    /// </summary>
    public FileTextBox()
    {
        InitializeComponent();

        txtValue.TextChanged += (_, _) =>
        {
            if (_suppressTextSync) return;
            SetCurrentValue(ValueProperty, txtValue.Text ?? string.Empty);
        };
    }

    /// <summary>
    /// Observes changes to registered Avalonia properties.
    /// When <see cref="ValueProperty"/> changes, updates the internal text box
    /// without causing a recursive update via <see cref="_suppressTextSync"/>.
    /// </summary>
    /// <param name="change">Property change event arguments supplied by Avalonia.</param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty)
        {
            string newValue = change.GetNewValue<string>() ?? string.Empty;
            if (txtValue.Text != newValue)
            {
                _suppressTextSync = true;
                txtValue.Text = newValue;
                _suppressTextSync = false;
            }
        }
    }

    /// <summary>
    /// Click handler for the "Browse" button.
    /// Opens the platform file picker via the current top-level's <see cref="IStorageProvider"/>,
    /// applying <see cref="OpenFileDialogTitle"/> and the parsed <see cref="OpenFileDialogFilter"/>.
    /// On successful selection the <see cref="ValueProperty"/> is set to the local file path
    /// when available via <see cref="IStorageFile.TryGetLocalPath"/>, otherwise the storage
    /// file's <see cref="IStorageFile.Name"/> is used.
    /// </summary>
    /// <param name="sender">Event sender (button).</param>
    /// <param name="e">Event args.</param>
    private async void btnBrowse_Click(object? sender, RoutedEventArgs e)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider is not { } storageProvider) return;

        IReadOnlyList<IStorageFile> result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = OpenFileDialogTitle,
            AllowMultiple = false,
            FileTypeFilter = ParseWpfFilter(OpenFileDialogFilter)
        });

        IStorageFile? file = result.FirstOrDefault();
        if (file is not null)
        {
            SetCurrentValue(ValueProperty, file.TryGetLocalPath() ?? file.Name);
        }
    }

    /// <summary>
    /// Parses a WPF-style filter string into the list of <see cref="FilePickerFileType"/>
    /// expected by Avalonia's <see cref="IStorageProvider"/>.
    /// </summary>
    /// <param name="filter">
    /// Pipe-delimited pairs of description/patterns, e.g.
    /// <c>"Image files (*.jpg;*.jpeg)|*.jpg;*.jpeg|All files (*.*)|*.*"</c>. May be null or empty.
    /// </param>
    /// <returns>
    /// A list of <see cref="FilePickerFileType"/> instances reflecting the parsed descriptions
    /// and file patterns. Returns an empty list when <paramref name="filter"/> is null or whitespace.
    /// </returns>
    internal static List<FilePickerFileType> ParseWpfFilter(string? filter)
    {
        List<FilePickerFileType> list = new();
        if (string.IsNullOrWhiteSpace(filter)) return list;

        string[] parts = filter.Split('|');
        for (int i = 0; i + 1 < parts.Length; i += 2)
        {
            string name = parts[i];
            string[] patterns = parts[i + 1].Split(';', StringSplitOptions.RemoveEmptyEntries);
            list.Add(new FilePickerFileType(name) { Patterns = patterns });
        }
        return list;
    }
}
