using Godot;
using ImGuiNET;
using Neat.Extensions;
using Vector2 = System.Numerics.Vector2;

namespace Neat;

public static unsafe class ContextMenu
{
    public const int MinWidth = 128;
    private static int _index = 0;
    
    public static bool Begin(string id, ImGuiPopupFlags flags = ImGuiPopupFlags.MouseButtonRight)
    {
        return ImGui.BeginPopupContextItem(id, flags);
    }

    public static bool Item(string text, string? icon = null)
    {
        ImGui.PushFont(Fonts.Roboto);

        _index++;
        var height = Fonts.RobotoSize * 1.5f;
        var pos = ImGui.GetCursorScreenPos();
        var size = ImGui.CalcTextSize(text);
        size.Y = height;
        size.X = Mathf.Max(size.X + 8, MinWidth);
        size.X += 4 + height;
        
        var textPos = new Vector2(pos.X + 4 + height, pos.Y + (height - Fonts.RobotoSize) / 2);

        var drawList = ImGuiNative.igGetWindowDrawList();
        
        ImGui.SetCursorScreenPos(pos + new Vector2(0,4));
        
        var clicked = ImGui.InvisibleButton(text + _index, size - new Vector2(0, 16));
        var hovered = ImGui.IsItemHovered();

        if (hovered)
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            var innerSize = size with { X = ImGui.GetContentRegionAvail().X };
            Util.SquircleFilled(drawList, pos, pos + innerSize, Style.ActiveTheme.BorderSurface.ToHex(), 8);
        }

        if (icon != null)
        {
            var iconPos = textPos + new Vector2(-height + (height/ 2) - Fonts.RobotoSize / 2f, 0);
            ImGui.SetCursorScreenPos(iconPos);
            ImGui.TextColored(Style.ActiveTheme.Primary,icon);
        }

        ImGui.SetCursorScreenPos(textPos);
        ImGui.Text(text);
        ImGui.PopFont();

        return clicked;
    }


    public static void Close()
    {
        ImGui.CloseCurrentPopup();
    }

    public static void Seperator()
    {
        ImGui.Dummy(new Vector2(0, 2));
        Util.HorizontalLine(Style.ActiveTheme.BorderSurface, 1);
        ImGui.Dummy(new Vector2(0, 2));
    }

    public static void End()
    {
        ImGui.Dummy(new Vector2(0, 2));
        ImGui.EndPopup();
        _index = 0;
    }
}