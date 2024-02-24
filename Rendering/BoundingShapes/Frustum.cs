using System.Numerics;

namespace FainEngine_v2.Rendering.BoundingShapes;
public struct Frustum
{
    public Plane LeftPlane;
    public Plane RightPlane;
    public Plane BottomPlane;
    public Plane TopPlane;
    public Plane NearPlane;
    public Plane FarPlane;

    public static Frustum Cube = new Frustum()
    {
        LeftPlane = new Plane(Vector3.UnitX, -1f),
        RightPlane = new Plane(-Vector3.UnitX, 1f),

        TopPlane = new Plane(-Vector3.UnitY, 1f),
        BottomPlane = new Plane(Vector3.UnitZ, -1f),

        NearPlane = new Plane(Vector3.UnitZ, -1f),
        FarPlane = new Plane(-Vector3.UnitZ, 1f)
    };

    public Frustum(Plane[] planes)
    {
        for (int i = 0; i < 6; i++)
        {
            this[i] = planes[i];
        }
    }

    public bool Intersects(BoundingBox bounds)
    {
        Span<Vector3> corners = stackalloc Vector3[8];
        bounds.GetVertices(ref corners);

        Span<Plane> planes = stackalloc Plane[6]
        {
            LeftPlane,
            RightPlane,
            BottomPlane,
            TopPlane,
            NearPlane,
            FarPlane,
        };

        // If ALL the corners are below ANY plane then they don't intersect
        foreach (Plane plane in planes)
        {
            int inCount = 8;
            foreach (Vector3 corner in corners)
            {
                if (!plane.IsAbove(corner))
                {
                    inCount -= 1;
                }
            }
            if (inCount <= 0)
                return false;
        }
        return true;
    }

    public Plane this[int i]
    {
        get
        {
            return i switch
            {
                0 => LeftPlane,
                1 => RightPlane,
                2 => BottomPlane,
                3 => TopPlane,
                4 => NearPlane,
                5 => FarPlane,
                _ => throw new IndexOutOfRangeException(),
            };
        }
        set
        {
            _ = i switch
            {
                0 => LeftPlane = value,
                1 => RightPlane = value,
                2 => BottomPlane = value,
                3 => TopPlane = value,
                4 => NearPlane = value,
                5 => FarPlane = value,
                _ => throw new IndexOutOfRangeException(),
            };
        }
    }
}
