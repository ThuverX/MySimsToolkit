using System.Diagnostics;
using MySimsToolkit.Scripts.Extensions;

namespace MySimsToolkit.Scripts.Formats.DBPF;

public record DbpfHeader
{
    public uint MajorVersion;
    public uint MinorVersion;
    public uint DateCreated;
    public uint DateModified;
    public uint IndexMajorVersion;
    public uint IndexEntryCount;
    public uint FirstIndexOffset;
    public uint IndexSize;
    public uint HoleEntryCount;
    public uint HoleOffset;
    public uint HoleSize;
    public uint IndexMinorVersion;
    public ulong IndexOffset;

    public static DbpfHeader Read(EndiannessAwareBinaryReader binaryReader)
    {
        var header = new DbpfHeader();
        
        var magic = binaryReader.ReadString(4);
    
        Trace.Assert(magic == "DBPF", $"Expected magic to be \"DBPF\" but got \"{magic}\"");
    
        header.MajorVersion = binaryReader.ReadUInt32();
        header.MinorVersion = binaryReader.ReadUInt32();
    
        binaryReader.BaseStream.Position += 12;
    
        header.DateCreated = binaryReader.ReadUInt32();
        header.DateModified = binaryReader.ReadUInt32();
    
        header.IndexMajorVersion = binaryReader.ReadUInt32();
        header.IndexEntryCount = binaryReader.ReadUInt32();
        header.FirstIndexOffset = binaryReader.ReadUInt32();
        header.IndexSize = binaryReader.ReadUInt32();
    
        header.HoleEntryCount = binaryReader.ReadUInt32();
        header.HoleOffset = binaryReader.ReadUInt32();
        header.HoleSize = binaryReader.ReadUInt32();
    
        header.IndexMinorVersion = binaryReader.ReadUInt32();
        header.IndexOffset = binaryReader.ReadUInt64();
    
        binaryReader.BaseStream.Position += 24;
    
        Trace.Assert(
            binaryReader.BaseStream.Position == 96,
            $"Invalid DBPF header size, expected 96 got {binaryReader.BaseStream.Position}"
        );

        return header;
    }
}