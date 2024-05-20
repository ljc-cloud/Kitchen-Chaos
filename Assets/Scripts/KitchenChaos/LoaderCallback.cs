using UnityEngine;

namespace KitchenChaos
{
    public class LoaderCallback : MonoBehaviour
    {
        private bool _isFirstCallback = true;

        private void Update()
        {
            if (_isFirstCallback) {
                _isFirstCallback = false;
                Loader.LoaderCallback();
            }

        }
    }
}
