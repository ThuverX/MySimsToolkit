using Godot;
using ImGuiNET;
using Neat.Extensions;
using Vector2 = System.Numerics.Vector2;

namespace Neat;

public static class Loader
{
    public static bool Default(float size = 80f)
    {
        var ret = false;
        var cursorPos = ImGui.GetCursorScreenPos();
        cursorPos = new Vector2(Mathf.Floor(cursorPos.X), Mathf.Floor(cursorPos.Y));
        var drawList = ImGui.GetWindowDrawList();
        var t = (float)ImGui.GetTime();
        var len = 2.5f;
        var blockSize = size / 3f;
        var pad = 0.05f;
        
        drawList.PushClipRect(cursorPos, cursorPos + new Vector2(size, size));
        
        float EaseInQuint(float x) => x * x * x * x * x;
        float EaseOutQuint(float x) => 1f - Mathf.Pow(1f - x, 5f);

        var l = 0.5f;
        var r = 1f - l;
        
        var overalTime = (t / len) % 1f;
        if (overalTime < l || overalTime > r) ret = true;

        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var lt = ((t / len) + (i * 0.03f + j * 0.03f)) % 1f;
                var p = 0f;
                var yp = 0f;
                
                if(lt < l) p = EaseOutQuint(lt * (1 / l)) - 1;
                if (lt < l)
                {
                    if (i == 0)
                    {
                        yp = EaseOutQuint(lt * (1 / l)) - 1;
                        p = 0;
                    }
                    if (i == 2)
                    {
                        yp = 1 - EaseOutQuint(lt * (1 / l));
                        p = 0;
                    }
                    
                }
                if (lt >= r) p = EaseInQuint((lt - r) * (1 / l));

                var pos = cursorPos + new Vector2(size * yp, size * p);
                var start = pos + new Vector2(i * blockSize + blockSize * pad, j * blockSize + blockSize * pad);
                var end = start + new Vector2(blockSize * (1f - pad * 2), blockSize * (1f - pad * 2));

                if (i == 1 && j == 0 || i == 2 && j == 1)
                {
                    end = start + new Vector2(blockSize * (1f - pad * 2), blockSize * 2 * (1f - pad));
                }

                if (i == 0 && j == 2)
                {
                    end = start + new Vector2(blockSize * 2 * (1f - pad), blockSize * (1f - pad * 2));
                }
                
                if((i == 1 && j == 1) || (i == 2 && j == 2) || (i == 1 && j == 2)) continue;
                
                drawList.AddRectFilled(start, end, Style.ActiveTheme.Primary.ToHex(), 2);
            }
        }

        drawList.PopClipRect();
        
        ImGui.Dummy(new Vector2(size, size));

        return ret;
    }
    
    public static bool Centered(float size = 80f)
    {
        var start = ImGui.GetCursorScreenPos();
        var avail = ImGui.GetContentRegionAvail();
        var center = avail / 2f - new Vector2(size, -size) / 2f;
        ImGui.SetCursorScreenPos(center);
        var ret = Default(size);
        ImGui.SetCursorScreenPos(start);

        return ret;
    }
}