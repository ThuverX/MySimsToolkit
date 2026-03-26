using ImGuiNET;
using MySimsToolkit.Scripts.Services;
using Neat;

namespace MySimsToolkit.Scripts.UI.Views.Saves;

public class MySimsSaveEditorView : ISaveView
{
    private string _townName = "";
    private string _playerName = "";
    private int _starCount = 0;
    private int _score = 0;
    private SaveService.SaveFileData _currentSave;

    public void Draw(double dt)
    {
        if (_currentSave == null) return;
        Input.Label("Town name: ");
        Input.Text("__savegame_townname", ref _townName);
        Input.Label("Sim name: ");
        Input.Text("__savegame_playername", ref _playerName);
    }

    public void Load(SaveService.SaveFileData save)
    {
        _currentSave = save;
        _townName = save.Name;
        _playerName = save.PlayerName;
    }

    public void Save(SaveService.SaveFileData save)
    {
    }
}