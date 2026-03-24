using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using MySimsToolkit.Scripts.Extensions;
using MySimsToolkit.Scripts.Formats.FileSystem.FileId;
using Vector3 = Godot.Vector3;
using Vector2 = Godot.Vector2;

namespace MySimsToolkit.Scripts.Formats.Model;

public record WindowsModel : IModel, IBinaryReadable<WindowsModel>
{
    public record Rig : IBinaryReadable<Rig>
    {
        public Dictionary<uint, Matrix4x4> Transforms = [];
        
        public static Rig Read(EndiannessAwareBinaryReader binaryReader)
        {
            var rig = new Rig();
            
            List<uint> bonehashes = [];
            
            var numBones = binaryReader.ReadUInt32();
            for (var i = 0; i < numBones; i++)
            {
                bonehashes.Add(binaryReader.ReadUInt32());
            }
            
            for (var i = 0; i < numBones; i++)
            {
                rig.Transforms.Add(bonehashes[i], binaryReader.ReadMatrix4x4());
            }

            return rig;
        }
    }

    public record Drawable : IBinaryReadable<Drawable>
    {
        public enum VertexKeyType
        {
            Vector2 = 1,
            Vector3 = 2,
            Single = 4,
            Unknown = 0
        }

        public record Face
        {
            public ushort A;
            public ushort B;
            public ushort C;
        }
        
        public record Vertex
        {
            public Vector3 Position = Vector3.Zero;
            public Vector3 Normal = Vector3.Zero;
            public Vector2[] Uvs = new Vector2[4];
            public bool[] UvsUsed = [false, false, false, false];

            public void SetByIndex(byte index, Vector2 vector2, byte subIndex)
            {
                if (index == 2)
                {
                    Uvs[subIndex] = vector2;
                    UvsUsed[subIndex] = true;
                }
                else
                {
                    throw new ArgumentException($"Expected index to be 2, got {index}", nameof(index));
                }
            }

            public void SetByIndex(byte index, Vector3 vector3, byte subIndex)
            {
                if (index == 0)
                {
                    Position = vector3;
                }
                else if (index == 1)
                {
                    Normal = vector3;
                }
                else if (index == 4)
                {
                    // weight?
                }
                else
                {
                    throw new ArgumentException(
                        $"Expected index to be 0 or 1, got {index}",
                        nameof(index)
                    );
                }
            }

            public void SetByIndex(byte index, float num, byte subIndex)
            {
                // if (index == 3)
                // {
                //     weight = num;
                // }
                // else
                // {
                //     throw new ArgumentException($"Expected index to be 3, got {index}", nameof(index));
                // }
            }
        }

        public record VertexKey
        {
            public uint Offset;
            public VertexKeyType Type;
            public byte Index;
            public byte SubIndex;
        }
        
        public Vector3 BoundsMin;
        public Vector3 BoundsMax;

        public ResourceKey MaterialKey;

        public List<Face> Faces = [];
        public List<Vertex> Vertices = [];
        public List<VertexKey> VertexKeys = [];
        public int RigIndex = -1;
        
