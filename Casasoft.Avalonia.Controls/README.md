### Casasoft Contemporary Carte de Visite Tools

# Casasoft.Avalonia.Controls

Porting su AvaloniaUI dei controlli custom usati dalla GUI WPF (`Casasoft.Xaml.Controls`,
pacchetto NuGet privato, WPF-only). Copre la Fase 1 del piano di migrazione
WPF → Avalonia: solo i controlli generici e riusabili, non quelli specifici del
dominio CCDV (che restano/andranno riscritti in `CCDV.Avalonia/Controls`, come già
avviene oggi in `CCDV/Controls` per `ColorPickerLabelControl`, `FontSelectorControl`,
`GravityControl`, `CommonOptionsControl`, `CommonCommandsControl`,
`MultipagePreviewBarControl`, `BoxImagesControl`).

Questo progetto **non referenzia `Common.csproj`**: dipende solo da `Avalonia` e da
`Magick.NET-Q16-AnyCPU` (necessario solo per `ImageViewer.SetImage(MagickImage)`).
Resta quindi riutilizzabile anche fuori da CCDV.

## Mappa dei controlli

| WPF (`Casasoft.Xaml.Controls`) | Avalonia (`Casasoft.Avalonia.Controls`) | Note |
|---|---|---|
| `FileTextBox` | `FileTextBox` | Stessa API (`Value`, `OpenFileDialogFilter`, `OpenFileDialogTitle`). Il filtro usa la stessa sintassi WPF (`"Descrizione\|*.ext1;*.ext2\|..."`), quindi gli attributi XAML esistenti si copiano invariati. Il picker usa `IStorageProvider` (async) invece di `Microsoft.Win32.OpenFileDialog`. |
| `FileTextBoxLabel` | `FileTextBoxLabel` | Aggiunge `Caption`; inoltra le altre proprietà al `FileTextBox` interno. |
| `NumericUpDown` | `NumericUpDown` | Avvolge il controllo nativo `Avalonia.Controls.NumericUpDown` (decimal) esponendo `Value`/`MinValue` come `int`, per riusare `Value="3" MinValue="1"` così com'è. Il `ContextMenu` per i preset DPI (72/150/300/600) si attacca direttamente al controllo, senza proprietà dedicate (`ContextMenu` è già ereditata da `Control`). |
| `ImageViewer` | `ImageViewer` | **Non esiste più `.ToBitmapSource()`** (era di `Magick.NET.SystemWindowsMedia`, WPF-only). Sostituito da `imageViewer.SetImage(MagickImage)`, che converte via PNG in-memory. Espone anche `Source` (Bitmap) diretta e `Clear()`. |
| `PangoTextEditor` | `PangoTextEditor` | Semplice editor multilinea (`Value`); il markup Pango è interpretato solo lato engine (`CreditCardEngine`), non c'è rendering live né nell'originale né qui. |

## Conversione immagini: prima/dopo

```csharp
// WPF (BaseForm.bwAnteprima_RunWorkerCompleted, ecc.)
image.Source = bm.ToBitmapSource();

// Avalonia
imageViewer.SetImage(bm);
```

## Come referenziarlo da CCDV.Avalonia

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
nei form WPF attuali)

## Cosa NON è ancora incluso

- Nessun equivalente di `PortableColorPicker` (usato da `ColorPickerLabelControl` in
  CCDV): da scegliere un pacchetto community Avalonia (es. `AvaloniaColorPicker`) o
  scriverne uno minimale — è fuori scope da questo progetto, va gestito nella Fase 2
  del piano insieme a `ColorPickerLabelControl`.
- Nessun equivalente di `XamlAnimatedGif` per `WaitForm`: valutare `AvaloniaGif` o
  sostituire con uno spinner nativo (`ProgressBar` indeterminato).
- Nessuna logica di stampa (dipende dalla decisione presa nel piano di migrazione,
  §2.1 — non compete ai controlli generici).

## Requisiti

| Dipendenza | Versione minima |
|---|---|
| .NET | 10.0 |
| Avalonia | 11.2.5 |
| Magick.NET-Q16-AnyCPU | ≥ 14.14.0 |
