using System.Numerics;
using ImGuiNET;
using Neat.Extensions;

namespace Neat;

public static unsafe class Pills
{
    public static  void Default(string text)
    {
        var pos = ImGui.GetCursorScreenPos();
        ImGui.PushFont(Fonts.RobotoTiny);
        
        var size = ImGui.CalcTextSize(text) + new Vector2(Util.GetGap() * 4, 4);
        var drawList = ImGui.GetWindowDrawList();
        
        ImGuiNative.ImDrawList_AddRectFilled(drawList, pos, pos + size, Style.ActiveTheme.Primary.ToHex(), 16, ImDrawFlags.RoundCornersAll);
        
        ImGui.Dummy(new Vector2(Util.GetGap() / 4f, 2));
        ImGui.SameLine();
        ImGui.SetCursorScreenPos(ImGui.GetCursorScreenPos() + new Vector2(0,2));
        ImGui.TextColored(Style.ActiveTheme.White, text);
        ImGui.PopFont();
    }
}