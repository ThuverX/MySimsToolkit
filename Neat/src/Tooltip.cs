using ImGuiNET;

namespace Neat;

public static unsafe class Tooltip
{
    public static void Text(string text)
    {
        if (ImGui.BeginItemTooltip())
        {
            ImGui.Text(text);
            ImGui.EndTooltip();
        }
    }
}