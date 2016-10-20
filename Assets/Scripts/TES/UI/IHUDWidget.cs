using UnityEngine;

namespace TESUnity.UI
{
    /// <summary>
    /// Defines a HUD element. In VR HUD elements are moved to a special canvas that follows the head's rotation and position.
    /// This type of widget must be small and must not appear for a long time.
    /// </summary>
    public interface IHUDWidget
    {
        void SetParent(Transform transform);
    }
}
