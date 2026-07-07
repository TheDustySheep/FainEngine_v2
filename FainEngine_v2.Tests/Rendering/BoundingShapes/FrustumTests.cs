using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Rendering.BoundingShapes;
using FainEngine_v2.Rendering.Cameras;
using System.Numerics;
using Plane = FainEngine_v2.Rendering.BoundingShapes.Plane;

namespace FainEngine_v2.Tests.Rendering.BoundingShapes;
public class FrustumTests
{
    private void GetFrustumPlanes(out Frustum frustum, out Plane[] planes)
    {
        planes = new Plane[6];
        planes[0] = new Plane(new Vector3(0.2381448f, -0.2721655f, -1.598973f), 44.82524f);
        planes[1] = new Plane(new Vector3(-1.598973f, -0.2721655f, 0.2381448f), 44.82524f);
        planes[2] = new Plane(new Vector3(-1.013747f, 1.394501f, -1.013747f), 36.74234f);
        planes[3] = new Plane(new Vector3(-0.3470805f, -1.938832f, -0.3470805f), 36.74234f);
        planes[4] = new Plane(new Vector3(-1.360841f, -0.5443366f, -1.360841f), 73.4747f);
        planes[5] = new Plane(new Vector3((float)1.364946E-05, (float)5.453825E-06, (float)1.364946E-05), 923.8687f);

        frustum = new Frustum(planes);
    }

    [Test]
    public void CreateFrustum()
    {
        GetFrustumPlanes(out var frustum, out var planes);

        for (int i = 0; i < 6; ++i)
        {
            if (frustum[i].Normal != planes[i].Normal)
            {
                if (Vector3.Normalize(frustum[i].Normal) == Vector3.Normalize(planes[i].Normal))
                {
                    Assert.That(planes[i].Normal, Is.EqualTo(frustum[i].Normal), "\tLooks like the normal of your frustum plane is NOT NORMALIZED!");
                    return;
                }
                else
                {
                    Assert.That(planes[i].Normal, Is.EqualTo(frustum[i].Normal), "Detected error in frustum, plane " + i);
                    return;
                }
            }
            if (Math.Abs(frustum[i].Distance - planes[i].Distance) > 0.0001f)
            {
                Assert.That(planes[i].Distance, Is.EqualTo(frustum[i].Distance), "Wrong distance for plane: " + i); 
                return;
            }
        }
        Assert.Pass();
    }

    [Test]
    public void IntersectsAABB()
    {
        Transform transform = new();
        Camera3D camera = new Camera3D(transform);
        Frustum frustum = camera.Frustum;

        Assert.Multiple(() =>
        {

            // In front
            Assert.That(frustum.Intersects(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1))), Is.True);

            // In front
            Assert.That(frustum.Intersects(new BoundingBox(new Vector3(0, 0, camera.Z_Far), new Vector3(1, 1, camera.Z_Far + 1))), Is.False);

            // Behind
            Assert.That(frustum.Intersects(new BoundingBox(new Vector3(-20, -20, -100), new Vector3(20, 20, -50))), Is.False);
        });
    }
}