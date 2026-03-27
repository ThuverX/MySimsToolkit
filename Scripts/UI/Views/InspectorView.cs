using System;
using System.IO;
using System.Linq;
using Godot;
using ImGuiGodot;
using ImGuiNET;
using MySimsToolkit.Scripts.AssetFileTypes;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Services;
using Neat;
using Button = Neat.Button;
using Vector2 = System.Numerics.Vector2;

namespace MySimsToolkit.Scripts.UI.Views;

public partial class InspectorView : View
{
    private IFileSystem.FileId _activeFile;
    private IFileSystem.FileStat _stat;
    public IFileSystem.FileId ActiveFile => _activeFile;
    
    
    private string _displayName;
    private string _displayFilename;
    private string _extension;
    private FileType _fileType;
    
    public override void Draw(double dt)
    {
        if (_activeFile == null) return;
        
        var avail = ImGui.GetContentRegionAvail();
        
        Text.Title(_displayFilename);
        Util.Seperator();
        
        Util.Gap();
        
        var pos = ImGui.GetCursorScreenPos();
        
        if (ThumbnailService.Instance.HasThumbnail(_activeFile))
        {
            var size = Mathf.Min(avail.X, 200);
            ImGui.SetCursorScreenPos(pos + new Vector2(ImGui.GetContentRegionAvail().X / 2 - size / 2, 0));
            ImGuiGD.Image(ThumbnailService.Instance.GetThumbnail(_activeFile), new Vector2(size));
            Util.Seperator();
            Util.Gap();
        }
        
        Pills.Default(_fileType.Name);
        ImGui.SameLine();

        Pills.Default(GetBytesReadable(_stat.Size));
        
        Util.Gap();
        
        Text.Default(_fileType.Description);
        
        Util.Gap();
        
        var halfWidth = avail.X / 2 - Util.GetGap();

        if (Button.Default("Export", width: halfWidth))
        {
            Export();
        }
        ImGui.SameLine();
        if (Button.Default("Save Raw", width: halfWidth))
        {
            SaveRaw();
        }
    }

    private static string GetBytesReadable(long i)
    {
        var absoluteI = (i < 0 ? -i : i);
        string suffix;
        double readable;
        switch (absoluteI)
        {
            case >= 0x1000000000000000:
                suffix = "EB";
                readable = (i >> 50);
                break;
            case >= 0x4000000000000:
                suffix = "PB";
                readable = (i >> 40);
                break;
            case >= 0x10000000000:
                suffix = "TB";
                readable = (i >> 30);
                break;
            case >= 0x40000000:
                suffix = "GB";
                readable = (i >> 20);
                break;
            case >= 0x100000:
                suffix = "MB";
                readable = (i >> 10);
                break;
            case >= 0x400:
                suffix = "KB";
                readable = i;
                break;
            default:
                return i.ToString("0 B");
        }
        readable = (readable / 1024);
        return readable.ToString("0.### ") + suffix;
    }
    
    private void SaveRaw()
    {
        var ext = _fileType.Extensions.Length <= 0 ? (_extension.Length > 0 ? _extension[1..] : "bin") : _fileType.Extensions[0][1..];
        var dialog = NativeFileDialogSharp.Dialog.FileSave(ext);
        if (!dialog.IsOk) return;
        
        using var stream = FileSystemService.Instance.OpenRead(_activeFile);
        using var fileStream = File.Create(dialog.Path);
        stream.CopyTo(fileStream);
    }

    private void Export()
    {
        if (ExporterService.Instance.GetOptions(_fileType).Length <= 0)
        {
            // TODO: Popup a message
            return;
        }
        
        var dialog = NativeFileDialogSharp.Dialog.FileSave(ExporterService.Instance.GetOptions(_fileType).Join(",").Replace(".", ""));
        if (dialog.IsOk)
        {
            ExporterService.Instance.Export(_fileType, _activeFile, dialog.Path);
        }
    }
    
    public void Open(IFileSystem.FileId fileId)
    {
        _activeFile = fileId;
        _stat = FileSystemService.Instance.Stat(fileId);
        
        _displayName = FileTranslatorService.Instance.GetNameForFileId(_activeFile);
        _displayFilename = Path.GetFileNameWithoutExtension(_displayName);
        _extension = Path.GetExtension(_displayName);
        _fileType = FileSystemService.Instance.GetFileTypeByExtension(_extension);
    }

    public bool IsOpen()
    {
        return _activeFile != null;
    }

    public void Close()
    {
        _activeFile = null;
    }
}