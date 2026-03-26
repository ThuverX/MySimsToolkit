using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using ImGuiGodot;
using ImGuiNET;
using MySimsToolkit.Scripts.AssetFileTypes;
using MySimsToolkit.Scripts.Nodes;
using MySimsToolkit.Scripts.Platforms;
using MySimsToolkit.Scripts.Services;
using Neat;
using Neat.Extensions;
using Button = Neat.Button;
using Input = Neat.Input;
using Vector2 = System.Numerics.Vector2;

namespace MySimsToolkit.Scripts.UI.Views.Browser;

public partial class BrowserView : View
{
    private static readonly PackageType PackageType = new();
    
    private readonly Stack<FileHierarchy.BrowserFolder> _folderMemory = new();
    private bool _loaded;
    private string _searchInput = "";

    public override void _Ready()
    {
        RuntimeRoot.Instance.OnReady(() =>
        {
            _loaded = true;
            _folderMemory.Clear();
            PushFolder(FileHierarchy.Build());
        });
    }

    private void PopFolder()
    {
        if (_folderMemory.Count > 1)
            _folderMemory.Pop();
    }

    private void PushFolder(FileHierarchy.BrowserFolder folder)
    {
        _folderMemory.Push(folder);
    }


    public override void Draw(double delta)
    {
        {
            if (Button.Icon(Icons.ArrowLeft))
            {
                PopFolder();
            }

            ImGui.SameLine();
            var widthLeft = ImGui.GetContentRegionAvail().X - Style.RootSize * 2 - Util.GetGap() * 4;
            Input.Text("search_input", ref _searchInput, placeholder: "Search...", icon: Icons.Search, width: widthLeft);
            ImGui.SameLine();
            Button.Icon(Icons.Filter);
            ImGui.SameLine();
            Button.Icon(Icons.LayoutGrid);
        }

        var avail = ImGui.GetContentRegionAvail();
        var splitPerc = Root.InspectorView.IsOpen() ? 0.7f : 1;
        var splitLeft = avail.X * splitPerc;
        var splitRight = avail.X * (1f - splitPerc);

        ImGui.BeginChild("__asset_browser_body", new Vector2(splitLeft, avail.Y));
        {
            if (!_loaded)
            {
                if (RuntimeRoot.Instance.GameType == GameType.None)
                {
                    Text.Centered("No game loaded!");
                }
                else
                {
                    Loader.Centered();
                }
            }
            else
            {
                var columns = ImGui.GetContentRegionAvail().X switch
                {
                    > 2400 => 7,
                    < 2400 and > 2000 => 6,
                    < 2000 and > 1600 => 5,
                    < 1600 and > 1300 => 4,
                    < 1300 and > 900 => 3,
                    < 900 and > 600 => 2,
                    < 600 => 1,
                    _ => 4
                };
                Grid.Begin(columns, 200);
                var currentFolder = _folderMemory.Peek();

                var folders = currentFolder.Folders;
                var files = currentFolder.Files.Where((file) =>
                {
                    if (_searchInput == string.Empty) return true;
                    var displayName = FileTranslatorService.Instance.GetNameForFileId(file.Id);
                    return displayName.Contains(_searchInput, StringComparison.InvariantCultureIgnoreCase);
                });
                
                foreach (var (displayName, value) in folders)
                {
                    Grid.BeginItem();

                    var startPos = ImGui.GetCursorScreenPos();
                    var cellSize = Grid.GetCellSize();

                    var availSize = ImGui.GetContentRegionAvail() - new Vector2(0, Style.RootSize);
                    var pos = ImGui.GetCursorScreenPos();

                    ImGui.SetCursorScreenPos(pos + (availSize / 2 - new Vector2(Fonts.MegaIconSize) / 2));
                    ImGui.PushFont(Fonts.MegaIcons);
                    ImGui.TextColored(Style.ActiveTheme.BorderControl, Icons.Folder);
                    ImGui.PopFont();

                    ImGui.SetCursorScreenPos(pos);

                    ImGui.Dummy(availSize);
                    {
                        ImGui.PushFont(Fonts.RobotoMono);

                        var widthAvail = ImGui.GetContentRegionAvail().X - Util.GetGap() * 2;
                        var textWidth = ImGui.CalcTextSize(displayName).X;

                        ImGui.Dummy(new Vector2(widthAvail / 2 - textWidth / 2, 0));
                        ImGui.SameLine();
                        Text.Default(displayName);
                        ImGui.PopFont();
                    }

                    if (Grid.EndItem(out var hover))
                    {
                        PushFolder(value);
                    }

                    if (hover)
                    {
                        Util.Squircle(startPos, startPos + cellSize, Style.ActiveTheme.Primary.ToHex(), 16, 1);
                    }
                }

                foreach (var file in files)
                {
                    var displayName = FileTranslatorService.Instance.GetNameForFileId(file.Id);
                    var displayFilename = Path.GetFileNameWithoutExtension(displayName);
                    var extension = Path.GetExtension(displayName);
                    var fileType = FileSystemService.Instance.GetFileTypeByExtension(extension);
                    
                    if(fileType.Equals(PackageType)) continue;

                    var active = Root.InspectorView.ActiveFile == file.Id;
                    Grid.BeginItem(false, active: active);
                    var startPos = ImGui.GetCursorScreenPos();
                    var cellSize = Grid.GetCellSize();
                    ImGui.Dummy(new Vector2(Util.GetGap()));

                    var size = ImGui.GetContentRegionAvail() - new Vector2(0, Style.RootSize);
                    size.X = size.Y;

                    var pos = ImGui.GetCursorScreenPos();

                    ImGui.Dummy(new Vector2(Util.GetGap()));
                    ImGui.SameLine();
                    ImGui.PushFont(Fonts.MediumIcons);
                    ImGui.TextColored(!active ? Style.ActiveTheme.BorderControl : Style.ActiveTheme.White, fileType.Icon);
                    ImGui.PopFont();
                    ImGui.SetCursorScreenPos(pos);

                    ImGui.SetCursorScreenPos(pos + new Vector2(ImGui.GetContentRegionAvail().X / 2 - size.X / 2, 0));
                    var imgScreenPos = ImGui.GetCursorScreenPos();

                    if (ThumbnailService.Instance.HasThumbnail(file.Id))
                    {
                        ImGuiGD.Image(ThumbnailService.Instance.GetThumbnail(file.Id), size);
                        Util.Squircle(imgScreenPos, imgScreenPos + size, Style.ActiveTheme.BorderSurface.ToHex(), 16,
                            1);
                    }

                    ImGui.SetCursorScreenPos(pos);

                    ImGui.Dummy(size);
                    {
                        ImGui.PushFont(Fonts.RobotoMono);

                        var widthAvail = ImGui.GetContentRegionAvail().X - Util.GetGap() * 2;
                        var textWidth = ImGui.CalcTextSize(displayFilename).X;

                        ImGui.Dummy(new Vector2(widthAvail / 2 - textWidth / 2, 0));
                        ImGui.SameLine();
                        Text.TextColored(displayFilename, !active ? Style.ActiveTheme.Black : Style.ActiveTheme.White);
                        ImGui.PopFont();
                    }

                    if (Grid.EndItem(out var hover))
                    {
                        Root.InspectorView.Open(file.Id);
                    }

                    if (hover)
                    {
                        Util.Squircle(startPos, startPos + cellSize, Style.ActiveTheme.Primary.ToHex(), 16, 1);
                    }

                    if (ImGui.IsItemVisible())
                    {
                        ThumbnailService.Instance.RequestThumbnail(fileType, file.Id);
                    }
                }

                Grid.End();
            }
        }
        ImGui.EndChild();

        if (Root.InspectorView.IsOpen())
        {
            ImGui.SameLine();
            Util.VerticalSeperator();
            ImGui.SameLine();
            ImGui.BeginChild("__inspector_pane", new Vector2(splitRight - Util.GetGap() * 4, avail.Y));
            Root.InspectorView.Draw(delta);
            ImGui.EndChild();
        }
    }
}