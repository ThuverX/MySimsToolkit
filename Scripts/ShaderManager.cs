// using Godot;
// using Godot.Collections;
//
// namespace MySimsToolkit.Scripts;
//
// public partial class ShaderManager : Node
// {
//     public static ShaderManager Instance { get; private set; }
//
//     [Export] public Dictionary<string, Shader> Shaders;
//
//     public override void _EnterTree()
//     {
//         Instance = this;
//     }
//
//     public ShaderMaterial GetMaterialForShader(string shaderName)
//     {
//         var material = new ShaderMaterial();
//
//         if (Shaders.TryGetValue(shaderName, out var shader))
//             material.Shader = shader;
//         
//         return material;
//     }
// }