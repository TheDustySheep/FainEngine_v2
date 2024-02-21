using System.Numerics;

namespace FainEngine_v2.UI;
public struct Rect
{
    public float X;
    public float Y;
    public float W; 
    public float H;

    public readonly float Width => W;
    public readonly float Height => H;

    public Rect(float x, float y, float w, float h)
    {
        X = x;
        Y = y;
        W = w;
        H = h;
    }

    public override string ToString()
    {
        return $"X: {X} Y: {H} W: {W} H: {H}";
    }
}
