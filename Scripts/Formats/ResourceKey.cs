using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MySimsToolkit.Scripts.Formats;

public partial record ResourceKey : IComparable
{
    [GeneratedRegex(@"0x(.+?)\!0x(.+?)\.(.+)")]
    private static partial Regex KeyRegex();
    
    public uint Type { get; set; }
    public ulong Instance { get; set; }
    public uint Group { get; set; }

    public static ResourceKey FromPath(string path)
    {
        var filename = Path.GetFileName(path);

        var match = KeyRegex().Match(filename);
        if (match.Success)
        {
            var group = match.Groups[1].Value;
            var instance = match.Groups[2].Value;
            var type = match.Groups[3].Value;

            return new ResourceKey
            {
                Group = Convert.ToUInt32(group, 16),
                Instance = Convert.ToUInt64(instance, 16),
                Type = type.StartsWith("0x") ? Convert.ToUInt32(type[2..], 16) : Fnv.FromString32(type),
            };
        }
        
        var extension = Path.GetExtension(path);
        if (extension.Length <= 0) extension = ".xml";
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);

        return new ResourceKey
        {
            Group = 0,
            Instance = Fnv.FromString64(fileNameWithoutExtension),
            Type = Fnv.FromString32(extension[1..]),
        };
    }

    public static ResourceKey FromPath(string path, Dictionary<string, uint> acceptedTypes)
    {
        var baseKey = FromPath(path);
        
        if(acceptedTypes is null) return baseKey;

        var extension = Path.GetExtension(path).ToLower();
        if (extension.Length <= 0) extension = ".xml";

        if (acceptedTypes.TryGetValue(extension[1..], out var type))
        {
            baseKey.Type = type;
        }

        return baseKey;
    }

    public static ResourceKey Read(EndiannessAwareBinaryReader binaryReader)
    {
        if (binaryReader.GetEndianness() != EndiannessAwareBinaryReader.Endianness.Big)
        {
            return new ResourceKey
            {
                Instance = binaryReader.ReadUInt64(),
                Type = binaryReader.ReadUInt32(),
                Group = binaryReader.ReadUInt32()
            };
        }

        var instanceLo = binaryReader.ReadUInt32();
        var instanceHi = binaryReader.ReadUInt32();

        return new ResourceKey
        {
            Instance = ((ulong)instanceHi << 32) | instanceLo,
            Type = binaryReader.ReadUInt32(),
            Group = binaryReader.ReadUInt32(),
        };

    }

    public override string ToString()
    {
        return $"0x{Group:X8}!0x{Instance:X16}.0x{Type:X8}";
    }

    public int CompareTo(ResourceKey? other)
    {
        if (other is null) return 1;

        var cmp = Group.CompareTo(other.Group);
        if (cmp != 0) return cmp;

        cmp = Instance.CompareTo(other.Instance);
        if (cmp != 0) return cmp;

        return Type.CompareTo(other.Type);
    }

    int IComparable.CompareTo(object? obj)
    {
        if (obj is ResourceKey key)
            return CompareTo(key);

        throw new ArgumentException($"Object must be of type {nameof(ResourceKey)}");
    }
}