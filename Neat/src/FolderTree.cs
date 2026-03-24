using System;
using System.Collections;
using System.Numerics;
using ImGuiNET;
using Neat.Extensions;

namespace Neat;

public static unsafe class FolderTree
{
    public enum Mode
    {
        BRANCH,
        LEAF
    }

    public static Stack KeyList = [];

    public static void SetOpen(string key, bool open) =>
        ImGui.GetStateStorage().SetInt((uint)key.GetHashCode(), open ? 1 : 2);

    public static bool IsOpen(string key) => ImGui.GetStateStorage().GetInt((uint)key.GetHashCode()) == 1;
    public static bool HasValue(string key) => ImGui.GetStateStorage().GetInt((uint)key.GetHashCode()) != 0;

    public static bool IsItemClicked()
    {
        return ImGui.GetStateStorage().GetBool((uint)"current_tree_item_clicked".GetHashCode());
    }

    public static bool Begin(string name, Mode mode = Mode.BRANCH, bool? open = null, string? icon = null,
        bool active = false)
    {
        ImGui.GetStateStorage().SetBool((uint)"current_tree_item_clicked".GetHashCode(), false);

        var height = Fonts.RobotoSize * 1.5f;
        var key = string.Join('/', KeyList.ToArray()) + '/' + name;
        var size = ImGui.GetContentRegionAvail();
        size.Y = height;
        var pos = ImGui.GetCursorScreenPos();

        if (mode == Mode.BRANCH && open.HasValue && !HasValue(key)) SetOpen(key, open.Value);


        var clicked = ImGui.InvisibleButton(key, size);
        var hovered = ImGui.IsItemHovered();

        var isHardClicked = hovered && clicked;

        if (clicked && mode == Mode.BRANCH)
            SetOpen(key, !IsOpen(key));

        if (isHardClicked)
            ImGui.GetStateStorage().SetBool((uint)"current_tree_item_clicked".GetHashCode(), true);

        var drawList = ImGuiNative.igGetWindowDrawList();

        var state = IsOpen(key);

        if (hovered)
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        if (state || hovered || active)
        {
            Util.SquircleFilled(drawList, pos - new Vector2(Util.GetIndent(), 0), pos + size,
                active ? Style.ActiveTheme.Primary.ToHex() : Style.ActiveTheme.BorderSurface.ToHex(), 16);
        }

        var textPos = new Vector2(pos.X, pos.Y + (height - Fonts.RobotoSize) / 2);

        ImGui.SetCursorScreenPos(textPos);
        if (mode == Mode.BRANCH)
            ImGui.TextColored(active ? Style.ActiveTheme.OnPrimary : Style.ActiveTheme.TextSecondary,
                state ? Icons.ChevronDown : Icons.ChevronRight);
        ImGui.SetCursorScreenPos(textPos + new Vector2(Fonts.MediumIconSize, 0));
        ImGui.TextColored(active ? Style.ActiveTheme.OnPrimary : Style.ActiveTheme.Primary,
            icon ?? (state ? Icons.FolderOpen : Icons.FolderClosed));
        ImGui.SetCursorScreenPos(textPos + new Vector2(Fonts.MediumIconSize * 2, 0));

        ImGui.TextColored(active ? Style.ActiveTheme.OnPrimary : Style.ActiveTheme.Text, name);

        if (state)
        {
            Util.Indent(Fonts.MediumIconSize);
            KeyList.Push(name);
        }

        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(size);


        return state;
    }

    public static void End()
    {
        if (KeyList.Count == 0) throw new InvalidOperationException();
        KeyList.Pop();

        Util.Unindent(Fonts.MediumIconSize);
    }
}