using System;
using Godot;
using MySimsToolkit.Scripts.Extensions;
using MySimsToolkit.Scripts.Nodes;

namespace MySimsToolkit.Scripts.Camera;

public partial class OrbitCamera : Camera3D
{
    [Export] public Node3D TargetNode { get; set; }
    
    [Export] public float OrbitSpeedYaw = 1.5f;
    [Export] public float OrbitSpeedPitch = 1.0f;
    [Export] public float MouseSensitivity = 0.005f;
    
    [Export] public float ZoomSpeed = 2.5f;
    [Export] public float MinDistance = 2.0f;
    [Export] public float MaxDistance = 80.0f;

    public float Yaw;
    public float Pitch;
    public float Distance = 25.0f;
    
    private Vector3 _offset = new (20,20,20);
    private Aabb _aabb;

    
    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion m && Input.IsMouseButtonPressed(MouseButton.Middle))
        {
            Yaw   -= m.Relative.X * MouseSensitivity;
            Pitch -= m.Relative.Y * MouseSensitivity;
        }
        
        if (e is InputEventMouseButton mb && mb.Pressed)
        {
            if (mb.ButtonIndex == MouseButton.WheelUp)
                Distance -= ZoomSpeed;
            else if (mb.ButtonIndex == MouseButton.WheelDown)
                Distance += ZoomSpeed;

            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
        }
    }

    public override void _EnterTree()
    {
        if(TargetNode != null)
            _aabb = TargetNode.GetAabb();
        
        OnLoad();
    }

    public void SetNode(Node3D node)
    {
        TargetNode = node;
        OnLoad();
    }

    private async void OnLoad()
    {
        // if (TargetNode is IAsyncLoader loader)
        // {
        //     await loader.WhenLoaded;
        //
        //     _aabb = TargetNode.GetAabb();
        // }
    }
    
    public override void _Process(double delta)
    {
        var center = _aabb.GetCenter();

        Pitch = Mathf.Clamp(Pitch, Mathf.DegToRad(-45), Mathf.DegToRad(90 + 25));

        var offsetDir = _offset.Normalized();
        var offset = offsetDir * Distance;

        offset = offset.Rotated(Vector3.Up, Yaw);

        var right = Vector3.Up.Cross(offset).Normalized();

        offset = offset.Rotated(right, Pitch);

        GlobalPosition = center + offset;
        LookAt(center, Vector3.Up);
    }

}