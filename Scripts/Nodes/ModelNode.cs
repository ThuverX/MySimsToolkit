using System.Threading.Tasks;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Formats.Model;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.Nodes;

public partial class ModelNode : LoaderNode3D
{
	public IFileSystem.FileId Resource;
	
	private AssetService.AssetHandle<IModel> _model;
	public override void _Ready()
	{
		_model = RequestAsset<IModel>(Resource);
	}
	
	protected override async Task OnAssetsLoaded()
	{
		foreach (var child in GetChildren())
		{
			child.QueueFree();
		}
		
		for (var i = 0; i < _model.Value.Meshes.Count; i++)
		{
			var meshNode = new MeshNode(_model.Value.Meshes[i]);
			meshNode.Name = "Drawable" + i;
			AddChild(meshNode);
		}
	}
}