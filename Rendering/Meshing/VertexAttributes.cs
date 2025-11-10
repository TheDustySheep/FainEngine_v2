using Silk.NET.OpenGL;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FainEngine_v2.Rendering.Meshing;

/// <summary>
/// Utility for setting vertex attributes on a VAO using reflection and caching metadata per vertex type.
/// </summary>
public static class VertexAttributes
{
    // Cache of attribute metadata per vertex type
    private static readonly ConcurrentDictionary<Type, VertexAttributeInfo[]> _attributeInfoCache =
        new ConcurrentDictionary<Type, VertexAttributeInfo[]>();

    /// <summary>
    /// Sets up vertex attributes for the specified VAO and vertex type.
    /// </summary>
    public static unsafe void SetVertexAttributes<TVertexType, TIndexType>(
        GL gl,
        VertexArrayObject<TVertexType, TIndexType> vao
    )
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        // Get or create metadata for this vertex type
        var infos = _attributeInfoCache.GetOrAdd(
            typeof(TVertexType),
            type => ReflectAttributeInfos(type)
        );

        uint stride = (uint)sizeof(TVertexType);

        foreach (var info in infos)
        {
            vao.VertexAttributePointer(
                index: info.Index,
                count: info.Count,
                type: info.Type,
                normalized: info.Normalized,
                stride: stride,
                offset: info.Offset
            );
        }
    }

    /// <summary>
    /// Reflects the vertex attributes for a given vertex type.
    /// </summary>
    private static VertexAttributeInfo[] ReflectAttributeInfos(Type vertexType)
    {
        // Get all instance fields, ordered by memory offset
        var fields = vertexType
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .OrderBy(f => Marshal.OffsetOf(vertexType, f.Name).ToInt32())
            .ToArray();

        uint stride = (uint)Marshal.SizeOf(vertexType);
        var infos = new VertexAttributeInfo[fields.Length];

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            // Resolve element count, type and size
            var meta = ResolveVertexAttribData(field.FieldType);
            if (meta is null)
            {
                throw new InvalidOperationException(
                    $"Unsupported vertex field type {field.FieldType}");
            }

            // Determine offset
            uint offset = (uint)Marshal.OffsetOf(vertexType, field.Name);

            // Determine normalization from attribute
            bool normalized = field.GetCustomAttribute<VertexSettingsAttribute>()?.Normalized ?? false;

            infos[i] = new VertexAttributeInfo(
                Index: (uint)i,
                Count: meta.Value.Count,
                Type: meta.Value.Type,
                Normalized: normalized,
                Offset: offset
            );
        }

        return infos;
    }

    /// <summary>
    /// Resolves the OpenGL attribute metadata for a single field type.
    /// Supports float, int, uint and structs of homogeneous primitive fields (1-4 components).
    /// </summary>
    private static VertexAttribData? ResolveVertexAttribData(Type type)
    {
        // Scalar primitives
        if (type == typeof(float))
            return new VertexAttribData(1, VertexAttribPointerType.Float, sizeof(float));
        if (type == typeof(int))
            return new VertexAttribData(1, VertexAttribPointerType.Int, sizeof(int));
        if (type == typeof(uint))
            return new VertexAttribData(1, VertexAttribPointerType.UnsignedInt, sizeof(uint));

        // Handle vector-like structs
        if (type.IsValueType && !type.IsPrimitive)
        {
            var fields = type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fields.Length < 1 || fields.Length > 4)
                return null;

            var baseType = fields[0].FieldType;
            if (fields.Any(f => f.FieldType != baseType))
                return null;

            if (baseType != typeof(float) && baseType != typeof(int) && baseType != typeof(uint))
                return null;

            int count = fields.Length;
            var glType = baseType switch
            {
                var t when t == typeof(float) => VertexAttribPointerType.Float,
                var t when t == typeof(int) => VertexAttribPointerType.Int,
                var t when t == typeof(uint) => VertexAttribPointerType.UnsignedInt,
                _ => throw new NotSupportedException($"Unsupported base type {baseType}")
            };

            uint size = (uint)(count * Marshal.SizeOf(baseType));
            return new VertexAttribData(count, glType, size);
        }

        // Unsupported type
        return null;
    }

    /// <summary>
    /// Represents the metadata needed to configure a single vertex attribute.
    /// </summary>
    private readonly record struct VertexAttributeInfo(
        uint Index,
        int Count,
        VertexAttribPointerType Type,
        bool Normalized,
        uint Offset
    );

    /// <summary>
    /// Temporary struct used during reflection to hold component metadata.
    /// </summary>
    private readonly record struct VertexAttribData(
        int Count,
        VertexAttribPointerType Type,
        uint Size
    );
}