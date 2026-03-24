// using System.Threading.Tasks;
// using Godot;
// using ImGuiGodot;
// using ImGuiNET;
// using MySimsToolkit.Scripts.Camera;
// using MySimsToolkit.Scripts.Nodes;
// using Neat;
// using Vector2 = System.Numerics.Vector2;
//
// namespace MySimsToolkit.Scripts.UI.Viewer;
//
// public partial class ViewerUi : SubViewport
// {
//     public static ViewerUi Instance { get; private set; }
//     [Export] public OrbitCamera OrbitCamera { get; set; }
//     [Export] public Camera3D FreeCamera { get; set; }
//
//     // private VisualNode _node;
//
//     public enum CameraMode
//     {
//         Orbit,
//         Free
//     }
//     
//     public void SetCamera(CameraMode mode)
//     {
//         if (mode == CameraMode.Orbit)
//         {
//             OrbitCamera.SetProcess(true);
//             OrbitCamera.MakeCurrent();
//             FreeCamera.SetProcess(false);
//         }
//         else
//         {
//             OrbitCamera.SetProcess(false);
//             FreeCamera.SetProcess(true);
//             FreeCamera.MakeCurrent();
//         }
//     }
//
//     public override void _Ready()
//     {
//         Instance = this;
//     }
//     
//     private Node3D? _hoveredNode;
//
//     public void Draw(double delta)
//     {
//         var avail = ImGui.GetContentRegionAvail();
//         var splitLeft = avail.X * 0.2f;
//         var splitRight = avail.X * (1f - 0.2f);
//         
//         ImGui.BeginChild("__viewer_world_list", new Vector2(splitLeft, avail.Y));
//         {
//             void RecursiveDrawTreeNode(Node node)
//             {
//                 var childCount = node.GetChildCount();
//
//                 if (childCount == 0)
//                 {
//                     FolderTree.Begin(node.Name, FolderTree.Mode.LEAF);
//                     return;
//                 }
//
//                 var open = FolderTree.Begin(node.Name);
//
//                 if (!open) return;
//                 foreach (var child in node.GetChildren())
//                 {
//                     RecursiveDrawTreeNode(child);
//                 }
//
//                 FolderTree.End();
//             }
//
//             if (_node != null)
//             {
//                 foreach (var child in _node.GetChildren())
//                 {
//                     RecursiveDrawTreeNode(child);
//                 }
//             }
//         }
//         ImGui.EndChild();
//         ImGui.SameLine();
//         
//         ImGui.BeginChild("__viewer_world_3d", new Vector2(splitRight, avail.Y));
//         {
//             var screenAvail = ImGui.GetContentRegionAvail();
//             Size = new Vector2I((int)screenAvail.X, (int)screenAvail.Y);
//             ImGuiGD.SubViewport(this);
//         }
//         ImGui.EndChild();
//
//         //
//         // if (Neat.Button.Icon(Icons.View))
//         // {
//         //     SetDebugDraw(DebugDraw == DebugDrawEnum.Wireframe ? DebugDrawEnum.Disabled : DebugDrawEnum.Wireframe);
//         // }
//     }
//
//     public override void _Process(double delta)
//     {
//         _hoveredNode = GetObjectUnderMouse();
//     }
//     
//     public Node3D? GetObjectUnderMouse()
//     {
//         var camera = GetCamera3D();
//         if (camera == null)
//             return null;
//
//         var mousePos = GetMousePosition();
//
//         var from = camera.ProjectRayOrigin(mousePos);
//         var dir = camera.ProjectRayNormal(mousePos);
//         var to = from + dir * 1000f;
//
//         var query = PhysicsRayQueryParameters3D.Create(from, to);
//         query.CollideWithAreas = true;
//         query.CollideWithBodies = true;
//
//         var spaceState = GetWorld3D().DirectSpaceState;
//         var result = spaceState.IntersectRay(query);
//
//         return result.Count > 0 ? result["collider"].As<Node3D>() : null;
//     }
//
//     public void SetObject(string resourceName)
//     {
//         SetCamera(CameraMode.Orbit);
//         _node?.QueueFree();
//
//         // _node = new VisualNode()
//         // {
//         //     ResourceName = resourceName,
//         // };
//         
//         OrbitCamera.SetNode(_node);
//         
//         AddChild(_node);
//     }
//
// }