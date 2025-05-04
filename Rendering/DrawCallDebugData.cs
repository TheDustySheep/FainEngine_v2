using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.Rendering
{
    public struct DrawCallDebugData
    {
        public uint TotalDrawCalls => OpaqueCalls + TransparentCalls + UICalls;
        public uint OpaqueCalls;
        public uint TransparentCalls;
        public uint UICalls;
    }
}
