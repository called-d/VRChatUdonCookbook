
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace called_D.UdonCookbook
{
    public class InteractProxy : UdonSharpBehaviour
    {
        [SerializeField] private UdonBehaviour _target;
        [SerializeField] private string _eventName;


        public override void Interact()
        {
            if (!Utilities.IsValid(_target))
            {
                Debug.LogError("_target is not valid.", this);
                return;
            }
            if (string.IsNullOrEmpty(_eventName))
            {
                Debug.LogError("_eventName is null or \"\".", this);
                return;
            }

            _target.SendCustomEvent(_eventName);
        }
    }
}
