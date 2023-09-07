using FainEngine_v2.Entities;

namespace FainEngine_v2.Core;

public static class EntityManager
{
    static readonly HashSet<IEntity> entities = new();

    public static T SpawnEntity<T>() where T : IEntity, new()
    {
        T entity = new();
        entities.Add(entity);
        return entity;
    }

    public static T SpawnEntity<T>(T entity) where T : IEntity
    {
        entities.Add(entity);
        return entity;
    }

    public static T SpawnEntity<T>(Func<T> factory) where T : IEntity
    {
        T entity = factory.Invoke();
        entities.Add(entity);
        return entity;
    }

    public static void DestroyEntity(IEntity entity)
    {
        entity.Dispose();
        entities.Remove(entity);
    }

    public static void Update()
    {
        foreach (var entity in entities)
        {
            entity.Update();
        }
    }

    public static void FixedUpdate()
    {
        foreach (var entity in entities)
        {
            entity.FixedUpdate();
        }
    }

    public static void Dispose()
    {
        foreach (var entity in entities)
        {
            entity.Dispose();
        }
    }
}
