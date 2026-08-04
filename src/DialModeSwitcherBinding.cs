using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using XP_Pen.TouchWheel.Addon.Enums;

namespace XP_Pen.TouchWheel.Addon;

[PluginName(PLUGIN_NAME)]
public class DialModeSwitcherBinding : DialModeSwitcherBase, IStateBinding
{
    public const string PLUGIN_NAME = "Dial Modes Bindings";
    private DialModeBindingsEnum _value;
    private bool _mouseModeEnabled = false;

    public readonly static string[] Bindings = [ "Toggle", "Mouse", "Wheel" ];

    [Property("Selected Mode"), 
     DefaultPropertyValue("Toggle"),
     ToolTip("Dial Mode Bindings: \n\n" +
             "Set the wheel mode to either Mouse Mode or Wheel Mode on XP-Pen Deco Pro Small and Medium\n" +
             "Toggle: Toggle between Mouse Mode and Wheel Mode\n" +
             "Mouse Mode: Set the wheel mode to Mouse Mode\n" +
             "Wheel Mode: Set the wheel mode to Wheel Mode"),
     PropertyValidated(nameof(Bindings))]
    public string? Selected
    {
        get => Bindings[(int)_value];
        set
        {
            try
            {
                _value = Enum.Parse<DialModeBindingsEnum>(value ?? "None");
            }
            catch(Exception ex)
            {
                Log.Write(PLUGIN_NAME, $"An exception occurred while parsing Selected Mode: {ex.Message}", LogLevel.Error);
            }
        } 
    }

    public void Press(TabletReference tablet, IDeviceReport report)
    {
        if (_value == DialModeBindingsEnum.None)
            return;

        switch (_value)
        {
            case DialModeBindingsEnum.Toggle:
                SetDialMode(_mouseModeEnabled = !_mouseModeEnabled);
                break;
            case DialModeBindingsEnum.Mouse:
                SetDialMode(true);
                break;
            case DialModeBindingsEnum.Wheel:
                SetDialMode(false);
                break;
        }
    }

    public void Release(TabletReference tablet, IDeviceReport report) { }
}