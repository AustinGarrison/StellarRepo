
using CallSOS.Utilities;

namespace CallSOS.Player.Interaction
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool CanHandleRaycast(Utilities.ObjectInteractController callingController);
        InteractItem GetInteractItem();
    }
}