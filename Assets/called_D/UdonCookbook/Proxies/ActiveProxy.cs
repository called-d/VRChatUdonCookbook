
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace called_D.UdonCookbook
{
    public class ActiveProxy : UdonSharpBehaviour
    {
        [SerializeField] private GameObject[] _syncTargets;
        [SerializeField] private UdonBehaviour[] _targets;
        [SerializeField] private string[] _activeEventNames;
        [SerializeField] private string[] _inactiveEventNames;

        void OnEnable()
        {
            foreach (var obj in _syncTargets)
            {
                if (!Utilities.IsValid(obj)) continue;
                obj.SetActive(true);
            }
            for (int i = 0; i < _targets.Length; i++)
            {
                if (!Utilities.IsValid(_targets[i])) continue;
                if (i >= _activeEventNames.Length || string.IsNullOrEmpty(_activeEventNames[i])) continue;
                _targets[i].SendCustomEvent(_activeEventNames[i]);
            }
        }

        void OnDisable()
        {
            foreach (var obj in _syncTargets)
            {
                if (!Utilities.IsValid(obj)) continue;
                obj.SetActive(false);
            }
            for (int i = 0; i < _targets.Length; i++)
            {
                if (!Utilities.IsValid(_targets[i])) continue;
                if (i >= _inactiveEventNames.Length || string.IsNullOrEmpty(_inactiveEventNames[i])) continue;
                _targets[i].SendCustomEvent(_inactiveEventNames[i]);
            }
        }
    }
}
