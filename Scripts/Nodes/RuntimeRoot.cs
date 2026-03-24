using System;
using Godot;
using Godot.Collections;
using MySimsToolkit.Scripts.Platforms;

namespace MySimsToolkit.Scripts.Nodes;

public partial class RuntimeRoot : Node
{
    public static RuntimeRoot Instance { get; private set; }
    public RuntimeContext Runtime { get; private set; }

    [Export] public GameType GameType;
    [Export] public string RootPath;
    [Export] public Dictionary<string, Shader> Shaders = [];
    [Export] public ThumbnailWorld Thumbnails;
    
    private event Action OnReadyEvent;

    public override void _EnterTree()
    {
        Instance = this;
        StartRuntime();
    }

    public override void _Process(double delta)
    {
        Runtime.Jobs.ExecuteMainThreadJobs();
    }

    private void StartRuntime()
    {
        var platform = GamePlatform.FromType(GameType);

        try
        {
            if (platform != null && !platform.IsValidPath(RootPath))
            {
                throw new Exception("Invalid root path: " + RootPath);
            }

            Runtime = new RuntimeContext(platform, RootPath);

            foreach (var keyValuePair in Shaders)
            {
                Runtime.Shaders.ShaderMap.Add(keyValuePair.Key, keyValuePair.Value);
            }

            Runtime.Thumbnails.World = Thumbnails;

            OnReadyEvent?.Invoke();
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            Restart(GameType.None);
        }
    }

    public void OnReady(Action callback)
    {
        OnReadyEvent += callback;
    }
    
    public void Restart(GameType newGame)
    {
        Runtime?.Dispose();
        GameType = newGame;
        StartRuntime();
    }
}