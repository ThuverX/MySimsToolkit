using System.Numerics;
using ImGuiNET;

namespace Neat;

public static class Window
{
    public static bool Begin(string name)
    {
        var res = ImGui.Begin(name);

        return res;
    }

    public static void BeginRootWindow()
    {
        ImGui.SetNextWindowPos(new Vector2(0,23));
        ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize - new Vector2(0,23));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
        ImGui.Begin("__root",  ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize);
    }

    public static void EndRootWindow()
    {
        ImGui.End();
        ImGui.PopStyleVar();
    }

    public static void End()
    {
        ImGui.End();
    }
}