        public static Drawable Read(EndiannessAwareBinaryReader binaryReader)
        {
            var drawable = new Drawable
            {
                MaterialKey = ResourceKey.Read(binaryReader),
                BoundsMin = binaryReader.ReadVector3(),
                BoundsMax = binaryReader.ReadVector3(),
            };
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            
            binaryReader.ReadUInt32();
            
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            
            var numVerts =  binaryReader.ReadUInt32();
            var numFaces = binaryReader.ReadUInt32();
            
            var numVertexKeys = binaryReader.ReadUInt32();

            for (var i = 0; i < numVertexKeys; i++)
            {
                drawable.VertexKeys.Add(
                    new VertexKey
                    {
                        Offset = binaryReader.ReadUInt32(),
                        Type = (VertexKeyType)binaryReader.ReadByte(),
                        Index = binaryReader.ReadByte(),
                        SubIndex = binaryReader.ReadByte(),
                    }
                );
            }
            
            var vertexArraySize = binaryReader.ReadUInt32();
            var vertexSize = vertexArraySize / numVerts;

            for (var i = 0; i < vertexArraySize; i += (int)vertexSize)
            {
                var start = binaryReader.BaseStream.Position;
                var vertex = new Vertex();

                foreach (var key in drawable.VertexKeys)
                {
                    binaryReader.BaseStream.Seek(start + key.Offset, SeekOrigin.Begin);

                    switch (key.Type)
                    {
                        case VertexKeyType.Vector2:
                        {
                            vertex.SetByIndex(key.Index, binaryReader.ReadVector2(), key.SubIndex);
                            continue;
                        }

                        case VertexKeyType.Vector3:
                        {
                            vertex.SetByIndex(key.Index, binaryReader.ReadVector3(), key.SubIndex);
                            continue;
                        }

                        case VertexKeyType.Single:
                        {
                            vertex.SetByIndex(key.Index, binaryReader.ReadSingle(), key.SubIndex);
                            continue;
                        }
                        // case VertexKeyType.UNKNOWN:
                        // default:
                        // throw new Exception($"Unknown VertexKeyType {key.type} ({(int)key.type})");
                    }
                }

                drawable.Vertices.Add(vertex);
            }

            binaryReader.ReadUInt32();
            for (var i = 0; i < numFaces; i++)
            {
                drawable.Faces.Add(
                    new Face
                    {
                        A = binaryReader.ReadUInt16(),
                        B = binaryReader.ReadUInt16(),
                        C = binaryReader.ReadUInt16(),
                    }
                );
            }

            drawable.RigIndex = binaryReader.ReadInt32();
            
            return drawable;
        }
    }
    
    public Dictionary<uint, string> Params = [];
    
    public List<Rig> Rigs = [];
    public List<Drawable> Drawables = [];
    
    public Vector3 BoundsMin;
    public Vector3 BoundsMax;
    
    public List<MeshInfo> Meshes
    {
        get
        {
            var meshes = new List<MeshInfo>();
            
            foreach (var drawable in Drawables)
            {
                var mesh = new MeshInfo
                {
                    Material = new ResourceKeyFileId(drawable.MaterialKey)
                };

                foreach (var drawableVertex in drawable.Vertices)
                {
                    mesh.Vertices.Add(new VertexInfo
                    {
                        Position = drawableVertex.Position,
                        Normal = drawableVertex.Normal,
                        TexCoord1 = drawableVertex.UvsUsed[0] ? drawableVertex.Uvs[0] : Vector2.Zero,
                        TexCoord2 = drawableVertex.UvsUsed[1] ? drawableVertex.Uvs[1] : Vector2.Zero,
                    });
                }
                
                foreach (var drawableFace in drawable.Faces)
                {
                    mesh.Triangles.Add(new TriangleInfo
                    {
                        Idx1 = drawableFace.A,
                        Idx2 = drawableFace.B,
                        Idx3 = drawableFace.C
                    });
                }
                
                meshes.Add(mesh);
            }
            
            return meshes;
        }
    }

    public static WindowsModel Read(EndiannessAwareBinaryReader binaryReader)
    {
        binaryReader.ReadByte();
        
        var magic = binaryReader.ReadString(4);

        Trace.Assert(magic == "WMDL", $"Expected magic to be \"WMDL\" but got \"{magic}\"");

        binaryReader.ReadUInt32();

        var model = new WindowsModel
        {
            BoundsMin = binaryReader.ReadVector3(),
            BoundsMax = binaryReader.ReadVector3(),
        };
        
        List<uint> extraParamKeys = [];

        var numExtraParams = binaryReader.ReadUInt32();

        if (numExtraParams != 0)
        {
            for (var i = 0; i < numExtraParams; i++)
            {
                extraParamKeys.Add(binaryReader.ReadUInt32());
            }
            
            binaryReader.ReadUInt32();
            for (var i = 0; i < numExtraParams; i++)
            {
                model.Params.Add(extraParamKeys[i], binaryReader.ReadCString());
            }
        }
        else
        {
            binaryReader.ReadByte();
        }
        
        var numRigs = binaryReader.ReadUInt32();
        for (var i = 0; i < numRigs; i++)
        {
            model.Rigs.Add(Rig.Read(binaryReader));
        }
        
        var numDrawables = binaryReader.ReadUInt32();
        for (var i = 0; i < numDrawables; i++)
        {
            model.Drawables.Add(Drawable.Read(binaryReader));
        }

        return model;
    }
}