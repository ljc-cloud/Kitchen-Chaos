using System;

namespace KitchenChaos.Interface
{
    /// <summary>
    /// 有进度条的物品
    /// </summary>
    public interface IHasProgress
    {
        public event EventHandler<OnProgressbarChangedEventArgs> OnProgressbarChanged;
        public class OnProgressbarChangedEventArgs : EventArgs
        {
            public float progressNormalized;
        }
    }
}