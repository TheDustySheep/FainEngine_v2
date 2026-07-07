namespace FainEngine_v2.Rendering.Meshing;

[AttributeUsage(AttributeTargets.Field)]
public class VertexSettingsAttribute : Attribute
{
    public bool Normalized { get; set; } = false;
}
