using FainEngine_v2.Core.GameObjects;

namespace FainEngine_v2.Entities;
public abstract class GameObject : IEntity
{
    public Transform Transform = new();

    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Dispose() { }
}
