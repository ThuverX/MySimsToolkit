using System.Numerics;
using ImGuiNET;

namespace Neat;

public static class ListItem
{
    public static void Default(string text)
    {
        ImGui.Dummy(new Vector2(0, 2));
        Text.Default(Icons.ChevronRight + '\t' + text);
        ImGui.Dummy(new Vector2(0, 2));
    }

    public static bool Link(string text)
    {
        ImGui.Dummy(new Vector2(0, 2));
        var ret = Text.Link(Icons.ChevronRight + '\t' + text);
        ImGui.Dummy(new Vector2(0, 2));
        return ret;
    }
}