namespace FainEngine_v2.Utils;
public class WorkerThread
{
    readonly Thread thread;
    bool isActive = true;

    public WorkerThread(string displayName, Action action)
    {
        thread = new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            while (isActive)
            {
                action.Invoke();
                Thread.Sleep(1);
            }
        });
        thread.Name = displayName;
        thread.Start();
    }

    public void Terminate() => isActive = false;
}
