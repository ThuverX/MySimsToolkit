using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MySimsToolkit.Scripts.Extensions;

namespace MySimsToolkit.Scripts.Nodes;

public partial class ThumbnailWorld : SubViewport
{
	[Export] public Camera3D Camera;
	[Export] public Node3D Root;
	
	private readonly List<Node> _children = [];

	public async Task<Texture2D> MakeScreenshot()
	{
		await ToSignal(RenderingServer.Singleton, RenderingServerInstance.SignalName.FramePostDraw);
		
		return ImageTexture.CreateFromImage(GetViewport().GetTexture().GetImage());
	}

	public void AddWorldChild(Node child)
	{
		_children.Add(child);
		Root.AddChild(child);
	}

	public void SetCameraToFrame()
	{
		Camera.Projection = Camera3D.ProjectionType.Perspective;
		var aabb = Root.GetAabb();
		if (aabb.Size == Vector3.Zero)
		{
			Camera.GlobalPosition = new Vector3(5, 5, 5);
			Camera.LookAt(Vector3.Zero, Vector3.Up);
			return;
		}

		var center = aabb.Position + aabb.Size * 0.5f;

		var radius = aabb.Size.Length() * 0.5f;

		var fovRad = Mathf.DegToRad(Camera.Fov);

		var distance = radius / Mathf.Tan(fovRad * 0.5f);

		distance *= 1.2f;

		var direction = new Vector3(1, 1, 1).Normalized();

		Camera.GlobalPosition = center + direction * distance;
		Camera.LookAt(center, Vector3.Up);
	}

	public void Clear()
	{
		foreach (var child in _children)
		{
			child.QueueFree();
		}
		
		_children.Clear();
	}
}
