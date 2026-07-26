# Casasoft.Avalonia.Controls

Porting su AvaloniaUI dei controlli custom del pacchetto NuGet WPF-only
[`Casasoft.Xaml.Controls`](https://www.nuget.org/packages/Casasoft.Xaml.Controls):
`FileTextBox`, `FileTextBoxLabel`, `NumericUpDown`, `ImageViewer`, `PangoTextEditor`.

Questo progetto **non referenzia altri progetti**: dipende solo da `Avalonia` e da
`Magick.NET-Q16-AnyCPU` (necessario solo per `ImageViewer.SetImage(MagickImage)`).
È quindi riutilizzabile in qualsiasi applicazione Avalonia, indipendentemente
dal progetto originario in cui è nato `Casasoft.Xaml.Controls`.

## Mappa dei controlli

| WPF (`Casasoft.Xaml.Controls`) | Avalonia (`Casasoft.Avalonia.Controls`) | Note |
|---|---|---|
| `FileTextBox` | `FileTextBox` | Stessa API (`Value`, `OpenFileDialogFilter`, `OpenFileDialogTitle`). Il filtro usa la stessa sintassi WPF (`"Descrizione\|*.ext1;*.ext2\|..."`), quindi gli attributi XAML esistenti si copiano invariati. Il picker usa `IStorageProvider` (async) invece di `Microsoft.Win32.OpenFileDialog`. |
| `FileTextBoxLabel` | `FileTextBoxLabel` | Aggiunge `Caption`; inoltra le altre proprietà al `FileTextBox` interno. |
| `NumericUpDown` | `NumericUpDown` | Avvolge il controllo nativo `Avalonia.Controls.NumericUpDown` (decimal) esponendo `Value`/`MinValue` come `int`, per riusare `Value="3" MinValue="1"` così com'è. Il `ContextMenu` si attacca direttamente al controllo, senza proprietà dedicate (`ContextMenu` è già ereditata da `Control`). |
| `ImageViewer` | `ImageViewer` | **Non esiste più `.ToBitmapSource()`** (era di `Magick.NET.SystemWindowsMedia`, WPF-only). Sostituito da `imageViewer.SetImage(MagickImage)`, che converte via PNG in-memory. Espone anche `Source` (Bitmap) diretta e `Clear()`. |
| `PangoTextEditor` | `PangoTextEditor` | Semplice editor multilinea (`Value`); il markup Pango è interpretato solo lato engine consumer, non c'è rendering live né nell'originale né qui. |

## Conversione immagini: prima/dopo

```csharp
// WPF
image.Source = bm.ToBitmapSource();

// Avalonia
imageViewer.SetImage(bm);
```

## Come referenziarlo

```xml
<ItemGroup>
  <ProjectReference Include="..\Casasoft.Avalonia.Controls\Casasoft.Avalonia.Controls.csproj" />
</ItemGroup>
```

E nel namespace XAML dei form:

```xml
xmlns:casasoft="clr-namespace:Casasoft.Avalonia.Controls;assembly=Casasoft.Avalonia.Controls"
```

(stesso ruolo di `xmlns:casasoft="clr-namespace:Casasoft.Xaml.Controls;assembly=Casasoft.Xaml.Controls"`
nei form WPF che usano il pacchetto originale)

## Cosa NON è ancora incluso

- Nessun equivalente di `PortableColorPicker`: da scegliere un pacchetto community
  Avalonia (es. `AvaloniaColorPicker`) o scriverne uno minimale.
- Nessun equivalente di `XamlAnimatedGif`: valutare `AvaloniaGif` o sostituire con
  uno spinner nativo (`ProgressBar` indeterminato).

## Requisiti

| Dipendenza | Versione minima |
|---|---|
| .NET | 10.0 |
| Avalonia | 11.2.5 |
| Magick.NET-Q16-AnyCPU | ≥ 14.15.0 |