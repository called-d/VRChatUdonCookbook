
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace called_D.UdonCookbook
{
    public class PlayerTriggerProxy : UdonSharpBehaviour
    {
        [SerializeField] private UdonBehaviour _target;
        [SerializeField] private string _enterEventName;
        [SerializeField] private string _exitEventName;

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player)) return;
            if (!player.isLocal) return;
            if (!Utilities.IsValid(_target)) return;
            if (string.IsNullOrEmpty(_enterEventName)) return;

            _target.SendCustomEvent(_enterEventName);
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player)) return;
            if (!player.isLocal) return;
            if (!Utilities.IsValid(_target)) return;
            if (string.IsNullOrEmpty(_exitEventName)) return;

            _target.SendCustomEvent(_exitEventName);
        }
    }
}
