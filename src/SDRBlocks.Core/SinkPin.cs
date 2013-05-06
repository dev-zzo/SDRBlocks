using System;

namespace SDRBlocks.Core
{
    public sealed class SinkPin : Pin, ISinkPin
    {
        public SinkPin()
        {
            this.signalRefilled = new SignalRefilledDelegate(this.OnRefilled);
        }

        public SignalRefilledDelegate RefilledEvent;

        protected override void OnSignalAttached(Signal signal)
        {
            signal.RefilledEvent += this.signalRefilled;
        }

        protected override void OnSignalDetached(Signal signal)
        {
            signal.RefilledEvent -= this.signalRefilled;
        }

        private readonly SignalRefilledDelegate signalRefilled;

        private void OnRefilled()
        {
            if (this.RefilledEvent != null)
            {
                this.RefilledEvent();
            }
        }
    }
}
