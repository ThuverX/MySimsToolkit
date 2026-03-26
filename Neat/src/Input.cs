using Godot;
using ImGuiNET;
using Neat.Extensions;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace Neat;

public static unsafe class Input
{
    public static void Text(string id, ref string input, float width = 0, float height = 0, uint maxLength = 256,
        string? placeholder = null, string? icon = null, Vector4? iconColor = null,
        ImGuiInputTextFlags inputTextFlags = ImGuiInputTextFlags.None)
    {
        PreAmble(width, height, icon, iconColor, out var innerSize, out var textPos, out var pos, out var size);
        ImGui.BeginChild("_inputtext" + id, innerSize, ImGuiChildFlags.AlwaysUseWindowPadding);
        ImGui.SetNextItemWidth(innerSize.X);
        ImGui.InputText("###" + id, ref input, maxLength, inputTextFlags);
        ImGui.EndChild();
        PostAmble(input, placeholder, textPos, pos, size);

        if (ContextMenu.Begin(id + "_contextmenu"))
        {
            if (ContextMenu.Item("Cut", Icons.Scissors))
            {
                ImGui.SetClipboardText(input);
                input = "";
                ContextMenu.Close();
            }

            if (ContextMenu.Item("Copy", Icons.ClipboardCopy))
            {
                if(input.Length > 0)
                    ImGui.SetClipboardText(input);
                ContextMenu.Close();
            }

            if (ContextMenu.Item("Paste", Icons.ClipboardPaste))
            {
                input = ImGui.GetClipboardText();
                ContextMenu.Close();
            }

            ContextMenu.End();
        }
    }

    public static void Label(string label)
    {
        Neat.Text.Default(label);
    }

    private static void PostAmble(string? input, string? placeHolder, Vector2 textPos, Vector2 pos, Vector2 size)
    {
        if (input != null && input.Length <= 0 && placeHolder != null)
        {
            ImGui.SetCursorScreenPos(textPos);
            ImGui.TextColored(Style.ActiveTheme.TextSecondary, placeHolder);
        }

        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor(2);

        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(size);
        ImGui.EndGroup();
    }

    private static void PreAmble(float width, float height, string? icon, Vector4? iconColor, out Vector2 innerSize,
        out Vector2 textPos, out Vector2 pos, out Vector2 size, bool outline = true)
    {
        if (width == 0) width = (int)ImGui.GetContentRegionAvail().X;
        if (height == 0) height = Style.RootSize;

        ImGui.BeginGroup();
        ImGui.Dummy(Vector2.Zero);
        size = new Vector2(width, height);
        pos = ImGui.GetCursorScreenPos();

        var roundedPos = new Vector2(Mathf.Round(pos.X), Mathf.Round(pos.Y));
        var roundedSize = new Vector2(Mathf.Round(size.X), Mathf.Round(size.Y));

        var drawList = ImGuiNative.igGetWindowDrawList();
        if(outline)
            Util.Squircle(drawList, roundedPos, roundedPos + roundedSize,
                Style.ActiveTheme.BorderSurface.ToHex(), 16, 1);

        //unstyle default input
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.ChildBg, Vector4.Zero);

        textPos = pos + new Vector2(8, height / 2f - Fonts.RobotoSize / 2f);
        innerSize = size - new Vector2(16, (height / 2f - Fonts.RobotoSize / 2f) / 2f);

        if (icon != null)
        {
            var centered = pos + (new Vector2(height, size.Y) / 2) -
                           (new Vector2(Fonts.MediumIconSize, Fonts.MediumIconSize) / 2);
            if (icon[0] < Icons.IconMin)
                centered += new Vector2(6, 1);
            if (icon[0] >= Icons.IconMin)
                ImGui.PushFont(Fonts.MediumIcons);
            ImGui.SetCursorScreenPos(centered);

            ImGui.TextColored(iconColor ?? Style.ActiveTheme.TextSecondary, icon);
            if (icon[0] >= Icons.IconMin)
                ImGui.PopFont();

            textPos += new Vector2(height - 8, 0);
            innerSize -= new Vector2(height - 4, 0);
        }

