using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Godot;
using MySimsToolkit.Scripts.Extensions;
using MySimsToolkit.Scripts.Formats.FileSystem.FileId;

namespace MySimsToolkit.Scripts.Formats.Material;

public record MaterialData : IMaterial
{
    public record MaterialParameter
    {
        public enum ParameterType
        {
            Color = 1,
            Integer = 2,
            Map = 4
        }
        
        public uint Name { get; set; }
        public ParameterType Type { get; set; }
        
        public int? IntegerValue { get; set; }
        public Color? ColorValue { get; set; }
        public ResourceKey? MapValue { get; set; }
        
        public static MaterialParameter Read(EndiannessAwareBinaryReader binaryReader, long mtrlOffset)
        {
            var param = new MaterialParameter
            {
                Name = binaryReader.ReadUInt32(),
                Type = (ParameterType) binaryReader.ReadUInt32()
            };
            
            var numFields = binaryReader.ReadUInt32();
            var valueOffset = binaryReader.ReadUInt32();

            using (binaryReader.Jump(mtrlOffset + valueOffset))
            {
                switch (param.Type)
                {
                    case ParameterType.Color:
                    {
                        switch (numFields)
                        {
                            case 1:
                            {
                                var value = binaryReader.ReadSingle();
                                param.ColorValue = new Color(value, value, value, value);
                                break;
                            }
                            case 2:
                            {
                                param.ColorValue = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), 0,
                                    0);
                                break;
                            }
                            case 3:
                            {
                                param.ColorValue = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(),
                                    binaryReader.ReadSingle(), 0);
                                break;
                            }
                            case 4:
                            {
                                param.ColorValue = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(),
                                    binaryReader.ReadSingle(), binaryReader.ReadSingle());
                                break;
                            }
                        }

                        break;
                    }
                    case ParameterType.Integer:
                        param.IntegerValue = binaryReader.ReadInt32();
                        break;
                    case ParameterType.Map:
                        param.MapValue = ResourceKey.Read(binaryReader);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return param;
        }
    }
    
    public uint Name;
    public uint Shader;
    public Dictionary<uint, MaterialParameter> Parameters = [];
    
    public static MaterialData Read(EndiannessAwareBinaryReader binaryReader, MaterialVersion materialVersion)
    {
        var data = new MaterialData();

        if (materialVersion == MaterialVersion.MySims)
        {
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();

            ResourceKey.Read(binaryReader);

            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();

            var matd = binaryReader.ReadString(4);

            Trace.Assert(matd == "MATD", $"Expected matd but got \"{matd}\"");

            binaryReader.ReadUInt32();

            data.Name = binaryReader.ReadUInt32();
        }
        else if (materialVersion is MaterialVersion.Kingdom or MaterialVersion.Agents)
        {
            var matd = binaryReader.ReadString(4);

            Trace.Assert(matd == "MATD", $"Expected matd but got \"{matd}\"");
            binaryReader.ReadUInt32();
        }

        data.Shader = binaryReader.ReadUInt32();
        
        binaryReader.ReadUInt32();
        
        var mtrlOffset = binaryReader.BaseStream.Position;

        var oldEndianness = binaryReader.GetEndianness();
        
        if (materialVersion is MaterialVersion.Kingdom or MaterialVersion.Agents)
        {
            binaryReader.SetEndianness(EndiannessAwareBinaryReader.Endianness.Big);
        }

        var mtrl = binaryReader.ReadString(4);

        Trace.Assert(mtrl == "MTRL", $"Expected MTRL but got \"{mtrl}\"");
        
        binaryReader.ReadUInt32();
        binaryReader.ReadUInt32();
        
        var numParams = binaryReader.ReadUInt32();

        for (var i = 0; i < numParams; i++)
        {
            var param = MaterialParameter.Read(binaryReader, mtrlOffset);
            data.Parameters.Add(param.Name, param);
        }
        
        binaryReader.SetEndianness(oldEndianness);

        return data;
    }

    public MaterialParameter GetParameter(uint name)
    {
        return Parameters.GetValueOrDefault(name);
    }


    public MaterialInfo Material
    {
        get
        {
            var info = new MaterialInfo();

            var textureParam = GetParameter(0x6cc0fd85);
            if (textureParam != null)
            {
                info.MainTexture = new ResourceKeyFileId(textureParam.MapValue);
            }
            
            return info;
        }
    }
}