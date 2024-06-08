using System;

namespace KitchenChaos.Interface
{
    /// <summary>
    /// �н���������Ʒ
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