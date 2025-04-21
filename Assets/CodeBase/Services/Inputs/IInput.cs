namespace Codebase.Services.Inputs
{
    public interface IInput
    {
        public bool Left { get; }
        public bool Right { get; }
        public bool Drag { get; }
        public bool Boost { get; }
        public bool Action { get; }
        public bool Pause { get; }
    }
}