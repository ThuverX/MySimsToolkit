using Godot;
using ImGuiNET;
using Neat.Extensions;
using Vector2 = System.Numerics.Vector2;

namespace Neat;

public static class Tabs
{
    public static void BeginTabs()
    {
        ImGui.BeginGroup();
    }

    public static void TabItem(ref string currentTab, string tabName)
    {
        
        if (Button(tabName, currentTab == tabName))
        {
            currentTab = tabName;
        }
        
        ImGui.SameLine();
    }
    
    private static unsafe bool Button(string text, bool active, float width = 0, float height = 0)
    {
        if(width == 0)
            width = ImGui.CalcTextSize(text).X + Style.RootSize;
        ImGui.BeginGroup();
        ImGui.Dummy(Vector2.Zero);
        var size = new Vector2(width, height != 0 ? height : Style.RootSize);
        var pos = ImGui.GetCursorScreenPos();
        
        var clicked = ImGui.InvisibleButton(text, size);
        var hovered = ImGui.IsItemHovered();
        
        pos = new Vector2(Mathf.Round(pos.X), Mathf.Round(pos.Y));
        size = new Vector2(Mathf.Round(size.X), Mathf.Round(size.Y));
        
        ImGui.SetCursorScreenPos(pos);
        
        var drawList = ImGuiNative.igGetWindowDrawList();


        if (hovered)
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        
        if(active) hovered = true;
        
        Util.Squircle(drawList, pos, pos + size, Style.ActiveTheme.BorderSurface.ToHex(), 16,  1, corners: Util.SquircleCorners.Top | Util.SquircleCorners.Left | Util.SquircleCorners.Right);
        Util.SquircleFilled(drawList, pos, pos + size,
            hovered ? Style.ActiveTheme.Primary.ToHex() : Style.ActiveTheme.Background.ToHex(), 16, corners: Util.SquircleCorners.Top | Util.SquircleCorners.Left | Util.SquircleCorners.Right);
        var centered = pos + (size / 2) - (ImGui.CalcTextSize(text) / 2);
        ImGui.SetCursorScreenPos(centered);
        ImGui.TextColored(hovered ? Style.ActiveTheme.OnPrimary : Style.ActiveTheme.Text, text);
        
        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(size);
        ImGui.EndGroup();
        
        return clicked;
    }

    public static void EndTabs()
    {
        ImGui.Dummy(Vector2.Zero);
        Util.HorizontalLine(Style.ActiveTheme.BorderSurface, 1);
        ImGui.EndGroup();
    }
}