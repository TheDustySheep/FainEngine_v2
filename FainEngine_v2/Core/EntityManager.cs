using FainEngine_v2.Entities;

namespace FainEngine_v2.Core;

public class EntityManager
{
    readonly HashSet<IEntity> _entities = new();

    public T Spawn<T>() where T : IEntity, new()
    {
        T entity = new();
        entity.SetEntityManager(this);
        _entities.Add(entity);
        return entity;
    }

    public T Spawn<T>(T entity) where T : IEntity
    {
        entity.SetEntityManager(this);
        _entities.Add(entity);
        return entity;
    }

    public T Spawn<T>(Func<T> factory) where T : IEntity
    {
        T entity = factory.Invoke();
        entity.SetEntityManager(this);
        _entities.Add(entity);
        return entity;
    }

    public void Despawn(IEntity entity)
    {
        entity.Dispose();
        _entities.Remove(entity);
    }

    public void DespawnAll()
    {
        foreach (var entity in _entities)
            entity.Dispose();

        _entities.Clear();
    }

    public void Update()
    {
        foreach (var entity in _entities)
        {
            entity.Update();
        }
    }

    public void FixedUpdate()
    {
        foreach (var entity in _entities)
        {
            entity.FixedUpdate();
        }
    }

    public void Dispose()
    {
        foreach (var entity in _entities)
        {
            entity.Dispose();
        }
    }
}
