using System.IO;

namespace MySimsToolkit.Scripts.Formats;

public interface IStreamReadable<TSelf>
    where TSelf : IStreamReadable<TSelf>
{
    static abstract TSelf Read(Stream stream);
}