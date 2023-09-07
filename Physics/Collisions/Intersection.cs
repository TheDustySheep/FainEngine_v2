using FainEngine_v2.Extensions;
using FainEngine_v2.Physics.Collisions.Shapes;
using FainEngine_v2.Utils;
using System.Numerics;
using Voxels.Utils.Collisions;
using Plane = FainEngine_v2.Physics.Collisions.Shapes.Plane;

namespace FainEngine_v2.Physics.Collisions;

public class Intersection
{

    // based on https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-box-intersection
    public static bool Intersects(Ray r, AABB aabb, out float dist)
    {
        float tmin, tmax, tymin, tymax, tzmin, tzmax;
        dist = 0f;
        tmin = ((r.Sign.X <= float.Epsilon ? aabb.Min.X : aabb.Max.X) - r.Point.X) * r.InvDir.X;
        tmax = ((r.Sign.X <= float.Epsilon ? aabb.Max.X : aabb.Min.X) - r.Point.X) * r.InvDir.X;
        tymin = ((r.Sign.Y <= float.Epsilon ? aabb.Min.Y : aabb.Max.Y) - r.Point.Y) * r.InvDir.Y;
        tymax = ((r.Sign.Y <= float.Epsilon ? aabb.Max.Y : aabb.Min.Y) - r.Point.Y) * r.InvDir.Y;

        if (tmin > tymax || tymin > tmax)
        {
            return false;
        }

        if (tymin > tmin)
        {
            tmin = tymin;
        }

        if (tymax < tmax)
        {
            tmax = tymax;
        }

        tzmin = ((r.Sign.Z <= float.Epsilon ? aabb.Min.Z : aabb.Max.Z) - r.Point.Z) * r.InvDir.Z;
        tzmax = ((r.Sign.Z <= float.Epsilon ? aabb.Max.Z : aabb.Min.Z) - r.Point.Z) * r.InvDir.Z;

        if (tmin > tzmax || tzmin > tmax)
        {
            return false;
        }

        if (tzmin > tmin)
        {
            tmin = tzmin;
        }

        if (tzmax < tmax)
        {
            tmax = tzmax;
        }
        dist = tmin;
        return true;
    }

    public static bool Intersects(AABB aabb, Ray r, out float dist)
    {
        return Intersects(r, aabb, out dist);
    }

    // based on https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
    public static bool Intersects(Ray r, Triangle tri)
    {
        var e1 = tri.B - tri.A;
        var e2 = tri.C - tri.A;
        var P = Vector3.Cross(r.Dir, e2);
        var det = Vector3.Dot(e1, P);

        if (det > -float.Epsilon && det < float.Epsilon) return false;

        float invDet = 1f / det;

        var T = r.Point - tri.A;
        var u = Vector3.Dot(T, P) * invDet;
        if (u < 0f || u > 1f) return false;

        var Q = Vector3.Cross(T, e1);
        var v = Vector3.Dot(r.Dir, Q * invDet);
        if (v < 0f || u + v > 1f) return false;

        var t = Vector3.Dot(e2, Q) * invDet;
        if (t > float.Epsilon)
        {
            return true;
        }

        return false;
    }

    public static bool Intersects(Triangle tri, Ray r)
    {
        return Intersects(r, tri);
    }

