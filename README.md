# WinForms ImageButton

Small sample button for an old ImageButton component.

![demo](demo.png)

Use the attached buttons (png) or create your own.

## Supported features

* Property `Transparent`: Transparency (determened by the color of the top-left pixel)
* Property `ClickOnHold`: Simulates long pressing a physical keyboard
* Automatic Disabled state (greyscale)
* Pushed state
* Pushed state based on a keyboard click

```csharp
private void Form1_KeyPress(object sender, KeyPressEventArgs e)
{
    if (e.KeyChar == '0')
    {
        Button0.PerformClick(200);
    }
}
```

