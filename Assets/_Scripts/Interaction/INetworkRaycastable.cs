using CallSOS.Utilities;

public interface INetworkRaycastable
{
    CursorType GetCursorType();
    bool CanHandleRaycast(ObjectInteractController callingController);
    NetworkedInteractItem GetInteractItem();
}
