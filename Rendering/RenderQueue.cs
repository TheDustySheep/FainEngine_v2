using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.Meshing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.Rendering
{
    internal class RenderQueue
    {
        Dictionary<RenderPass, Dictionary<Material, List<RenderInstance>>> _queue;

        public RenderQueue()
        {
            _queue = new();
            foreach (var pass in Enum.GetValues<RenderPass>())
            {
                _queue.Add(pass, new Dictionary<Material, List<RenderInstance>>());
            }
        }

        internal void Enqueue(IMesh mesh, Material mat, Matrix4x4 model)
        {
            var pass = mat.RenderPass;

            var queue = _queue[pass];
            
            if (!queue.TryGetValue(mat, out var list))
            {
                list = new List<RenderInstance>();
                queue.Add(mat, list);
            }

            list.Add(new RenderInstance()
            {
                Mesh = mesh,
                ModelMatrix = model,
            });
        }

        public Dictionary<Material, List<RenderInstance>> Opaque()
        {
            return _queue[RenderPass.Opaque];
        }

        public Dictionary<Material, List<RenderInstance>> Transparent()
        {
            return _queue[RenderPass.Transparent];
        }

        public void Clear()
        {
            foreach (var item in _queue)
            {
                item.Value.Clear();
            }
        }
    }
}
