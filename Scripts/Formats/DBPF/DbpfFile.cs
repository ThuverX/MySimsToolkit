using System;
using System.Collections.Generic;
using System.IO;

namespace MySimsToolkit.Scripts.Formats.DBPF;

public record DbpfFile
{
    
    public Dictionary<ResourceKey, DbpfIndex> Entries => Index.Entries;
    
    public DbpfHeader Header { get; set; }
    public IDbpfIndex Index { get; set; }
    
    public static DbpfFile Read(EndiannessAwareBinaryReader binaryReader)
    {
        var file = new DbpfFile
        {
            Header = DbpfHeader.Read(binaryReader),
        };
        
        binaryReader.BaseStream.Seek((long)file.Header.IndexOffset, SeekOrigin.Begin);

        file.Index = (file.Header.MajorVersion, file.Header.MinorVersion, file.Header.IndexMajorVersion, file.Header.IndexMinorVersion) switch
        {
            (2,0,0,3) => DbpfIndexV2003.Read(binaryReader, file.Header.IndexEntryCount),
            (3,0,0,2) => DbpfIndexV3002.Read(binaryReader),
            _ => throw new Exception("Unsupported DBPF version")
        };

        return file;
    }
}