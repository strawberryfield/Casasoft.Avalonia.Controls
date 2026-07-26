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
/// Multi-line text editor for Pango-markup back-side text (credit cards).
/// Replacement for the WPF <c>Casasoft.Xaml.Controls.PangoTextEditor</c>, used as
/// <c>backText.Value</c> in <c>CreditCardForm</c>.
/// </summary>
public partial class PangoTextEditor : UserControl
{
    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<PangoTextEditor, string>(nameof(Value), string.Empty);

    /// <summary>
    /// Gets or sets the text content of the editor, storing Pango-markup formatted text.
    /// </summary>
    /// <value>The current text value. Defaults to an empty string.</value>
    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Flag to prevent recursive synchronization between the text control and the <see cref="Value"/> property.
    /// </summary>
    private bool _suppressTextSync;

    /// <summary>
    /// Initializes a new instance of the <see cref="PangoTextEditor"/> class.
    /// Sets up the component and configures the text change event handler to synchronize with the <see cref="Value"/> property.
    /// </summary>
    public PangoTextEditor()
    {
        InitializeComponent();

        txt.TextChanged += (_, _) =>
        {
            if (_suppressTextSync) return;
            SetCurrentValue(ValueProperty, txt.Text ?? string.Empty);
        };
    }

    /// <summary>
    /// Handles property changes, specifically synchronizing the underlying text control when the <see cref="Value"/> property changes.
    /// </summary>
    /// <param name="change">The property change event arguments containing information about the changed property.</param>
    /// <remarks>
    /// This method prevents infinite recursion by using the <see cref="_suppressTextSync"/> flag
    /// to suppress the text change event handler during programmatic updates.
    /// </remarks>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty)
        {
            string newValue = change.GetNewValue<string>() ?? string.Empty;
            if (txt.Text != newValue)
            {
                _suppressTextSync = true;
                txt.Text = newValue;
                _suppressTextSync = false;
            }
        }
    }
}
