using System;
using System.IO;

namespace MySimsToolkit.Scripts;

public readonly struct StreamJump : IDisposable
{
    private readonly Stream _stream;
    private readonly long _pos;

    public StreamJump(Stream stream, long newPos)
    {
        _stream = stream;
        _pos = stream.Position;
        stream.Seek(newPos, SeekOrigin.Begin);
    }

    public void Dispose()
    {
        _stream.Seek(_pos, SeekOrigin.Begin);
    }
}