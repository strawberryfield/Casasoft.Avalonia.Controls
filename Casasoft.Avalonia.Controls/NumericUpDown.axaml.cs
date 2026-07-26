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
/// Int-based numeric stepper. Replacement for the WPF
/// <c>Casasoft.Xaml.Controls.NumericUpDown</c> used for rows/columns/cube size,
/// DPI, box thickness and padding. Internally wraps Avalonia's own (decimal-based)
/// <see cref="Avalonia.Controls.NumericUpDown"/>.
/// </summary>
public partial class NumericUpDown : UserControl
{
    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<NumericUpDown, int>(nameof(Value), 0);

    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly StyledProperty<int> MinValueProperty =
        AvaloniaProperty.Register<NumericUpDown, int>(nameof(MinValue), 0);

    public int MinValue
    {
        get => GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    public NumericUpDown()
    {
        InitializeComponent();

        nud.ValueChanged += (_, _) =>
        {
            if (nud.Value.HasValue)
                SetCurrentValue(ValueProperty, (int)nud.Value.Value);
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty)
        {
            int newValue = change.GetNewValue<int>();
            if (nud.Value != newValue) nud.Value = newValue;
        }
        else if (change.Property == MinValueProperty)
        {
            nud.Minimum = change.GetNewValue<int>();
        }
    }
}
