using System.Numerics;
using ImGuiNET;

namespace Neat;

public class Text
{
    public static void Default(string text)
    {
        ImGui.Text(text ?? string.Empty);
    }

    public static void Title(string text)
    {
        ImGui.PushFont(Fonts.RobotoBig);
        Default(text);
        ImGui.PopFont();
    }

    public static bool Link(string text)
    {
        var textSize = ImGui.CalcTextSize(text);
        var pos = ImGui.GetCursorScreenPos();
        var clicked = ImGui.InvisibleButton(text, textSize);
        var hovered = ImGui.IsItemHovered();

        if (hovered)
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        
        ImGui.SetCursorScreenPos(pos);
        ImGui.TextColored(hovered ? Style.ActiveTheme.Primary : Style.ActiveTheme.Text, text);
        return clicked;
    }
    
    public static void Selectable(string text, string id)
    {
        ImGui.BeginGroup();
        ImGui.PushID(id);
        {
            var textSize = ImGui.CalcTextSize(text);
            textSize.Y += ImGui.GetStyle().FramePadding.Y;

            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, Vector4.Zero);
            
            ImGui.InputTextMultiline(
                "###" + id,
                ref text,
                (uint)text.Length,
                textSize,
                ImGuiInputTextFlags.ReadOnly | ImGuiInputTextFlags.NoHorizontalScroll
            );
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();
        }
        ImGui.PopID();
        ImGui.EndGroup();
        if (ContextMenu.Begin(id + "_contextmenu"))
        {
            if (ContextMenu.Item("Copy", Icons.ClipboardCopy))
            {
                ImGui.SetClipboardText(text);
                ContextMenu.Close();
            }
            ContextMenu.End();
        }
    }

    public static void Centered(string text)
    {
        var parts = text.Split("\n");
        var size = ImGui.CalcTextSize(text);
        var y = 0f;
        foreach (var part in parts)
        {
            var partSize = ImGui.CalcTextSize(part);
            var start = ImGui.GetCursorScreenPos();
            var avail = ImGui.GetContentRegionAvail();
            var center = (avail / 2f - new Vector2(partSize.X, -size.Y) / 2f) + new Vector2(0, y);
            ImGui.SetCursorScreenPos(center);
            Default(part);
            ImGui.SetCursorScreenPos(start);
            y += partSize.Y;
        }
    }
}