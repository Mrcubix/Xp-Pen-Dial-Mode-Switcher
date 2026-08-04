using System.Runtime.Serialization;

namespace XP_Pen.TouchWheel.Addon.Enums;

public enum DialModeBindingsEnum
{
    None = 0,
    Toggle = 1,

    [EnumMember(Value = "Mouse Mode")]
    Mouse = 2,

    [EnumMember(Value = "Wheel Mode")]
    Wheel = 3,
}