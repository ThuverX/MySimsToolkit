using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MySimsToolkit.Scripts.Formats.Material;
using MySimsToolkit.Scripts.Formats.Model;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.Nodes;

public partial class MeshNode : LoaderNode3D
{
	public int MaterialIndex = 0;
	
	private MeshInfo _meshInfo;
	private MeshInstance3D _meshInstance;
	
	private AssetService.AssetHandle<IMaterialSet> _materialSet;
	
	public override void _Ready()
	{
		_materialSet = RequestAsset<IMaterialSet>(_meshInfo.Material);
	}
	
	public MeshNode(MeshInfo meshInfo)
	{
		_meshInfo = meshInfo;
	}
	
	private void BuildMesh()
	{
		var mesh = new ArrayMesh();

		var arrays = new Godot.Collections.Array();
		arrays.Resize((int)Mesh.ArrayType.Max);

		var vertexPositions = new Godot.Collections.Array<Vector3>();
		var vertexNormals   = new Godot.Collections.Array<Vector3>();
		var vertexUVs       = new Godot.Collections.Array<Vector2>();
		var vertexUVs2      = new Godot.Collections.Array<Vector2>();
		var indices			= new Godot.Collections.Array<int>();

		foreach (var v in _meshInfo.Vertices)
		{
			vertexPositions.Add(v.Position);
			vertexNormals.Add(v.Normal);
			vertexUVs.Add(v.TexCoord1);
			vertexUVs2.Add(v.TexCoord2);
		}

		foreach (var f in _meshInfo.Triangles)
		{
			indices.Add((int)f.Idx3);
			indices.Add((int)f.Idx2);
			indices.Add((int)f.Idx1);
		}

		arrays[(int)Mesh.ArrayType.Vertex] = vertexPositions.ToArray();
		arrays[(int)Mesh.ArrayType.Normal] = vertexNormals.ToArray();
		arrays[(int)Mesh.ArrayType.TexUV]  = vertexUVs.ToArray();
		arrays[(int)Mesh.ArrayType.TexUV2] = vertexUVs2.ToArray();
		arrays[(int)Mesh.ArrayType.Index]  = indices.ToArray();

		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

		_meshInstance = new MeshInstance3D();
		_meshInstance.Mesh = mesh;
		_meshInstance.CreateTrimeshCollision();

		AddChild(_meshInstance);
	}

	protected override async Task OnAssetsLoaded()
	{
		try
		{
			_meshInstance?.QueueFree();

			BuildMesh();

			var godotMaterial = ShaderService.Instance.GetMaterialForShader("unlit");
			
			_meshInstance?.Mesh.SurfaceSetMaterial(0, godotMaterial);
			
			var material = await AssetService.Instance.Load<IMaterial>(_materialSet.Value.Materials[0]).AsTask();
			var texture = await AssetService.Instance.Load<Texture2D>(material.Material.MainTexture).AsTask();
			
			if (texture != null)
			{
				godotMaterial.SetShaderParameter("Texture", texture);
			}
		}
		catch (Exception e)
		{
			// ignored
		}
	}
}