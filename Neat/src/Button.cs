using Godot;
using ImGuiNET;
using Neat.Extensions;
using Vector2 = System.Numerics.Vector2;

namespace Neat;

public static unsafe class Button
{
    public static bool WithIcon(string icon, string text, float width = 0)
    {
        return Default($"{icon}   {text}", width);
    }

    public static bool Icon(string icon)
    {
        ImGui.BeginGroup();
        ImGui.Dummy(Vector2.Zero);

        ImGui.PushFont(Fonts.MediumIcons);
        var size = new Vector2(Style.RootSize, Style.RootSize);
        var pos = ImGui.GetCursorScreenPos();
        
        var clicked = ImGui.InvisibleButton(icon, size);
        var hovered = ImGui.IsItemHovered();
        
        pos = new Vector2(Mathf.Floor(pos.X), Mathf.Floor(pos.Y));
        size = new Vector2(Mathf.Floor(size.X), Mathf.Floor(size.Y));
        
        ImGui.SetCursorScreenPos(pos);
        
        var drawList = ImGuiNative.igGetWindowDrawList();

        if (hovered)
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        
        Util.Squircle(drawList, pos, pos + size, Style.ActiveTheme.BorderSurface.ToHex(), 16, 1);
        Util.SquircleFilled(drawList, pos, pos + size,
            hovered ? Style.ActiveTheme.Primary.ToHex() : Style.ActiveTheme.Background.ToHex(), 16);
        var centered = pos + (size / 2) - (new Vector2(Fonts.MediumIconSize, Fonts.MediumIconSize) / 2);
        ImGui.SetCursorScreenPos(centered);
        ImGui.TextColored(hovered ? Style.ActiveTheme.OnPrimary : Style.ActiveTheme.Text, icon);
        
        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(size);
        ImGui.PopFont();
        ImGui.EndGroup();

        return clicked;
    }
    
    public static bool Default(string text, float width = 0, float height = 0)
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
        
        Util.Squircle(drawList, pos, pos + size, Style.ActiveTheme.BorderSurface.ToHex(), 16,  1);
        Util.SquircleFilled(drawList, pos, pos + size,
            hovered ? Style.ActiveTheme.Primary.ToHex() : Style.ActiveTheme.Background.ToHex(), 16);
        var centered = pos + (size / 2) - (ImGui.CalcTextSize(text) / 2);
        ImGui.SetCursorScreenPos(centered);
        ImGui.TextColored(hovered ? Style.ActiveTheme.OnPrimary : Style.ActiveTheme.Text, text);
        
        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(size);
        ImGui.EndGroup();
        
        return clicked;
    }
}