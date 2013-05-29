using System;

namespace SDRBlocks.Core
{
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

        /// <summary>
        /// Whether another block is attached (via a signal) to this pin.
        /// </summary>
        public bool IsConnected { get { return this.AttachedSignal != null && this.AttachedSignal.IsConnected; } }

        /// <summary>
        /// Which block owns this pin.
        /// Should be of no use outside of the core module.
        /// </summary>
        internal IDspBlock Owner { get; private set; }

        #region Implementation details

        private Signal attachedSignal;

        private void AttachSignal(Signal signal)
        {
            this.DetachSignal();
            this.attachedSignal = signal;
        }

        private void DetachSignal()
        {
            this.attachedSignal = null;
        }

        #endregion
    }
}
