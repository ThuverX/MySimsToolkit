using System.Collections.Generic;

namespace MySimsToolkit.Scripts.Formats.Model;

public interface IModel
{
    public List<MeshInfo> Meshes { get; }
}