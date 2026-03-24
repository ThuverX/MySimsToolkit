using System;
using System.Collections.Generic;
using Godot;
using MySimsToolkit.Scripts.Nodes;
using MySimsToolkit.Scripts.Platforms;

namespace MySimsToolkit.Scripts.Services;

public class ShaderService : IDisposable
{
    public static ShaderService Instance => RuntimeRoot.Instance.Runtime.Shaders;
    
    public Dictionary<string, Shader> ShaderMap = new();

    public void RegisterShader(string handle, Shader shader)
    {
        ShaderMap.Add(handle, shader);
    }

    public Shader GetShader(string handle)
    {
        return ShaderMap.GetValueOrDefault(handle);
    }

    public ShaderMaterial GetMaterialForShader(string handle)
    {
        var mat = new ShaderMaterial();
        mat.Shader = ShaderMap.GetValueOrDefault(handle);
        return mat;
    }

    public void Dispose()
    {
        ShaderMap.Clear();
    }
}