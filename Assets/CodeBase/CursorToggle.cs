using UnityEngine;

public class CursorToggle
{
    private bool _isCursorVisible;

    public void ToggleCursorVisibility()
    {
        _isCursorVisible = !_isCursorVisible;

        Cursor.visible = _isCursorVisible;
        Cursor.lockState = _isCursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void Enable()
    {
        _isCursorVisible = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Disable()
    {
        _isCursorVisible = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}