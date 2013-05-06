using System;

namespace SDRBlocks.Core
{
    public abstract class Pin : IPin
    {
        #region IPin Members

        public Signal AttachedSignal
        {
            get
            {
                return this.attachedSignal;
            }
            set
            {
                if (value != null)
                {
                    this.AttachSignal(value);
                }
                else
                {
                    this.DetachSignal();
                }
            }
        }

        public bool IsConnected { get { return this.AttachedSignal != null && this.AttachedSignal.IsConnected; } }

        public event PinEvent SignalAttachedEvent;

        public event PinEvent SignalDetachedEvent;

        #endregion

        protected abstract void OnSignalAttached(Signal signal);
        protected abstract void OnSignalDetached(Signal signal);

        private Signal attachedSignal;

        private void AttachSignal(Signal signal)
        {
            this.DetachSignal();

            signal.NotifyOnAttach(this);
            this.attachedSignal = signal;
            this.OnSignalAttached(signal);

            if (this.SignalAttachedEvent != null)
            {
                this.SignalAttachedEvent();
            }
        }

        private void DetachSignal()
        {
            if (this.attachedSignal != null)
            {
                Signal signal = this.attachedSignal;

                this.attachedSignal = null;

                signal.NotifyOnDetach(this);
                this.OnSignalDetached(signal);

                if (this.SignalDetachedEvent != null)
                {
                    this.SignalDetachedEvent();
                }
            }
        }

    }
}
