using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class InputService
{
    public Vector2 Movement { get; private set; }
    public bool EscapePressed { get; private set; }
    public GameState CurrentState { get; private set; } = GameState.Gameplay;

    public void Update()
    {
        Vector2 direction = Vector2.zero;
        bool escape = false;

#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) direction.y += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) direction.y -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) direction.x += 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) direction.x -= 1f;
            escape |= keyboard.escapeKey.wasPressedThisFrame;
        }
#else
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector2(horizontal, vertical);
        escape = Input.GetKeyDown(KeyCode.Escape);
#endif

        Movement = direction.sqrMagnitude > 1f ? direction.normalized : direction;
        EscapePressed = escape;
    }

    public void SetState(GameState state)
    {
        CurrentState = state;
    }

    public void ConsumeEscape()
    {
        EscapePressed = false;
    }
}
