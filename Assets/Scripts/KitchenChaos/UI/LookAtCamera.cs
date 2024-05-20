using System;
using UnityEngine;

namespace KitchenChaos.UI
{
    public class LookAtCamera : MonoBehaviour
    {
        private enum Mode
        {
            LookAt,
            LookAtInvert,
            CameraForward,
            CameraForwardInvert,
        }

        [SerializeField] private Mode mode;
        private void LateUpdate()
        {
            switch (mode)
            {
                case Mode.LookAt:
                    transform.LookAt(Camera.main.transform);
                    break;
                case Mode.LookAtInvert:
                    var lookDir = transform.position - Camera.main.transform.position;
                    transform.LookAt(transform.position + lookDir);
                    break;
                case Mode.CameraForward:
                    transform.forward = Camera.main.transform.forward;
                    break;
                case Mode.CameraForwardInvert:
                    transform.forward = -Camera.main.transform.forward;
                    break;
            }
        }
    }
}
