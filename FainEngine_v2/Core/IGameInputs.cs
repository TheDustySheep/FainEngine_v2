using Silk.NET.Input;
using System.Numerics;

namespace FainEngine_v2.Core;
public interface IGameInputs
{
    CursorMode CursorMode { get; }
    Vector2 MouseDelta { get; }
    Vector2 MousePosition { get; }
    Vector2 ScrollDelta { get; }

    void ExitProgram();
    bool IsKeyDown(Key key);
    bool IsKeyHeld(Key key);
    bool IsKeyUp(Key key);
    bool IsMouseDown(MouseButton key);
    bool IsMouseHeld(MouseButton key);
    bool IsMouseUp(MouseButton key);
    void SetCursorMode(CursorMode cursorMode);
}