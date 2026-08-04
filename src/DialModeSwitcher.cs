using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace XP_Pen.TouchWheel.Addon;

[PluginName(PLUGIN_NAME)]
public class DialModeSwitcher : DialModeSwitcherBase, IPositionedPipelineElement<IDeviceReport>
{
    #region Constants

    public const string PLUGIN_NAME = "Dial Mode Switcher";

    #endregion

    #region Fields

    private bool _isInitialized = false;

    #endregion

    #region Properties

    public PipelinePosition Position => PipelinePosition.None;

    [BooleanProperty("Set Dial to Mouse Mode", ""),
     DefaultPropertyValue(false),
     ToolTip("Dial Mode Switcher: \n\n" + 
             "Set the current mode for the dial on XP-Pen Deco Pro Small & Medium to the selected value. \n" +
             "Ticked : Trackpad Mode, Unticked : Wheel Mode")]
    public bool MouseModeEnabled { get; set; }

    #endregion

    #region Event Handlers

    public event Action<IDeviceReport>? Emit;

    #endregion

    #region Methods

    public void Consume(IDeviceReport value)
        => Emit?.Invoke(value);

    public override void PostInitialize()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;
            SetDialMode(MouseModeEnabled);
        }
    }

    #endregion
}
