namespace FainEngine_v2.Utils;
public class WorkerThread
{
    Thread thread;

    public WorkerThread(Action action)
    {
        thread = new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            while (true)
            {
                action.Invoke();
                Thread.Sleep(1);
            }
        });

        thread.Start();
    }
}
