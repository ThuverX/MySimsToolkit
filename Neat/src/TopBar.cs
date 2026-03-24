using System.Numerics;
using ImGuiNET;

namespace Neat;

public static class TopBar
{
    public static void Begin()
    {
        ImGui.BeginGroup();
    }

    public static void End()
    {
        ImGui.EndGroup();
        ImGui.Dummy(new Vector2(0,2));
    }
}