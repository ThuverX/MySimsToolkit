using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Formats.Level;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.Nodes;

public partial class LevelNode : LoaderNode3D
{
	public IFileSystem.FileId Resource;
	
	private AssetService.AssetHandle<ILevel> _level;

	public override void _EnterTree()
	{
		_level = RequestAsset<ILevel>(Resource);
	}

	protected override async Task OnAssetsLoaded()
	{
		foreach (var child in GetChildren())
		{
			child.QueueFree();
		}
		
		foreach (var fileId in _level.Value.Parts)
		{
			var model = new ModelNode();
			model.Resource = fileId;
			
			AddChild(model);
		}

		foreach (var objectInfo in _level.Value.Objects)
		{
			var model = new ModelNode();
			model.Resource = objectInfo.FileId;
			
			model.Position = objectInfo.Position;
			model.RotationDegrees = objectInfo.Rotation;
			model.Scale = objectInfo.Scale;
			
			AddChild(model);
		}
	}
}