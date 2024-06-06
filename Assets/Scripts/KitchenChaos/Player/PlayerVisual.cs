using UnityEngine;

namespace KitchenChaos.Player
{
    public class PlayerVisual : MonoBehaviour
    {
        [SerializeField] private MeshRenderer headRenderer;
        [SerializeField] private MeshRenderer bodyRenderer;

        private Material _material;

        private void Awake()
        {
            _material = new Material(headRenderer.material);
            headRenderer.material = _material;
            bodyRenderer.material = _material;
        }

        public void SetPlayerColor(Color color)
        {
            _material.color = color;
        }
    }
}

