using System.Numerics;
using ImGuiNET;

namespace Neat.Extensions;

public static class Vector4Extensions
{
    public static uint ToHex(this Vector4 value)
    {
        return ImGui.ColorConvertFloat4ToU32(value);
    }
}