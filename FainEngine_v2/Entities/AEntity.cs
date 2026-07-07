using FainEngine_v2.Core;

namespace FainEngine_v2.Entities;
public abstract class AEntity : IEntity
{
    protected AEntity(EntityManager manager)
    {
        Manager = manager;
    }

    public EntityManager Manager { get; set; }


}
