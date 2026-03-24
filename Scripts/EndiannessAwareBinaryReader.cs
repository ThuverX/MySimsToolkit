using System;
using System.IO;
using System.Text;

namespace MySimsToolkit.Scripts;

public class EndiannessAwareBinaryReader : BinaryReader
{
    public enum Endianness
    {
        Little,
        Big,
    }
    
    public const Endianness DefaultEndianness = Endianness.Little;

    private Endianness _endianness = DefaultEndianness;
    
    public EndiannessAwareBinaryReader(Stream input) : base(input)
    {
    }

    public EndiannessAwareBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
    {
    }

    public EndiannessAwareBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
    {
    }

    public EndiannessAwareBinaryReader(Stream input, Endianness endianness) : base(input)
    {
        _endianness = endianness;
    }

    public EndiannessAwareBinaryReader(Stream input, Encoding encoding, Endianness endianness) : base(input, encoding)
    {
        _endianness = endianness;
    }

    public EndiannessAwareBinaryReader(Stream input, Encoding encoding, bool leaveOpen, Endianness endianness) : base(input, encoding, leaveOpen)
    {
        _endianness = endianness;
    }

    public Endianness SetEndianness(Endianness endianness)
    {
        var oldEndianness = _endianness;
        _endianness = endianness;
        
        return oldEndianness;
    }

    public Endianness GetEndianness()
    {
        return _endianness;
    }

    public override short ReadInt16() => ReadInt16(_endianness);

    public override int ReadInt32() => ReadInt32(_endianness);

    public override long ReadInt64() => ReadInt64(_endianness);

    public override ushort ReadUInt16() => ReadUInt16(_endianness);

    public override uint ReadUInt32() => ReadUInt32(_endianness);

    public override ulong ReadUInt64() => ReadUInt64(_endianness);

    public short ReadInt16(Endianness endianness) => BitConverter.ToInt16(ReadForEndianness(sizeof(short), endianness));

    public int ReadInt32(Endianness endianness) => BitConverter.ToInt32(ReadForEndianness(sizeof(int), endianness));

    public long ReadInt64(Endianness endianness) => BitConverter.ToInt64(ReadForEndianness(sizeof(long), endianness));

    public ushort ReadUInt16(Endianness endianness) => BitConverter.ToUInt16(ReadForEndianness(sizeof(ushort), endianness));

    public uint ReadUInt32(Endianness endianness) => BitConverter.ToUInt32(ReadForEndianness(sizeof(uint), endianness));

    public ulong ReadUInt64(Endianness endianness) => BitConverter.ToUInt64(ReadForEndianness(sizeof(ulong), endianness));

    public byte[] ReadBytesWithEndianness(int bytesToRead)
    {
        return ReadForEndianness(bytesToRead, _endianness);
    }
    
    private byte[] ReadForEndianness(int bytesToRead, Endianness endianness)
    {
        var bytesRead = ReadBytes(bytesToRead);

        if ((endianness == Endianness.Little && !BitConverter.IsLittleEndian)
            || (endianness == Endianness.Big && BitConverter.IsLittleEndian))
        {
            Array.Reverse(bytesRead);
        }

        return bytesRead;
    }
}
