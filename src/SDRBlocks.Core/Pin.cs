using System;

namespace SDRBlocks.Core
{
    public delegate void SignalAttachDelegate(Pin sender, Signal signal);

    public abstract class Pin
    {
        public Pin(IDspBlock owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Access the attached signal.
        /// </summary>
        public Signal AttachedSignal
        {
            get { return this.signal; }
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

        /// <summary>
        /// Whether another block is attached (via a signal) to this pin.
        /// </summary>
        public bool IsConnected { get { return this.AttachedSignal != null && this.AttachedSignal.IsConnected; } }

        /// <summary>
        /// Which block owns this pin.
        /// Should be of no use outside of the core module.
        /// </summary>
        internal IDspBlock Owner { get; private set; }

        private Signal signal;

        private void AttachSignal(Signal s)
        {
            this.DetachSignal();
            this.signal = s;
            this.signal.NotifyOnAttach(this);
        }

        private void DetachSignal()
        {
            if (this.signal != null)
            {
                this.signal.NotifyOnDetach(this);
                this.signal = null;
            }
        }
    }
}
