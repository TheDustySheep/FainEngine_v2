using FainEngine_v2.Rendering.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.UI
{
    class UIMaterial : Material
    {
        public UIMaterial(Shader shader, Texture2D atlas) : base(shader)
        {
            SetTexture(atlas, "fontAtlas");
        }
    }
}
