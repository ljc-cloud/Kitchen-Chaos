using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public class IconTemplate : MonoBehaviour
    {
        [SerializeField] private Image image;
        
        public void UpdateImage(Sprite sprite)
        {
            image.sprite = sprite;
        }
    }

}
