
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace called_D.UdonCookbook
{
    public class SampleToggleButton : UdonSharpBehaviour
    {
        [SerializeField] private GameObject _toggleActiveObject;


        public override void Interact()
        {
            if (!Utilities.IsValid(_toggleActiveObject)) return;
            _toggleActiveObject.SetActive(!_toggleActiveObject.activeSelf);
        }
    }
}
