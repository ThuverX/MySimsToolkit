namespace MySimsToolkit.Scripts.Formats;

public interface IBinaryReadable<TSelf>
    where TSelf : IBinaryReadable<TSelf>
{
    static abstract TSelf Read(EndiannessAwareBinaryReader reader);
}