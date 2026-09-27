
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace called_D.UdonCookbook
{
    public class PickupProxy : UdonSharpBehaviour
    {
        [SerializeField] private UdonBehaviour _target;
        [SerializeField] private string _dropEventName;
        [SerializeField] private string _pickupEventName;
        [SerializeField] private string _pickupUseDownEventName;
        [SerializeField] private string _pickupUseUpEventName;


        public override void OnPickup()
        {
            if (!Utilities.IsValid(_target))
            {
                Debug.LogError("_target is not valid.", this);
                return;
            }
            if (string.IsNullOrEmpty(_pickupEventName)) return;
            _target.SendCustomEvent(_pickupEventName);
        }

        public override void OnDrop()
        {
            if (!Utilities.IsValid(_target))
            {
                Debug.LogError("_target is not valid.", this);
                return;
            }
            if (string.IsNullOrEmpty(_dropEventName)) return;
            _target.SendCustomEvent(_dropEventName);
        }

        public override void OnPickupUseDown()
        {
            if (!Utilities.IsValid(_target))
            {
                Debug.LogError("_target is not valid.", this);
                return;
            }
            if (string.IsNullOrEmpty(_pickupUseDownEventName)) return;
            _target.SendCustomEvent(_pickupUseDownEventName);
        }

        public override void OnPickupUseUp()
        {
            if (!Utilities.IsValid(_target))
            {
                Debug.LogError("_target is not valid.", this);
                return;
            }
            if (string.IsNullOrEmpty(_pickupUseUpEventName)) return;
            _target.SendCustomEvent(_pickupUseUpEventName);
        }
    }
}
