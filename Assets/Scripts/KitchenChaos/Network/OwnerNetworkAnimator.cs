using Unity.Netcode.Components;

namespace Unity.Multyplayer.Samples.Utilities.ClientAuthority
{
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        /// <summary>
        /// Client Auth Animator
        /// </summary>
        /// <returns></returns>
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}