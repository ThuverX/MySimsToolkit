using System;
using System.Threading.Tasks;
using Godot;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Nodes;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.ThumbnailHandlers;

public static class Generic3DHandler
{
    public static async Task LevelHandler(ThumbnailWorld world, IFileSystem.FileId path)
    {
        var level = new LevelNode();
        level.Resource = path;
            
        world.AddWorldChild(level);
        
        await level.WhenFullyLoaded;
        
        world.SetCameraToFrame();
    }
    
    public static async Task ModelHandler(ThumbnailWorld world, IFileSystem.FileId path)
    {
        var model = new ModelNode();
        model.Resource = path;
            
        world.AddWorldChild(model);
        
        await model.WhenFullyLoaded;
        
        world.SetCameraToFrame();
    }

    public static async Task TextureHandler(ThumbnailWorld world, IFileSystem.FileId path)
    {
        try
        {
            var meshInstance = new MeshInstance3D();
            meshInstance.Mesh = new PlaneMesh();

            world.AddWorldChild(meshInstance);

            var godotMaterial = ShaderService.Instance.GetMaterialForShader("unlit");

            var texture = await AssetService.Instance.Load<Texture2D>(path).AsTask();

            var aspect = texture.GetWidth() / (float)texture.GetHeight();

            meshInstance.Mesh.SurfaceSetMaterial(0, godotMaterial);
            meshInstance.Scale = new Vector3(aspect, 1, 1);

            godotMaterial.SetShaderParameter("Texture", texture);

            world.Camera.GlobalPosition = new Vector3(0, 2 * aspect, 0);
            world.Camera.LookAt(Vector3.Zero, Vector3.Forward);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}