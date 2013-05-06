using System;

namespace SDRBlocks.Core
{
    public sealed class SourcePin : Pin, ISourcePin
    {
        public SourcePin()
        {
            this.signalConsumed = new SignalConsumedDelegate(this.OnConsumed);
        }

        public event SignalConsumedDelegate ConsumedEvent;

        protected override void OnSignalAttached(Signal signal)
        {
            signal.ConsumedEvent += this.signalConsumed;
        }

        protected override void OnSignalDetached(Signal signal)
        {
            signal.ConsumedEvent -= this.signalConsumed;
        }

        private readonly SignalConsumedDelegate signalConsumed;

        private void OnConsumed()
        {
            if (this.ConsumedEvent != null)
            {
                this.ConsumedEvent();
            }
        }
    }
}
