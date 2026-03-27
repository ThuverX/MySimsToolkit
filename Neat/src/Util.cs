using System;
using System.Collections.Generic;
using Godot;
using ImGuiNET;
using Neat.Extensions;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace Neat;

public static unsafe class Util
{
    public static void Seperator()
    {
        HorizontalLine(Style.ActiveTheme.BorderSurface, 1);
    }

    public static void VerticalSeperator()
    {
        VerticalLine(Style.ActiveTheme.BorderSurface, 1);
    }

    public static void HorizontalLine(Vector4 color, int thickness)
    {
        var width = ImGui.GetContentRegionAvail().X;
        var drawList = ImGuiNative.igGetWindowDrawList();
        var pos = ImGui.GetCursorScreenPos();
        ImGuiNative.ImDrawList_AddLine(drawList, pos, pos + new Vector2(width, 0), color.ToHex(), thickness);
        ImGui.Dummy(new Vector2(width, thickness));
    }

    public static void VerticalLine(Vector4 color, int thickness)
    {
        var height = ImGui.GetContentRegionAvail().Y;
        var drawList = ImGuiNative.igGetWindowDrawList();
        var pos = ImGui.GetCursorScreenPos();
        ImGuiNative.ImDrawList_AddLine(drawList, pos, pos + new Vector2(0, height), color.ToHex(), thickness);
        ImGui.Dummy(new Vector2(thickness, height));
    }

    [Flags]
    public enum SquircleCorners
    {
        None = 0,
        Top = 1 << 0,
        Right = 1 << 1,
        Bottom = 1 << 2,
        Left = 1 << 3,

        All = Top | Right | Bottom | Left
    }

    private static void SquircleBase(
        ImDrawListPtr drawList,
        Vector2 minPos,
        Vector2 maxPos,
        float radius,
        SquircleCorners corners)
    {
        float startX = minPos.X;
        float endX = maxPos.X;
        float startY = minPos.Y;
        float endY = maxPos.Y;

        bool roundTop = corners.HasFlag(SquircleCorners.Top);
        bool roundRight = corners.HasFlag(SquircleCorners.Right);
        bool roundBottom = corners.HasFlag(SquircleCorners.Bottom);
        bool roundLeft = corners.HasFlag(SquircleCorners.Left);

        float rTL = (roundTop && roundLeft) ? radius : 0f;
        float rTR = (roundTop && roundRight) ? radius : 0f;
        float rBR = (roundBottom && roundRight) ? radius : 0f;
        float rBL = (roundBottom && roundLeft) ? radius : 0f;

        drawList.PathLineTo(new Vector2(startX, startY + rTL));

        if (rTL > 0)
        {
            drawList.PathBezierCubicCurveTo(
                new Vector2(startX, startY),
                new Vector2(startX, startY),
                new Vector2(startX + rTL, startY));
        }
        else
        {
            drawList.PathLineTo(new Vector2(startX, startY));
        }

        drawList.PathLineTo(new Vector2(endX - rTR, startY));

        if (rTR > 0)
        {
            drawList.PathBezierCubicCurveTo(
                new Vector2(endX, startY),
                new Vector2(endX, startY),
                new Vector2(endX, startY + rTR));
        }
        else
        {
            drawList.PathLineTo(new Vector2(endX, startY));
        }

        drawList.PathLineTo(new Vector2(endX, endY - rBR));

        if (rBR > 0)
        {
            drawList.PathBezierCubicCurveTo(
                new Vector2(endX, endY),
                new Vector2(endX, endY),
                new Vector2(endX - rBR, endY));
        }
        else
        {
            drawList.PathLineTo(new Vector2(endX, endY));
        }

        drawList.PathLineTo(new Vector2(startX + rBL, endY));

        if (rBL > 0)
        {
            drawList.PathBezierCubicCurveTo(
                new Vector2(startX, endY),
                new Vector2(startX, endY),
                new Vector2(startX, endY - rBL));
        }
        else
        {
            drawList.PathLineTo(new Vector2(startX, endY));
        }
        
        drawList.PathLineTo(new Vector2(startX, startY + rTL));
    }

    public static void Squircle(Vector2 minPos, Vector2 maxPos, uint color, float radius, int thickness)
    {
        var drawList = ImGuiNative.igGetWindowDrawList();
        Squircle(drawList, minPos, maxPos, color, radius, thickness);
    }

    public static void Squircle(ImDrawListPtr drawList, Vector2 minPos, Vector2 maxPos, uint color, float radius,
        int thickness, SquircleCorners corners = SquircleCorners.All)
    {
        SquircleBase(drawList, minPos, maxPos, radius, corners);

        drawList.PathStroke(color, ImDrawFlags.None, thickness);
    }


    public static void SquircleFilled(Vector2 minPos, Vector2 maxPos, uint color, float radius, SquircleCorners corners = SquircleCorners.All)
    {
        var drawList = ImGuiNative.igGetWindowDrawList();
        SquircleFilled(drawList, minPos, maxPos, color, radius, corners);
    }

    public static void SquircleFilled(ImDrawListPtr drawList, Vector2 minPos, Vector2 maxPos, uint color, float radius, SquircleCorners corners = SquircleCorners.All)
    {
        SquircleBase(drawList, minPos, maxPos, radius, corners);

        drawList.PathFillConvex(color);
    }

    public static void Gap()
    {
        ImGui.Dummy(new Vector2(0, GetGap()));
    }

    private static int _indent = 0;

    public static int GetIndent() => _indent;

    public static void Indent(int value)
    {
        _indent += value;
        ImGui.Indent(value);
    }

    public static void Unindent(int value)
    {
        _indent -= value;
        ImGui.Unindent(value);
    }

    public static int GetGap()
    {
        return (int)ImGui.GetStyle().FramePadding.X;
    }
}