    // based on https://gist.github.com/yomotsu/d845f21e2e1eb49f647f
    public static bool Intersects(Triangle tri, AABB aabb)
    {
        float p0, p1, p2, r;

        Vector3 center = aabb.Center, extents = aabb.Max - center;

        Vector3 v0 = tri.A - center,
            v1 = tri.B - center,
            v2 = tri.C - center;

        Vector3 f0 = v1 - v0,
            f1 = v2 - v1,
            f2 = v0 - v2;

        Vector3 a00 = new Vector3(0, -f0.Z, f0.Y),
            a01 = new Vector3(0, -f1.Z, f1.Y),
            a02 = new Vector3(0, -f2.Z, f2.Y),
            a10 = new Vector3(f0.Z, 0, -f0.X),
            a11 = new Vector3(f1.Z, 0, -f1.X),
            a12 = new Vector3(f2.Z, 0, -f2.X),
            a20 = new Vector3(-f0.Y, f0.X, 0),
            a21 = new Vector3(-f1.Y, f1.X, 0),
            a22 = new Vector3(-f2.Y, f2.X, 0);

        // Test axis a00
        p0 = Vector3.Dot(v0, a00);
        p1 = Vector3.Dot(v1, a00);
        p2 = Vector3.Dot(v2, a00);
        r = extents.Y * MathF.Abs(f0.Z) + extents.Z * MathF.Abs(f0.Y);

        if (MathF.Max(-MathUtils.Max(p0, p1, p2), MathUtils.Min(p0, p1, p2)) > r)
        {
            return false;
        }

        // Test axis a01
        p0 = Vector3.Dot(v0, a01);
        p1 = Vector3.Dot(v1, a01);
        p2 = Vector3.Dot(v2, a01);
        r = extents.Y * MathF.Abs(f1.Z) + extents.Z * MathF.Abs(f1.Y);

        if (MathF.Max(-MathUtils.Max(p0, p1, p2), MathUtils.Min(p0, p1, p2)) > r)
        {
            return false;
        }

        // Test axis a02
        p0 = Vector3.Dot(v0, a02);
        p1 = Vector3.Dot(v1, a02);
        p2 = Vector3.Dot(v2, a02);
        r = extents.Y * MathF.Abs(f2.Z) + extents.Z * MathF.Abs(f2.Y);

        if (MathF.Max(-MathUtils.Max(p0, p1, p2), MathUtils.Min(p0, p1, p2)) > r)
        {
            return false;
        }

        // Test axis a10
        p0 = Vector3.Dot(v0, a10);
        p1 = Vector3.Dot(v1, a10);
        p2 = Vector3.Dot(v2, a10);
        r = extents.X * MathF.Abs(f0.Z) + extents.Z * MathF.Abs(f0.X);
        if (MathF.Max(-MathUtils.Max(p0, p1, p2), MathUtils.Min(p0, p1, p2)) > r)
        {
            return false;
        }

        // Test axis a11
        p0 = Vector3.Dot(v0, a11);
        p1 = Vector3.Dot(v1, a11);
        p2 = Vector3.Dot(v2, a11);
        r = extents.X * MathF.Abs(f1.Z) + extents.Z * MathF.Abs(f1.X);

        if (MathF.Max(-MathUtils.Max(p0, p1, p2), MathUtils.Min(p0, p1, p2)) > r)
        {
            return false;
        }

        // Test axis a12
        p0 = Vector3.Dot(v0, a12);
        p1 = Vector3.Dot(v1, a12);
        p2 = Vector3.Dot(v2, a12);
        r = extents.X * MathF.Abs(f2.Z) + extents.Z * MathF.Abs(f2.X);

        if (MathF.Max(-MathUtils.Max(p0, p1, p2), MathUtils.Min(p0, p1, p2)) > r)
        {
            return false;
        }

        // Test axis a20
        p0 = Vector3.Dot(v0, a20);
        p1 = Vector3.Dot(v1, a20);
        p2 = Vector3.Dot(v2, a20);
        r = extents.X * MathF.Abs(f0.Y) + extents.Y * MathF.Abs(f0.X);

        if (MathF.Max(-MathUtils.Max(p0, p1, p2), MathUtils.Min(p0, p1, p2)) > r)
        {
            return false;
        }

        // Test axis a21
        p0 = Vector3.Dot(v0, a21);
        p1 = Vector3.Dot(v1, a21);
        p2 = Vector3.Dot(v2, a21);
        r = extents.X * MathF.Abs(f1.Y) + extents.Y * MathF.Abs(f1.X);

        if (MathF.Max(-MathUtils.Max(p0, p1, p2), MathUtils.Min(p0, p1, p2)) > r)
        {
            return false;
        }

        // Test axis a22
        p0 = Vector3.Dot(v0, a22);
        p1 = Vector3.Dot(v1, a22);
        p2 = Vector3.Dot(v2, a22);
        r = extents.X * MathF.Abs(f2.Y) + extents.Y * MathF.Abs(f2.X);

        if (MathF.Max(-MathUtils.Max(p0, p1, p2), MathUtils.Min(p0, p1, p2)) > r)
        {
            return false;
        }

        if (MathUtils.Max(v0.X, v1.X, v2.X) < -extents.X || MathUtils.Min(v0.X, v1.X, v2.X) > extents.X)
        {
            return false;
        }

        if (MathUtils.Max(v0.Y, v1.Y, v2.Y) < -extents.Y || MathUtils.Min(v0.Y, v1.Y, v2.Y) > extents.Y)
        {
            return false;
        }

        if (MathUtils.Max(v0.Z, v1.Z, v2.Z) < -extents.Z || MathUtils.Min(v0.Z, v1.Z, v2.Z) > extents.Z)
        {
            return false;
        }


        var normal = Vector3.Cross(f1, f0).Normalized();
        var pl = new Plane(normal, Vector3.Dot(normal, tri.A));
        return Intersects(pl, aabb);
    }

    public static bool Intersects(AABB aabb, Triangle tri)
    {
        return Intersects(tri, aabb);
    }

    public static bool Intersects(Plane pl, AABB aabb)
    {
        Vector3 center = aabb.Center,
            extents = aabb.Max - center;

        var r = extents.X * MathF.Abs(pl.Normal.X) + extents.Y * MathF.Abs(pl.Normal.Y) + extents.Z * MathF.Abs(pl.Normal.Z);
        var s = Vector3.Dot(pl.Normal, center) - pl.Distance;

        return MathF.Abs(s) <= r;
    }

    public static bool Intersects(AABB aabb, Plane pl)
    {
        return Intersects(aabb, pl);
    }

}
