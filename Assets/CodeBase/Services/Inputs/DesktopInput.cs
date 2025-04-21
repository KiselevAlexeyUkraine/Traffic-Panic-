using UnityEngine;

namespace Codebase.Services.Inputs
{
    public class DesktopInput : IInput
    {
        public bool Left => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        public bool Right => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        public bool Drag => Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        public bool Boost => Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        public bool Action => Input.GetKeyDown(KeyCode.E);
        public bool Pause => Input.GetKeyDown(KeyCode.Escape);
    }
}

