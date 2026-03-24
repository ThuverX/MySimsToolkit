using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MySimsToolkit.Scripts.Nodes;

namespace MySimsToolkit.Scripts.Extensions;

public static class Node3DExtensions
{
    public static Aabb GetAabb(this Node3D node,
        bool ignoreTopLevel = true,
        Transform3D boundsTransform = default
    )
    {
        Aabb box;
        Transform3D transform;

        if (boundsTransform == default)
            transform = node.GlobalTransform;
        else
            transform = boundsTransform;

        if (node == null)
        {
            return new Aabb(
                new Vector3(-0.2f, -0.2f, -0.2f),
                new Vector3(0.4f, 0.4f, 0.4f)
            );
        }

        Transform3D topXform = transform.AffineInverse() * node.GlobalTransform;

        if (node is VisualInstance3D visual)
            box = visual.GetAabb();
        else
            box = new Aabb();

        box = topXform * box;

        for (int i = 0; i < node.GetChildCount(); i++)
        {
            if (node.GetChild(i) is Node3D child)
            {
                if (!(ignoreTopLevel && child.TopLevel))
                {
                    Aabb childBox = child.GetAabb(ignoreTopLevel, transform);
                    box = box.Merge(childBox);
                }
            }
        }

        return box;
    }
}