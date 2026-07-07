namespace FainEngine_v2.ECS;
public abstract class SystemBase
{
    public abstract void Update(EntityStore store);
}

public abstract class System<T1> : SystemBase
    where T1 : struct, IComponent
{
    public override void Update(EntityStore store)
    {
        foreach (var arch in store.Query(typeof(T1)))
        {
            var a = (T1[])arch.Columns[typeof(T1)];
            for (int i = 0; i < arch.Count; i++)
                Execute(ref a[i]);
        }
    }

    protected abstract void Execute(ref T1 c1);
}

public abstract class System<T1, T2> : SystemBase
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    public override void Update(EntityStore store)
    {
        foreach (var arch in store.Query(typeof(T1), typeof(T2)))
        {
            var a = (T1[])arch.Columns[typeof(T1)];
            var b = (T2[])arch.Columns[typeof(T2)];

            for (int i = 0; i < arch.Count; i++)
                Execute(ref a[i], ref b[i]);
        }
    }

    protected abstract void Execute(ref T1 c1, ref T2 c2);
}

public abstract class System<T1, T2, T3> : SystemBase
    where T1 : struct, IComponent
    where T2 : struct, IComponent
    where T3 : struct, IComponent
{
    public override void Update(EntityStore store)
    {
        foreach (var arch in store.Query(typeof(T1), typeof(T2), typeof(T3)))
        {
            var a = (T1[])arch.Columns[typeof(T1)];
            var b = (T2[])arch.Columns[typeof(T2)];
            var c = (T3[])arch.Columns[typeof(T2)];

            for (int i = 0; i < arch.Count; i++)
                Execute(ref a[i], ref b[i], ref c[i]);
        }
    }

    protected abstract void Execute(ref T1 c1, ref T2 c2, ref T3 c3);
}