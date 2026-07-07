using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.Meshing;
using System.Numerics;

namespace FainEngine_v2.Rendering.Sprites;
public class SpriteRenderer
{
    private readonly AMesh<SpriteVertex, int> _mesh;
    private readonly SpriteMaterial _material;
    private bool _isDirty = true;

    private readonly List<Sprite> _sprites = new();

    public SpriteRenderer(Shader shader, Texture2D baseTexture)
    {
        Console.WriteLine("Created sprite renderer");
        _mesh = new AMesh<SpriteVertex, int>();
        _mesh.ClipBounds = false;
        _material = new SpriteMaterial(shader, baseTexture);
    }

    private SpriteVertex[] _verts = Array.Empty<SpriteVertex>();
    private int[] _tris = Array.Empty<int>();
    private void RebuildMesh()
    {
        if (_verts.Length < _sprites.Count * 4)
            _verts = new SpriteVertex[_sprites.Count * 4];

        if (_tris.Length < _sprites.Count * 6)
            _tris = new int[_sprites.Count * 6];

        int v = 0;
        int t = 0;

        for (int s = 0; s < _sprites.Count; s++)
        {
            var sprite = _sprites[s];

            // bottom-left
            _verts[v + 0] = new SpriteVertex
            {
                Position = new Vector2(sprite.  Min.X, sprite.  Min.Y),
                TexCoord = new Vector2(sprite.UVMin.X, sprite.UVMin.Y),
                Depth = sprite.Depth
            };            
            
            // top-left
            _verts[v + 1] = new SpriteVertex
            {
                Position = new Vector2(sprite.  Min.X, sprite.  Max.Y),
                TexCoord = new Vector2(sprite.UVMin.X, sprite.UVMax.Y),
                Depth = sprite.Depth
            };            
            
            // top-right
            _verts[v + 2] = new SpriteVertex
            {
                Position = new Vector2(sprite.  Max.X, sprite.  Max.Y),
                TexCoord = new Vector2(sprite.UVMax.X, sprite.UVMax.Y),
                Depth = sprite.Depth
            };

            // bottom-right
            _verts[v + 3] = new SpriteVertex
            {
                Position = new Vector2(sprite.  Max.X, sprite.  Min.Y),
                TexCoord = new Vector2(sprite.UVMax.X, sprite.UVMin.Y),
                Depth = sprite.Depth
            };

            // two triangles
            _tris[t + 0] = v + 0;
            _tris[t + 1] = v + 1;
            _tris[t + 2] = v + 2;

            _tris[t + 3] = v + 2;
            _tris[t + 4] = v + 3;
            _tris[t + 5] = v + 0;

            v += 4;
            t += 6;
        }

        _mesh.SetData(_verts, _tris);
        _isDirty = false;
    }

    public void Draw()
    {
        if (_isDirty)
            RebuildMesh();

        GameGraphics.DrawMesh(
            _mesh,
            _material,
            Matrix4x4.Identity
        );
    }

    public void AddSprite(Sprite sprite)
    {
        _sprites.Add(sprite);
        _isDirty = true;
    }

    public void Clear()
    {
        _sprites.Clear();
        _isDirty = true;
    }

    private struct SpriteVertex
    {
        public Vector2 Position;
        public Vector2 TexCoord;
        public float Depth;
    }

    public struct Sprite
    {
        public Vector2 Min;
        public Vector2 Max;
        public Vector2 UVMin;
        public Vector2 UVMax;
        public float Depth;

        public static Sprite Default = new Sprite()
        {
            Min   = Vector2.Zero,
            Max   = Vector2.One,
            UVMin = Vector2.Zero,
            UVMax = Vector2.One,
            Depth = 0,
        };
    }
}
