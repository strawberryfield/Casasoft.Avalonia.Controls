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
/// <summary>
/// A user control that wraps an Avalonia numeric up/down control and exposes
/// bindable styled properties for the current <see cref="Value"/> and the
/// <see cref="MinValue"/> allowed. Synchronizes changes between the internal
/// numeric control (`nud`) and the styled properties to keep UI and view-model
/// state consistent.
/// </summary>
public partial class NumericUpDown : UserControl
{
    /// <summary>
    /// The styled backing property for <see cref="Value"/>.
    /// Default value is <c>0</c>.
    /// </summary>
    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<NumericUpDown, int>(nameof(Value), 0);

    /// <summary>
    /// Gets or sets the current integer value of the control.
    /// Setting this property updates the styled property store and will notify
    /// any bindings. Changes to the internal numeric control are propagated to
    /// this property via the <see cref="nud"/> ValueChanged handler.
    /// </summary>
    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// The styled backing property for <see cref="MinValue"/>.
    /// Default value is <c>0</c>.
    /// </summary>
    public static readonly StyledProperty<int> MinValueProperty =
        AvaloniaProperty.Register<NumericUpDown, int>(nameof(MinValue), 0);

    /// <summary>
    /// Gets or sets the minimum allowed value for the control.
    /// This value is propagated to the internal numeric control's <c>Minimum</c>.
    /// </summary>
    public int MinValue
    {
        get => GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericUpDown"/> control.
    /// Calls <see cref="InitializeComponent"/> and hooks the internal numeric
    /// control's ValueChanged event to keep the styled <see cref="Value"/>
    /// property in sync when the user changes the value through the UI.
    /// </summary>
    public NumericUpDown()
    {
        InitializeComponent();

        nud.ValueChanged += (_, _) =>
        {
            if (nud.Value.HasValue)
                SetCurrentValue(ValueProperty, (int)nud.Value.Value);
        };
    }

    /// <summary>
    /// Called when any Avalonia styled property on this control changes.
    /// Synchronizes changes of <see cref="Value"/> and <see cref="MinValue"/>
    /// to the internal numeric control (<c>nud</c>), ensuring the visual
    /// state matches the property values.
    /// </summary>
    /// <param name="change">Information about the property change.</param>
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
