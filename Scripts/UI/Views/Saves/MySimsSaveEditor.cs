using ImGuiNET;
using MySimsToolkit.Scripts.Services;
using Neat;

namespace MySimsToolkit.Scripts.UI.Views.Saves;

public static class MySimsSaveEditor
{
    private static string TownName = "";
    private static string PlayerName = "";
    public static void Draw(double dt, SaveService.SaveFileData save)
    {
        Input.Label("Town name: ");
        Input.Text("__savegame_townname", ref TownName);
        Input.Label("Sim name: ");
        Input.Text("__savegame_playername", ref PlayerName);
    }
}