using System;
using System.Collections.Generic;
using System.IO;
using MySimsToolkit.Scripts.Extensions;

namespace MySimsToolkit.Scripts.Formats.Nintendo;

// enum ImageFormat : u32 {
// I4 = 0,
// I8 = 1,
// IA4 = 2,
// IA8 = 3,
// RGB565 = 4,
// RGB5A3 = 5,
// RGBA8 = 6,
// };
//
// struct RGB5A3 {
//     be u16 col;
// };
//
// struct ImageHeader {
//     be u16 height;
//     be u16 width;
//     be ImageFormat format;
//     be u32 imageDataOffset;
//     be u32 wraps;
//     be u32 wrapt;
//     be u32 minfilter;
//     be u32 magfilter;
//     float lodBias;
//     be u8 edgeLodEnable;
//     be u8 minLod;
//     be u8 maxLod;
//     be u8 unpacked;
//     u64 jump = $;
//     
//         $ = imageDataOffset;
//     RGB5A3 pixels[width * height];
//     
//         $ = jump;
// };
//
// struct Image {
//     ImageHeader header;
// };
//
// struct ImageTableItem {
//     Image* image : be u32;
//     be u32 paletteHeaderOffset;
// };
//
// struct TplFile {
//     be u32 version;
//     be u32 count;
//     be u32 tableOffset;
//     $ = tableOffset;
//     ImageTableItem images[count];
// };
//
// TplFile file @ $;

public class TplFile
{
    public List<TplImage> TplImages { get; set; } = [];

    public record TplImage
    {
        public enum ImageFormat
        {
            I4 = 0,
            I8 = 1,
            Ia4 = 2,
            Ia8 = 3,
            Rgb565 = 4,
            Rgb5A3 = 5,
            Rgba8 = 6,
            Cmpr = 14
        }

        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public ImageFormat Format { get; set; }
        public byte[] Data { get; set; }

        public static TplImage Read(EndiannessAwareBinaryReader binaryReader)
        {
            var image = new TplImage
            {
                Height = binaryReader.ReadUInt16(),
                Width = binaryReader.ReadUInt16(),
                Format = (ImageFormat)binaryReader.ReadUInt32()
            };

            var dataOffset = binaryReader.ReadUInt32();

            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();

            binaryReader.ReadSingle();

            binaryReader.ReadByte();
            binaryReader.ReadByte();
            binaryReader.ReadByte();
            binaryReader.ReadByte();

            image.Data = new byte[image.Width * image.Height * 4];

            using (binaryReader.Jump(dataOffset))
            {
                ReadTplImageData(binaryReader, image.Format, image.Width, image.Height, image.Data);
            }

            return image;
        }

