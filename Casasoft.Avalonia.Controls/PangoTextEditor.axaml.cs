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

namespace Casasoft.Avalonia.Controls;

/// <summary>
/// Multi-line text editor for Pango-markup back-side text (credit cards).
/// Replacement for the WPF <c>Casasoft.Xaml.Controls.PangoTextEditor</c>, used as
/// <c>backText.Value</c> in <c>CreditCardForm</c>.
/// </summary>
public partial class PangoTextEditor : UserControl
{
    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<PangoTextEditor, string>(nameof(Value), string.Empty);

    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private bool _suppressTextSync;

    public PangoTextEditor()
    {
        InitializeComponent();

        txt.TextChanged += (_, _) =>
        {
            if (_suppressTextSync) return;
            SetCurrentValue(ValueProperty, txt.Text ?? string.Empty);
        };
    }

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
