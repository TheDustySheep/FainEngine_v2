using FainEngine_v2.Rendering.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.UI.FontSystem;
internal class UIMaterial : Material
{
    public UIMaterial(Shader shader, Texture fontTexture) : base(shader)
    {
        SetTexture(fontTexture, "textAtlas");
    }
}
