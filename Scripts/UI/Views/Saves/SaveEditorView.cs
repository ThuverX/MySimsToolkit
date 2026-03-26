using System.Numerics;
using ImGuiNET;
using MySimsToolkit.Scripts.Nodes;
using MySimsToolkit.Scripts.Platforms;
using MySimsToolkit.Scripts.Services;
using Neat;
using Neat.Extensions;

namespace MySimsToolkit.Scripts.UI.Views.Saves;

public partial class SaveEditorView : View
{
    private SaveService.SaveFileData _currentSave;
    
    private readonly ISaveView _saveView = new MySimsSaveEditorView();
    
    public override void Draw(double dt)
    {
        var avail = ImGui.GetContentRegionAvail();
        var splitLeft = avail.X * 0.3f;
        var splitRight = avail.X * (1f - 0.3f);
        
        ImGui.BeginChild("__save_editor_list", new Vector2(splitLeft, avail.Y));
        Grid.Begin(1, 70, 8);
        foreach (var saveFileData in SaveService.Instance.Saves)
        {
            var active = _currentSave?.Equals(saveFileData) ?? false;
            Grid.BeginItem(active: active);
            
            var startPos = ImGui.GetCursorScreenPos();
            var cellSize = Grid.GetCellSize();
            
            ImGui.Dummy(new Vector2(8,8));
            ImGui.Dummy(new Vector2(8,1));
            ImGui.SameLine();
            Text.TitleColored(saveFileData.Name, active ? Style.ActiveTheme.White : Style.ActiveTheme.Text);
            ImGui.Dummy(new Vector2(8,1));
            ImGui.SameLine();
            Text.TextColored(saveFileData.PlayerName, active ? Style.ActiveTheme.White : Style.ActiveTheme.Text);
            if (Grid.EndItem(out var hover))
            {
                _currentSave = saveFileData;
                _saveView.Load(saveFileData);
            }
            
            if (hover)
            {
                Util.Squircle(startPos, startPos + cellSize, Style.ActiveTheme.Primary.ToHex(), 16, 1);
            }
        }
        Grid.End();
        ImGui.EndChild();
        
        ImGui.SameLine();
        
        ImGui.BeginChild("__save_editor_view", new Vector2(splitRight, avail.Y));
        if (RuntimeRoot.Instance.GameType == GameType.MySimsCozyBundlePc || RuntimeRoot.Instance.GameType == GameType.MySimsPc)
        {
            _saveView.Draw(dt);
        }
        
        ImGui.EndChild();
    }
}