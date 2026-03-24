using System;
using System.Collections.Generic;
using Godot;
using ImGuiNET;
using Neat.Extensions;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace Neat;

public static unsafe class Grid
{
    private struct GridState
    {
        public int ColumnWidth;
        public int RowHeight;
        public int ColumnCount;
        public int RowIndex;
        public int ColumnIndex;
        public float Gap;
    }

    private static readonly Stack<GridState> StateStack = new();
    
    public static void Begin(int columns, int height, float gap = 4)
    {
        ImGui.BeginGroup();
        var totalWidth = ImGui.GetContentRegionAvail().X;
        var columnWidth = totalWidth / columns - Util.GetGap();
        StateStack.Push(new GridState { ColumnWidth = (int)columnWidth, RowHeight = height, ColumnCount = columns, RowIndex = 0, ColumnIndex = 0, Gap = gap });
    }

    public static void BeginItem(bool outline = true, bool active = false)
    {
        var state = StateStack.Peek();
        if(state.ColumnIndex <= state.ColumnCount - 1)
            ImGui.SameLine(0, state.Gap + 1);
        else
        {
            ImGui.SetCursorScreenPos(ImGui.GetCursorScreenPos() + new Vector2(0, state.Gap - 3));
        }
                
        if(state.ColumnIndex == 0)
            ImGui.Dummy(new Vector2(20,0));

        if (active)
        {
            var drawList = ImGuiNative.igGetWindowDrawList();
            Util.SquircleFilled(drawList, ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + GetCellSize(), Style.ActiveTheme.Primary.ToHex(), 16);

        }
        
        if (outline && !active)
        {
            var drawList = ImGuiNative.igGetWindowDrawList();
            Util.Squircle(drawList, ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + GetCellSize(), Style.ActiveTheme.BorderSurface.ToHex(), 16,1);
        }
        
        ImGui.PushStyleColor(ImGuiCol.ChildBg, Vector4.Zero);
        ImGui.BeginChild(state.ColumnIndex + "__" + state.RowIndex, new Vector2(state.ColumnWidth, state.RowHeight), ImGuiChildFlags.None);
    }

    public static void BeginItemRaw()
    {
        var state = StateStack.Peek();
        if(state.ColumnIndex <= state.ColumnCount - 1)
            ImGui.SameLine(0, state.Gap + 1);
        else
        {
            ImGui.SetCursorScreenPos(ImGui.GetCursorScreenPos() + new Vector2(0, state.Gap - 3));
        }
        
        if(state.ColumnIndex == 0)
            ImGui.Dummy(new Vector2(20,0));
    }

    public static void EndItemRaw()
    {
        var state = StateStack.Pop();
        if (state.ColumnIndex >= state.ColumnCount)
        {
            state.ColumnIndex = 0;
            state.RowIndex++;
        }
        state.ColumnIndex++;

        StateStack.Push(state);
    }

    public static int GetColumnWidth()
    {
        if(StateStack.Count == 0) throw new InvalidOperationException("No grid state available. Did you forget to call Grid.Begin?");
        var state = StateStack.Peek();
        return state.ColumnWidth;
    }

    public static int GetRowHeight()
    {
        if(StateStack.Count == 0) throw new InvalidOperationException("No grid state available. Did you forget to call Grid.Begin?");
        var state = StateStack.Peek();
        return state.RowHeight;
    }

    public static Vector2 GetCellSize()
    {
        return new Vector2(GetColumnWidth(), GetRowHeight());
    }

    public static bool EndItem() => EndItem(out _);
    public static bool EndItem(out bool hover)
    {
        ImGui.EndChild();
        hover = ImGui.IsItemHovered();
        if(hover) 
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        var clicked = ImGui.IsItemClicked();
        ImGui.PopStyleColor();
        var state = StateStack.Pop();
        if (state.ColumnIndex >= state.ColumnCount)
        {
            state.ColumnIndex = 0;
            state.RowIndex++;
        }
        state.ColumnIndex++;

        StateStack.Push(state);

        return clicked;
    }

    public static void End()
    {
        if(StateStack.Count == 0) throw new InvalidOperationException("No grid state available. Did you forget to call Grid.Begin?");
        ImGui.EndGroup();
        StateStack.Pop();
    }
}