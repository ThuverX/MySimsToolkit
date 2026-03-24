using System.Text;

namespace MySimsToolkit.Scripts.Formats;

public static class Fnv
{
    private const uint Prime32 = 0x01000193;
    private const ulong Prime64 = 0x00000100000001B3;
    private const uint Offset32 = 0x811C9DC5;
    private const ulong Offset64 = 0xCBF29CE484222325;

    public static uint Create32(byte[] bytes)
    {
        var hash = Offset32;

        foreach (var t in bytes)
        {
            hash *= Prime32;
            hash ^= t;
        }
        return hash;
    }

    public static ulong Create64(byte[] bytes)
    {
        var hash = Offset64;

        foreach (var t in bytes)
        {
            hash *= Prime64;
            hash ^= t;
        }
        return hash;
    }

    public static uint FromString32(string str)
    {
        return Create32(Encoding.UTF8.GetBytes(str.ToLower()));
    }

    public static ulong FromString64(string str)
    {
        return Create64(Encoding.UTF8.GetBytes(str.ToLower()));
    }
}
