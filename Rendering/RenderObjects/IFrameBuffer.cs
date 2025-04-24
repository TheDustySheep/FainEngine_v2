using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.Rendering.RenderObjects;
public interface IFrameBuffer : IDisposable
{
    public void Bind();
}
