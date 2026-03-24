using System.Buffers.Binary;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;
using Vector4 = Godot.Vector4;

namespace MySimsToolkit.Scripts.Extensions;

public static class EndiannessAwareBinaryReaderExtensions
{
    
    public static StreamJump Jump(this EndiannessAwareBinaryReader br, long absolutePos)
        => new(br.BaseStream, absolutePos);
    public static string ReadString(this EndiannessAwareBinaryReader reader, int size)
    {
        var bytes = reader.ReadBytesWithEndianness(size);
        
        return Encoding.Default.GetString(bytes);
    }

    public static string ReadCString(this EndiannessAwareBinaryReader reader)
    {
        List<byte> bytes = [];

        while (true)
        {
            var b = reader.ReadByte();
            if (b == 0)
                break;
            bytes.Add(b);
        }

        return Encoding.Default.GetString([.. bytes]);
    }

    public static Vector3 ReadVector3(this EndiannessAwareBinaryReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();
        return new Vector3(x, y, z);
    }

    public static Vector4 ReadVector4(this EndiannessAwareBinaryReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();
        var w = reader.ReadSingle();
        return new Vector4(x, y, z, w);
    }

    public static Vector2 ReadVector2(this EndiannessAwareBinaryReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        return new Vector2(x, y);
    }

    public static Matrix4x4 ReadMatrix4x4(this EndiannessAwareBinaryReader reader)
    {
        var m = new Matrix4x4
        {
            M11 = reader.ReadSingle(),
            M12 = reader.ReadSingle(),
            M13 = reader.ReadSingle(),
            M14 = reader.ReadSingle(),
            M21 = reader.ReadSingle(),
            M22 = reader.ReadSingle(),
            M23 = reader.ReadSingle(),
            M24 = reader.ReadSingle(),
            M31 = reader.ReadSingle(),
            M32 = reader.ReadSingle(),
            M33 = reader.ReadSingle(),
            M34 = reader.ReadSingle(),
            M41 = reader.ReadSingle(),
            M42 = reader.ReadSingle(),
            M43 = reader.ReadSingle(),
            M44 = reader.ReadSingle()
        };
        return m;
    }
}