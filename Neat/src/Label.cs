using Godot;
using ImGuiNET;
using Neat.Extensions;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace Neat;

public static unsafe class Label
{
    private const int MinWidth = 128 + 32;
    private const int Height = Fonts.RobotoSize + 12;

    private static void PreLabel(string label)
    {
        ImGui.BeginGroup();
        ImGui.Dummy(System.Numerics.Vector2.Zero);
        var pos = ImGui.GetCursorScreenPos();
        var size = ImGui.CalcTextSize(label, (float)MinWidth);
        size.X = MinWidth - Util.GetIndent();
        size.Y = Mathf.Max(size.Y, Height);
        var centered = pos + new Vector2(4, 4);
        ImGui.SetCursorScreenPos(centered);
        ImGui.BeginChild(label + "_prelabel", size);
        ImGui.TextWrapped(label);
        ImGui.EndChild();
        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(size);
        ImGui.EndGroup();
    }

    public static void Text(string label, ref string text)
    {
        ImGui.BeginGroup();

        PreLabel(label);
        ImGui.SameLine();

        var itemHeight = ImGui.GetItemRectSize().Y;
        var pos = ImGui.GetCursorScreenPos();
        var centered = pos + new Vector2(0, itemHeight / 2f - Height / 2f - 4);

        ImGui.SetCursorScreenPos(centered);
        Input.Text(label + "_label", ref text, height: Height);
        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X, itemHeight));

        ImGui.EndGroup();
    }
    
    public static void Readonly(string label, string text)
    {
        ImGui.BeginGroup();

        PreLabel(label);
        ImGui.SameLine();

        var itemHeight = ImGui.GetItemRectSize().Y;
        var pos = ImGui.GetCursorScreenPos();
        var centered = pos + new Vector2(0, itemHeight / 2f - Height / 2f - 4);

        ImGui.SetCursorScreenPos(centered);
        Input.Text(label + "_label", ref text, height: Height, inputTextFlags: ImGuiInputTextFlags.ReadOnly);
        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X, itemHeight));

        ImGui.EndGroup();
    }

    public static void Float(string label, ref float value, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        ImGui.BeginGroup();

        PreLabel(label);
        ImGui.SameLine();

        var itemHeight = ImGui.GetItemRectSize().Y;
        var pos = ImGui.GetCursorScreenPos();
        var centered = pos + new Vector2(0, itemHeight / 2f - Height / 2f - 4);

        ImGui.SetCursorScreenPos(centered);
        Input.Float(label + "_label_x", ref value, height: Height, inputTextFlags: flags);

        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X, itemHeight));

        ImGui.EndGroup();
    }

    public static void Int(string label, ref int value, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        ImGui.BeginGroup();

        PreLabel(label);
        ImGui.SameLine();

        var itemHeight = ImGui.GetItemRectSize().Y;
        var pos = ImGui.GetCursorScreenPos();
        var centered = pos + new Vector2(0, itemHeight / 2f - Height / 2f - 4);

        ImGui.SetCursorScreenPos(centered);
        Input.Int(label + "_label_x", ref value, height: Height, inputTextFlags: flags);

        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X, itemHeight));

        ImGui.EndGroup();
    }

    public static void Vector2(string label, ref Vector2 vector, int rounding = 2, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        ImGui.BeginGroup();

        PreLabel(label);
        ImGui.SameLine();

        var itemHeight = ImGui.GetItemRectSize().Y;
        var pos = ImGui.GetCursorScreenPos();
        var centered = pos + new Vector2(0, itemHeight / 2f - Height / 2f - 4);
        var itemWidth = ImGui.GetContentRegionAvail().X / 2 - 4;

        ImGui.SetCursorScreenPos(centered);
        Input.Float(label + "_label_x", ref vector.X, height: Height, rounding: rounding, width: itemWidth, inputTextFlags: flags);
        ImGui.SameLine();
        Input.Float(label + "_label_y", ref vector.Y, height: Height, rounding: rounding, width: itemWidth, inputTextFlags: flags);

        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X, itemHeight));

        ImGui.EndGroup();
    }

    public static void Vector3(string label, ref Vector3 vector, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        ImGui.BeginGroup();

        PreLabel(label);
        ImGui.SameLine();

        var itemHeight = ImGui.GetItemRectSize().Y;
        var pos = ImGui.GetCursorScreenPos();
        var centered = pos + new Vector2(0, itemHeight / 2f - Height / 2f - 4);
        var itemWidth = ImGui.GetContentRegionAvail().X / 3 - 6;

        ImGui.SetCursorScreenPos(centered);
        Input.Float(label + "_label_x", ref vector.X, height: Height, width: itemWidth, inputTextFlags: flags);
        ImGui.SameLine();
        Input.Float(label + "_label_y", ref vector.Y, height: Height, width: itemWidth, inputTextFlags: flags);
        ImGui.SameLine();
        Input.Float(label + "_label_z", ref vector.Z, height: Height, width: itemWidth, inputTextFlags: flags);

        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X, itemHeight));

        ImGui.EndGroup();
    }

    public static void Position(string label, ref Vector3 vector, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        ImGui.BeginGroup();

        PreLabel(label);
        ImGui.SameLine();

        var itemHeight = ImGui.GetItemRectSize().Y;
        var pos = ImGui.GetCursorScreenPos();
        var centered = pos + new Vector2(0, itemHeight / 2f - Height / 2f - 4);
        var itemWidth = ImGui.GetContentRegionAvail().X / 3 - 6;

        ImGui.SetCursorScreenPos(centered);
        Input.Float(label + "_label_x", ref vector.X, height: Height, width: itemWidth, icon: "X",
            iconColor: Style.AxisColorX, inputTextFlags: flags);
        ImGui.SameLine();
        Input.Float(label + "_label_y", ref vector.Y, height: Height, width: itemWidth, icon: "Y",
            iconColor: Style.AxisColorY, inputTextFlags: flags);
        ImGui.SameLine();
        Input.Float(label + "_label_z", ref vector.Z, height: Height, width: itemWidth, icon: "Z",
            iconColor: Style.AxisColorZ, inputTextFlags: flags);

        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X, itemHeight));

        ImGui.EndGroup();
    }

    public static void Quaternion(string label, ref Quaternion quat, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        ImGui.BeginGroup();

        PreLabel(label);
        ImGui.SameLine();

        var itemHeight = ImGui.GetItemRectSize().Y;
        var pos = ImGui.GetCursorScreenPos();
        var centered = pos + new Vector2(0, itemHeight / 2f - Height / 2f - 4);
        var itemWidth = ImGui.GetContentRegionAvail().X / 4 - 6;

        ImGui.SetCursorScreenPos(centered);
        Input.Float(label + "_label_x", ref quat.X, height: Height, width: itemWidth, inputTextFlags: flags);
        ImGui.SameLine();
        Input.Float(label + "_label_y", ref quat.Y, height: Height, width: itemWidth, inputTextFlags: flags);
        ImGui.SameLine();
        Input.Float(label + "_label_z", ref quat.Z, height: Height, width: itemWidth, inputTextFlags: flags);
        ImGui.SameLine();
        Input.Float(label + "_label_w", ref quat.W, height: Height, width: itemWidth, inputTextFlags: flags);

        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X, itemHeight));

        ImGui.EndGroup();
    }

    public static void Vector4(string label, ref Vector4 vector, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        ImGui.BeginGroup();

        PreLabel(label);
        ImGui.SameLine();

        var itemHeight = ImGui.GetItemRectSize().Y;
        var pos = ImGui.GetCursorScreenPos();
        var centered = pos + new Vector2(0, itemHeight / 2f - Height / 2f - 4);
        var itemWidth = ImGui.GetContentRegionAvail().X / 4 - 6;

        ImGui.SetCursorScreenPos(centered);
        Input.Float(label + "_label_x", ref vector.X, height: Height, width: itemWidth, inputTextFlags: flags);
        ImGui.SameLine();
        Input.Float(label + "_label_y", ref vector.Y, height: Height, width: itemWidth, inputTextFlags: flags);
        ImGui.SameLine();
        Input.Float(label + "_label_z", ref vector.Z, height: Height, width: itemWidth, inputTextFlags: flags);
        ImGui.SameLine();
        Input.Float(label + "_label_w", ref vector.W, height: Height, width: itemWidth, inputTextFlags: flags);

        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X, itemHeight));

        ImGui.EndGroup();
    }

    public static void Slider(string label, ref float value, float min = 0, float max = 1)
    {
        ImGui.BeginGroup();

        PreLabel(label);
        ImGui.SameLine();

        var itemHeight = ImGui.GetItemRectSize().Y;
        var pos = ImGui.GetCursorScreenPos();
        var centered = pos + new Vector2(0, itemHeight / 2f - Height / 2f - 4);

        ImGui.SetCursorScreenPos(centered);
        Input.Slider(label + "_label", ref value, height: Height, min: min, max: max);

        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X, itemHeight));

        ImGui.EndGroup();
    }

    public static void CheckBox(string label, ref bool value)
    {
        ImGui.BeginGroup();

        PreLabel(label);
        ImGui.SameLine();

        var itemHeight = ImGui.GetItemRectSize().Y;
        var pos = ImGui.GetCursorScreenPos();
        var centered = pos + new Vector2(0, itemHeight / 2f - Height / 2f - 4);

        ImGui.SetCursorScreenPos(centered);
        Input.Checkbox(label + "_label", ref value, height: Height);

        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X, itemHeight));

        ImGui.EndGroup();
    }

    public static bool BeginSection(string label)
    {
        ImGui.BeginGroup();
        ImGui.Dummy(System.Numerics.Vector2.Zero);

        var state = !ImGui.GetStateStorage().GetBool((uint)(label + "_collapsing_header").GetHashCode());

        var pos = ImGui.GetCursorScreenPos();
        var size = ImGui.CalcTextSize(label);
        size.X = ImGui.GetContentRegionAvail().X;
        size.Y = Mathf.Max(size.Y, Height);

        if (ImGui.InvisibleButton(label + "_collapsing_header_button", size))
            ImGui.GetStateStorage().SetBool((uint)(label + "_collapsing_header").GetHashCode(), state);

        var hovered = ImGui.IsItemHovered();

        if (hovered)
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            var drawList = ImGuiNative.igGetWindowDrawList();
            Util.SquircleFilled(drawList, pos,
                pos + size - new Vector2(0, 2),
                Style.ActiveTheme.BorderSurface.ToHex(), 16);
        }

        var textPos = pos + new Vector2(4, 4);

        ImGui.SetCursorScreenPos(textPos);
        ImGui.Text((state ? Icons.ChevronDown : Icons.ChevronRight) + " " + label);

        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(new Vector2(size.X, size.Y));
        if (state)
            Util.Indent(20);

        return state;
    }

    public static void EndSection()
    {
        Util.Unindent(20);
        ImGui.EndGroup();
        Util.HorizontalLine(Style.ActiveTheme.BorderSurface, 1);
    }
}