        public static void ReadTplImageData(
            EndiannessAwareBinaryReader br,
            ImageFormat format,
            int width,
            int height,
            byte[] dst)
        {
            if (format == ImageFormat.Cmpr)
            {
                for (int by = 0; by < height; by += 8)
                {
                    for (int bx = 0; bx < width; bx += 8)
                    {
                        for (int subY = 0; subY < 2; subY++)
                        {
                            for (int subX = 0; subX < 2; subX++)
                            {
                                int blockStartX = bx + subX * 4;
                                int blockStartY = by + subY * 4;

                                ushort c0 = br.ReadUInt16();
                                ushort c1 = br.ReadUInt16();
                                uint bits = br.ReadUInt32();

                                byte r0 = (byte)((c0 >> 11) & 0x1F);
                                byte g0 = (byte)((c0 >> 5) & 0x3F);
                                byte b0 = (byte)(c0 & 0x1F);
                                r0 = (byte)((r0 << 3) | (r0 >> 2));
                                g0 = (byte)((g0 << 2) | (g0 >> 4));
                                b0 = (byte)((b0 << 3) | (b0 >> 2));

                                byte r1 = (byte)((c1 >> 11) & 0x1F);
                                byte g1 = (byte)((c1 >> 5) & 0x3F);
                                byte b1 = (byte)(c1 & 0x1F);
                                r1 = (byte)((r1 << 3) | (r1 >> 2));
                                g1 = (byte)((g1 << 2) | (g1 >> 4));
                                b1 = (byte)((b1 << 3) | (b1 >> 2));

                                byte r2, g2, b2, a2, r3, g3, b3, a3;
                                if (c0 > c1)
                                {
                                    r2 = (byte)((2 * r0 + r1) / 3);
                                    g2 = (byte)((2 * g0 + g1) / 3);
                                    b2 = (byte)((2 * b0 + b1) / 3);
                                    a2 = 255;

                                    r3 = (byte)((r0 + 2 * r1) / 3);
                                    g3 = (byte)((g0 + 2 * g1) / 3);
                                    b3 = (byte)((b0 + 2 * b1) / 3);
                                    a3 = 255;
                                }
                                else
                                {
                                    r2 = (byte)((r0 + r1) / 2);
                                    g2 = (byte)((g0 + g1) / 2);
                                    b2 = (byte)((b0 + b1) / 2);
                                    a2 = 255;

                                    r3 = g3 = b3 = 0;
                                    a3 = 0; // transparent
                                }

                                for (int py = 0; py < 4; py++)
                                {
                                    for (int px = 0; px < 4; px++)
                                    {
                                        int bitIndex = 30 - 2 * (py * 4 + px); // MSB -> LSB
                                        int sel = (int)((bits >> bitIndex) & 0x3);
                                        
                                        byte r, g, b, a;
                                        switch (sel)
                                        {
                                            case 0: r = r0; g = g0; b = b0; a = 255; break;
                                            case 1: r = r1; g = g1; b = b1; a = 255; break;
                                            case 2: r = r2; g = g2; b = b2; a = a2; break;
                                            case 3: r = r3; g = g3; b = b3; a = a3; break;
                                            default: r = g = b = a = 0; break;
                                        }

                                        int dstX = blockStartX + px;
                                        int dstY = blockStartY + py;

                                        if (dstX >= width || dstY >= height)
                                            continue;

                                        int idx = (dstY * width + dstX) * 4;
                                        dst[idx + 0] = r;
                                        dst[idx + 1] = g;
                                        dst[idx + 2] = b;
                                        dst[idx + 3] = a;
                                    }
                                }
                            }
                        }
                    }
                }

                return;
            }

            (int blockW, int blockH) = format switch
            {
                ImageFormat.I4 => (8, 8),
                ImageFormat.I8 => (8, 4),
                ImageFormat.Ia4 => (8, 4),
                ImageFormat.Ia8 => (4, 4),
                ImageFormat.Rgb565 => (4, 4),
                ImageFormat.Rgb5A3 => (4, 4),
                ImageFormat.Rgba8 => (4, 4),
                _ => throw new NotSupportedException()
            };

            bool i4High = true;
            byte i4Current = 0;

            for (int by = 0; by < height; by += blockH)
            {
                for (int bx = 0; bx < width; bx += blockW)
                {
                    for (int y = 0; y < blockH; y++)
                    {
                        for (int x = 0; x < blockW; x++)
                        {
                            int px = bx + x;
                            int py = by + y;

                            byte r, g, b, a;

                            switch (format)
                            {
                                case ImageFormat.I4:
                                {
                                    if (i4High)
                                    {
                                        i4Current = br.ReadByte();
                                        r = (byte)(i4Current >> 4);
                                        i4High = false;
                                    }
                                    else
                                    {
                                        r = (byte)(i4Current & 0xF);
                                        i4High = true;
                                    }

                                    r = (byte)((r << 4) | r);
                                    g = r;
                                    b = r;
                                    a = 255;
                                    break;
                                }

                                case ImageFormat.I8:
                                {
                                    byte v = br.ReadByte();
                                    r = g = b = v;
                                    a = 255;
                                    break;
                                }

                                case ImageFormat.Ia4:
                                {
                                    byte v = br.ReadByte();
                                    a = (byte)((v >> 4) & 0xF);
                                    r = g = b = (byte)(v & 0xF);
                                    a = (byte)((a << 4) | a);
                                    r = (byte)((r << 4) | r);
                                    g = r;
                                    b = r;
                                    break;
                                }

                                case ImageFormat.Ia8:
                                {
                                    a = br.ReadByte();
                                    byte v = br.ReadByte();
                                    r = g = b = v;
                                    break;
                                }

                                case ImageFormat.Rgb565:
                                {
                                    ushort val = br.ReadUInt16();
                                    r = (byte)((val >> 11) & 0x1F);
                                    g = (byte)((val >> 5) & 0x3F);
                                    b = (byte)(val & 0x1F);
                                    r = (byte)((r << 3) | (r >> 2));
                                    g = (byte)((g << 2) | (g >> 4));
                                    b = (byte)((b << 3) | (b >> 2));
                                    a = 255;
                                    break;
                                }

                                case ImageFormat.Rgb5A3:
                                {
                                    ushort val = br.ReadUInt16();
                                    if ((val & 0x8000) != 0)
                                    {
                                        a = 255;
                                        r = (byte)((val >> 10) & 0x1F);
                                        g = (byte)((val >> 5) & 0x1F);
                                        b = (byte)(val & 0x1F);
                                        r = (byte)((r << 3) | (r >> 2));
                                        g = (byte)((g << 3) | (g >> 2));
                                        b = (byte)((b << 3) | (b >> 2));
                                    }
                                    else
                                    {
                                        a = (byte)((val >> 12) & 0x7);
                                        r = (byte)((val >> 8) & 0xF);
                                        g = (byte)((val >> 4) & 0xF);
                                        b = (byte)(val & 0xF);
                                        a = (byte)((a << 5) | (a << 2) | (a >> 1));
                                        r = (byte)((r << 4) | r);
                                        g = (byte)((g << 4) | g);
                                        b = (byte)((b << 4) | b);
                                    }

                                    break;
                                }

                                case ImageFormat.Rgba8:
                                {
                                    a = br.ReadByte();
                                    r = br.ReadByte();
                                    g = br.ReadByte();
                                    b = br.ReadByte();
                                    break;
                                }

                                default:
                                    throw new NotSupportedException();
                            }

                            // write if inside bounds
                            if (px >= width || py >= height)
                                continue;

                            int idx = (py * width + px) * 4;
                            dst[idx + 0] = r;
                            dst[idx + 1] = g;
                            dst[idx + 2] = b;
                            dst[idx + 3] = a;
                        }
                    }
                }
            }
        }
    }

