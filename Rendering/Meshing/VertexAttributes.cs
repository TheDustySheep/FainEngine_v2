using FainEngine_v2.Core;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.Rendering.Meshing
{
    public static class VertexAttributes
    {

        public static unsafe void SetVertexAttributes<TVertexType, TIndexType>(
            GL gl,
            VertexArrayObject<TVertexType, TIndexType> vao
        )
            where TVertexType : unmanaged
            where TIndexType : unmanaged
        {
            // Bind vertex array
            vao.Bind();

            var fields = typeof(TVertexType)
                .GetFields()
                .OrderBy(f => Marshal.OffsetOf(typeof(TVertexType), f.Name).ToInt32())
                .ToArray();

            uint runningOffset = 0;
            for (uint i = 0; i < fields.Length; i++)
            {
                // Field Metadata
                var field = fields[i];
                var gl_meta = GL_Meta_Lookup[field.FieldType];
                var attribs = Attribute.GetCustomAttributes(field);

                // Field Data
                uint index = i;
                int data_count = gl_meta.GL_Data_Count;
                var type = gl_meta.GL_Data_Type;
                bool normalized = false;
                uint stride = (uint)sizeof(TVertexType);

                // Custom Attribute Settings
                foreach (var attrib in attribs)
                {
                    if (attrib is VertexSettingsAttribute vertexSettings)
                    {
                        normalized = vertexSettings.Normalized;
                    }
                }

                vao.VertexAttributePointer(index, data_count, type, normalized, stride, runningOffset);

                // Update Running Offset
                runningOffset += gl_meta.Size;
            }
            gl.EnableVertexAttribArray(0);
        }

        #region Vertex Attribute Lookup
        private static readonly Dictionary<Type, VertexAttribData> GL_Meta_Lookup = new()
    {
        {
            typeof(uint),
            new VertexAttribData
            {
                Size = sizeof(uint),
                GL_Data_Count = 1,
                GL_Data_Type = VertexAttribPointerType.UnsignedInt,
            }
        },
        {
            typeof(int),
            new VertexAttribData
            {
                Size = sizeof(int),
                GL_Data_Count = 1,
                GL_Data_Type = VertexAttribPointerType.Int,
            }
        },
        {
            typeof(float),
            new VertexAttribData
            {
                Size = sizeof(float),
                GL_Data_Count = 1,
                GL_Data_Type = VertexAttribPointerType.Float,
            }
        },
        {
            typeof(Vector2),
            new VertexAttribData
            {
                Size = 2 * sizeof(float),
                GL_Data_Count = 2,
                GL_Data_Type = VertexAttribPointerType.Float,
            }
        },
        {
            typeof(Vector3),
            new VertexAttribData
            {
                Size = 3 * sizeof(float),
                GL_Data_Count = 3,
                GL_Data_Type = VertexAttribPointerType.Float,
            }
        },
        {
            typeof(Vector4),
            new VertexAttribData
            {
                Size = 4 * sizeof(float),
                GL_Data_Count = 4,
                GL_Data_Type = VertexAttribPointerType.Float,
            }
        },
    };

        private struct VertexAttribData
        {
            public required uint Size;
            public required VertexAttribPointerType GL_Data_Type;
            public required int GL_Data_Count;
        }
        #endregion
    }
}