        ImGui.SetCursorScreenPos(textPos);
    }

    public static void Float(string id, ref float input, float width = 0, int rounding = 2, float height = 0,
        string? icon = null, Vector4? iconColor = null, ImGuiInputTextFlags inputTextFlags = ImGuiInputTextFlags.None)
    {
        PreAmble(width, height, icon, iconColor, out var innerSize, out var textPos, out var pos, out var size);
        ImGui.BeginChild("_inputfloat" + id, innerSize, ImGuiChildFlags.AlwaysUseWindowPadding);
        ImGui.SetNextItemWidth(innerSize.X);
        ImGui.InputFloat("###" + id, ref input, 0, 0, "%." + rounding + "f", inputTextFlags);
        ImGui.EndChild();
        PostAmble(null, null, textPos, pos, size);
    }

    public static void Slider(string id, ref float input, float width = 0, int rounding = 2, float height = 0,
        string? icon = null, Vector4? iconColor = null, float min = 0, float max = 1)
    {
        PreAmble(width, height, icon, iconColor, out var innerSize, out var textPos, out var pos, out var size);

        var leftSize = innerSize * new Vector2(0.75f, 1);
        var rightSize = innerSize * new Vector2(0.2f, 1);

        ImGui.BeginChild("c_inputfloatslider" + id, leftSize, ImGuiChildFlags.AlwaysUseWindowPadding);
        ImGui.SetNextItemWidth(leftSize.X);

        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.001f);
        ImGui.SliderFloat("###" + "_inputfloatslider" + id, ref input, min, max, "%." + rounding + "f");
        ImGui.PopStyleVar();
        ImGui.EndChild();

        ImGui.SameLine();
        ImGui.Dummy(new Vector2(0.5f, 0));
        ImGui.SameLine();

        ImGui.BeginChild("c_inputfloatslider_text" + id, rightSize, ImGuiChildFlags.AlwaysUseWindowPadding);
        ImGui.SetNextItemWidth(rightSize.X);
        ImGui.InputFloat("###" + "_inputfloatslider_text" + id, ref input, 0, 0, "%." + rounding + "f");
        ImGui.EndChild();

        var proc = input / (max - min);
        proc = Mathf.Clamp(proc, 0, 1);
        var headPos = proc * (leftSize.X - 16) + 8;

        var drawList = ImGuiNative.igGetWindowDrawList();

        var str = string.Format("{0:F" + rounding + "}", input);
        var strSize = ImGui.CalcTextSize(str);
        var strWidth = strSize.X;

        Util.SquircleFilled(drawList, textPos + new Vector2(4, innerSize.Y / 2 - 4),
            textPos + new Vector2(leftSize.X - 2.5f, innerSize.Y / 2 - 4),
            Style.ActiveTheme.BorderSurface.ToHex(), 8);
        Util.SquircleFilled(drawList, textPos + new Vector2(4, innerSize.Y / 2 - 4),
            textPos + new Vector2(headPos, innerSize.Y / 2 - 4),
            Style.ActiveTheme.Primary.ToHex(), 8);
        ImGuiNative.ImDrawList_AddCircleFilled(drawList, textPos + new Vector2(headPos, innerSize.Y / 2 - 4), 6,
            Style.ActiveTheme.Primary.ToHex(), 16);
        ImGuiNative.ImDrawList_AddLine(drawList, pos + new Vector2(leftSize.X + 12, 0),
            pos + new Vector2(leftSize.X + 12, 0) + new Vector2(0, height), Style.ActiveTheme.BorderSurface.ToHex(), 1);
        var cPos = ImGui.GetCursorScreenPos();
        var headTextPos =
            textPos + (proc > 0.5 ? new Vector2(headPos - strWidth - 10, 0) : new Vector2(headPos + 10, 0));

        ImGui.SetCursorScreenPos(headTextPos);
        Util.SquircleFilled(drawList, headTextPos - new Vector2(4, 4),
            headTextPos + strSize + new Vector2(4, 4),
            Style.ActiveTheme.Background.ToHex(), 8);
        ImGui.Text(str);
        ImGui.SetCursorScreenPos(cPos);
        PostAmble(null, null, textPos, pos, size);
    }

    public static void Int(string id, ref int input, float width = 0, float height = 0,
        string? icon = null, Vector4? iconColor = null, ImGuiInputTextFlags inputTextFlags = ImGuiInputTextFlags.None)
    {
        PreAmble(width, height, icon, iconColor, out var innerSize, out var textPos, out var pos, out var size);
        ImGui.BeginChild("_inputfloat" + id, innerSize, ImGuiChildFlags.AlwaysUseWindowPadding);
        ImGui.SetNextItemWidth(innerSize.X);
        ImGui.InputInt("###" + id, ref input, 0, 0, inputTextFlags);
        ImGui.EndChild();
        PostAmble(null, null, textPos, pos, size);
    }
    public static void Checkbox(string id, ref bool input, float width = 0, int rounding = 2, float height = 0,
        string? icon = null, Vector4? iconColor = null)
    {
        PreAmble(width, height, icon, iconColor, out var innerSize, out var textPos, out var pos, out var size);
        ImGui.BeginChild("_inputcheckbox" + id, innerSize, ImGuiChildFlags.AlwaysUseWindowPadding);
        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.001f);
        ImGui.Checkbox("###" + id, ref input);
        var hovered = ImGui.IsItemHovered();
        ImGui.PopStyleVar();

        var drawList = ImGuiNative.igGetWindowDrawList();

        ImGui.SetCursorScreenPos(textPos);

        if (hovered)
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

        ImGui.SetCursorScreenPos(textPos + new Vector2(2, 1));
        if (input || hovered)
        {
            Util.SquircleFilled(drawList, textPos,
                textPos + new Vector2(Fonts.RobotoSize, Fonts.RobotoSize),
                hovered ? Style.ActiveTheme.Primary.ToHex() : Style.ActiveTheme.BorderSurface.ToHex(), 8);

            ImGui.TextColored(hovered ? Style.ActiveTheme.OnPrimary : Style.ActiveTheme.Text, Icons.Check);
            
        }
        else
        {
            Util.Squircle(drawList, textPos,
                textPos + new Vector2(Fonts.RobotoSize, Fonts.RobotoSize),
                Style.ActiveTheme.BorderSurface.ToHex(), 8, 1);
        }
        ImGui.EndChild();

        PostAmble(null, null, textPos, pos, size);
    }
}