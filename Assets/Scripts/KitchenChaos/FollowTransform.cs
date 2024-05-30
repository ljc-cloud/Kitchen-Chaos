using UnityEngine;

namespace KitchenChaos
{
    public class FollowTransform : MonoBehaviour
    {

        private Transform _targetTransform;

        public void SetTargetTransform(Transform targetTransform) => _targetTransform = targetTransform;

        private void LateUpdate()
        {
            if (_targetTransform != null)
            {
                transform.position = _targetTransform.position;
                transform.rotation = _targetTransform.rotation;
            }
        }
    }

}
