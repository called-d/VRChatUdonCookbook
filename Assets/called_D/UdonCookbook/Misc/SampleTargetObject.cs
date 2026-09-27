
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace called_D.UdonCookbook.Misc
{
    public class SampleTargetObject : UdonSharpBehaviour
    {
        private readonly string TAG = "[<color=#ff88ff>SampleTargetObject</color>]";

        public void _EventA()
        {
            Debug.Log($"{TAG}: Event A を受け取りました", this);
            transform.rotation = Random.rotation;
        }
    }
}
