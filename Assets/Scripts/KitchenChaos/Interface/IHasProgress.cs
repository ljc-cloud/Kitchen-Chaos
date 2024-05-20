using System;

namespace KitchenChaos.Interface
{
    public interface IHasProgress
    {
        public event EventHandler<OnProgressbarChangedEventArgs> OnProgressbarChanged;
        public class OnProgressbarChangedEventArgs : EventArgs
        {
            public float progressNormalized;
        }
    }
}