using Silk.NET.Input;
using Silk.NET.Windowing;
using System.Numerics;

namespace FainEngine_v2.Core;
public static class GameInputs
{
    private static IWindow? Window;

    static HashSet<Key> KeysDown = new();
    static HashSet<Key> KeysHeld = new();
    static HashSet<Key> KeysUp   = new();
    
    public static Vector2 ScrollDelta { get; private set; } = Vector2.Zero;

    private static Vector2 _lastMousePosition = Vector2.Zero;
    public static Vector2 MousePosition { get; private set; } = Vector2.Zero;
    public static Vector2 MouseDelta { get; private set; } = Vector2.Zero;

    internal static void SetWindow(IWindow window)
    {
        Window = window;
        IInputContext input = window.CreateInput();
        var primaryKeyboard = input.Keyboards.FirstOrDefault();
        if (primaryKeyboard != null)
        {
            primaryKeyboard.KeyDown += KeyDown;
            primaryKeyboard.KeyUp += KeyUp;
        }
        for (int i = 0; i < input.Mice.Count; i++)
        {
            input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
            input.Mice[i].MouseMove += OnMouseMove;
            input.Mice[i].Scroll += OnMouseWheel;
        }
    }

    public static void Tick()
    {
        KeysDown.Clear();
        KeysUp.Clear();
        MouseDelta = MousePosition - _lastMousePosition;
        _lastMousePosition = MousePosition;
    }

    public static bool IsKeyDown(Key key) => KeysDown.Contains(key);
    public static bool IsKeyHeld(Key key) => KeysHeld.Contains(key);
    public static bool IsKeyUp(Key key) => KeysUp.Contains(key);

    private static unsafe void OnMouseMove(IMouse mouse, Vector2 position)
    {
        MousePosition = position;
    }

    private static unsafe void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
    {
        ScrollDelta = new Vector2(scrollWheel.X, scrollWheel.Y);
    }

    private static void KeyDown(IKeyboard keyboard, Key key, int arg3)
    {
        if (key == Key.Escape)
        {
            Window?.Close();
        }

        KeysHeld.Add(key);
    }

    private static void KeyUp(IKeyboard keyboard, Key key, int arg3)
    {
        KeysHeld.Remove(key);
    }

    public static void ExitProgram()
    {
        Window?.Close();
    }
}