    public static TplFile Read(EndiannessAwareBinaryReader binaryReader)
    {
        var ogEndianness = binaryReader.SetEndianness(EndiannessAwareBinaryReader.Endianness.Big);

        var tplFile = new TplFile();
        var magic = binaryReader.ReadUInt32();

        switch (magic)
        {
            case 0x20af30:
            {
                var imagesCount = binaryReader.ReadUInt32();
                var imagesTableOffset = binaryReader.ReadUInt32();

                binaryReader.BaseStream.Seek(imagesTableOffset, SeekOrigin.Begin);

                for (var i = 0; i < imagesCount; i++)
                {
                    var imageHeaderOffset = binaryReader.ReadUInt32();
                    var imagePaletteHeaderOffset = binaryReader.ReadUInt32();

                    binaryReader.BaseStream.Seek(imageHeaderOffset, SeekOrigin.Begin);
                    tplFile.TplImages.Add(TplImage.Read(binaryReader));
                }

                break;
            }
            case 0x14fe0149:
            {
                binaryReader.ReadUInt32();
                binaryReader.ReadUInt32();
                binaryReader.ReadUInt32();
                binaryReader.ReadUInt32();
                binaryReader.ReadUInt32();
                var width = binaryReader.ReadUInt16();
                var height = binaryReader.ReadUInt16();

                binaryReader.ReadByte();
                binaryReader.ReadByte();
                binaryReader.ReadByte();

                var format = (TplImage.ImageFormat)binaryReader.ReadByte();

                binaryReader.ReadUInt32();
                binaryReader.ReadUInt32();
                binaryReader.ReadUInt32();

                binaryReader.ReadUInt32();
                binaryReader.ReadUInt32();
                binaryReader.ReadUInt32();

                var imageOffset = binaryReader.ReadUInt32();

                var image = new TplImage
                {
                    Width = width,
                    Height = height,
                    Format = format,
                    Data = new byte[width * height * 4]
                };

                using (binaryReader.Jump(imageOffset))
                {
                    TplImage.ReadTplImageData(binaryReader, image.Format, image.Width, image.Height, image.Data);
                }

                tplFile.TplImages.Add(image);
                break;
            }
        }

        binaryReader.SetEndianness(ogEndianness);

        return tplFile;
    }
}