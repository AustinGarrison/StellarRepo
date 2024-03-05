
using CallSOS.Utilities;

namespace CallSOS.Player.Interaction
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool CanHandleRaycast(CursorController callingController);
        InteractItem GetInteractItem();
    }
}