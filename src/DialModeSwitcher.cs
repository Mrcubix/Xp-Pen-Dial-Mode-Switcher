using HidSharp;
using OpenTabletDriver;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace XP_Pen.TouchWheel.Addon;

[PluginName(PLUGIN_NAME)]
public class DialModeSwitcher : IPositionedPipelineElement<IDeviceReport>
{
    #region Constants

    public const string PLUGIN_NAME = "Dial Mode Switcher";
    public const int SUPPORTED_VENDOR = 10429;

    private static readonly byte[] _mouseModeInitData = [ 0x02, 0xb4, 0x02, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 ];
    private static readonly byte[] _wheelModeInitData = [ 0x02, 0xb4, 0x02, 0x01, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00 ];
    public static readonly int[] _supportedProducts = [ 2308 ];

    #endregion

    #region Fields

    private TabletReference _tablet = null!;
    private Driver _driver = null!;

    #endregion

    #region Properties

    [TabletReference]
    public TabletReference Tablet
    {
        get => _tablet;
        set
        {
            if (value is TabletReference tablet)
            {
                _tablet = tablet;
                Initialize();
            }
        }
    }

    [Resolved]
    public IDriver Driver
    {
        get => _driver;
        set
        {
            if (value is Driver driver)
                _driver = driver;
        }
    }

    public PipelinePosition Position => PipelinePosition.None;

    [BooleanProperty("Set Dial to Mouse Mode", ""),
     DefaultPropertyValue(false),
     ToolTip("Ticked : Trackpad Mode, Unticked : Wheel Mode")]
    public bool MouseModeEnabled { get; set; }

    #endregion

    #region Event Handlers

    public event Action<IDeviceReport>? Emit;

    #endregion

    #region Methods

    public void Consume(IDeviceReport value)
        => Emit?.Invoke(value);

    public void Initialize()
    {
        if (_tablet == null || _driver == null)
        {
            Log.Write(PLUGIN_NAME, "Tablet or Driver is null", LogLevel.Error);
            return;
        }

        // fetch the device first
        var trees = _driver.InputDevices.Where(x => x.Properties.Name == Tablet.Properties.Name);

        if (!trees.Any())
        {
            Log.Write(PLUGIN_NAME, "Device not found", LogLevel.Error);
            return;
        }
        else if (trees.Count() > 1)
        {
            Log.Write(PLUGIN_NAME, "Multiple devices found", LogLevel.Warning);
        }

        var tree = trees.First();
        
        if (tree == null || tree.InputDevices.Count == 0)
        {
            Log.Write(PLUGIN_NAME, "Device or endpoint not found", LogLevel.Error);
            return;
        }

        // check if the device is supported
        if (tree.Properties.DigitizerIdentifiers.Any(x => x.VendorID != SUPPORTED_VENDOR || _supportedProducts.Contains(x.ProductID) == false))
        {
            Log.Write(PLUGIN_NAME, "Device is not supported", LogLevel.Error);
            return;
        }

        var device = tree.InputDevices[0];

        // fetch the report stream
        if (device.ReportStream is not HidStream _reportStream)
        {
            Log.Write(PLUGIN_NAME, "Failed to get report stream", LogLevel.Error);
            return;
        }

        byte[] init = MouseModeEnabled ? _mouseModeInitData : _wheelModeInitData;
        string mode = MouseModeEnabled ? "Mouse Mode" : "Wheel Mode";

        // send the init data
        try
        {
            _reportStream.Write(init);
            Log.Write(PLUGIN_NAME, $"Switched to {mode}");
        }
        catch (Exception ex)
        {
            Log.Write(PLUGIN_NAME, $"An exception occurred while switching to {mode}: {ex.Message}", LogLevel.Error);
        }
    }

    #endregion
}
