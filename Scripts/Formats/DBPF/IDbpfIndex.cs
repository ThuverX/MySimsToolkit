using System.Collections.Generic;

namespace MySimsToolkit.Scripts.Formats.DBPF;

public interface IDbpfIndex
{
    public Dictionary<ResourceKey, DbpfIndex> Entries { get; }
}