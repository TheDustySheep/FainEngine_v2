using Silk.NET.Input;
using Silk.NET.Windowing;
using System.Numerics;

namespace FainEngine_v2.Core;
public class GameInputs : IGameInputs
{
    readonly IWindow? _window;
    readonly IInputContext _input;

    readonly HashSet<Key> KeysDown = new();
    readonly HashSet<Key> KeysHeld = new();
    readonly HashSet<Key> KeysUp = new();

    readonly HashSet<MouseButton> MouseDown = new();
    readonly HashSet<MouseButton> MouseHeld = new();
    readonly HashSet<MouseButton> MouseUp = new();

    public Vector2 ScrollDelta { get; private set; } = Vector2.Zero;

    private Vector2 _lastMousePosition = Vector2.Zero;
    public Vector2 MousePosition { get; private set; } = Vector2.Zero;
    public Vector2 MouseDelta { get; private set; } = Vector2.Zero;
    public CursorMode CursorMode { get; private set; } = CursorMode.Raw;

    internal GameInputs(IWindow window)
    {
        _window = window;
        _input = window.CreateInput();
        _input.ConnectionChanged += OnConnectionChange;

        var primaryKeyboard = _input.Keyboards.FirstOrDefault();

        if (primaryKeyboard != null)
        {
            primaryKeyboard.KeyDown += OnKeyDown;
            primaryKeyboard.KeyUp += OnKeyUp;
        }
        for (int i = 0; i < _input.Mice.Count; i++)
        {
            _input.Mice[i].MouseMove += OnMouseMove;
            _input.Mice[i].Scroll += OnMouseWheel;
            _input.Mice[i].MouseDown += OnMouseDown;
            _input.Mice[i].MouseUp += OnMouseUp;
        }

        SetCursorMode(CursorMode.Raw);
    }

    internal void Reset()
    {
        KeysDown.Clear();
        KeysUp.Clear();
        MouseDown.Clear();
        MouseUp.Clear();
        MouseDelta = MousePosition - _lastMousePosition;
        _lastMousePosition = MousePosition;
    }


    #region Public Methods
    public bool IsKeyDown(Key key) => KeysDown.Contains(key);
    public bool IsKeyHeld(Key key) => KeysHeld.Contains(key);
    public bool IsKeyUp(Key key) => KeysUp.Contains(key);

    public bool IsMouseDown(MouseButton key) => MouseDown.Contains(key);
    public bool IsMouseHeld(MouseButton key) => MouseHeld.Contains(key);
    public bool IsMouseUp(MouseButton key) => MouseUp.Contains(key);

    public void ExitProgram()
    {
        _window?.Close();
    }

    public void SetCursorMode(CursorMode cursorMode)
    {
        CursorMode = cursorMode;
        for (int i = 0; i < _input.Mice.Count; i++)
            _input.Mice[i].Cursor.CursorMode = cursorMode;
    }
    #endregion

    #region Event Handlers
    private void OnConnectionChange(IInputDevice device, bool state)
    {
        SetCursorMode(CursorMode);
    }

    private void OnKeyDown(IKeyboard keyboard, Key key, int arg3)
    {
        if (key == Key.Escape)
        {
            _window?.Close();
        }

        KeysHeld.Add(key);
        KeysDown.Add(key);
    }

    private void OnKeyUp(IKeyboard keyboard, Key key, int arg3)
    {
        KeysUp.Add(key);
        KeysHeld.Remove(key);
    }

    private void OnMouseMove(IMouse mouse, Vector2 position)
    {
        MousePosition = position;
    }

    private void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
    {
        ScrollDelta = new Vector2(scrollWheel.X, scrollWheel.Y);
    }

    private void OnMouseDown(IMouse mouse, MouseButton button)
    {
        MouseHeld.Add(button);
        MouseDown.Add(button);
    }

    private void OnMouseUp(IMouse mouse, MouseButton button)
    {
        MouseUp.Add(button);
        MouseHeld.Remove(button);
    }
    #endregion
}
