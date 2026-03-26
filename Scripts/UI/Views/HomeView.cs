using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Godot;
using ImGuiGodot;
using ImGuiNET;
using MySimsToolkit.Scripts.Nodes;
using MySimsToolkit.Scripts.Platforms;
using Neat;
using Neat.Extensions;
using Button = Neat.Button;
using Vector2 = System.Numerics.Vector2;

namespace MySimsToolkit.Scripts.UI.Views;

public partial class HomeView : View
{
    [Export] public Godot.Collections.Dictionary<string, Texture2D> Textures = [];
    
    
    private record GameSelector(string Name, string Icon, GameType  GameType);
    
    private readonly List<GameSelector> _icons = [
        new ("MySims Cozy Bundle", "cozy_bundle_icon", GameType.MySimsCozyBundlePc),
        new ("MySims Kingdom Cozy Bundle", "cozy_bundle_kingdom_icon", GameType.MySimsKingdomCozyBundlePc),
        new ("MySims (2008)", "og_icon", GameType.MySimsPc),
        new ("MySims Agents (Wii)", "agents_wii", GameType.MySimsAgentsWii),
    ];
    
    public override void Draw(double dt)
    {
        if (RuntimeRoot.Instance.GameType != GameType.None)
        {
            return;
        }
        
        Grid.Begin(3, 200, 8);
        var size = Grid.GetCellSize();
        foreach (var gameSelector in _icons)
        {
            var text = gameSelector.Name;
            var active = RuntimeRoot.Instance.GameType == gameSelector.GameType;
            if (active) text += "\n(Loaded)";
            Grid.BeginItemRaw();
            if (BigButton(text, gameSelector.Icon, size, active))
            {
                var dialog = NativeFileDialogSharp.Dialog.FileOpen("exe,dol,bnr");
                if (dialog.IsOk)
                {
                    RuntimeRoot.Instance.RootPath = Path.GetDirectoryName(dialog.Path);
                    RuntimeRoot.Instance.Restart(gameSelector.GameType);
                }
            }
            Grid.EndItemRaw();
        }
        
        Grid.BeginItemRaw();
        
        if(BigButton("Detect game", "search_icon", size, false, true)) {
            var dialog = NativeFileDialogSharp.Dialog.FileOpen("exe,dol,bnr");
            if (dialog.IsOk)
            {
                var platform = GamePlatform.DetectPlatform(dialog.Path);
                RuntimeRoot.Instance.Restart(platform);
            }
        }
        
        Grid.EndItemRaw();
        Grid.End();
    }
    
    public unsafe bool BigButton(string text, string texture, Vector2 inSize, bool active, bool tint = false)
    {
        if(inSize.X == 0)
            inSize.X = ImGui.CalcTextSize(text).X + Style.RootSize;
        ImGui.BeginGroup();
        ImGui.Dummy(Vector2.Zero);
        var size = inSize;
        var pos = ImGui.GetCursorScreenPos();
        
        var clicked = ImGui.InvisibleButton(text, size);
        var hovered = ImGui.IsItemHovered();
        
        pos = new Vector2(Mathf.Round(pos.X), Mathf.Round(pos.Y));
        size = new Vector2(Mathf.Round(size.X), Mathf.Round(size.Y));
        
        ImGui.SetCursorScreenPos(pos);
        
        var drawList = ImGuiNative.igGetWindowDrawList();

        if (hovered && !active)
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

        if (active) hovered = true;
        
        Util.Squircle(drawList, pos, pos + size, Style.ActiveTheme.BorderSurface.ToHex(), 16,  1);
        Util.SquircleFilled(drawList, pos, pos + size,
            hovered ? Style.ActiveTheme.Primary.ToHex() : Style.ActiveTheme.Background.ToHex(), 16);

        var tex = Textures[texture];
        
        ImGui.PushClipRect(pos, pos + size, false);
        var imgsize = new Vector2(size.Y) * 1.1f;
        var imgpos = pos + size - new Vector2(imgsize.Y * 0.8f);

        ImGuiNative.ImDrawList_AddImage(drawList,
            (IntPtr)tex.GetRid().Id,
            imgpos, imgpos + imgsize,
            Vector2.Zero,
            Vector2.One, !hovered && tint ? Style.ActiveTheme.Primary.ToHex() : Style.ActiveTheme.Background.ToHex());
        ImGui.PopClipRect();

        var topleft = pos + new Vector2(Util.GetGap(), Util.GetGap()) * 4;
        ImGui.SetCursorScreenPos(topleft);
        ImGui.TextColored(hovered ? Style.ActiveTheme.OnPrimary : Style.ActiveTheme.Text, text);
        
        ImGui.SetCursorScreenPos(pos);
        ImGui.Dummy(size);
        ImGui.EndGroup();
        return clicked;
    }
}