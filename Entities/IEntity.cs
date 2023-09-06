namespace FainEngine_v2.Entities;
public interface IEntity
{
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Dispose() { }
}
