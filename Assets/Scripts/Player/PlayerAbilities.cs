using UnityEngine;

namespace Ascent.Player
{
    public class PlayerAbilities : MonoBehaviour
    {
        [Header("Unlockable Abilities")]
        public bool CanDash = false;
        public bool CanWallJump = false; // Level 2 Wall Climb/Jump support
        public bool CanWallSlide = true; // Still false initially if you want to unlock it with WallJump, but usually WallJump = Cling/Jump
        public bool CanGroundPound = false;

        public void UnlockDash()
        {
            CanDash = true;
        }

        public void UnlockWallSlide()
        {
            CanWallSlide = true;
        }

        public void UnlockWallJump()
        {
            CanWallJump = true;
        }

        public void UnlockGroundPound()
        {
            CanGroundPound = true;
        }
    }
}
