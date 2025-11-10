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
