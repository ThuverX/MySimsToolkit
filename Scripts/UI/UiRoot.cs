using Godot;
using ImGuiGodot;
using ImGuiNET;
using MySimsToolkit.Scripts.Nodes;
using MySimsToolkit.Scripts.Platforms;
using MySimsToolkit.Scripts.UI.Views;
using Neat;
using BrowserView = MySimsToolkit.Scripts.UI.Views.Browser.BrowserView;
using SaveEditorView = MySimsToolkit.Scripts.UI.Views.Saves.SaveEditorView;
using Window = Neat.Window;

namespace MySimsToolkit.Scripts.UI;

public partial class UiRoot : Node
{
    [Export] public FontFile RobotoRegular;
    [Export] public FontFile RobotoMono;
    [Export] public FontFile RobotoBold;
    [Export] public FontFile Lucide;
    
    [Export] public BrowserView BrowserView;
    [Export] public SaveEditorView SaveEditorView;
    [Export] public HomeView HomeView;

    private string _currentPage = "Home";
    
    public override void _Ready()
    {
        ImGuiGD.ResetFonts();
        
        ImGuiGD.AddFont(RobotoRegular, Fonts.RobotoSize);
        ImGuiGD.AddFont(Lucide, 14, true, [Icons.IconMin, Icons.IconMax], true);
        ImGuiGD.AddFont(Lucide, Fonts.MediumIconSize, false, [Icons.IconMin, Icons.IconMax], true);
        ImGuiGD.AddFont(Lucide, Fonts.BigIconSize, false, [Icons.IconMin, Icons.IconMax], true);
        ImGuiGD.AddFont(Lucide, Fonts.MegaIconSize, false, [Icons.IconMin, Icons.IconMax], true);
        ImGuiGD.AddFont(RobotoMono, Fonts.RobotoSize);
        ImGuiGD.AddFont(RobotoRegular, Fonts.RobotoTinySize);
        ImGuiGD.AddFont(RobotoBold, Fonts.RobotoBigSize);
        
        ImGuiGD.RebuildFontAtlas();
        
        Fonts.RobotoBig = ImGui.GetIO().Fonts.Fonts[6];
        Fonts.RobotoTiny = ImGui.GetIO().Fonts.Fonts[5];
        Fonts.RobotoMono = ImGui.GetIO().Fonts.Fonts[4];
        Fonts.MegaIcons = ImGui.GetIO().Fonts.Fonts[3];
        Fonts.BigIcons = ImGui.GetIO().Fonts.Fonts[2];
        Fonts.MediumIcons = ImGui.GetIO().Fonts.Fonts[1];
        Fonts.Roboto = ImGui.GetIO().Fonts.Fonts[0];
        
        Style.Apply(Style.Light.GetTheme());
        
        BrowserView.SetUiRoot(this);
        SaveEditorView.SetUiRoot(this);
        HomeView.SetUiRoot(this);
    }

    public void OpenPage(string page)
    {
        _currentPage = page;
    }
    
    public override void _Process(double delta)
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                var isGameTypeNone = RuntimeRoot.Instance.GameType != GameType.None;
                if (!isGameTypeNone)
                {
                    ImGui.BeginDisabled();
                }
                
                if (ImGui.MenuItem("Close game"))
                {
                    RuntimeRoot.Instance.Restart(GameType.None);
                }

                if (!isGameTypeNone)
                {
                    ImGui.EndDisabled();
                }
                
                if (ImGui.MenuItem("Quit"))
                {
                    GetTree().Quit();
                }
                
                ImGui.EndMenu();
            }
            
            if (ImGui.BeginMenu("Edit"))
            {
                ImGui.EndMenu();
            }
            
            if (ImGui.BeginMenu("View"))
            {
                ImGui.EndMenu();
            }
            
            if (ImGui.BeginMenu("Window"))
            {
                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }
        
        Window.BeginRootWindow();
        
        Tabs.BeginTabs();
        Tabs.TabItem(ref _currentPage, "Home");
        Tabs.TabItem(ref _currentPage, "Saves");
        Tabs.TabItem(ref _currentPage, "Assets");
        Tabs.TabItem(ref _currentPage, "Strings");
        Tabs.EndTabs();

        switch (_currentPage)
        {
            case "Home":
                HomeView.Draw(delta);
                break;
            case "Saves":
                SaveEditorView.Draw(delta);
                break;
            case "Assets":
                BrowserView.Draw(delta);
                break;
        }
        
        
        Window.EndRootWindow();
    }
}