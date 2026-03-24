using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using Neat.Extensions;

namespace Neat;

public static unsafe class Table
{
    public struct SortingSettings
    {
        public string? Key;
        public bool Ascending;
    }

    public static int RowHeight = Style.RootSize;

    public static bool Header(string id, ref SortingSettings settings, List<string> headers)
    {
        var sortingChanged = false;
        ImGui.BeginTable(id, headers.Count);

        int idx = 0;
        foreach (var header in headers)
        {
            ImGui.TableNextColumn();
            ImGui.BeginGroup();
            
            var pos = ImGui.GetCursorScreenPos();
            var size = new Vector2(ImGui.GetColumnWidth(), Style.RootSize);
            
            var drawList = ImGuiNative.igGetWindowDrawList();

            var flags = ImDrawFlags.RoundCornersNone;
            if (idx == 0)
                flags = ImDrawFlags.RoundCornersLeft;
            if(idx == headers.Count - 1)
                flags = ImDrawFlags.RoundCornersRight;
        
            Util.SquircleFilled(drawList, pos, pos + size, Style.ActiveTheme.BorderSurface.ToHex(), 16);

            var clicked = ImGui.InvisibleButton(header, size);
            var hovered = ImGui.IsItemHovered();
            
            if(hovered)
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

            if (clicked)
            {
                if (settings.Key == header) settings.Ascending = !settings.Ascending;
                else settings.Ascending = false;
                settings.Key = header;
                sortingChanged = true;
            }

            var centerPos = pos + new Vector2(Style.RootSize / 4f, size.Y / 2 - Fonts.RobotoSize / 2f);
            ImGui.SetCursorScreenPos(centerPos);

            ImGui.TextColored(hovered ? Style.ActiveTheme.Primary : Style.ActiveTheme.Text, header);

            if (settings.Key == header)
            {
                ImGui.SameLine();
                ImGui.TextColored(hovered ? Style.ActiveTheme.Primary : Style.ActiveTheme.Text, settings.Ascending ? Icons.ChevronUp : Icons.ChevronDown);
            }
            
            ImGui.SetCursorScreenPos(pos);
            
            ImGui.Dummy(size);
            ImGui.EndGroup();
            idx++;
        }

        return sortingChanged;
    }

    public static void Row()
    {
        ImGui.TableNextRow();
    }

    public static bool Column(string text, string? icon = null, bool monospaced = false, bool clickable = false, bool selectable = false)
    {
        ImGui.TableNextColumn();
        ImGui.BeginGroup();
            
        var pos = ImGui.GetCursorScreenPos();
        var size = new Vector2(ImGui.GetColumnWidth(), Style.RootSize);
        var clicked = ImGui.InvisibleButton("###row_" + ImGui.TableGetRowIndex(), size);
        var hovered = ImGui.IsItemHovered();
        if (!clickable) hovered = false;
        ImGui.SetCursorScreenPos(pos);
    
        var drawList = ImGuiNative.igGetWindowDrawList();
    
        var centerPos = pos + new Vector2(Style.RootSize / 4f, size.Y / 2 - Fonts.RobotoSize / 2f);
        ImGui.SetCursorScreenPos(centerPos);

        if (hovered)
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

            var innerPos = pos + new Vector2(0, 4);
            var innerSize = size - new Vector2(0, 4 * 2);
        
            Util.SquircleFilled(drawList, innerPos, innerPos + innerSize, Style.ActiveTheme.BorderSurface.ToHex(), 16);
        }

        if (icon != null)
        {
            ImGui.TextColored(Style.ActiveTheme.Text, icon);
            ImGui.SetCursorScreenPos(centerPos + new Vector2(Fonts.MediumIconSize, 0));
        }
    
        if (monospaced)
            ImGui.PushFont(Fonts.RobotoMono);
    
        if(selectable)
            Text.Selectable(text, "###selectable_" + ImGui.TableGetRowIndex() + "" + ImGui.TableGetColumnIndex());
        else
            ImGui.TextColored(Style.ActiveTheme.Text, text);
    
        if (monospaced)
            ImGui.PopFont();
    
        ImGui.SetCursorScreenPos(pos);
    
        ImGuiNative.ImDrawList_AddLine(drawList,pos  + new Vector2(0, size.Y), pos + size, Style.ActiveTheme.BorderSurface.ToHex(), 1);

        ImGui.EndGroup();
        return clicked;
    }

    public static void End()
    {
        ImGui.EndTable();
    }
}